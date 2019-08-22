using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class CastSkillState:BaseState
{
    public class  EnterParams
    {
        
    }
    public override void Enter(object info = null)
    {
        base.Enter(info);
        //1.播放动作，只控制上半身
        //2.播放特效
    }

    public override void Update()
    {
        base.Update();
    }

    public override void Exit(int nextStateID)
    {
        base.Exit(nextStateID);
    }
}
