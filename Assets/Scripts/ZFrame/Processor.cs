﻿using System;
using System.Collections.Generic;
using UnityEngine;
using ZLib;

namespace ZFrame
{
    /// <summary>
    /// Processor基类
    /// </summary>
    public class Processor// :IProcessor
    {
        /// <summary>
        /// Processor基类
        /// </summary>
        /// <param name="__module">所属模块</param>
        public Processor(Module __module)
        {
            module = __module;

            //初始化
            OnInit();
        }

        //--------------------------------------------------------
        // public API
        //--------------------------------------------------------
        /// <summary>
        /// 所属模块
        /// </summary>
        public Module module { get; private set; }

    //        /// <summary>
    //        /// 派发事件
    //        /// </summary>
    //        /// <param name="__mEvent">ModuleEvent事件</param>
    //        public void DispatchEvent(ModuleEvent __mEvent)
    //        {
    //            ModuleEventManager.DispatchEvent(__mEvent);
    //        }
    /// <summary>
    /// 派发事件
    /// </summary>
    /// <param name="__key">唯一标识</param>
    /// <param name="__data">数据</param>
    public void DispatchEvent(string __key, object __data)
        {
            ModuleEventManager.DispatchEvent(__key, __data);
        }

        //--------------------------------------------------------
        // internal API(供框架自身调用的API， 用于注册和移除事件监听)
        //--------------------------------------------------------
        //此类的唯一标识（即该类的完全限定名）
        private string ownerKey
        {
            get { return GetType().ToString(); }
        }

        //注册事件
        internal void RegisterEvents()
        {
            //注册消息监听
            List<string> t_meClassArr = ListenModuleStringEvents(); 
            if (t_meClassArr != null && t_meClassArr.Count > 0)
            {
             
                t_meClassArr.ForEach(p =>
                {
                    Debug.LogError("RegisterEvents:" + p);
                    ModuleEventManager.AddEvent(p, ReceivedModuleEventHandle, this.ownerKey);
                });
            }
            List<Type> meClassArr = ListenModuleEvents();
            if (meClassArr != null && meClassArr.Count > 0)
            {
                meClassArr.ForEach(p =>
                {
                    ModuleEventManager.AddEvent(p.GetHashCode(), ReceivedModuleEventHandle, this.ownerKey);
                });
            }
        }

        //移除事件
        internal void RemoveEvents()
        {
            ModuleEventManager.RemoveEventByOwner(this.ownerKey);
        }

        //解析事件，之后交给处理函数
        private void ReceivedModuleEventHandle(Notification __notification)
        {
            var data = __notification.body;
            //处理
            if (!string.IsNullOrEmpty(__notification.name))
            {
                ReceivedModuleEvent(__notification.name, data);
            }
           
            //如果是ModuleEvent， 再处理
            if (data is ModuleEvent)
            {
                ModuleEvent meEvent = __notification.body as ModuleEvent;
                //处理
                ReceivedModuleEvent(meEvent);
            }
            else if(__notification.iname!=null)
            {
                ReceivedModuleEvent((int)__notification.iname, data);
            }
            return;
        }
        internal void internal_OnRegister()
        {
            OnRegister();
        }
        internal void internal_OnRemove()
        {
            OnRemove();
        }
        /// <summary>
        /// 当每帧刷新时
        /// </summary>
        internal void internal_OnUpdate()
        {
            OnUpdate();
        }


        //--------------------------------------------------------
        // 以下为需要覆写的函数
        //--------------------------------------------------------
        /// <summary>
        /// 当初始化时
        /// </summary>
        virtual protected void OnInit() { }

        /// <summary>
        /// 当被注册时
        /// </summary>
        virtual protected void OnRegister() { }

        /// <summary>
        ///  当被移除时
        /// </summary>
        virtual protected void OnRemove() { }

        /// <summary>
        /// 当每帧刷新时
        /// </summary>
        virtual protected void OnUpdate() { }

        /// <summary>
        /// 监听的事件类的集合(注意此函数在框架中只能调用一次！！！)
        /// 请注意：返回为事件的CLASS(这些CLASS必须继承自ModuleEvent)的数组
        /// </summary>
        /// <returns></returns>
        virtual protected List<string> ListenModuleStringEvents()
        {
            return null;
        }
        /// <summary>
        /// 注册类型的事件，而不是字符串
        /// </summary>
        /// <returns></returns>
        virtual protected List<Type> ListenModuleEvents()
        {
            return null;
        }
        //        /// <summary>
        //        /// 处理事件(只有当消息是ModuleEvent类型时才会执行)
        //        /// </summary>
        //        /// <param name="__me">模块事件</param>
        //        virtual protected void ReceivedModuleEvent(ModuleEvent __me) { }
        /// <summary>
        /// 处理事件(此函数必然会执行)
        /// </summary>
        /// <param name="__key">唯一标识</param>
        /// <param name="__data">数据</param>
        virtual protected void ReceivedModuleEvent(string __key, object __data) { }
        /// <summary>
        /// 消息专用
        /// </summary>
        /// <param name="__key"></param>
        /// <param name="__data"></param>
        virtual protected void ReceivedModuleEvent(int __key, object __data) { }
        /// <summary>
        /// Module继承专用
        /// </summary>
        /// <param name="__moduleEvent"></param>
        virtual protected void ReceivedModuleEvent(ModuleEvent __moduleEvent) { }
    }
}
