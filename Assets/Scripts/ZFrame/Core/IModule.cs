using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZFrame
{
    /// <summary>
    /// Module接口
    /// </summary>
    internal interface IModule
    {
        //--------------------------------------------------------
        // public API(注册 、查询、移除 Processor（实例管理，每一个Module互补干涉）)
        //--------------------------------------------------------
        /// <summary>
        /// 注册Processor
        /// </summary>
        /// <param name="__processor"></param>
        //void RegisterProcessor(Processor __processor);
        /// <summary>
        /// 移除Processor(返回被移除的Processor)
        /// </summary>
        /// <param name="__processorClass"></param>
        /// <returns></returns>
        //Processor RemoveProcessor(Type __processorClass);
        //T RemoveProcessor<T>() where T : Processor;

        /// <summary>
        /// 检查是否存在
        /// </summary>
        /// <param name="__processorClass"></param>
        /// <returns></returns>
        bool HasProcessor(Type __processorClass);
        bool HasProcessor<T>() where T : Processor;

        /// <summary>
        /// 查找Processor
        /// </summary>
        /// <param name="__processorClass"></param>
        /// <returns></returns>
        Processor GetProcessor(Type __processorClass);
        T GetProcessor<T>() where T : Processor;


        //--------------------------------------------------------
        // 以下为需要覆写的函数
        //--------------------------------------------------------
        /// <summary>
        /// 当初始化时
        /// </summary>
        void OnInit();

        /// <summary>
        /// 当被注册时
        /// </summary>
        void OnRegister();

        /// <summary>
        ///  当被移除时
        /// </summary>
        void OnRemove();
        ///// <summary>
        /////  当每帧刷新时
        ///// </summary>
        //void OnUpdate();

        /// <summary>
        /// 注册的Processor的集合
        /// 请注意：返回为Processor的实例数组
        /// </summary>
        /// <returns></returns>
        List<Processor> ListProcessors();
    }
}
