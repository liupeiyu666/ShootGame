using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Engine.Effect
{
    /// <summary>
    /// 完成数据控制，以及数据交换
    /// </summary>
    public class BaseRule:MonoBehaviour
    {

        public Enum_RuleType m_ruleType {  get; protected set; }

        private BaseEffectController m_effectController;

        protected GameObject m_gameObject
        {
            get { return m_effectController.gameObject; }
        }
        protected EffectShareData m_shareDate
        {
            get { return m_effectController.m_shareDate; }
        }

        /// <summary>
        /// 初始化控制器
        /// </summary>
        /// <param name="p_controller"></param>
        public void InitController(BaseEffectController p_controller)
        {
            m_effectController = p_controller;
        }

        public virtual void OnTriggerEnter(Collider p_collider)
        {

        }
        /// <summary>
        /// 初始化需要的数据,第一个表示规则的类型ID，之后数据表示参数
        /// </summary>
        /// <param name="p_params"></param>
        public virtual void InitParams(string[] p_params)
        {
        }

        #region 功能函数
        /// <summary>
        /// 是否初始化完成
        /// </summary>
        private bool m_isInit;
        /// <summary>
        /// 初始化检测,与OnruleComplete成对出现，否则使用对象池会出现问题
        /// </summary>
        protected void CheckInit()
        {
            if (!m_isInit)
            {
                OnInit();
                m_isInit = true;
            }
        }
        public void OnRuleComplete()
        {
            m_isInit = false;
        }
        protected virtual void OnInit()
        {
        }

        protected void Unload()
        {
            EffectManager.instance.Unload(m_effectController);
        }

        #endregion
    }

    public enum Enum_RuleType
    {
        Born,
        Servive,
        Move,
        Dead,
        Expand,
    }
}
