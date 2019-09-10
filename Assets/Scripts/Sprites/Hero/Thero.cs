using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BehaviorDesigner.Runtime.Tasks;
using Engine.Effect;
using Tgame.Game.Table;
using UnityEngine;

/// <summary>
/// 对外接口
/// </summary>
public class THero:TSprite
{
    #region
    //伪单例，因为可以保证这个实例只会有一个
    public static THero instance;

    #endregion
    public override void Startup()
    {
        instance = this;
        //1.加载外显
        GetComp<MainLoaderComp>().Start();
        InitData();
        base.Startup();
    }
    private SpriteFSM fsm;
    private AnimComp animComp;
    private GORootComp goRootComp;
    private MoveComp moveComp;
    protected override void InitComp()
    {
        base.InitComp();
        //1.初始化加载
        AddComp<MainLoaderComp>();
        goRootComp=AddComp<GORootComp>();
        fsm=AddComp<SpriteFSM>();
        animComp=AddComp<AnimComp>();
        moveComp = AddComp<MoveComp>();
        InitCompEvent();
      
        InitFsm();
    }
    /// <summary>
    /// 主要处理各个comp之间的流程
    /// </summary>
    protected void InitCompEvent()
    {
        //1.主体加载完成，开始进行结构设置
        GetComp<MainLoaderComp>().eventListener += (a, b) =>
        {
            goRootComp.Start();
            //2.初始化动画， 由于gorootcomp中不存在异步和多线程所以直接用就可以
            animComp.InitBody(goRootComp.bodyctrlTrm.GetComponent<Animator>(), CB_GetBodyClipName);
        };
       
    }

    #region 状态机

    protected void InitFsm()
    {
        fsm.AddState(new CastSkillState());
        fsm.eventListener += OnFsmEvent;
    }

    private void OnFsmEvent(System.Object sender, XEventArgs data)
    {
        switch (data.name)
        {
            case FSMEvent.EVENT_FSM_STATE_COMPLETE:
                
                break;
        }
    }

    #endregion

    #region 初始化Data数据
    /// <summary>
    /// 初始化角色数据
    /// </summary>
    protected void InitData()
    {
        Table_role t_role = Table_role.GetPrimary(m_data.GetData<CreatData>().role_id);
        OperateData t_operateData=new OperateData();
        t_operateData.m_moveSpeed = t_role.move_speed;
        t_operateData.m_rotatSpeed = t_role.rotate_speed;
        m_data.AddData(t_operateData);
    }


    #endregion
    #region 动作映射
    string CB_GetBodyClipName(string animName)
    {
        return animName;
        //if (goRoot.mountctrlTrm != null)
        //{//有坐骑
        //    return AnimEnum.STAND;//坐骑上的其他动作，都映射成stand
        //}
        //else
        //{//没坐骑
        //    return animName;//使用真实动作
        //}
    }


    #endregion
    #region 对外方法
    /// <summary>
    /// 移动和转向的方法   p_state生效的参数 0表示只有移动生效，1表示只有转向生效  2表示都生效
    /// </summary>
    /// <param name="p_moveDir"></param>
    /// <param name="p_rotateDir"></param>
    public void MoveAndRotate(Vector3 p_moveDir,Vector3 p_rotateDir, int p_state)
    {
        switch (p_state)
        {
            //只操作位移，双摇杆都在移动
            case 0:
                moveComp.PositionMove(p_moveDir);
                break;
            //只操作旋转，双摇杆都在移动
            case 1:
                //根据转动的方向与玩家的正面方向的夹角判断播放的移动动作
                CaculateAni(p_rotateDir);
                moveComp.RotationMove(p_rotateDir);
                break;
            //只有左摇杆移动
            case 2:
                //播放动画
                moveComp.PositionMove(p_moveDir);
                moveComp.RotationMove(p_rotateDir);
                break;
        }

        //if (fsm.IsCurrStateID(StateEnum.DIR_MOVE))
        //{
        //    DirMoveState t_state = (DirMoveState) fsm.FindState(StateEnum.DIR_MOVE);
        //    switch (p_state)
        //    {
        //        case 0:
        //            t_state.PositionMove(p_moveDir);
        //            break;
        //        case 1:
        //            t_state.RotationMove(p_rotateDir);
        //            break;
        //        case 2:
        //            t_state.PositionMove(p_moveDir);
        //            t_state.RotationMove(p_rotateDir);
        //            break;
        //    }
        //}
        //else
        //{
        //    SwitchState(StateEnum.DIR_MOVE,new DirMoveState.EnterParams(p_moveDir, p_rotateDir, p_state));
        //}
    }

    private void CaculateAni(Vector3 p_rotateDir)
    {
        if (m_spacialComp.faceToDir!= Vector3.zero)
        {
            float t_angle = Vector3.Angle(p_rotateDir, m_spacialComp.faceToDir)%180;
            bool t_rightSide = Vector3.Dot(m_spacialComp.faceToDir,p_rotateDir) >= 0;
            if (t_angle<=22.5f)
            {
                animComp.CrossFade(AnimEnum.RUN_FORWARD, 0.1f, 0);
            }
            else if (t_angle <= 67.5f)
            {
                if (t_rightSide)
                {
                    animComp.CrossFade(AnimEnum.RUN_F_R, 0.1f, 0);
                }
                else
                {
                    animComp.CrossFade(AnimEnum.RUN_F_R, 0.1f, 0);
                }
            }
            else if (t_angle < 112.5f)
            {
                if (t_rightSide)
                {
                    animComp.CrossFade(AnimEnum.RUN_RIGHT, 0.1f, 0);
                }
                else
                {
                    animComp.CrossFade(AnimEnum.RUN_LEFT, 0.1f, 0);
                }
            }
            else if (t_angle < 157.5f)
            {
                if (t_rightSide)
                {
                    animComp.CrossFade(AnimEnum.RUN_B_R, 0.1f, 0);
                }
                else
                {
                    animComp.CrossFade(AnimEnum.RUN_B_L, 0.1f, 0);
                }
            }
            else
            {
                animComp.CrossFade(AnimEnum.RUN_BACK, 0.1f, 0);
            }
        }
       
    }

    public void Attack(int p_skillId,List<TSprite> p_targetList=null)
    {
        SwitchState(StateEnum.CAST_SKILL,new CastSkillState.EnterParams(){m_skillId = p_skillId,m_targetList = p_targetList });
        //Table_skill t_skill= Table_skill.GetPrimary(p_skillId);
        ////1.直接播放特效即可
        //EffectShareData t_data = new EffectShareData();
        //t_data.m_bornTrans = goRootComp.GetBindPos(t_skill.bind_pos);
        //EffectManager.instance.CreatEffectByController(t_skill.effectid, t_data);
    }

    public void SwitchState(int stateID, System.Object info = null)
    {
        //1.做状态机的逻辑处理
        if (StateCheck(stateID)&& BuffCheck())
        {
            //2.进入
            fsm.SwitchState(stateID, info);
        }
    }


    private bool StateCheck(int p_stateId)
    {
        switch (fsm.GetCurrStateID())
        {
             
        }
        return true;
    }

    private bool BuffCheck()
    {
        return true;
    }
    public override Transform GetBindPos(int p_index)
    {
        //1.如果有武器，获取武器的挂点，如果没有就算了
        
        return goRootComp.GetBindPos(p_index);
    }
    #endregion
}
