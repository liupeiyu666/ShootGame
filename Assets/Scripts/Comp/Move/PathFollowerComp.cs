 
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System; 


 /**
 * 沿着路径跑的组件,
 * 负责动态时的位置和旋转
 */ 
public class PathFollowerComp :SpriteComp
{	
	public const string EVENTTYPE_ARRIVED="EVENTTYPE_ARRIVED";

	#region 基本


	public override void OnAdded(){
		//OnUpdate在StarFollow()时启动
		//UGEEnter.sprsTimeMgr.AddUpdate(OnUpdate);
	}

	public override void OnRemoved(){
		//UGEEnter.sprsTimeMgr.RemoveUpdate(OnUpdate);
        TTicker.RemoveUpdate(OnUpdate);
	}
	#endregion

//	//是否执行更新，因为RemoveUpdate消耗还是比较大的
//	bool enableUpdate=false;

	//************对外发出事件回调***************

	
    /// <summary>
    /// 路径点列表,public是为了给bigmapWindow用的
	/// 注意这个是每3个元素为一组xyz,为了优化性能而做
    /// </summary>
	public List<int> nodeList;//
	private int _nodeIdx;

	//寻径用的，另个相邻节点的起始和结束位置
	private Vector3 _stepStartPosi ;
	private Vector3 _stepEndPosi ;
	private TTimer _stepTimer = new TTimer ();//跑向目标节点需要的时间

	//默认的移动速度
	private float moveSpeed=4.8f;

	//!!时间补偿，在第一帧用,=当前时间-收到消息的时间+本客户端服务器网络延迟+消息发出者与服务器的网络延迟
	private float correctDeltaTime=0;



    /// <summary>
    /// 启动一次寻径.注意THero主动移动的时候，没有correctDeltaTime
    /// </summary>
    /// <param name="p_nodeList">路径节点</param>
    /// <param name="p_moveSpeed">移动速度</param>
	public void StartFollow (List<int> p_nodeList,float p_moveSpeed,float p_correctDeltaTime=0){

		nodeList = p_nodeList;
		moveSpeed=p_moveSpeed;

		correctDeltaTime=p_correctDeltaTime;

		//路径为空
		if (nodeList.Count < 3) {
			return;
		}
		
		//!!潜在问题，如果拐弯处有障碍物，则人可能处于有障碍物的位置,此时寻径会失败，需要其他机制配合
		//不加这个，跑步的时候会有顿卡
		if (nodeList.Count > 3) {//直接跑向下一目标要
			_nodeIdx = 3;
		} else {
			_nodeIdx = 0;
		}
		
		//!!注意!!必须清零！可能导致多段路径不同步.非常细节的问题,因为TTimer会越界curtime>fTime
		_stepTimer.SetTime(0);

		//不加这个，人不会启动跑动
		NextNode ();
        TTicker.AddUpdate(OnUpdate);
     
	}
	
    /// <summary>
    /// 清除寻径
    /// </summary>
	public void StopFollow (){
		nodeList=null;
		_nodeIdx = 0;
        TTicker.RemoveUpdate(OnUpdate);
	}

	//TODO 这里有缺陷,不动的时候收到ChangeMoveSpeed()的时候，还会导致问tUpdate里重新设置一下路径
	//--0919 jhy 寻路过程中变速问题造成拉扯的修复
	bool bChangeSpeedWhenNode = false;

	//TODO YH 关于中途变速需要考虑清楚
    /// <summary>
    /// 改变移动速度
    /// </summary>
    /// <param name="p_moveSpeed">新的移动速度</param>
	public void ChangeMoveSpeed(float p_moveSpeed){
		if(moveSpeed!=p_moveSpeed){
			moveSpeed=p_moveSpeed;
			bChangeSpeedWhenNode = true;
		}
	}

    /// <summary>
    /// 获得移动速度
    /// </summary>
    /// <returns>移动速度</returns>
	public float GetMoveSpeed(){
		return moveSpeed;
	}
	
	void OnUpdate (){

		float deltaTime=Time.deltaTime+correctDeltaTime;
		correctDeltaTime=0;//时间用完就清零

		_stepTimer.Update (deltaTime);
		
		//插值设置好位置,否则都是走到了不稳定位置，潜藏了危险,xoz插值，y会由spacialComp来计算
		spacial.worldPosi = Vector3.Lerp (_stepStartPosi, _stepEndPosi, _stepTimer.GetPercent());
//		var pos = Vector3.Lerp (_stepStartPosi, _stepEndPosi, _stepTimer.GetPercent());
//		spacial.SetPos(pos.x,pos.z);

		if (_stepTimer.isReady ()) {//到达一个节点
			if (!IsPathEnd ()) {//没到达最终目标（本路段）
				_nodeIdx+=3;
				NextNode ();
			}else{ //到达目标

				spacial.worldPosi=_stepEndPosi;//1202 kk+ 正常到达目标则位置校正为最终位置
				this.OnReachEndOfPath ();
			}
		} else {
			//--0919 jhy 寻路过程中变速问题造成拉扯的修复
			if(bChangeSpeedWhenNode)
			{
				NextNode ();//变速，重新计算时间
				bChangeSpeedWhenNode = false;
			}
			
			//--
			//设置角度(转身速度),这个消耗大，慎用.这个是跑步中逐步转向
			spacial.FaceTo(_stepEndPosi,true);
		}

		//-
		//Debug.Log("-------pathFollower.OnUpdate",cachedTrm.name,moveSpeed,spacial.worldPosi,Time.realtimeSinceStartup);
	}
	
	private void NextNode (){
		
		_stepStartPosi= spacial.worldPosi;
		_stepEndPosi = new Vector3 (nodeList[_nodeIdx]  +0.5f, 0, nodeList[_nodeIdx+2]  +0.5f);
		
		_stepStartPosi.y = 0;
		_stepEndPosi.y = 0;

		//!!注意这里必须是xoz平面的距离
		float dis = VectorUtils.GetXOZDistance(_stepStartPosi,_stepEndPosi);

		float time = UGEUtils.SafeDivide(dis,moveSpeed);

		if (time == 0) {
			return;
		}
		
		//12.05 KK+ 
		//!!非常重要的时间偏移设置，因为StepTimer使用Time.deltaTime驱动的，
		//又使用GetPercent获得时间，这样可以导致时间溢出（超过100%了）,在长路径情况下，导致客户端比服务器慢
		//原理是这一段跑慢了，下一段就跑快点找回来
		float overflowTime =_stepTimer.curtime - _stepTimer.fTime;//??
		if(overflowTime>0){
			time-=overflowTime;
		}
		
		_stepTimer.SetTime (time);		
	}
	
	// 检查路径是否完成
	bool IsPathEnd (){
		if (nodeList == null){
			return true;
		}

		if (_nodeIdx >= nodeList.Count - 3){
			return true;
		}

		return false;
	}
	

    /// <summary>
     	//是否到达了指定格子,如果跟目标格子距离小于0.25,就认为到达了
        //这个用来当退出BastNaviState时，检查是否需要发出刹车指令
    /// </summary>
    /// <returns></returns>
	public bool IsReachPathEnd(){
		//安全检查
		if(nodeList==null || nodeList.Count==0){
			//Debug.LogError("PathFollower.IsArrivePathEnd，路径非法");
			//这种情况是自然到达了目标，heroRunToAim.OnArrived,路径会被清理
			return true;
		}

		bool isNear=false;
        if(nodeList.Count>3)
        {
            var endPosi = new Vector3(nodeList[nodeList.Count-3]  + 0.5f, 0, nodeList[nodeList.Count-1] + 0.5f);
			isNear = VectorUtils.GetXOZDistance(spacial.worldPosi, endPosi) < 0.25f;
        }

		return isNear;
	}

	//到达了此段路径的终点
	private void OnReachEndOfPath (){
		//停止导航，释放资源
		this.StopFollow ();
			
		//对外发送事件，到达目标
		this.dispatchEvent(EVENTTYPE_ARRIVED);

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
}//end of class