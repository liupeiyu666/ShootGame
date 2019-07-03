using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Object = System.Object;

namespace Engine.Effect
{
    /// <summary>
    /// 2019.6.27需要序列化组装，由于规则无法确定，所以需要自定义一套组装的机制
    /// 2019.6.28由于需要自定义序列的未知行太大，以及部分数据结构的序列化过去复杂，这里采用更改为rule采用挂在的方式
    ///          使用mono进行序列化，虽然不是很美观，但是可以简单的解决序列化以及编辑的问题
    /// </summary>
    public class EffectController:BaseEffectController
    {
        public Transform m_taget;
        public override void CombinRules()
        {
            m_shareDate.m_bornPos=new Vector3(10.5f,2.6f,3.7f);

            m_shareDate.m_targetPos=new Vector3(100,0,0);

            m_shareDate.m_targetTrans = m_taget;
            //1.获取这个物体上的所有rule类
            BaseRule[] t_allRules= gameObject.GetComponents<BaseRule>();
            for (int i = 0; i < t_allRules.Length; i++)
            {
                AddRule(t_allRules[i]);
            }
            //AddRule(PositionBorn);
            //AddRule(TimeLimitServive);
            //AddRule(StraightPointPosMove);
            //AddRule(NormalDead);
        }
    }
}
