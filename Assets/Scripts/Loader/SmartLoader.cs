using UnityEngine;
using System;
using System.IO;

/**
 * 之前需要手动做大量this==null,this.gameObject==null的判断，耗时耗力，这里要统一
 * NEXT STEP 注意，将来动态加载还有很多可优化之处
 * 
 * 注意两个Load方式的区别，出现两种方式是为了使用对象池(GOPool)用法的需要
 * 1:LoadPrefab()注意这个加载到的是prefab,需要由使用者去Instantiate(prefab),
 * 2:Load(这个会创建好GO,因为有可能创建的对象来自对象池)
 */ 

/// <summary>
/// 一个Loader,负责加载素材
/// 对UGE.loadMgr.Load(xxx,OnLoadOK)进行更好的封装，更易于使用，
/// 带Cancle()功能，
/// 支持使用对象池提取对象（前提是对象已入库）
/// 支持下载完毕后的空指针判断
/// 
/// 处理情况诸如
/// 很可能等加载到了东西之后，原来请求加载的宿主已经被销毁
///  smartloader是一对一的。  不要用1个smartloader在不同位置 加载2个东西!!!  错误用法，如：1个smartLoader加载了一个go，又用它加载1个go并放在其他位置。显示中看到2个go。实际是1个smartloader。这样会造成资源卸载问题!!!
/// smartLoader管理加载go的对象池。外部不用管怎么处理对象池的事情.
/// 	go加载完成，进入回调方法中可能对其发生变化。这时候就在load的时候传入恢复方法。
/// 	
/// 【注意】smartLoader返回的是实例化出来的GO，不是prefab！
/// </summary>
public class SmartLoader
{
	#region 等待中、加载中
	/// <summary>
	/// 请求中的url
	/// </summary>
	private string _requestUrl="";//请求加载的最新的url
    private string _requestAssetName = "";  //请求加载的assetName
	private Action<GameObject> _requestCallback = null;
	#endregion

	#region 加载完成
	/// <summary>
	/// 加载完成的url
	/// </summary>
	private string _loadedUrl = "";
    private string _loadedAssetName = "";
    private Action<GameObject> _loadedCallback = null;
	#endregion

	//保留 加载下载的对象。 方便进入对象池
	private GameObject _resGO = null;

    ///<summary>
    ///重要设置，是否使用prefab池				
    /// </summary>
    bool useGOPool = true;
	/// <summary>
	/// 进入对象池前，要恢复的操作 (1对1的，所以这里是一类的)
	/// </summary>
	public Action<GameObject> recoverGO = null;

	/// <summary>
	/// 采用对象池类型
	/// </summary>
	int poolType = 1;
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="p_poolType">
	/// 1:UGE.poolMrg  -3D(默认)
	/// 2:UGE.uiHudPoolMrg -2D 血条
	/// </param>
	public SmartLoader (int p_poolType = 1, bool p_useGOPool=true)
    {
		poolType = p_poolType;
        useGOPool = p_useGOPool;
    }

	/// <summary>
	/// 请求加载的最新的url
	/// </summary>
	/// <returns>The request URL.</returns>
	public string GetRequestUrl(){
		return _requestUrl;
	}
	public string GetLoadedUrl(){
		return _loadedUrl;
	}
    public string GetRequestAssetName(){
        return _requestAssetName;
    }
    public string GetLoadedAssetName(){
        return _loadedAssetName;
    }

    /// <summary>
    /// isdebug模式的UI的prefab
    /// </summary>
    private GameObject uiLoaderPrefab;
    /// <summary>
    /// UI专用的Load，用于处理UI的isdebug模式本地加载，详细用法见参数说明
    /// </summary>
    /// <param name="assetName">UI的名称</param>
    /// <param name="callback">完成回调</param>
    /// <param name="isdebug">是否UI的isdebug模式</param>
    /// <param name="urlFunc">UI的名称到网络url的方法，传入UI名称和是否debug模式，传出对应的url</param>
    public void LoadUI(string assetName, Action<GameObject> callback, bool isdebug, Func<string, bool, string> urlFunc)
    {
        if (isdebug)
        {
            if (uiLoaderPrefab == null)
            {
                uiLoaderPrefab = Resources.Load(urlFunc(assetName, isdebug)) as GameObject;
            }
            callback(GameObject.Instantiate(uiLoaderPrefab));
        }
        else
        {
            Load(urlFunc(assetName, isdebug), callback);
        }
    }

    /// <summary>
    /// UI专用的Unload，用于处理UI的isdebug模式本地加载
    /// </summary>
    /// <param name="isdebug">是否UI的isdebug模式</param>
    /// <param name="isNow">是否马上卸载</param>
    public void UnloadUI(bool isdebug, bool isNow = false)
    {
        if (isdebug)
        {
            uiLoaderPrefab = null;
        }
        else
        {
            Unload(isNow);
        }
    }

	/// <summary>
	/// 加载资源
	/// </summary>
	/// <param name="url"></param>
	/// <param name="p_callback"></param>
    public void Load(string url, Action<GameObject> callback, int priority = 100, bool isFree = true)
	{
    
        string assetName = Path.GetFileNameWithoutExtension(url);
        this.Load(url, assetName, callback, priority, isFree);
    }

    public void Load(string url, string assetName, Action<GameObject> callback, int priority = 100, bool isFree = true,object param=null)
    {
        //当前正在加载中，那么就要取消当前加载的
        if (_requestUrl != "")
        {
            if (url == _requestUrl && assetName == _requestAssetName)
            {
                //请求的，和加载中的一样!!!! -> 执行继续加载
                return;
            }
            Cancel();
        }
        if (_loadedUrl != "" && url == _loadedUrl && assetName == _loadedAssetName)
        {
            //请求的，和加载完成的一样!!!! -> 忽略当前请求
            _loadedCallback(_resGO);
            return;
        }

        //记录要“新加载”的
        _requestUrl = url;
        _requestAssetName = assetName;
        _requestCallback = callback;

        //使用prefab池,并且在池里有空闲,这个会导致瞬间回调
        if (useGOPool)
        {
            //从对象池里取到东西
            var e = GetFreeItem();

            if (e != null)
            {
                if (_requestCallback != null)
                {           //这里是容错!!! 不应该为null
                    //TTrace.p ("取到了缓存",currUrl);
                    LoadOver(e);
                    return;
                }
            }
        }
        //没使用对象池，或者使用了之后找不到
        LoadManager.instance.LoadObject<GameObject>(url, OnLoadOK,null);
        //UGE.loadMgr.LoadObj(url, assetName, OnLoadOK, null, true, priority, isFree);
    }

    /// <summary>
    /// 回调前
    /// </summary>
    /// <param name="go">Go.</param>
    void LoadOver(GameObject go){
		//加载完成
		//1.清理之前加载完成的内容
		if(_loadedUrl != ""){
			ClearLoaded();
		}
		//2.保存当前加载完成的内容
		_loadedUrl = _requestUrl;		//保存加载完成的地址
        _loadedAssetName = _requestAssetName;
		_loadedCallback = _requestCallback;
			//清理 - “加载中、等待中”
		_requestUrl = "";
        _requestAssetName = "";
        _requestCallback = null;
		//3.保存对象 - 派发go
		_resGO = go;
		_loadedCallback( _resGO );
	}
	
	/// <summary>
	/// 取消(等待加载、加载中)
	/// 注意：
	/// 	加载完成的，这里没有处理
	/// </summary>
	/// <returns>true:取消成功。 false:取消失败，因为已经加载完成</returns>
	public bool Cancel(){
		if(_requestUrl != ""){
			//UGE.loadMgr.Cancel(_requestUrl, _requestAssetName, OnLoadOK);
			_requestUrl = "";
		    _requestAssetName = "";
            _requestCallback = null;
			return true;
		}
		return false;
	}

	/// <summary>
	/// 卸载资源(等待加载、加载中、加载完成)
	/// </summary>
	/// <param name="isNow">是否马上卸载</param>
	public void Unload(bool isNow=false){
//		TTrace.p("SmartLoader.Unload, _requestUrl = " + _requestUrl);
		//清理加载中、等待加载的
		Cancel();
		//清理加载完成的
		ClearLoaded(isNow);
	}

	/// <summary>
	/// 清理加载完成的
	/// </summary>
	void ClearLoaded(bool isNow=false){
		if(_loadedUrl != ""){
			if(useGOPool){
				//存入对象池
				if(_resGO == null){
					//说明没有加载完成，不用进对象池
					//UGE.loadMgr.Unload(_loadedUrl, _loadedAssetName, OnLoadOK, true, isNow);
				}else{
					//如果有变动，需要设置恢复方法
					if(recoverGO != null){
						recoverGO(_resGO);
						recoverGO = null;
					}
					RecycleItem();
					_resGO = null;	//存入后，就可以不持有了
				}
			}else{
				//这里管理删除所加载的go。 因为由smartLoader创建的，所以也又它处理
				if(_resGO != null){
					GameObject.DestroyObject( _resGO );
					_resGO = null;
				}
				//UGE.loadMgr.Unload(_loadedUrl, _loadedAssetName, OnLoadOK, true, isNow);
			}
		}
		//清理的存储的加载完成的内容
		_loadedUrl = "";
		_loadedCallback = null;
		_resGO = null;
	}
	/// <summary>
	/// 加载完成事件
	/// </summary>
	void OnLoadOK(GameObject prefab, object param){
		//
		
		LoadOver( GameObject.Instantiate(prefab) as GameObject );
	}

	#region 对象池操作
	/// <summary>
	/// 回收到对象池中
	/// </summary>
	void RecycleItem(){
	    GOPoolV3.instance.RecycleItem(_loadedUrl, _loadedAssetName, _resGO, true);
    }
	/// <summary>
	/// 获取对象池中go
	/// </summary>
	GameObject GetFreeItem(){
		GameObject resultGO = null;
        var itemGO = GOPoolV3.instance.GetFreeItem(_requestUrl, _requestAssetName);
        if (itemGO != null)
        {
            resultGO = itemGO;
        }
        return resultGO;
	}
	#endregion
}