using UnityEngine;
using System.Collections;

/// <summary>
/// bodyctrl animator 接受事件帧对象
/// @author Wade
/// @date 2016.8.10
/// 这个是挂在 bodyctrlTrm上的
/// </summary>
public class BodyAnimEvent : MonoBehaviour {
	/// <summary>
	/// 这里直接持有 动作事件帧 组件
	/// 由组件去做事情
	/// </summary>
	public AnimEventComp animEventComp;
	public void OnStartEffect(AnimationEvent ev){
		animEventComp.OnStartEffect(ev);
	}
	public void OnStartSound(AnimationEvent ev){
		animEventComp.OnStartSound(ev);
    }
    public void CallBreath()
    {
        //animEventComp.OnCallBreath();
    }
}
