using System;
using ZFrame;

/// <summary>
/// 统一消息的构造 给未来在性能上优化消息的时候做基础
/// @author Lpy
/// </summary>
public static class MEFactory
{
    public static T New<T>(params object[] data) where T : ModuleEvent, new()
    {
        var me = new T();
        if (data != null)
        {
            me.Init(data);
        }
        return me;
    }
    ///// <summary>
    ///// 通过参数构造
    ///// </summary>
    ///// <typeparam name="T"></typeparam>
    ///// <param name="_init"></param>
    ///// <returns></returns>
    //public static T New<T>(object[] data) where T : ModuleEvent, new()
    //{
    //    var me = new T();
    //    if (data != null)
    //    {
    //        me.Init(data);
    //    }
    //    return me;
    //}
    /// <summary>
    /// 通过参数构造
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="_init"></param>
    /// <returns></returns>
    public static T New<T>(Action<T> _init=null) where T : ModuleEvent, new()
    {
        var me = new T();

        if (_init != null)
        {
            _init(me);
        }

        return me;
    }
    /// <summary>
    /// 将p_type转化为对应类型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="p_type"></param>
    /// <returns></returns>
    public static T GetType<T>(object p_type) where T : ModuleEvent
    {
        T a = (T)p_type;
        return a;
    }
}