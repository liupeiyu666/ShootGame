using System.Text;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// go池V3，根据类型做不同的处理
/// 	含Update
/// type{
/// 	1:
/// 		超指定数量->销毁
/// 		时间到了->销毁
/// 	2:
/// 		超过了指定的数量
/// 			后，开始采用时间->销毁
/// 	3:
/// 		时间到了->销毁
/// }
/// 存在对象池中的方式
/// 	setAction
/// 	移除屏幕外
/// 是否更改父级
/// 	UI setParent 消耗很大。所以一般UI禁止重新设置父级
/// 
/// 备注：
/// 	进入对象池之前，go如果已经被修改，要恢复go后再放入(尤其要干掉引用关系)
/// ---
/// 1个类型 一种对象池
/// 根据需求自行定制所需
/// 
/// 替代Uge的GOPoolV2的创建方法： 时间到了销毁
/// 	GOPoolV3 pool = new GOPoolV3(3, 1, true);
/// 替换Uge的Map中使用的MapPool： 
/// 	GOPoolV3 pool = new GoPoolV3(1, 1, true);
/// UI方面使用方法：
/// 	GOPoolV3 pool = new GoPoolV3(3, 2, false);
/// UI血条使用：
/// 	GOPoolV3 pool = new GoPoolV3(2, 2, false);
/// 	
/// 备注：
///     只要需要unload，那么就需要传入assetName
/// </summary>
public class GOPoolV3
{
    #region 单例

    private GOPoolV3()
    {
    }

    private static GOPoolV3 _instance;

    public static GOPoolV3 instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GOPoolV3(1,1,true,new GameObject("Pool").transform);
            }

            return _instance;
        }
    }

    #endregion
    /// <summary>
    /// 某对象如果30秒不使用，则销毁
    /// </summary>
    public const float LIFE_SEC=10f;
	/// <summary>
	/// 一次清理数量
	/// 一次清理，最多3个
	/// 	oneClearNum和clearCount不同。假设满了清理6个，那么就是2次update才能完成6个操作。保帧频操作
	/// </summary>
	public const int oneClearNum = 3;
	#region 设置参数
	/// <summary>
	/// 池中最大数量
	/// </summary>
	public int MaxCount = 150;
	/// <summary>
	/// 当满了，每次清理几个
	/// </summary>
	public int clearCount = 3;
	#endregion

	//用于存放PoolItemV3		--	考虑换成公共的
	ScriptPoolV1 scriptPool = new ScriptPoolV1();
	//1:共存		2:超数量了，开始根据时间删		3:时间到了，销毁
	int _type;
	public int type{
		get{
			return _type;
		}
	}
	//1:setAction(false)	2:移除屏幕外-5000
	int goHideType;
	//是否更改父级: true:更改(存放在 uge对象池子节点下); false:不更改
	bool isNeedSetParent;
    /// <summary>
    /// 存放回收对象父节点
    /// </summary>
    private Transform poolContainer;
	public GOPoolV3(int p_type, int p_goHideType, bool p_isNeedSetParent, Transform p_poolContainer)
    {
		_type = p_type;
		goHideType = p_goHideType;
		isNeedSetParent = p_isNeedSetParent;
        poolContainer = p_poolContainer;
    }

	//用于保存 池的对象名	//dict变成list，内部是是否复杂的。   未来dictionary如果也是这样的用法，最好封装成一个类
	List<string> poolNameList = new List<string>();
    //真实存储  dict[path+assetName] = List[PoolItemV3];
    Dictionary<string, List<PoolItemV3>> itemDict = new Dictionary<string, List<PoolItemV3>>();
	
	/// <summary>
	/// 返回保存对象的个数
	/// </summary>
	/// <param name="name">goName</param>
	public int Size(string p_name, string assetName){
		List<PoolItemV3> list;
		if(itemDict.TryGetValue(UGEUtils.MergeStr(p_name, assetName), out list)){
			return list.Count;
		}
		return 0;
	}
	
	/// <summary>
	/// 释放所有
	/// </summary>
	/// <param name="isImmediately">立刻销毁		true是</param>
	public void UnloadAll(){
		int i=0;
		int ilen, j;
		List<PoolItemV3> list = null;
		for(ilen=poolNameList.Count; i<ilen; i++){
			list = itemDict[poolNameList[i]];
			for(j=0; j<list.Count;){
				RemoveItem(list, list[j]);
			}
		}
		poolNameList.Clear();
		itemDict.Clear();
		//--考虑换成公共的，清理此类而已
		scriptPool.ClearAll();
	}
	/// <summary>
	/// 通过名称卸载go组
	/// </summary>
	/// <param name="goName">Go name.</param>
	public void UnloadGoListByName(string p_name, string p_assetName)
	{
	    string keyStr = UGEUtils.MergeStr(p_name, p_assetName);
        List<PoolItemV3> list;
		if(itemDict.TryGetValue(keyStr, out list)){
			for(var j=0; j<list.Count;){
				RemoveItem(list, list[j]);
			}
			itemDict.Remove(keyStr);
			poolNameList.Remove(keyStr);	//从循环列表中删除
		}
		list = null;
	}

	/// <summary>
	/// 获取空闲对象
	/// </summary>
	/// <param name="filename">名字</param>
	/// <returns>对象池空闲对象</returns>
	public GameObject GetFreeItem(string p_name, string p_assetName)
	{
	    string keyStr = UGEUtils.MergeStr(p_name, p_assetName); 
        List<PoolItemV3> list;
       
		if(itemDict.TryGetValue(keyStr, out list)){
			for(int i =0, ilen=list.Count; i<ilen; i++){
				PoolItemV3 item = list[i];
				if(item.isUpdated && item.key==keyStr){
					//得到要返回的go
					var resultGO = item.go;
					//修改go
					if(goHideType != 1){
						resultGO.transform.position = item.originalPos;	//恢复位置
					}
                    //这个操作，统一由外界自行处理。因为可能外部不想显示呢，这里只是返回对象
                    //					else{
                    //						resultGO.SetActive(true);						//外部恢复-显示
				    //					}
				    //从list中删除，减少update压力
				 //   Debug.LogError("GET=====:" + itemDict.ContainsKey(keyStr) + "   " + keyStr+"  "+Time.frameCount);
                    list.RemoveAt(i);
					//放入脚本池
					item.go = null;			//清理引用
					scriptPool.Recycle("PoolItemV3", item);
					//
					return resultGO;
				}
			}
		}
		return null;
	}
	
	///<summary>定期销毁不再使用的对象</summary>
	public void tUpdate(){
		int i, ilen, j;
		int clearNum = 0;
		List<PoolItemV3> list;
		PoolItemV3 item;
		//组循环
		for(i=0, ilen=poolNameList.Count; i<ilen; i++){
			//具体list循环
			if(itemDict.TryGetValue(poolNameList[i], out list)){
				for(j=0; j<list.Count;){
					item = list[j];
					item.isUpdated=true;//标志上说刷新了，可以用了
					if(item.go == null){
						clearNum++;
						RemoveItem(list, item, false);
						//判断是否已经到了清理的最大数量
						if(clearNum == oneClearNum){
							break;
						}
					}else if(item.isForever==false){		//是否可以销毁的
						bool isJudTimer = true;
						if(type == 2){
							//只有超过了指定的数量，才会开始判断销毁go
							if(list.Count <= MaxCount){
								isJudTimer = false; 
							}
						}
						//需要判断时间
						if(isJudTimer){
							if(Time.fixedTime- item.recycleTime > LIFE_SEC){
//								Debug.Log("时间到，清理="+ item.name);
								clearNum++;
								RemoveItem(list, item);
								//判断是否已经到了清理的最大数量
								if(clearNum == oneClearNum){
									break;
								}
							}else{
								j++;
							}
						}else{
							j++;
						}
					}else{
						j++;
					}
				}
			}else{
			
			}
		}
	}
	
	/// <summary>
    /// 回收go, forever是说不受计时器限制
	/// </summary>
	/// <param name="p_name"></param>
	/// <param name="p_assetName"></param>
	/// <param name="p_go"></param>
	/// <param name="p_isUnload">销毁的时候 是否需要被调用unload</param>
    /// <param name="isActive">
    /// true:进入对象池不隐藏，外界要进行操作(eg:淡出功能，进入对象池后，go再淡出)
    /// false:1:setAction(false)	2:移除屏幕外-5000
    /// </param>
    /// <param name="p_isForever">是否持久存在,玩家头顶血条要持久存在，出于性能考虑	--统一放入此对象池，是为了外界使用方法。</param>
	public void RecycleItem(string p_name, string p_assetName, GameObject p_go, bool p_isUnload=false, bool isActive=false, bool p_isForever=false){
		PoolItemV3 item = scriptPool.Get<PoolItemV3>("PoolItemV3");
		//加入到循环 Dict List中
		List<PoolItemV3> list;
	    string keyStr = UGEUtils.MergeStr(p_name, p_assetName);
	    if (itemDict.ContainsKey(keyStr))
	    {
	        //Debug.LogError("BEFORE添加啦啦啦啦=====:" + itemDict.ContainsKey(keyStr) + "   " + itemDict[keyStr].Count+"   "+Time.frameCount);
        }
	   
        if (itemDict.TryGetValue(keyStr, out list)){
			list.Add(item);
		}else{
			list = new List<PoolItemV3>();
			list.Add(item);
			itemDict.Add(keyStr, list);
			poolNameList.Add(keyStr);			//进入到循环列表中
		}
	   
        //设置数据
        item.key = keyStr;
		item.name = p_name;
	    item.assetName = p_assetName;
		item.go = p_go;
        //item.go.name = "Wait_" + p_go.name;       //这里不在更改go的名称；减少拼组字符串   //Add Wade 0425+
		item.recycleTime = Time.fixedTime;
		item.isForever=p_isForever;
		item.isUpdated=false;				//需要经过一次update才能继续使用,否则会面临延迟销毁（帧结尾）的冲突！！
		item.isUnload = p_isUnload;
		
		//回收的对象放到自己下面
		if(isNeedSetParent)
		{
		   // Debug.LogError("END添加啦啦啦啦=====:" + itemDict.ContainsKey(keyStr) + "   " + itemDict[keyStr].Count + "   " + Time.frameCount);
            item.go.transform.parent = poolContainer;
		}
		
		//冻结		//true:进入对象池不隐藏，外界要进行操作(eg:淡出功能，进入对象池后，go再淡出)
		if(isActive == false){		
			if(goHideType == 1){
				item.go.SetActive(false);
			}else{
//				Debug.Log("血条进入对象池~~" + p_name);
				item.originalPos = item.go.transform.position;
				item.go.transform.position = new Vector3(item.originalPos.x, -5000f, item.originalPos.z);
			}
		}

		if(_type == 1){
			//判断如果超过了，最大值就删除时间较早的
			if(list.Count > MaxCount){
				CheckItem(list);
			}
		}
	}
	
	/// <summary>
	/// 检查是否超过了指定大小，如果超过了，就删除
	/// </summary>
	void CheckItem(List<PoolItemV3> list){
		//排序
		int i, j, iLen=list.Count-1, jLen=list.Count;
		PoolItemV3 item01, item02, itemLoc;
		for(i=0; i<iLen; i++){
			item01 = list[i];
			for(j=i+1; j<jLen; j++){
				item02 = list[j];
				if(item01.recycleTime < item02.recycleTime){
					itemLoc = list[j];
					list[j] = list[i];
					list[i] = itemLoc;
				}
			}
		}
		//清理
		for(i=0; i<clearCount; i++){
			if(list.Count > 0){
				var index = list.Count - 1;
				var item = list[index];
				if(item.isForever==false){	//是否不能删除
					//删除
					RemoveItem(list, item );
				}
			}else{
				break;
			}
		}
	}
	
	/// <summary>
	/// 删除list中 固定位置资源
	/// </summary>
	/// <param name="index">索引id</param>
	void RemoveItem(List<PoolItemV3> list, PoolItemV3 item, bool isDestroy=true){
		if(isDestroy){
			GameObject.DestroyObject(item.go, 0);
		}
		if(item.isUnload){
			//UGE.loadMgr.Unload(item.name, item.assetName, null);
		}
		list.Remove(item);
		//下面不判断list等于0，不清理dict和nameList。
	}

    /// <summary>
    /// 输出
    /// </summary>
    public string Dump()
    {
        StringBuilder sb = new StringBuilder("GOPoolV3.Dump");
        sb.AppendLine();
        int i = 0;
        int ilen, j;
        List<PoolItemV3> list = null;
        for (ilen = poolNameList.Count; i < ilen; i++)
        {
            list = itemDict[poolNameList[i]];
            sb.Append("key=");
            sb.Append(poolNameList[i]);
            sb.Append("  |  count=");
            sb.Append(list.Count);
            sb.AppendLine();
        }
        return sb.ToString();
    }
}

/// <summary>
/// 池 元素
/// </summary>
public class PoolItemV3 : ScriptPoolItem
{
    /// <summary>
    /// path + assetName; 方便查找；以免每次都string+
    /// </summary>
    public string key;
	/// <summary>
	/// go名字( path )
	/// </summary>
	public string name;
    /// <summary>
    /// 用于unload资源；  unload资源需要path和assetName
    /// </summary>
    public string assetName;
	/// <summary>
	/// 对象池内GameObject
	/// </summary>
	public GameObject go;
	/// <summary>
	/// go的原始位置
	/// </summary>
	public Vector3 originalPos=Vector3.zero;
	/// <summary>
	/// 时间戳,被回收时的时间
	/// </summary>
	public float recycleTime;
	/// <summary>
	/// 回收到池子里的东西，要经过一次update以后才能继续使用，否则新设置的值有可能会被Destory掉,因为Destory总是在Update之后执行
	/// </summary>
	public bool isUpdated;
	/// <summary>
	/// 是否需要卸载资源
	/// uge.loadMgr.unload
	/// </summary>
	public bool isUnload = false;
	/// <summary>
	/// 是否持久存在,玩家头顶血条要持久存在，出于性能考虑	--统一放入此对象池，是为了外界使用方法。(坏处:增加update循环次数)
	/// </summary>
	public bool isForever;

	//减少循环的次数，item采用脚本池
//	/// <summary>
//	/// 是否空闲了
//	/// </summary>
//	public bool isFree;
}
/// <summary>
/// Script pool basic.
/// </summary>
public class ScriptPoolItem
{
    /// <summary>
    /// 脚本名称		//未来getType()代替掉。因为网络协议，现在有些没用对象池，而回收网络协议的时候，要都回收。特此用name的方式做的
    /// </summary>
    public string scriptName = "";
    /// <summary>
    /// 出对象池前，所调用方法
    /// </summary>
    public virtual void OnAfterPopFromPool() { }

    /// <summary>
    /// 进对象池前，所调用方法
    /// </summary>
    public virtual void OnBeforePushToPool() { }

    public virtual bool Init(object[] objs) { return true; }


}