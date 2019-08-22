// Author:kangkai
using UnityEngine;
using System.Collections;

/// <summary>
/// 发送事件用的通用类，用于被某些自制MonoBehaivior子类继承，使可以对外发送事件
/// 不实用unity原生事件机制是由于其效率低不安全
/// Panle在用这些
/// </summary>
public class EventDispatcherMonoBehaviour:MonoBehaviour
{
	/// <summary>
	/// 事件处理函数需要按此代理声明格式实现
	/// </summary>
	/// <param name="sender">事件发送者</param>
	/// <param name="data">事件携带的参数</param>
	public delegate void OnXEventDelegate (System.Object sender,XEventArgs data);

	/// <summary>
	/// 用法
	/// 添加事件
	/// eventListner+=OnXXXEvent;
	/// 删除事件
	/// eventListner-=OnXXXEvent;
	/// 
	/// void OnXXXEvent(System.object sender,XEventArgs ea){
	/// }
	/// </summary>
	public event OnXEventDelegate eventListener;

	/// <summary>
	/// 派发事件
	/// </summary>
	/// <param name="eventName">事件名称</param>
	/// <param name="data">附加数据,默认为null</param>
	public void dispatchEvent (string eventName, System.Object data=null)
	{
		//Debug.Log("EventDispatcherMonoBehaviour.dispatchEvent goName:"+gameObject.name+" eid:"+eventName);
		if (eventListener != null) {
			eventListener (this, new XEventArgs (eventName, data));//注意这里每次都有new XEventArges的开销
		} else {
			//Debug.LogError ("!!!CantSendEvent,Listerner is nul"+eventName);
		}
	}	
}
	

