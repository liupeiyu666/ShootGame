using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Engine.Effect
{
    /// <summary>
    /// 监听其他模块发出的生存结束的信息
    /// </summary>
    [Serializable]
    [Des("监听检测")]
    public class ListenOtherRuleServie : ServiveRule
    {
        private bool m_revieve = false;
        public override bool ServiveCheck(float p_deltaTime)
        {
            CheckInit();
           
            return m_revieve;
        }

        protected override void OnInit()
        {
            m_revieve = false;
          //  Debug.LogError("AddEventListener");
            AddEventListener(RuleListener);

        }

        void RuleListener(string p_name)
        {
            switch (p_name)
            {
                case RuleEvent_OnArrive:
              //      Debug.LogError("RuleListener"+m_revieve);
                    m_revieve = true;
                    RemoveEventListener(RuleListener);
                    break;
            }
        }
    }
}
