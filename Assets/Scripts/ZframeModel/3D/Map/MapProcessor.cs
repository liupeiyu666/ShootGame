using System;
using System.Collections.Generic;
using Tgame.Game.Table;
using UnityEngine;
using ZframeModel.ZEvent.Camera;
using ZframeModel.ZEvent.Sprites;
using ZFrame;

/// <summary>
/// @author Lpy
/// </summary>
public class MapProcessor : BaseProcessor
{
    public MapProcessor(Module __module) : base(__module)
    {
    }

    #region 字符串指令--启动流和单机使用

    protected override List<string> ListenModuleStringEvents()
    {
        return new List<string>();
    }

    sealed override protected void ReceivedModuleEvent(string __key, object __data)
    {
        switch (__key)
        {
            case "":
            {
            }
                break;
        }
    }

    #endregion

    #region ModelEvent指令--游戏内使用

    protected override List<Type> ListenModuleEvents()
    {
        return new List<Type>()
        {
            typeof(ME_SwitchMap),
        };
    }

    sealed override protected void ReceivedModuleEvent(ModuleEvent __ME)
    {
        if (__ME.GetType() == typeof(ME_SwitchMap))
        {
            StartSwitchMap(MEFactory.GetType<ME_SwitchMap>(__ME).m_mapId);
        }
    }

    #endregion

    void StartSwitchMap(int p_mapID)
    {
       
    }
    void OnMapEvent(System.Object sender, XEventArgs ea)
    {
       
    }
}