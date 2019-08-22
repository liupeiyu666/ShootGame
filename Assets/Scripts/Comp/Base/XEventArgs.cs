using System; 

/// <summary>
/// 自定义事件参数类
/// </summary>
public class XEventArgs:EventArgs{
	/// <summary>
	/// 事件名称
	/// </summary>
	public string name;

	/// <summary>
	/// 事件携带数据
	/// </summary>
	public System.Object data;
	
	public XEventArgs (string p_name, System.Object p_data){
		this.data = p_data;
		this.name = p_name;
	}
}