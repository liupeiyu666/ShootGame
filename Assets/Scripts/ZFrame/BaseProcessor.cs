using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using ZFrame;


/// <summary>
/// 基本Processor（扩展，增加网络通讯消息处理的功能）
/// </summary>
public abstract class BaseProcessor : Processor
{
    /// <summary>
    /// 重载构造函数
    /// </summary>
    /// <param name="_module"></param>
    public BaseProcessor(Module _module)
        : base(_module)
    {

        //创建消息号与消息CLASS对照表（这样就可以使用从服务端发来的消息号，找到对应的解析类
        var list = ListenModuleEvents();
        if (list != null && list.Count > 0)
        {
           

            int count = list.Count;
            for (int i = 0; i < count; i++)
            {
               
            }
        }
    }
    private uint StringToInt(string str)
    {
        if (string.IsNullOrEmpty(str))
            return 0;

        // 正则表达式剔除非数字字符（不包含小数点.） 
        //str = Regex.Replace(str, @"[^\d\d]", "");
        str = Regex.Replace(str, @"^.+Msg|_.+$", "");
        // 如果是数字，则转换为int类型 
        if (Regex.IsMatch(str, @"^[+-]?\d*[.]?\d*$"))
        {
            uint i = 0;
            if (uint.TryParse(str, out i))
                return i;

            return i;
        }

        return 0;
    }

    /// <summary>
    /// 获取当前的Module 类型数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    protected T GetMoule<T>() where T : Module
    {
        var rModule = (T)module;

        if (rModule != null)
            return rModule;
        return default(T);
    }




    #region 事件处理

    /// <summary>
    /// 处理事件方法
    /// </summary>
    /// <param name="__key">事件唯一标识</param>
    /// <param name="__data">数据</param>
    protected sealed override void ReceivedModuleEvent(int __key, object __data)
    {

    }

    /// <summary>
    /// 处理网络事件消息方法
    /// </summary>
    /// <param name="__msg"></param>
    protected virtual void ReceivedMessage(object __msg) { }

    #endregion

    #region 网络消息

    /// <summary>
    /// 向服务器发送消息
    /// </summary>
    /// <typeparam name="T">消息类型</typeparam>
    /// <param name="msg">消息内容</param>
    //public void SendToServer<T>(T msg) where T : Message
    //{
    //    SocketManager.instance.SendMsgToServer(msg);
    //}

    #endregion
}