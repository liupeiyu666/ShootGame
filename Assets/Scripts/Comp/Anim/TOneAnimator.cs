using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System; 
/// <summary>
/// 20160309 kk改进,封装animator的使用，方便调用
/// 注意当goRoot.bodyAnimator,goRoot.moutnAnimator被销毁时，动画指针也要随之销毁
/// 例如player.anim.bodyTAnimator.Initialize()来初始化，则也要同时调用Clear()来清理指针
/// </summary>
public class TOneAnimator
{

	//对应的挂件的type,比如翅膀，动画武器等（一般就是翅膀了，带动画的挂件不多
	//注意对于body和mount的动画，此值无效
	public string attachPartType=null;//String.Empty;

	//动作映射，把外部调用的AnimName, 映射成具体的clipName.这个是为了
	Func<string,string> callback_GetClipNameFromAnimName;

	//
	Animator _animator;

	//Add Lpy 2016.5.3 为了不破坏原来的数据结构，新建一个字典用来存储动画时长。
	Dictionary<string, float> _clipLengthDict = new Dictionary<string, float> ();

	public void  Initialize (string p_attachPartType,Animator p_animator,Func<string,string> p_callback_GetClipNameFromAnimName)
	{
		//Debug.Log ("TOneAnimator.Initialize");
		attachPartType=p_attachPartType;

		_animator = p_animator;

		if (_animator == null) {
			Debug.Log ("!!!TOneAnimator.Initialize 某个animator是null");
			return;
		}

		//动作映射
		callback_GetClipNameFromAnimName=p_callback_GetClipNameFromAnimName;
		AnimationClip[] clips = _animator.runtimeAnimatorController.animationClips;
		int len = clips.Length;
		for (int i = 0; i<len; i++) {

		    var clip=clips[i];
			_clipLengthDict[clip.name]=clip.length;
		}


	}

	//清理
	public void Clear ()
	{
		_animator = null;

		_clipLengthDict.Clear();
	}

	//从宏观命令映射到具体命令.比如状态机MoveToAimState发出WALK执行，这里会做动作映射,映射为"walk","run"或者“hurtwalk”等
	string ConvertAnimNameToClipName(string animName){
		//设好默认值防止未空
		var clipName = animName;
		//执行动作映射
		if (callback_GetClipNameFromAnimName != null) {
			clipName = callback_GetClipNameFromAnimName (animName);
		}
		return clipName;
	}


	//获取一个clip的时间
	public float GetClipLength (string animName)
	{

		string clipName=ConvertAnimNameToClipName(animName);
		//Add Lpy 2016.5.3 获取新的动画时长. 原有方式不能获得时间,获取返回都是1
		//var clip=GetClip(clipName);
		//if(clip!=null){
		//    return clip.length;
		//}else{
		//    Debug.Log ("TOneAnimator无此动作"+clipName);
		//    return 0;
		//}
		if (_clipLengthDict.ContainsKey (clipName)) {
			return _clipLengthDict [clipName];
		}
		return 0;
	}

	public void ChangeSpeed (float speed)
	{
		if (_animator != null) {
			_animator.speed = speed;
		}
	}

	public void CrossFade (string animName, float durTime = 0.1f, int layer = 0)
	{
		if (_animator == null) {
			//Debug.Log ("TOneAnimator.CrossFade Animator已销毁");
			return;
		}

		//执行动作映射
		string clipName=ConvertAnimNameToClipName(animName);

		_animator.CrossFade (clipName, durTime, layer, 0);
//		//强制切换状态（translation时强制切换）
//		_animator.Update (0);
//		if (_animator.GetNextAnimatorStateInfo (0).fullPathHash == 0) {
//			_animator.CrossFade (clipName, durTime, layer, 0);
//		} else {
//			_animator.Play (_animator.GetNextAnimatorStateInfo (0).fullPathHash);
//			_animator.Update (0);
//			_animator.CrossFade (clipName, durTime, layer, 0);
//		}

	}

	//播放一个动画，从头开始播放,不带动画融合,停在最后一帧.实际项目中极少用到
	public void PlayOnce (string animName)
	{
		if (_animator == null) {
			Debug.Log ("TOneAnimator.PlayOnce _animator为空");
			return;
		}

		//执行动作映射
		string clipName=ConvertAnimNameToClipName(animName);

		_animator.Play (clipName);
	}

	//-停在固定的某帧
	public void StopAt(string animName,float normalizedTime){
		if (_animator == null) {
			Debug.Log ("TOneAnimator.StopAt _animator为空");
			return;
		}

		//执行动作映射
		string clipName=ConvertAnimNameToClipName(animName);

		_animator.Play (clipName,0,normalizedTime);

		//暂停
		ChangeSpeed(0);
	}
    

	#region 上下半身分离

	public void BeginMaskLayer ()
	{
		if (_animator == null) {
			Debug.Log ("TOneAnimator.BeginMaskLayer _animator为空");
			return;
		}
		_animator.SetLayerWeight (1, 1);//1层，权重为1，遮罩生效
	}

	public void ResetMaskLayer ()
	{
		if (_animator == null) {
			Debug.Log ("TOneAnimator.ResetMaskLayer _animator为空");
			return;
		}

		_animator.SetLayerWeight (1, 0);//1层，遮罩关闭

	}

	#endregion

	//-获取全部动作名，调试用的
	public List<string> GetClipNames ()
	{
		List<string> clipNameList = new List<string> ();
		foreach (var k in _clipLengthDict.Keys) {
			clipNameList.Add (k);
		}
		return clipNameList;
	}

	public void Dump(){
	}
}