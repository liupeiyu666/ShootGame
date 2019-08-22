using System;
using System.Collections.Generic;
using UnityEngine;
using ZFrame;


/// <summary>
/// 游戏模块列表（模块管理器）
/// </summary>
public class ModuleList : Frame
{
    override protected List<Module> ListModules()
    {
        List<Module> t_tempList=new List<Module>();
        //反射一下吧
        var classes = typeof(ModuleList).Assembly.GetTypes();
        var att = typeof(Module);
        for (int i = 0; i < classes.Length; i++)
        {
            if (classes[i].IsSubclassOf(att))
            {
                Module obj = (Module)Activator.CreateInstance(classes[i], true);
                t_tempList.Add(obj);
            }
        }

        return t_tempList;
        //谁创建和负责的模块要标明
        //目的是1,便于后期查找负责人. 2,便于以后有分支合并到主干的需求时,较大程度避免冲突
        return new List<Module>()
        {
                new GameLaunchModel(),
        };
    }
}
