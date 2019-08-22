

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
 

/// <summary>
/// 游戏实用工具类
/// </summary>
public class UGEUtils
{
    /// <summary>
    /// 专门用于 path和assetName的 字符串+操作；
    /// 其他请用其他方法
    /// </summary>
    public static string MergeStr(string str01, string str02)
    {
        return string.IsNullOrEmpty(str02) ? str01 : str01 + str02;
    }

    /// <summary>
    /// Add Lpy 被封装进去的时间计时
    /// </summary>
    //public static float realtimeSinceStartup;
    //获得在GUI屏幕坐标系的坐标，供SpriteDebugInfoComp用
    public static bool GetScreenCoordFromWorldCoord(Vector3 worldPosi,out Vector2 screenPosi){
		screenPosi=Vector2.zero;
		var p=Camera.main.WorldToScreenPoint(worldPosi);
		if(p.z<=0){
			
			return false;//摄像机后的不显示
		}

		screenPosi.x=p.x;
		screenPosi.y=Screen.height-p.y;
		return true;
	}


	//TODO 经测试，此函数无效！
	//强制设置lod的层级
	public static void ForceLod(GameObject go,int lod){
		if(go!=null){
			var ll=go.transform.GetComponentsInChildren<LODGroup>();
			foreach(var l in ll){
				l.ForceLOD(lod);
				l.enabled=false;
			}
		}
	}
	//!!非常有用的检查trm是否有有效的代码
	//如果逻辑里引用了某trm,如果此trm的gameObject被Destory里，则当前帧时，trm.parent=null,trm!=null,trm.gameObject还有。
	//因为destory是在帧结束的时候执行
	//到了下一帧，trm会变成null
	public static bool IsValidTransform(Transform trm){
		if(trm==null || trm.parent==null){
			return false;//trm被销毁了
		}
		return true;
	}



	#region 查找child
	//这个有小写
	static public Transform GetChildByName(Transform ts , string name)
	{
		if (ts == null)
			return null;

		if(ts.name.ToLower() ==name.ToLower())
			return ts;

		Transform ret;
		foreach(Transform tc in ts)
		{
			ret = GetChildByName(tc,name);	
			if(ret!=null)
			{
				return ret;
			}
		}
		return null;	
	}

	//!!这个没有小写
	static public void GetChildByName(Transform ts, string name,ref List<Transform> list)
	{
		if (ts == null)
			return;

		if (ts.name == name)
		{
			list.Add(ts);
		}

		foreach (Transform tc in ts)
		{
			GetChildByName(tc, name,ref list);
		} 
	}

	static public Transform[] GetAllChildByName(Transform ts, string name)
	{
		List<Transform> list = new List<Transform>();
		GetChildByName(ts, name,ref list);
		return list.ToArray();
	}
	#endregion

	public static T GetOrAddComponent<T>(GameObject go)where T:MonoBehaviour{
		T comp=go.GetComponent<T>();
		if(comp==null){
			comp=go.AddComponent<T>();
		}
		return comp;
	}

	/// <summary>
	/// 设置Transform的Parent,并重置坐标为0,角度为0,scale为1
	/// </summary>
	/// <param name="child"></param>
	/// <param name="parent"></param>
	public static void SetParent(Transform child,Transform parent){
		if(child!=null && parent!=null && parent!=child){//parent!=child很重要
			child.parent=parent;
			child.localPosition=Vector3.zero;
			child.localScale=Vector3.one;
			child.localRotation=Quaternion.identity;
		}
	}
	
	/// <summary>
	/// 销毁GameObject
	/// </summary>
	/// <param name="go">GameObject</param>
	public static void DestroyGameObject(GameObject go){
		if(go==null){
			return;
		}
		GameObject.Destroy(go);
	}

	/// <summary>
	/// 移除并销毁children
	/// </summary>
	/// <param name="trm">要移除children的对象</param>
	public static  void CleanChildren(Transform trm){
		//Debug.Log ("TGameUtil.CleanChildren",trm.name);
		while(trm.childCount > 0 ){

			Transform  tc  = trm.GetChild(0);
			
			DestroyGameObject(tc.gameObject);
		}
	}

	/// <summary>
    /// 根据距离slef的远近，排序目标
	/// 注意排序是在xoz平面排序的
	/// </summary>
	/// <param name="targetTrmList">目标列表</param>
	/// <param name="self">自己</param>
	public static void SortTargetTrmListByDistance(List<Transform> targetTrmList,Transform self){
		//var om=UGE.sprsMgr;
		targetTrmList.Sort(delegate(Transform a1,Transform a2){
			
			float d1=VectorUtils.GetXOZDistance(self.position,a1.position);//TODO 这个做平方优化后期
			float d2=VectorUtils.GetXOZDistance(self.position,a2.position);
			
			if(d1==d2){
				return 0;
			}
			return d1>d2?1:-1;
		});
	}

	/// <summary>
	/// 把逗号分隔的浮点数转化成Vector3
	/// </summary>
	/// <param name="str">逗号分隔的浮点数</param>
	/// <returns>转换后的Vector3</returns>
	public static Vector3 StringToVector3(string str){
		string [] arr = str.Split(',');
    	return new Vector3(float.Parse(arr[0]),float.Parse(arr[1]),float.Parse(arr[2]));
	}

	/// <summary>
	/// 设置Gameobject的影子
	/// </summary>
	/// <param name="go"></param>
	/// <param name="castShadow"></param>
	/// <param name="receiveShadow"></param>
	public static void SetAllShadow(GameObject go,bool castShadow,bool receiveShadow){
		var list=FindInChildren<Renderer>(go);
		foreach(var r in list){
			r.castShadows=castShadow;
			r.receiveShadows=receiveShadow;
		}
	}

	/// <summary>
	/// 清理GO对象的children的所有collider,这个是为了让掉落物不能被点取而做
	/// </summary>
	/// <param name="go">清理对象</param>
	public static void RemoveAllCollider(GameObject go){
		Collider[] cArr = go.GetComponentsInChildren<Collider>(true); //1129 kk+ true
		foreach(var c in cArr){
			GameObject.Destroy(c);
		}
	}

	//2014/7/31 LBS+
	/// <summary>
    /// 获取Transform及其下层的包围除ParticleSystem所有Render的大包围盒
	/// </summary>
	/// <typeparam name="ExtendedRender"></typeparam>
	/// <param name="trm">transform</param>
	/// <returns></returns>
	public static Bounds GetRenderCompsBounds< ExtendedRender >( Transform trm )where ExtendedRender : Renderer{ 
		ExtendedRender[] renders = trm.GetComponentsInChildren<ExtendedRender>(true);//1129 kk+ true
		if( renders.Length > 0 ){
			Bounds bounds = renders[0].bounds;
			for( int i=1; i<renders.Length; i++ ){
				Bounds boundsI = renders[i].bounds;
				//将bound的大小扩展到包含boundsI和bound的大小
				bounds.Encapsulate( boundsI.min );
				bounds.Encapsulate( boundsI.max );
			}
			return bounds;
		}else{
			return new Bounds();
		}
	}

	/// <summary>
	/// 设置一个对象下面的所有碰撞是否启用
	/// </summary>
	public static void EnableAllCollider(GameObject go, bool enable){
		Collider[] cArr = go.GetComponentsInChildren<Collider>(true);//1129 kk+ true
		foreach(var c in cArr){
			c.enabled = enable;
		}
	}
	


	/// <summary>
	/// 替换Transform的shader
	/// </summary>
	/// <param name="ts">transform</param>
	/// <param name="oldShader">旧shader</param>
	/// <param name="newShader">新shader</param>
	public static void ReplaceShader(Transform ts, Shader oldShader, Shader newShader) 
	{
		if (ts == null)
            return;

        if (ts.gameObject.GetComponent<Renderer>() != null)
        {
            foreach (Material mt in ts.gameObject.GetComponent<Renderer>().materials)
            {
				if(mt.shader==oldShader)
				{
                	mt.shader = newShader;
				}
            }
        }

        foreach (Transform child in ts)
        {
            ReplaceShader(child, oldShader, newShader);
        }		
	}


	/// <summary>
	/// 替换Transform的shader
	/// </summary>
	/// <param name="ts">transform</param>
	/// <param name="oldShader">旧shader名称</param>
	/// <param name="newShader">新shader名称</param>
	public static void ReplaceShader(Transform ts, string oldShader, Shader newShader) 
	{
		if (ts == null)
            return;

        if (ts.gameObject.GetComponent<Renderer>() != null)
        {
            foreach (Material mt in ts.gameObject.GetComponent<Renderer>().materials)
            {
				if(mt.shader.name==oldShader)
				{
                	mt.shader = newShader;
				}
            }
        }

        foreach (Transform child in ts)
        {
            ReplaceShader(child, oldShader, newShader);
        }		
	}
	

	/// <summary>
	/// string字符串传换成int数组
	/// </summary>
	/// <param name="str">string字符</param>
	/// <param name="c">char类型</param>
	/// <returns></returns>
	public static int[] ConvertStrToIntArr(string str,char c){
		string[] sArr=str.Split(c);
		int[] a=new int[sArr.Length];
		for(var i=0;i<sArr.Length;i++){
			int result;
			if(int.TryParse(sArr[i],out result)){
				a[i]=result;
			}
		}
		
		return a;
	}
	
	//scale是精度，1,10是到0.1,100是到0.01

	public static bool FloatEqual(float a,float b,int precision){
		return ((int)(a*precision))==((int)(b*precision));
	}
	
    /// <summary>
    /// string字符串转换成int列表
    /// </summary>
    /// <param name="str"></param>
    /// <param name="c"></param>
    /// <returns></returns>
	public static List<int> ConvertStrToIntList(string str,char c){
		
		List<int> list=new List<int>();
		
		if(string.IsNullOrEmpty(str)){
			return list;
		}
		
		string[] sArr=str.Split(c);
		for(var i=0;i<sArr.Length;i++){
			int result;
			if(int.TryParse(sArr[i],out result)){
				list.Add(result);
			}
		}
		
		return list;
	}

	


#region	提取组件
    /// <summary>
    /// 获取GameObject下孩子对应的Component
    /// </summary>
    /// <typeparam name="T">Component类型</typeparam>
    /// <param name="go">该GameObject</param>
    /// <returns></returns>
	public static List<T> FindInChildren<T>(GameObject go) where T : Component
	{
		return FindInChildren<T>(go, false);
	}
	
	static List<T> FindInChildren<T>(GameObject go, bool jsutOne) where T : Component
	{
		List<T> cps = new List<T>();
		
		if(go != null)
			FindInChildren<T>(go.transform, cps, jsutOne);
		
		return cps;
	}
	
	static void FindInChildren<T>(Transform trans, List<T> cps, bool justOne) where T : Component
	{
		T cp = trans.GetComponent<T>();
		if(cp != null)
			cps.Add(cp);
		
		if(justOne && cps.Count == 1)
			return;
		
		for(int i = 0; i < trans.childCount; i++)
		{
			FindInChildren<T>(trans.GetChild(i), cps, justOne);
			if(justOne && cps.Count == 1)
				break;
		}
	}
	
//	public static T[] GetComponentsInChildren<T>(GameObject go) where T : Component
//	{
//		List<T> cps = FindInChildren<T>(go, false);
//		return cps.ToArray();
//	}
	
	public static T GetComponentInChildren<T>(GameObject go) where T : Component
	{
		List<T> cps = FindInChildren<T>(go, true);
		if(cps.Count < 1)
			return null;
		
		return cps[0];
	}

	static float Spring(float current , float target1, float K )  {
		
		if (Mathf.Abs(current - target1) <= 0.001) {
			return target1;
		}
	
		float  result1  = current + (target1 - current) / K;
	
		return result1;
	}
	
	public static Vector3 Spring(Vector3 current,Vector3 target1,float K=3){
		Vector3 r;
		r.x=Spring(current.x,target1.x,K);
		r.y=Spring(current.y,target1.y,K);
		r.z=Spring(current.z,target1.z,K);
		return r;
		
	}
#endregion
	
	/// <summary>
    /// 提示字上的数值显示，需要多少和持有多少,数量满足则绿色，不然是红色
	/// </summary>
    /// <param name="haveCount">持有多少</param>
    /// <param name="needCount">需要多少</param>
	/// <returns></returns>
	public static string MakeColorString(int haveCount,int needCount){
		if(haveCount>=needCount){
			return "[00ff00]"+haveCount+"/"+needCount+"[-]";
		}else{
			return "[ff0000]"+haveCount+"/"+needCount+"[-]";
		}
	}
	


    /// <summary>
    /// b不为0返回a/b，b为0返回defaultValue
    /// </summary>
    /// <param name="a">float</param>
    /// <param name="b">float</param>
    /// <param name="defaultValue">float</param>
    /// <returns></returns>
	public static float SafeDivide( float a, float b, float defaultValue=0 ){
		if( b==0 ){
			Debug.LogError("TGameUtils.SafeDevide中b为0,可能导致严重BUG,返回默认值"+defaultValue);
			return defaultValue;
		}else{
			return a/b;
		}
	}

    /// <summary>
    ///  停止指定transform特效的播放
    /// </summary>
    /// <param name="trm">transform</param>
	public static void StopParticles( Transform trm ){
		ParticleSystem[] particles = trm.GetComponentsInChildren<ParticleSystem>();
		for( int i=0; i<particles.Length; i++ ){
			particles[i].Stop();
		}
	}

	/// <summary>
	/// Gets the walk animation play speed.
	/// </summary>
	/// <returns>The walk animation play speed.</returns>
	/// <param name="moveSpeed">Move speed.</param>
	/// <param name="currmovespeed">Currmovespeed.</param>
	public static float GetWalkAnimPlaySpeed(float moveSpeed,float currmovespeed){
		if (moveSpeed < currmovespeed) {
			return moveSpeed / currmovespeed;
		}else{
			return 1;
		}
	}

    public static void SetLayers(GameObject p_go,int p_layer,bool p_containsChild)
    {
        p_go.layer = p_layer;
        if (p_containsChild)
        {
            Transform[] t_child = p_go.GetComponentsInChildren<Transform>();
            for (int i = 0; i < t_child.Length; i++)
            {
                t_child[i].gameObject.layer = p_layer;
            }
        }
    }

    #region 数据类型转换

    public static void GetValueFromString<T>(string t_str,out T t_result)
    {
      //  Debug.LogError("**********=====:" + t_str + "   " + typeof(T));
     
        if (typeof(T) == typeof(int))
        {
            t_result = (T)(object)GetIntFromString(t_str);
        }
        else if (typeof(T) == typeof(float))
        {
            t_result = (T)(object)GetFloatFromString(t_str);
        }
        else if (typeof(T) == typeof(byte))
        {
            t_result = (T)(object)GetByteFromString(t_str);
        }
        else if (typeof(T) == typeof(double))
        {
            t_result = (T)(object)GetDoubleFromString(t_str);
        }
        else if (typeof(T) == typeof(string))
        {
            t_result = (T)(object)t_str;
        }
        else
        {
            t_result = default(T);
        }
    }

    /// <summary>
    /// 字符串转换为 byte
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static byte GetByteFromString(string str)
    {
        byte b = 0;
        if (byte.TryParse(str, out b))
            return b;

        return b;
    }

    /// <summary>
    /// 字符串转换为 sbyte
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static sbyte GetSByteFromString(string str)
    {
        sbyte b = -1;
        if (sbyte.TryParse(str, out b))
            return b;

        return b;
    }

    /// <summary>
    /// 字符串转换为 short
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static short GetShortFromString(string str)
    {
        short s = -1;
        if (short.TryParse(str, out s))
            return s;

        return s;
    }

    /// <summary>
    /// 字符串转换为 float
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static float GetFloatFromString(string str)
    {
        float f = -1f;
        if (float.TryParse(str, out f))
            return f;

        return f;
    }

    /// <summary>
    /// 字符串转换为 double
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static double GetDoubleFromString(string str)
    {
        double d = -1;
        if (double.TryParse(str, out d))
            return d;

        return d;
    }

    /// <summary>
    /// 字符串转换为 int
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static int GetIntFromString(string str)
    {
        int i = -1;
        if (int.TryParse(str, out i))
            return i;

        return i;
    }

    /// <summary>
    /// 字符串转换为 uint
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static uint GetUIntFromString(string str)
    {
        uint i = 0;
        if (uint.TryParse(str, out i))
            return i;

        return i;
    }

    /// <summary>
    /// 字符串转换为 long
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static long GetLongFromString(string str)
    {
        long l = -1;
        if (long.TryParse(str, out l))
            return l;

        return l;
    }

    /// <summary>
    /// 字符串转换为 bool
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static bool GetBoolFromString(string str)
    {
        bool b = false;
        if (bool.TryParse(str, out b))
            return b;

        return b;
    }

    /// <summary>
    /// 字符串转换为 char
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static char GetCharFromString(string str)
    {
        char c = char.MinValue;
        if (char.TryParse(str, out c))
            return c;

        return c;
    }

    /// <summary>
    /// 字符串转换为 UInt16
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static UInt16 GetUInt16FromString(string str)
    {
        UInt16 i = 0;
        if (UInt16.TryParse(str, out i))
            return i;

        return i;
    }

    /// <summary>
    /// 字符串转换为 UInt32
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static UInt32 GetUInt32FromString(string str)
    {
        UInt32 i = 0;
        if (UInt32.TryParse(str, out i))
            return i;

        return i;
    }

    /// <summary>
    /// 字符串转换为 UInt32
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static UInt64 GetUInt64FromString(string str)
    {
        UInt64 i = 0;
        if (UInt64.TryParse(str, out i))
            return i;

        return i;
    }

    #endregion
    #region 字符串转换

    /// <summary>
    /// string 转换成 byte[]
    /// </summary>
    /// <param name="str">需要转换的string数据</param>
    /// <returns></returns>
    public static byte[] StringToBytes(string str)
    {
        if (string.IsNullOrEmpty(str))
            return null;

        return Encoding.UTF8.GetBytes(str);
    }

    /// <summary>
    /// byte[] 转换成 string
    /// </summary>
    /// <param name="b">需要转换的byte[]数据</param>
    /// <returns></returns>
    public static string BytesToString(byte[] b)
    {
        if (b == null || b.Length == 0)
            return null;

        return Encoding.UTF8.GetString(b);
    }

    /// <summary>
    /// 从字符串中提取数据 例如 10xxff 返回的就是10
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static int StringToInt(string str)
    {
        if (string.IsNullOrEmpty(str))
            return 0;

        // 正则表达式剔除非数字字符（不包含小数点.） 
        str = Regex.Replace(str, @"[^\d\d]", "");
        // 如果是数字，则转换为int类型 
        if (Regex.IsMatch(str, @"^[+-]?\d*[.]?\d*$"))
        {
            return GetIntFromString(str);
        }

        return 0;
    }

    #endregion

}//end of class
