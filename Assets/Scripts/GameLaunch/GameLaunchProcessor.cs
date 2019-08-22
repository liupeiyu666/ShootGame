
using System.Collections.Generic;
using UnityEngine;
using ZFrame;

public class GameLaunchProcessor : BaseProcessor
{
    public GameLaunchProcessor(Module __module) : base(__module)
    {
    }

    private BaseLaunch m_baseLaunch;
    sealed override protected void ReceivedModuleEvent(string __key, object __data)
    {
        Main.GetInstance().m_baseLaunch.ReceivedModuleEvent(__key, __data);
    }

    protected override List<string> ListenModuleStringEvents()
    {
        return Main.GetInstance().m_baseLaunch.ListenModuleEvents();
    }

}