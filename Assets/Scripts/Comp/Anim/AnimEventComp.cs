using UnityEngine;

using System; 
using System.Collections;

/// <summary>
/// 动作事件帧组件	-	放在引擎里.因为基本保持不可变!!!
/// @author Wade
/// @date 2016.8.10
/// 有隐患:
/// 	如果之前技能没有播放完，又播其它技能。此时targetTrm也修改了，就会造成问题
/// 备注：
/// 	如果未来做一个技能：
/// 		把2个敌人绑在一起。类中可以添加一个srcTrm
/// 
/// -----
/// 事件方法：
/// 	播放动作前，调用SetAnimEventInfo();
/// 	OnStartEffect()中调用DoCastSkill()方法
/// 采用tticker：
/// 	更改动作前，调用SetAnimEventInfo();
/// 	在更改动作后，调用DelaySkillEffect()方法
/// 
/// -----
/// </summary>
public class AnimEventComp : SpriteComp {

	#region 基本


	public override void OnAdded(){
		
	}

	public override void OnRemoved(){
		
	}
	#endregion

	Transform srcTrm;
	/// <summary>
	/// 目标对象(这里要先记录，因为内部无法得到外部对象，并且外部对象很可能修改)
	/// </summary>
	Transform targetTrm;
	/// <summary>
	/// 鼠标所在的3d位置   //获取要显示的位置。 当AtomEffect src为point的时候，就会根据这个值做显示
	/// </summary>
	Vector3 mouse3dPos;
	/// <summary>
	/// 技能释放震动
	/// </summary>
	bool isShake;
	/// <summary>
	/// 技能缩放
	/// </summary>
	Vector3 scale;

	/// <summary>
	/// 特效索引
	/// </summary>
	int effectIndex=0;
	/// <summary>
	/// 声音索引
	/// </summary>
	int soundIndex=0;
	/// <summary>
	/// 动作关联特效id组
	/// </summary>
	string[] effectIdArr;
	/// <summary>
	/// 动作关联声音id组
	/// </summary>
	string[] soundIdArr;
	/// <summary>
	/// 技能效果描述字符串
	/// </summary>
	string modalityData;

	//
	Action<string> callbackFuncPlayAnim=null;
	//SkillEffManagerV2.SkillEffParams skillEffParam=null;

	#region 公开方法
	//事件帧 方式：
	/// <summary>
	/// bodyctrl加载完成，后设置,注意这个不能在OnAdded()里写
	/// 	给animator(bodyctrl)添加监听事件帧的脚本
	/// </summary>
	public void Setup(){

		var bodyAnimEvent=UGEUtils.GetOrAddComponent<BodyAnimEvent>( goRoot.bodyctrlTrm.gameObject);

//		var bodyAnimEvent = goRoot.bodyctrlTrm.GetComponent<BodyAnimEvent>();
//		if(bodyAnimEvent == null){
//			bodyAnimEvent = goRoot.bodyctrlTrm.gameObject.AddComponent<BodyAnimEvent>();
//		}
		bodyAnimEvent.animEventComp = this;
	}
	
	//采用tticker 方式：
	/// <summary>
	/// 采用tticker释放技能
	/// "SetAnimEventInfo"一样需要设置
	///		因为走的是update，出现的时间会出现误差
	/// 	如果中途技能被打断了，特效还会继续播。或者用代码删除tticker技能等待。否则会造成也继续播
	/// </summary>
	/// <param name="delayTimer">Delay timer.</param>
	public void DelaySkillEffect(float delayTimer){
		DoCastSkill( delayTimer );
	}
	/// <summary>
	/// 具体的显示技能效果方法
	/// </summary>
	/// <param name="delayTimer">Delay timer.</param>
	void DoCastSkill(float delayTimer){
	
	}
	#endregion

	#region 事件帧方法
	public void OnStartEffect(AnimationEvent ev){
//		DoCastSkill(0f);				//采用事件帧方式
		Debug.Log("OnStartEffect");
	}
	public void OnStartSound(AnimationEvent ev){
		
	}
    #endregion
    #region Comp get set
    //对视图的快捷引用,显示对象层级快捷引用
    GORootComp _goRoot;
    GORootComp goRoot
    {
		get{
			if(_goRoot==null){
				//加的安全检查
				//!!这里会影响性能，如果是物goRootComp组件，则胡会每次调用都取
				var comp=GetSpriteComp<GORootComp>();//.NAME) as GORootComp;
				if(comp==null){
					return null;
				}
				
				_goRoot= comp;
			}
			return _goRoot;
		}
	}
	#endregion
}
