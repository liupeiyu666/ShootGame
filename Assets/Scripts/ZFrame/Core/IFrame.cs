using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZFrame
{
    /// <summary>
    /// Frame接口
    /// </summary>
    internal interface IFrame
    {
        //--------------------------------------------------------
        // 注册 、查询、移除 Module
        //--------------------------------------------------------
        /// <summary>
        /// 注册Module
        /// </summary>
        /// <param name="__module"></param>
        //void RegisterModule(Module __module);
       
        /// <summary>
        /// 移除module(返回被移除的Module)
        /// </summary>
        /// <param name="__moduleClass"></param>
        /// <returns></returns>
        Module RemoveModule(Type __moduleClass);
        T RemoveModule<T>() where T : Module;

        /// <summary>
        /// 指定module是否存在
        /// </summary>
        /// <param name="__moduleClass"></param>
        /// <returns></returns>
        bool HasModule(Type __moduleClass);
        bool HasModule<T>() where T : Module;

        /// <summary>
        /// 查找module
        /// </summary>
        /// <param name="__moduleClass"></param>
        /// <returns></returns>
        //Module GetModule(Type __moduleClass);
        //T GetModule<T>() where T : Module;


        /// <summary>
        ///  刷新
        /// </summary>
        //void OnUpdate();



        //--------------------------------------------------------
        // 以下为需要覆写的函数
        //--------------------------------------------------------
        /// <summary>
        /// 注册的Module的集合（会在构造中被自动注册）
        /// 请注意：返回为Module的实例数组
        /// </summary>
        /// <returns></returns>
        List<Module> ListModules();
    }
}
