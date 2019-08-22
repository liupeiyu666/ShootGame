using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZFrame
{
    /// <summary>
    /// Processor接口
    /// </summary>
    internal interface IProcessor
    {
        //--------------------------------------------------------
        // public API
        //--------------------------------------------------------
        /// <summary>
        /// 所属模块
        /// </summary>
        Module module { get; }

//        /// <summary>
//        /// 派发事件
//        /// </summary>
//        /// <param name="__e"></param>
		void DispatchEvent(string __key, object __data);



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

        /// <summary>
        ///  当每帧刷新时
        /// </summary>
        void OnUpdate();

        /// <summary>
        /// 监听的事件类的集合
        /// 请注意：返回为事件的CLASS(这些CLASS必须继承自ModuleEvent)的数组
        /// </summary>
        /// <returns></returns>
        List<Type> ListenModuleEvents();

        /// <summary>
        /// 处理事件
        /// </summary>
        /// <param name="__me"></param>
		void ReceivedModuleEvent(string __key, object __data);
    }
}
