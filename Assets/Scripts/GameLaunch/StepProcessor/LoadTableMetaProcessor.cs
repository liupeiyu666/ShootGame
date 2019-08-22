using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Tgame.Game.Table;
using UnityEngine;
using ZTool.Table;
using Module = ZFrame.Module;

public class LoadTableMetaProcessor : BaseProcessor
{
    public LoadTableMetaProcessor(Module __module) : base(__module)
    {
    }

    sealed override protected void ReceivedModuleEvent(string __key, object __data)
    {
        switch (__key)
        {
            case LaunchStepCommands.Start_LaunchLoadTable:
                {
                    //开始读取配表
                    //1.配表加载
                    TableManager.instance.OnLoadComplete = OnMetaLoadOK;
                    TableManager.instance.StartLoad();
                   
                }
                break;
        }
    }

    protected override List<string> ListenModuleStringEvents()
    {
        return new List<string>(){ LaunchStepCommands.Start_LaunchLoadTable };
    }

    void OnMetaLoadOK()
    {
        Debug.LogError("******************************");
        DispatchEvent(LaunchStepCommands.End_LaunchLoadTable, null);
    }
}