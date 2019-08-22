using System;
using System.Collections.Generic;
using ZframeModel.ZEvent.Sprites;
using ZFrame;

/// <summary>
/// @author Lpy
/// </summary>
public class GameStartProcessor : BaseProcessor
{
    public GameStartProcessor(Module __module) : base(__module)
    {
    }

    #region 字符串指令--启动流和单机使用

    protected override List<string> ListenModuleStringEvents()
    {
        return new List<string>(){LaunchStepCommands.Start_GamePlay};
    }

    sealed override protected void ReceivedModuleEvent(string __key, object __data)
    {
        switch (__key)
        {
            case LaunchStepCommands.Start_GamePlay:
            {
               
                //开始游戏---进入选择角色等等，根据游戏内容进行设置
                //--这里先做加载英雄，加载地图
                Frame.DispatchEvent(MEFactory.New<ME_Hero_Creat>(1));
                Frame.DispatchEvent(MEFactory.New<ME_SwitchMap>());
            }
                break;
        }
    }

    #endregion

    #region ModelEvent指令--游戏内使用

    protected override List<Type> ListenModuleEvents()
    {
        return new List<Type>();
    }

    sealed override protected void ReceivedModuleEvent(ModuleEvent __ME)
    {
        if (__ME.GetType() == typeof(ModuleEvent))
        {

        }
    }

    #endregion
}