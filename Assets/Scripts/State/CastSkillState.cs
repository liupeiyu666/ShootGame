using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Effect;
using Tgame.Game.Table;

public class CastSkillState:BaseState
{
    public class  EnterParams
    {
        public int m_skillId;
        public List<TSprite> m_targetList;
    }
    public CastSkillState()
    {
        ID = StateEnum.CAST_SKILL;
    }

    private EnterParams m_params;
    public override void Enter(object info = null)
    {
        base.Enter(info);
        m_params= info as EnterParams;
        Table_skill t_skill = Table_skill.GetPrimary(m_params.m_skillId);
        //1.直接播放特效即可
        EffectShareData t_data = new EffectShareData();
        t_data.m_bornTrans = sprite.GetBindPos(t_skill.bind_pos);
        EffectManager.instance.CreatEffectByController(t_skill.effectid, t_data);
        //完成
        fsm.dispatchEvent(FSMEvent.EVENT_FSM_STATE_COMPLETE);
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
