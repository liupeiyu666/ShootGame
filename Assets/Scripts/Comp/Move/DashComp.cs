using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/**
 * 冲刺的,被击退击
 *
 *这个不要处理朝向了，因为可能是始终面向怪向与怪连线的反方向跳，比如斗士距离怪特别近的时候的向后跳
 */ 
/// </summary>
public class DashComp : SpriteComp
{
    /// <summary>
    /// 冲刺完成，派发事件
    /// </summary>
	public const string EVENT_COMPLETE="EVENT_COMPLETE";

	#region 基本


	public override void OnAdded(){
	}

	public override void OnRemoved(){
       // TTicker.RemoveUpdate(OnUpdate);
	}
	#endregion

	SafeTimer dashTimer =new SafeTimer(0);

	Vector3 startPosi;

	Vector3 endPosi;

	float jumpHeight;



    /// <summary>
    /// 	处理闪现，快速移动到目标,飞行
    //  time单位是ms
    /// </summary>
    /// <param name="npos">结束位置</param>
    /// <param name="time">时间</param>
    /// <param name="p_jumpHeight">高度</param>
	public void StartDash(Vector3 npos,float time,float p_jumpHeight)
	{
		startPosi=spacial.worldPosi;
		endPosi = npos;
	
		dashTimer.Start(time);
		dashTimer.Reset();
		
		jumpHeight=p_jumpHeight;
		
		//如果时间是0，则要直接设置,在update的时候会直接返回timer.IsReady()=true
		if(time==0){
			spacial.worldPosi=npos;
		}

		spacial.isFlying=true;
      //  TTicker.AddUpdate(OnUpdate);
	}

	//这个被状态机状态调度，保证安全的update
	// //由于使用状态GState内驱动update， 不需要Stop()和event机制通知了,所以就不需要stop之类的了，简化结构设计
	void OnUpdate(){
		//if(dashTimer.isReady()==false)
		//{	
		//	dashTimer.Update(Time.deltaTime);
			
		//	var percent=dashTimer.GetPercent();
		//	//Debug.Log ("dc",percent);
			
		//	var posi= Vector3.Lerp(startPosi, endPosi , percent);
		//	//0115 修改bug，以前是startPosi.y,导致在有高度差的地表移动时，会扎进土里，慢慢移动上来
		//	posi.y = posi.y + Mathf.Sin(percent * Mathf.PI ) * jumpHeight;//在世界的总高度
			
		//	spacial.worldPosi=posi;

		//}else{
		//	//TODO Wade 2014.12.9 + 引发bug - 注释掉
		//	//this.enabled=false;
		//	//UGEEnter.sprsTimeMgr.RemoveUpdate(OnUpdate);
  //          TTicker.RemoveUpdate(OnUpdate);

		//	this.dispatchEvent(EVENT_COMPLETE);

		//}
	}

	//对空间位置信息的快捷引用
	SpacialComp _spacial;
	SpacialComp spacial{
		get{
			if(_spacial==null){
				_spacial=GetSpriteComp<SpacialComp>();//.NAME) as SpacialComp;
			}
			return _spacial;
		}
	}
}