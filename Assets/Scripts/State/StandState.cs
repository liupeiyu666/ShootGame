using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

/**
 * 0904 kk+
 * 站立状态,这个是播放那个动画，可以根据进入或者退出战斗状态来做好
 */

public class PlayerStandState : BaseState
{
    public PlayerStandState()
    {
        ID = StateEnum.IDLE;
    }

    public override void Enter(System.Object info = null)
    {
       //1.播放站立动作
        anim.CrossFade(AnimEnum.STAND);

    }

    public override void Update()
    {

    }

}


