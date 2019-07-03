using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Engine.Effect
{
    /// <summary>
    /// 发射扩展规则，根据时间去发射，包括发射的方向位置等
    /// </summary>
    [Des("时间发射器")]
    public class ShootExpandRule:ExpandRule
    {
        /// <summary>
        /// 射击设置
        /// </summary>
        public ShootSetting m_shootSetting;

        private SafeTimer m_pointShootTimer=new SafeTimer();

        private SafeTimer m_oneShootTimer = new SafeTimer();
        /// <summary>
        /// 发射的特效的下标
        /// </summary>
        private int m_shootEffectIndex = 0;
        /// <summary>
        /// 发射的节点下标
        /// </summary>
        private int m_shootTransIndex = 0;
        /// <summary>
        /// 是否初始化成功
        /// </summary>
        private bool m_initOK = true;
        public override void DoExpand(float p_deltaTime)
        {
            CheckInit();
            if (m_initOK)
            {
                DoOneShoot();
            }
        }

        protected override void OnInit()
        {
            base.OnInit();
            m_initOK = true;
            if (m_shootSetting.m_shootEffetIdList.Count==0||m_shootSetting.m_bornPosList.Count==0||(m_shootSetting.m_shootTimes<=0&&m_shootSetting.m_oneShootSpaceTime==0))
            {
                Debug.LogError("特效发射初始化信息失败："+m_gameObject.name);
                m_initOK = false;
                return;
            }
            m_pointShootTimer.Start(m_shootSetting.m_pointShootSpaceTime);
            m_oneShootTimer.Start(m_shootSetting.m_oneShootSpaceTime);
        }

        void DoOneShoot(bool p_force=false)
        {
            bool t_numOk = false,t_timeCheck=false;
            //计算剩余数量,-1表示没有次数限制
            if (m_shootSetting.m_shootTimes>0|| m_shootSetting.m_shootTimes==-1)
            {
                t_numOk = true;
            }

            if (m_oneShootTimer.IsOK()&& m_pointShootTimer.IsOK())
            {
                t_timeCheck = true;
            }
            
            if (p_force|| t_numOk&& t_timeCheck)
            {
                //创建特效
                EffectShareData m_shareData = new EffectShareData();
                m_shareData.m_bornTrans = m_shootSetting.m_bornPosList[m_shootTransIndex];
                int effectId = m_shootSetting.m_shootEffetIdList[m_shootEffectIndex];
                EffectManager.instance.CreatEffectByController(effectId, m_shareData);

                m_shootTransIndex++;
                m_shootEffectIndex++;
                //计算下标,并检测一次轮询是否完成
                m_shootEffectIndex = m_shootEffectIndex >= m_shootSetting.m_shootEffetIdList.Count ? 0 : m_shootEffectIndex;
                if (m_shootTransIndex== m_shootSetting.m_bornPosList.Count)
                {
                    //轮询完成，启动下一次
                    m_oneShootTimer.Start(m_shootSetting.m_oneShootSpaceTime);
                    m_shootTransIndex = 0;
                    m_shootSetting.m_shootTimes = m_shootSetting.m_shootTimes > 0 ? m_shootSetting.m_shootTimes--: m_shootSetting.m_shootTimes;
                }
                //子弹没有轮询
                m_shootTransIndex = m_shootTransIndex >= m_shootSetting.m_bornPosList.Count ? 0 : m_shootTransIndex;
                //启动间隔查询，如果没有间隔，就直接下一个了
                if (m_shootSetting.m_pointShootSpaceTime<=0)
                {
                    DoOneShoot();
                }
                else
                {
                    //间隔时间内开始计时
                    if (m_pointShootTimer.IsOK())
                    {
                        m_pointShootTimer.Start(m_shootSetting.m_pointShootSpaceTime);
                        DoOneShoot();
                    }
                }
            }
        }
        
    }
    /// <summary>
    /// 发射的设置
    /// 位置、个数根据设置的节点获取
    /// </summary>
    [Serializable]
    public class ShootSetting
    {
        /// <summary>
        /// 出生的位置点，这个就直接在制作的时候设置好挂点即可，如果没有就默认是自己
        /// </summary>
        public List<Transform> m_bornPosList=new List<Transform>();
        /// <summary>
        /// 需要发射的特效的id
        /// </summary>
        public  List<int> m_shootEffetIdList=new List<int>();
        /// <summary>
        /// 每一发的间隔，指的是每个节点发射的间隔时间
        /// </summary>
        public float m_pointShootSpaceTime;
        /// <summary>
        /// 一个轮询的发射间隔时间
        /// </summary>
        public float m_oneShootSpaceTime;

        /// <summary>
        /// 发射次数  小于0表示不限次数
        /// </summary>
        public int m_shootTimes = -1;

    }
}
