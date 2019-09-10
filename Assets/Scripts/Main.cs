using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ZTool.Table;


public class Main:MonoBehaviour
{
    private static Main _instance;

    public static Main GetInstance()
    {
        return _instance;
    }
    public BaseLaunch m_baseLaunch = new OffLineLaunch();
    //游戏模块列表
    ModuleList _moduleList;

    void Awake()
    {
        _instance = this;
    }

    void Start()
    {
        //ZFrame初始化
        _moduleList = new ModuleList();
        m_baseLaunch.Start();

    }

    void Update()
    {
        
        TTicker.tUpdate();
        GOPoolV3.instance.tUpdate();
        if (Input.GetKeyDown(KeyCode.P))
        {
            CreatData data = new CreatData();
            data.m_avatarId = 1000;
            data.role_id = 1;
            data.m_posx = 0.5f;
            data.m_posz = 0.8f;
            SpriteFactory.Create<THero>(data);
        }
        LoadManager.instance.Update();
      //  Camera.main.SetReplacementShader();
    }
}

