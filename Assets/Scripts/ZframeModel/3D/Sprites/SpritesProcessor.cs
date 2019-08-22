using System;
using System.Collections.Generic;
using UnityEngine;
using ZframeModel.ZEvent.Sprites;
using ZFrame;
/// <summary>
/// @author Lpy
/// </summary>
public class SpritesProcessor : BaseProcessor
{
    public SpritesProcessor(Module __module) : base(__module)
    {
    }
    #region ModelEvent指令--游戏内使用
    protected override List<Type> ListenModuleEvents()
    {
        return new List<Type>()
        {
            typeof(ME_Sprites_Refresh),
            typeof(ME_Hero_Creat)
        };
    }
    sealed override protected void ReceivedModuleEvent(ModuleEvent __ME)
    {
        if (__ME.GetType()==typeof(ME_Sprites_Refresh))
        {
            //刷新位置
            SpriteRefresh();
        }
        else if (__ME.GetType() == typeof(ME_Hero_Creat))
        {
            ME_Hero_Creat t_temp = __ME as ME_Hero_Creat;
            CreatHero(t_temp.m_data);
        }
    }
    #endregion


    #region 刷新位置

    void SpriteRefresh()
    {
        Vector3 t_pos = THero.instance.m_spacialComp.worldPosi;
        THero.instance.m_spacialComp.ResetPosi(t_pos.x, t_pos.z);
    }

    #endregion

    #region 创建英雄

    void CreatHero(int p_roleId)
    {
        CreatData data = new CreatData();
        data.m_avatarId = 1000;
        data.role_id = 1;
        data.m_posx = 0.5f;
        data.m_posz = 0.8f;
        SpriteFactory.Create<THero>(data);
        Frame.DispatchEvent(MEFactory.New<ME_Hero_CreatOk>());
    }

    void OnHeroCreadComplete()
    {
       
    }

    #endregion
}