using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Engine.Effect;

public class EffectShootTest : MonoBehaviour
{
    public List<EffectDataInfo> m_effectList;
    public int m_bulletID;
    public EffectShareData m_shareData;
    // Use this for initialization
    void Start()
    {
        EffectManager.instance.m_isTest = true;
        TestEffectLoader.m_resDic.Clear();
        foreach (var effect in m_effectList)
        {
            TestEffectLoader.m_resDic.Add(effect.m_effectID,effect.m_go);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            EffectManager.instance.CreatEffectByController(m_bulletID, m_shareData);
        }
    }
    private void OnGUI()
    {
        GUI.Label(new Rect(50,50,100,50),"按F发射" );
    }
}
[Serializable]
public class EffectDataInfo
{
    public int m_effectID;
    public GameObject m_go;
}