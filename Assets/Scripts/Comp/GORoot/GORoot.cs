using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;


///<summary>
/// 一个精灵的GameObject游戏对象的层级结构的快捷引用(嵌套层级结构)
///</summary>
public class GORoot:TEventDispatcher
{

	//引擎内部通信机制，删除body,mount,wing的时候，通知AnimComp要被通知到，以便进行卸载TOneAnimator内对animator的高速缓存
	internal const string EVENT_REMOVE_BODY = "EVENT_REMOVE_BODY";
	internal const string EVENT_REMOVE_ATTACHPART = "EVENT_REMOVE_ATTACHPART";

	/// <summary>
	/// 精灵的各个部位构成,
	/// zlf 20160926 修改过 GO 改 trm
	/// </summary>
	public Transform rootTrm,

		bodyContainerTrm,
		bodyctrlTrm,
		hudTrm;

    /// <summary>
    /// 身上挂件的go的容器
    /// </summary>
    Dictionary<string,GameObject> attachPartDict=new Dictionary<string,GameObject>();

	//注意，以下原生animation组件的引用，供AnimComp操控动画用

	public Animator 
		bodyAnimator;

	//挂件的动画的指针,如果没有，则为空，空也要记录
	Dictionary<string,Animator> attachPartAnimatorDict=new Dictionary<string,Animator>();

	//清理所有内容			//Wade 10.26+
	internal void ClearAll(){
		RemoveBody();
		CleanAttachParts();
	}

	/// <summary>
	/// 添加body,如果是player,加的是bone_anim,如果是怪，加的是全套
	/// </summary>
	/// <param name="p_go">加载到的精灵主体gameObject</param>
	/// <returns></returns>
	public void AddBody (GameObject p_go)
	{
		
		bodyContainerTrm = p_go.transform;

		bodyContainerTrm.gameObject.SetActive (true);//这个是必须的，因为从PrefabPool里得到的东西,之前放入prefabPool的时候active=false了
		
		//把新加载到的挂在原来的go下面
		UGEUtils.SetParent (bodyContainerTrm, rootTrm);
      
         //动画组件
         bodyctrlTrm = bodyContainerTrm.GetChild(0);

		bodyAnimator = bodyctrlTrm.GetComponent<Animator> ();

		//设置hud
		hudTrm = bodyctrlTrm.Find("bone_hud");
	}

    


/// <summary>
/// 移除body
/// </summary>
public void RemoveBody ()
	{
		if (bodyContainerTrm != null) {
			//GameObject.Destroy (bodyContainerTrm.gameObject);
//			UGEUtils.DestroyGameObject(bodyContainerTrm.gameObject);	//Wade 10.25+让smartloader管理
		}
		
		bodyContainerTrm = null;

		bodyctrlTrm = null;
		
		hudTrm = null;

		bodyAnimator = null;

		//对外发事件，主要是通知AnimComp
		this.dispatchEvent (EVENT_REMOVE_BODY, null);
	}


	#region 挂件
	public void AddAttachPart(string type,GameObject go,bool hasAnimator){
		//1,添加go
		if(attachPartDict.ContainsKey(type)){
			Debug.LogError("!!!!GORoot.AddAttachPart 重复名字"+type);
			return;
		}

		go.name="AttachPart_"+type;

		attachPartDict.Add(type,go);

		UGEUtils.SetParent(go.transform,rootTrm);

		//2,提取animator并存储
		//动画挂接位置参见类前说明
		Animator animator=null;
		if(hasAnimator){//如果需要使用animator,需要go的contentGO
			var trm=go.transform;

			//!!这里没有加安全判断
			var a=trm.GetChild(0);
			if(a!=null){
				var b=a.GetChild(0);
				if(b!=null){
					animator=b.GetComponent<Animator>();
				}
			}


			//animator=trm.GetChild(0).GetChild(0).GetComponent<Animator>();
		}
		if(attachPartAnimatorDict.ContainsKey(type)){
			attachPartAnimatorDict[type]=animator;
		}else{
			attachPartAnimatorDict.Add(type,animator);//这个可能是null
		}

	}

	public void RemoveAttachPart(string type){
		//1,go销毁
		GameObject go;
		if(attachPartDict.TryGetValue(type,out go)){
//			UGEUtils.DestroyGameObject(go);					//Wade 10.25+让smartloader管理
			go = null;
			attachPartDict.Remove(type);
		}
		//2,animator释放
		if(attachPartAnimatorDict.ContainsKey(type)){
			attachPartAnimatorDict.Remove(type);
		}

		//--发送事件，主要用于动画清理
		this.dispatchEvent (EVENT_REMOVE_ATTACHPART, type);
	}

	public GameObject GetAttachPart(string type){
		GameObject go=null;
		attachPartDict.TryGetValue(type,out go);
		if(go==null){
			Debug.LogError("!!!!GORoot.GetAttachPart失败"+type);
		}

		return go;
	}

	public void CleanAttachParts(){
		
		foreach(var kv in attachPartDict){

//			UGEUtils.DestroyGameObject(kv.Value);		//Wade 10.25+让smartloader管理

			//--发送事件，主要用于动画清理,AnimComp内对goRoot.xxAnimator的指针
			this.dispatchEvent (EVENT_REMOVE_ATTACHPART, kv.Key);
		}

		attachPartDict.Clear();//挂件go清理

		attachPartAnimatorDict.Clear();//挂件animator清理
	}


	public Animator GetAttachPartAnimator(string type){
		Animator animator=null;
		attachPartAnimatorDict.TryGetValue(type,out animator);

		return animator;
	}
	#endregion
}//end of class