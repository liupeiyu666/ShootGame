using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using Engine.Effect;
using UnityEngine;

/// <summary>
/// 临时怪物类
/// </summary>
public class TMonster:MonoBehaviour
{
    public float m_shootTime = 3;

    public Transform m_shootTrans;
    
    SafeTimer m_safeTimer=new SafeTimer();
    private EffectShareData m_shareData=new EffectShareData();
    void Start()
    {
        EffectManager.instance.CreatEffectByController(2001, m_shareData);
        m_safeTimer.Start(m_shootTime);
        m_shareData.m_bornTrans = m_shootTrans;
    }

    void Update()
    {
        if (m_safeTimer.IsOK())
        {
           // EffectManager.instance.CreatEffectByController(2001, m_shareData);
            m_safeTimer.Reset();
        }
    }
}