/// <summary>
/// 发送事件用的通用类，用于被某些自制MonoBehaivior子类继承，使可以对外发送事件
/// 不使用unity原生事件机制是由于其效率低不安全
/// </summary>
public class TEventDispatcher
{
	//通用事件名
    /// <summary>
    /// 初始化完毕
    /// </summary>
	public const string EVENTTYPE_COMPLETE="EVENTTYPE_COMPLETE";

	/// <summary>
	/// 事件处理函数需要按此代理声明格式实现
	/// </summary>
	/// <param name="sender">事件发送者</param>
	public delegate void OnXEventDelegate (System.Object sender,XEventArgs data);

	/// <summary>
	/// 用法
	/// 添加事件
	/// eventListner+=OnXXXEvent;
	/// 删除事件		//TODO All+ Add Wade 1215+注意要删除!!!
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
	/// <param name="data">时间携带的数据</param>
	public void dispatchEvent (string eventName, System.Object data=null){
		if (eventListener != null) {
			eventListener (this, new XEventArgs(eventName, data));
		}
	}

	/// <summary>
	/// 清理所有时间		//Add Wade 1215+去掉引用关系
	/// </summary>
	public void RemoveAllEvent(){
		eventListener = null;
	}
}