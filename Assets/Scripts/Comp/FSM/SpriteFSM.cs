using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System; 
using System.Reflection;

/// <summary>
/// 状态机控制类
/// </summary>
public class SpriteFSM : SpriteComp
{

	#region 基本


	public override void OnAdded ()
	{
		//UGEEnter.sprsTimeMgr.AddUpdate (OnUpdate);
        TTicker.AddUpdate(OnUpdate);
	}

	public override void OnRemoved ()
	{
        TTicker.RemoveUpdate(OnUpdate);
		//UGEEnter.sprsTimeMgr.RemoveUpdate (OnUpdate);
		ClearState ();		//Wade 10.25+
	}

	#endregion

	//___________________Property_____________________
	//--状态机内部的状态
	Dictionary<int,SpriteFSMState> stateDict=new Dictionary<int, SpriteFSMState>();

	/// <summary>
	/// 状态机当前状态
	/// </summary>
	protected SpriteFSMState currState = null;


  
	
	/// <summary>
	/// 开发时看状态用的, 是否打印出状态转换
	/// </summary>
	public bool debugSwitchState = false;

	/// <summary>
	/// 初始化状态机
	/// </summary>
	/// <param name="p_sprite">状态机作用的对象</param>
	public void Setup ()
	{
		
	}

	void OnUpdate ()
	{
		if (currState != null) {
			currState.Update ();
		}
	}

	//Wade 10.25+
	internal void ClearState ()
	{
		if (currState != null) {
			currState.Exit (-1);
			currState = null;
		}
	}

	/// <summary>
	/// 添加状态
	/// </summary>
	/// <param name="s">具体状态</param>
	public void AddState (SpriteFSMState  s)
	{

		if (FindState (s.ID) != null) {
			throw new UnityException ("!!FSM.AddState已经有此ID的状态，禁止重复添加!!hasStateID:" + s.ID);
		}
		//T.p("fsm.AddState",id,s.getID());
		stateDict [s.ID] = s;

		s.Initialize (this, sprite);
		
		s.OnRegister ();//单独放在这里是方便子类覆写
	}

	/// <summary>
	/// 查找状态
	/// </summary>
	/// <param name="s">状态id</param>
	public SpriteFSMState FindState (int stateID)
	{
		if (stateDict.ContainsKey (stateID)) {
			return stateDict [stateID];
		}
		return null;
	}

	/// <summary>
	/// 删除状态
	/// </summary>
	/// <param name="s">状态id</param>
	public void RemoveState (int stateID)
	{
		if (stateDict.ContainsKey (stateID)) {
			stateDict.Remove (stateID);
		}
	}

	/// <summary>
	/// 状态机切换状态
	/// </summary>
	/// <param name="stateID">状态id</param>
	/// <param name="info">状态机携带参数.传入状态的Enter()方法</param>
	public void SwitchState (int stateID, System.Object info = null)
	{
		if (debugSwitchState) {
			Debug.Log(sprite + ".FSM.SwitchState:"+ (currState == null ? 0 : currState.ID)+ "->"+ stateID+ "time:" + Time.realtimeSinceStartup);
		}
        //Debug.LogError(currState.ID+ "--------------------"+ currState);
		if (currState != null) {
            //Debug.LogError(currState.ID + "--------------------" + currState);
            //T.p("退出State:"+curState.getID());
            currState.Exit (stateID);
		}

		SpriteFSMState nextState = FindState (stateID);
		if (nextState != null) {
			currState = nextState;

			currState.Enter (info);
		} else {
			Debug.LogError("!!FSM.SwitchState,nextState=null " + stateID + " " + sprite);
		}
	}

	/// <summary>
	/// 取得当前的状态，这个挺有用的，用来判断当前的状态
	/// </summary>
	/// <returns>当前的状态</returns>
	public SpriteFSMState GetCurrState ()
	{
		return currState;
	}

	/// <summary>
	/// 取得当前的状态id
	/// </summary>
	/// <returns>当前的状态id</returns>
	public int GetCurrStateID ()
	{
		if (currState != null) {
			return currState.ID;
		} else {
			return 0;
		}
	}

	/// <summary>
	/// 是否当前状态
	/// </summary>
	/// <param name="stateID">=状态id</param>
	/// <returns>是则返回true</returns>
	public bool IsCurrStateID (int stateID)
	{
		if (currState == null) {
			return false;
		} else {
			return currState.ID == stateID;
		}
	}
}