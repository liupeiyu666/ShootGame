using System;
using System.Collections.Generic;
using ZframeModel.ZEvent.Sprites;
using ZFrame;
using AdvancedUtilities.Cameras;
using AdvancedUtilities.Cameras.Components;
using Tgame.Game.Table;
using UnityEngine;
using ZframeModel.ZEvent.Camera;

/// <summary>
/// @author Lpy
/// </summary>
public class CameraProcessor : BaseProcessor
{
    public CameraProcessor(Module __module) : base(__module)
    {
    }

    #region 字符串指令--启动流和单机使用

    protected override List<string> ListenModuleStringEvents()
    {
        return new List<string>(){LaunchStepCommands.Start_LaunchInitCamera };
    }

    sealed override protected void ReceivedModuleEvent(string __key, object __data)
    {
        switch (__key)
        {
            case LaunchStepCommands.Start_LaunchInitCamera:
                {
                    CameraInit();
                    DispatchEvent(LaunchStepCommands.End_LaunchInitCamera,null);
                }
                break;
        }
    }

    #endregion

    #region ModelEvent指令--游戏内使用

    protected override List<Type> ListenModuleEvents()
    {
        return new List<Type>(){typeof(ME_Hero_CreatOk),typeof(ME_SetCameraParam) };
    }

    sealed override protected void ReceivedModuleEvent(ModuleEvent __ME)
    {
        if (__ME.GetType() == typeof(ME_Hero_CreatOk))
        {
            //设置摄像机
            Debug.LogError(GameInput.instance.m_cameraController);
            Debug.LogError(GameInput.instance.m_cameraController.Target);
            Debug.LogError(GameInput.instance.m_cameraController.Target.Target);
            GameInput.instance.m_cameraController.Target.Target = THero.instance.transform;
        }
        else if (__ME.GetType() == typeof(ME_SetCameraParam))
        {
            SetCamra(((ME_SetCameraParam)__ME).m_cameraParmId);
        }
    }

    #endregion

    void CameraInit()
    {
       
    }
    /// <summary>
    /// 通过配表设置摄像机的参数
    /// </summary>
    /// <param name="p_id"></param>
    void SetCamra(int p_id)
    {
     
    }


}