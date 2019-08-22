using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tgame.Game.Table;
using UnityEngine;

public class MainLoaderComp : SpriteComp
{
    /// <summary>
    /// 加载完成
    /// </summary>
    public static string OnLoadComplete = "OnLoadComplete";
    public override void OnAdded()
    {

    }

    public override void OnRemoved()
    {
        
    }

    public GameObject m_body;

    SmartLoader m_bodyLoader=new SmartLoader();
    public void Start()
    {
        Table_avatar t_avatarTable = Table_avatar.GetPrimary(sprite.m_data.GetData<CreatData>().m_avatarId);
        m_bodyLoader.Load(t_avatarTable.url,OnLoadOK);
    }

    void OnLoadOK(GameObject p_go)
    {
        m_body = p_go;
        dispatchEvent(OnLoadComplete,null);
    }
}
