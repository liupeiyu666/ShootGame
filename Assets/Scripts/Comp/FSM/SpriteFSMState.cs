using UnityEngine;
using System.Collections;
using System.Collections.Generic;
 

/// <summary>
/// 一个状态,如跑步，跳跃，攻击，被攻击，采集等
/// </summary>
public abstract class SpriteFSMState{
	//______________Property____________________
	/// <summary>
    /// stateID
	/// </summary>
	public int ID;

	///<summary>持有自己master的引用，对外发出事件，切换状态用</summary>
	protected SpriteFSM fsm ;

	///<summary>持被外部注入，方便操作sprite中各个部件的状态</summary>
	protected TSprite sprite;


	///<summary>安全计时器--不应该放在基类里面</summary>
	protected SafeTimer safeTimer=new SafeTimer();

	/// <summary>
	/// 通过id获取状态
	/// </summary>
	public  SpriteFSMState (){

	}


	/// <summary>
	/// 初始化函数
	/// </summary>
	/// <param name="p_fsm">状态机</param>
	/// <param name="p_baseSprite">状态服务的sprite</param>
	public virtual void Initialize(SpriteFSM p_fsm,TSprite p_baseSprite){
		fsm=p_fsm;
		sprite=p_baseSprite;
	}

	///<summary>被添加到状态机以后， 子类请覆写</summary>
	public virtual void OnRegister (){}

	///<summary>
	/// ##进入状态，进入新状态时会被调用,子类请覆写
	///</summary>
	///<param name="info">在进入一个状态的时候被传入的参数，比如如果是进入移动状态，则要传入移动目的地</param>
	public virtual void Enter (System.Object info=null){}

	///<summary>##执行状态， 会每帧调用,子类请覆写</summary>
	public virtual void Update (){}

//    ///<summary>##执行状态， 会每个Fix帧调用,子类请覆写</summary>
//    public virtual void FixedUpdate() { }

	///<summary>##退出状态, 切换状态时会被调用,子类请覆写
	/// 注意：状态机退出时不能执行导致状态转换的逻辑！！！否则会状态递归！！
	/// </summary>
	public virtual void Exit (int nextStateID){}
}