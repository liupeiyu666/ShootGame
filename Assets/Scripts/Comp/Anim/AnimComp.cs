using UnityEngine;
using System; 
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// <para>依赖GORootComp</para>
/// <para>动画组件，专门负责动画控制的</para>
/// 特性 1:透明加载
/// 		模型没加载到的时候，此组件会记录动作名，等加载到之后，调AnimComp.Refresh()动作会被刷新
/// 特性 2:一控三动画
/// 	支持动画同步(精灵的多个动画同步)，
/// 	比如:人不骑马时播放走路动画，人骑马的时候，人播放马上坐着的动画，马播放走路动画.
/// 	再比如有武器时的跑步动画和空手跑是不一样的，这个内部会做处理,外部只管调用即可
/// 特性 3:帧冻结
/// 	定住动画播放一段时间
/// 特性 4：动作锁定
/// 	支持播放某个动作的时候，锁定一段时间，会屏蔽掉某些动作，带锁定完毕后继续播放
/// 
/// 注意命名:
/// AnimName是表示宏观动作，被高层逻辑使用
/// ClipName是真实的动作名，是Animation里的动作名
/// </summary>
public class AnimComp : SpriteComp
{
	#region 基本


	public override void OnAdded(){

	}

	public override void OnRemoved(){
		//帧冻结
		if(callbackID_UnFrozeFrame!=-1){
			callbackID_UnFrozeFrame=-1;
			//UGEEnter.sprsTimeMgr.RemoveCallback(UnFrozeFrame); 
            TTicker.RemoveCallback(UnFrozeFrame);
		}
	}
	#endregion

	//-封装好的TOneAnimator,是对body和mount的Animator的封装，方便使用
	public TOneAnimator bodyTAnimator = new TOneAnimator (); //对外暴露是为了动作融合、上下半身分离用
	#region 各个动画部件的初始化和获取、清理

	public void InitBody (Animator p_animator,Func<string,string> p_callback_GetClipNameFromAnimName){
		bodyTAnimator.Initialize("body",p_animator,p_callback_GetClipNameFromAnimName);//-1,只是占位符
	}

	public void ClearBody (){
		bodyTAnimator.Clear();
	}

	#endregion

	#region 透明加载和一控多(上层逻辑不关系是否加载到了动画，只管下命令即可)

	//记录当前的数据，等加载到了真正的素材，继续播放
	//当动画真正被加载到之前，收到了播放动画的命令，则要记录下来，待素材真正加载到之后，继续播放之前动作
	class AnimCmd
	{
		public CmdType cmdType;
		//1是PlayOnce,2是CrossFade,3是StopAtEnd
		public string animName;
		//外部动画名(高层下达的)
		public float crossFadeTime = 0.3f;
		public float animSpeed = 1;

		public int crossFadeLayer=0;

		//-停留在哪一帧
		public float stopAtNormalizedTime=0;
		//记录正在播放的动画的速度,注意坐骑速度要变
		//public WrapMode animMode;//0309 kk+ 经测试，这个已经没用了
	}

	//AnimCmd里的animType
	enum CmdType
	{
		CrossFade,
		PlayOnce,
		StopAt
	}

	//当前的动作，做透明加载用
	AnimCmd currCmd = new AnimCmd ();

	/// <summary>
	/// 获取当前动画Name
	/// </summary>
	/// <returns>当前动画Name</returns>
	public string GetCurrAnimName ()
	{
		return currCmd.animName == null ? String.Empty : currCmd.animName;
	}

	#endregion

	#region 播放动画

	/// <summary>
	/// 当精灵外显部件发生变化的时候（主体模型加载到，切换武器，切换坐骑，变养等）调用，同步一下动画
	/// 使用注意
	/// 	avatarComp,切换动作完毕后，应该通知外部，继续播放动画
	/// 	mountComp,加载坐骑动作完毕后，也应该通知，调整动画
	/// 	goRoot.SetActive(true)被重新激活后，也要调用这个,否则动画是停止的
	/// 	有换装，坐骑等被加载到了，会导致一个动画重置，对技能释放是有一定影响的，潜在bug
	/// 	注意isImmediately的用法，从坐骑上下来，应该是瞬间把玩家的骑乘动作切换成站立或者跑动(否则出现玩家从地上坐起来的尴尬)
	/// </summary>
	/// <param name="isImmediately">是否立即切换动画，否则有动作融合过渡</param>
	public void Refresh (bool isNow = false)
	{

		////Debug.Log ("AnimComp.Refresh",isImmediately,name,animCmd.cmdType,animCmd.animName);

		if (isNow) {
			currCmd.crossFadeTime = 0;
		}

		//
		ChangeSpeed (currCmd.animSpeed);

		//恢复之前的动画（在没加载到素材的时候设置的动画）

		switch(currCmd.cmdType){
		case CmdType.PlayOnce:
			PlayOnce (currCmd.animName);
			break;
		case  CmdType.CrossFade:
			CrossFade (currCmd.animName,currCmd.crossFadeTime,currCmd.crossFadeLayer);
			break;
		case CmdType.StopAt:
			StopAt(currCmd.animName,currCmd.stopAtNormalizedTime);
			break;
		}


	}

	//改变速度
	public void ChangeSpeed (float playSpeed)
	{

		currCmd.animSpeed = playSpeed;

		bodyTAnimator.ChangeSpeed (playSpeed);
	}

	/// <summary>
	/// 动画切换，有平滑过渡，透明加载，1控3
	/// </summary>
	/// <param name="animName">宏观动作名，用AnimEnum内常量传递.比如AnimEnum.WALK,会被映射成具体动作名，被各个原生动画Animation使用</param>
	/// <param name="mode">循环还是单次</param>
	/// <param name="durTime">动画过渡时间</param>
	/// <param name="playSpeed">动画播放速度</param>
	/// <param name="layer">播放的Layer，用于上下半身分离</param>
	public void CrossFade (string animName, float durTime = 0.1f, int layer = 0)
	{
		////Debug.Log ("AnimComp.CrossFade",name,animName,durTime,"时间:"+Time.realtimeSinceStartup);

		currCmd.cmdType = CmdType.CrossFade;
		currCmd.animName = animName;
		currCmd.crossFadeTime = durTime;
		currCmd.crossFadeLayer=layer;
		//currCmd.animMode=mode;
		//currCmd.animSpeed=playSpeed;

		//1:人动作
		bodyTAnimator.CrossFade (animName, durTime, layer);
	}

	/// <summary>
	/// 动画切换，只播放一次。一般不推荐使用。透明加载，1控3
	/// 这个极少使用，只有Photo在用，一般不推荐使用!推荐使用CrossFade()
	/// </summary>
	/// <param name="animName">AnimEnum名</param>
	/// <param name="playSpeed">动画速度</param>
	/// <param name="mode">单次还是循环</param>
	public void PlayOnce (string animName)
	{
		////Debug.Log (name,"AnimComp.PlayOnce",animName,playSpeed,mode);

		currCmd.cmdType = CmdType.PlayOnce;
		currCmd.animName = animName;
		//currCmd.animSpeed=playSpeed;
		//currCmd.animMode=mode;

		//1:人动作
		bodyTAnimator.PlayOnce (animName);
	}

	public void StopAt(string animName,float normalizeTime){
		////Debug.Log (name,"AnimComp.PlayOnce",animName,playSpeed,mode);

		currCmd.cmdType = CmdType.StopAt;
		currCmd.animName = animName;
		//currCmd.animSpeed=playSpeed;
		//currCmd.animMode=mode;

		//1:人动作
		bodyTAnimator.StopAt (animName,normalizeTime);
	}

	#endregion

	#region 获取动画时间

	public float GetBodyClipLength (string animName)
	{
		return bodyTAnimator.GetClipLength (animName);
	}

	#endregion

	#region 帧冻结

	///<summary>帧冻结</summary>	
	public void FrozeFrame (float frozeTime)
	{
		//		//Debug.Log (name+":FrozeFrame"+frozeTime+":"+currClipName);
		bodyTAnimator.ChangeSpeed (0);
		//Invoke ("UnFrozeFrame", frozeTime);
        callbackID_UnFrozeFrame =/*UGEEnter.sprsTimeMgr.AddDelayCallback(UnFrozeFrame,frozeTime)*/TTicker.AddDelayCallback(frozeTime, UnFrozeFrame);
	}

	///<summary>清理帧冻结</summary>	
	int callbackID_UnFrozeFrame=-1;
	public void UnFrozeFrame ()
	{
		callbackID_UnFrozeFrame=-1;

		bodyTAnimator.ChangeSpeed (currCmd.animSpeed);
        TTicker.RemoveCallback(UnFrozeFrame);
	}

	#endregion

	//-输出调试信息
	public void Dump ()
	{
		//Debug.Log ("AnimComp.bodyAnimation");
		bodyTAnimator.Dump ();
	}
}