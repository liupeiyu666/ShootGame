using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// 子弹基类，生命周期和Mono一致
/// 子弹主要模块：出生规则、生存规则、运动规则、死亡规则
/// 出生规则：出生的位置、角度
/// 生存规则：这个会和死亡规则有交叉部分，主要决定子弹的生存方式，时间、碰撞、外界控制等等
/// 运动规则：生存过程中的运动方式
/// 死亡规则：主要是死亡之后需要执行的事情，不参与生存方式的计算，比如，死亡后发射另一个子弹，死亡后触发另一个事件等等,死亡
///            规则没有持续性，（如果后期希望添加持续性设计，需要再添加一个状态）
/// ===================================================================
/// 每个子弹的描述都是有规则组成，不同类型的子弹可以有不同的规则组成。
/// 规则是描述子弹的充分但不必要条件。一个子弹可以什么规则都没有，根据具体需要开发出不同的规则，然后策划进行搭配
/// 同一个规则可以有多个描述
/// ===================================================================
/// 出生规则->>生存规则->>死亡规则串行执行         其中运动规则，并行执行
/// </summary>
namespace Engine.Effect
{

    /// <summary>
    /// 特效控制器，不同的类型的特效，需要扩充对应的控制器
    /// </summary>
    public class BaseEffectController:MonoBehaviour
    {
        public EffectShareData m_shareDate=new EffectShareData();
        
        protected List<BornRule> m_bornRuleList=new List<BornRule>();
     
        protected List<ServiveRule> m_serviveRuleList = new List<ServiveRule>();
      
        protected List<MoveRule> m_moveRuleList = new List<MoveRule>();
    
        protected List<DeadRule> m_deadRuleList = new List<DeadRule>();

        protected List<ExpandRule> m_expandRuleList = new List<ExpandRule>();

        protected List<int> m_testList = new List<int>();

        protected EffectLifeState m_state=EffectLifeState.None;
        public void AddRule(BaseRule p_baseRule)
        {
          
            switch (p_baseRule.m_ruleType)
            {
                case Enum_RuleType.Born:
                    m_bornRuleList.Add(p_baseRule as BornRule);
                    break;
                case Enum_RuleType.Servive:
                    m_serviveRuleList.Add(p_baseRule as ServiveRule);
                    break;
                case Enum_RuleType.Move:
                    m_moveRuleList.Add(p_baseRule as MoveRule);
                    break;
                case Enum_RuleType.Dead:
                    m_deadRuleList.Add(p_baseRule as DeadRule);
                    break;
                case Enum_RuleType.Expand:
                    m_expandRuleList.Add(p_baseRule as ExpandRule);
                    break;
                default:
                    break;

            }
        }

        protected void InitRules()
        {
            for (int i = 0; i < m_bornRuleList.Count; i++)
            {
                m_bornRuleList[i].InitController(this);
            }
            for (int i = 0; i < m_serviveRuleList.Count; i++)
            {
                m_serviveRuleList[i].InitController(this);
            }
            for (int i = 0; i < m_moveRuleList.Count; i++)
            {
                m_moveRuleList[i].InitController(this);
            }
            for (int i = 0; i < m_deadRuleList.Count; i++)
            {
                m_deadRuleList[i].InitController(this);
            }
        }
        /// <summary>
        /// 组装规则
        /// </summary>
        public virtual void CombinRules()
        {
        }

        #region 系统函数
        void Awake()
        {
          //  Debug.LogError("Awake");
            CombinRules();
            InitRules();
        }

        void Start()
        {
          //  Debug.LogError("Start");
            // DoRules();
        }

        void Update()
        {
            DoRules();
            //根据状态设置移动
            DoMove();
            //执行扩展
            DoExpand();
        }

        void OnTriggerEnter(Collider p_collider)
        {
            //对生存和运动规则进行调用
            for (int i = 0; i < m_serviveRuleList.Count; i++)
            {
                m_serviveRuleList[i].OnTriggerEnter(p_collider);
            }
            for (int i = 0; i < m_moveRuleList.Count; i++)
            {
                m_moveRuleList[i].OnTriggerEnter(p_collider);
            }
        }

        #endregion


        void DoRules()
        {
            switch (m_state)
            {
                case EffectLifeState.None:
                    m_state = EffectLifeState.Born;
                    DoRules();
                    break;
                case EffectLifeState.Born:
                    {
                        //出生规则与的方式
                        bool t_bornOK = true;
                        for (int i = 0; i < m_bornRuleList.Count; i++)
                        {
                            t_bornOK = t_bornOK && m_bornRuleList[i].CheckBorn(Time.deltaTime);
                        }

                        if (t_bornOK)
                        {
                            m_state = EffectLifeState.Servive;
                            //调用结束
                            for (int i = 0; i < m_bornRuleList.Count; i++)
                            {
                                m_bornRuleList[i].OnRuleComplete();
                            }
                            DoRules();
                        }
                    }


                    break;
                case EffectLifeState.Servive:
                    {
                        //生存方式或的规则
                        bool t_ServiveOk = false;
                        for (int i = 0; i < m_serviveRuleList.Count; i++)
                        {
                            t_ServiveOk = t_ServiveOk || m_serviveRuleList[i].ServiveCheck(Time.deltaTime);
                        }

                        if (t_ServiveOk)
                        {
                            m_state = EffectLifeState.Dead;
                            for (int i = 0; i < m_serviveRuleList.Count; i++)
                            {
                                m_serviveRuleList[i].OnRuleComplete();
                            }
                            DoRules();
                        }

                    }

                    break;
                case EffectLifeState.Dead:
                    {
                        for (int i = 0; i < m_moveRuleList.Count; i++)
                        {
                            m_moveRuleList[i].OnRuleComplete();
                        }
                        for (int i = 0; i < m_deadRuleList.Count; i++)
                        {
                            m_deadRuleList[i].DoDead();
                        }
                        //OVER--
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        void DoMove()
        {
            if (m_state!=EffectLifeState.Dead)
            {
                for (int i = 0; i < m_moveRuleList.Count; i++)
                {
                    m_moveRuleList[i].UpdateMove(Time.deltaTime);
                }
            }
        }

        void DoExpand()
        {
            if (m_state != EffectLifeState.Dead)
            {
                for (int i = 0; i < m_expandRuleList.Count; i++)
                {
                    m_expandRuleList[i].DoExpand(Time.deltaTime);
                }
            }
        }
    }

    public enum EffectLifeState
    {
        None,
        Born,
        Servive,
        Dead,
    }

}

