using System;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 简单的全局TICK(精度为刷一帧的时间)(静态)
/// 含数字滚动,刷帧回调 和 延时回调
/// 
//类似这样使用：
//添加回调
//Tick.AddCallback(scene.Dispose, 5, 1);
//移除回调
//Tick.RemoveCallback(scene.Dispose);

//添加数字滚动
//Action<float> onUpdate = (___per) =>
//{
//    ZLog.Log(___per);
//};
//Action onComplete = () =>
//{
//    ZLog.Log("ok");
//};
//uint tweenID = Tick.StartTweenFloat(50,100,10,onUpdate,onComplete,3);
//移除数字滚动
//Tick.KillTweenFloat(tweenID);
///删除ToArray操作，tick机制自身0GC.加入了复用机制。@cmy 20180604
/// </summary>
static public class TTicker
{
    /// <summary>
    /// 全局时间控制
    /// </summary>
    public static float timeScale
    {
        set
        {
            Time.timeScale = value;

            if (value == 0)
                throw new Exception("不能设置为0");
            _reciprocalTimeScale = 1 / Time.timeScale;
        }

        get { return Time.timeScale; }
    }

    private static float _reciprocalTimeScale = 1;

    /// <summary>
    /// TimeScale的倒数 每次都计算好了的.
    /// </summary>
    public static float reciprocalTimeScale
    {
        get
        {
            return _reciprocalTimeScale;
        }
    }

    #region realTime
    /// <summary>
    /// 是使用Time.realtimeSinceStartup 还是Time.time
    /// 默认为true
    /// </summary>
    public static bool useRealtimeSinceStartup = true;
    private static float _realtimeSinceStartup = 0f;
    private static float _realDeltaTime = 0f;
    /// <summary>
    /// 获取时间
    /// 如果useRealtimeSinceStartup为true，则返回Time.realtimeSinceStartup
    /// 否则返回Time.time
    /// </summary>
    public static float realTime
    {
        get
        {
            if (useRealtimeSinceStartup) return Time.realtimeSinceStartup;
            else return Time.time;
        }
    }
    /// <summary>
    /// 获取每帧时间
    /// 如果useRealtimeSinceStartup为true，则返回真是的本次刷帧和上次刷帧的时间差
    /// 否则返回Time.deltaTime
    /// </summary>
    public static float realDeltaTime
    {
        get
        {
            if (useRealtimeSinceStartup) return _realDeltaTime;
            else return Time.deltaTime;
        }
    }
    /// <summary>
    /// 用realtimeSinceStartup算的差值.
    /// </summary>
    public static float DeltaTimeSinceStartUp
    {
        get
        {
            return _realDeltaTime;
        }
    }
    /// <summary>
    /// 获取固定刷帧间隔时间,  封装一下 Time.fixedDeltaTime
    /// </summary>
    public static float fixDeltaTime
    {
        get
        {
            return Time.fixedDeltaTime;
        }
    }
    #endregion

    class TickItem : ReuseItem
    {
        long _tickDoFlag = -1;

        public override void OnInit()
        {
            base.OnInit();

            //刚加进来的，设置为当前frameCount，最终，如果在一个fun里，如果增删了另外的fun，则当前此update，忽略刚加进来的。
            //@cmy 20180604
            //有可能在线程里AddUpdate操作，因此这里不能调用Time，改为静态变量
            _tickDoFlag = TTicker._tickDoFlag;
        }

        protected virtual void Do()
        {
        }

        public void CheckDo()
        {
            if (this._tickDoFlag != TTicker._tickDoFlag)
            {
                this._tickDoFlag = TTicker._tickDoFlag;
                Do();
            }
        }
    }

    class TickActionItem : TickItem
    {
        public Action Fun;

        protected override void Do()
        {
            if (Fun != null)
            {
                Fun();
            }
        }

        public override void OnRecycle()
        {
            base.OnRecycle();
            Fun = null;
        }
    }

    class TickCallBackItem : TickItem
    {
        public CallBackData Data;

        protected override void Do()
        {
            if (Data != null)
            {
                var selfTime = Data.ignoreTimeScale ? _realtimeSinceStartup : Time.time;
                //如果不需要延时或已达到时间间隔
                if (Data.interval == 0 || selfTime - Data.lastTime >= Data.interval)
                {
                    //执行次数加1
                    Data.curRepeat++;

                    //执行回调
                    Data.Execute();

                    if (!this.IsFree)//Data.Execute可能触发Recyle,这里拦截。
                    {
                        //如果达到执行次数要求则移除
                        if (Data.repeat != 0 && Data.curRepeat >= Data.repeat)
                        {
                            //从字典中移除
                            _cbdList.Recycle(this);
                        }
                        else
                        {
                            //更新执行时间
                            Data.lastTime = selfTime;
                        }
                    }
                }
            }
        }
    }

    class TickTweenFloatItem : TickItem
    {
        public TweenFloatData Data;

        protected override void Do()
        {
            if (!this.IsFree && Data != null)
            {
                var selfTime = Data.ignoreTimeScale ? _realtimeSinceStartup : Time.time;

                //如果总时间已经达到
                if (selfTime - Data.startTime >= Data.duration)
                {
                    //执行回调
                    if (Data.onUpdate != null) Data.onUpdate(Data.to);
                    if (Data.onComplete != null) Data.onComplete();
                    //移除
                    _tweenFloatList.Recycle(this);
                }
                else
                {
                    //更新from
                    Data.curValue = Data.from + (selfTime - Data.startTime) / Data.duration * (Data.to - Data.from);

                    //如果不需要延时或已达到间隔
                    if (Data.interval == 0 || Data.curValue - Data.lastValue >= Data.interval)
                    {
                        //更新执行时间
                        Data.lastTime = selfTime;
                        Data.lastValue = Data.curValue;

                        //执行回调
                        if (Data.onUpdate != null) Data.onUpdate(Data.curValue);
                    }
                }
            }
        }
    }

    class TickItemList<T> : ReuseList<T> where T : TickItem, new()
    {
        public void Do()
        {
            for (int i = 0; i < Count; i++)
            {
                var tickItem = this[i];
                if (!tickItem.IsFree)
                {
                    tickItem.CheckDo();

                    //如果是因为执行了Item，导致了已经执行过的Item索引有变更，那么就重新遍历执行
                    if (Count > i)
                    {
                        if (this[i] != tickItem)
                        {
                            Do();
                            return;
                        }
                    }
                    else
                    {
                        Do();
                        return;
                    }
                }
            }
        }
    }

    static long _tickDoFlag = 0;
    static TickItemList<TickCallBackItem> _cbdList = new TickItemList<TickCallBackItem>();
    static TickItemList<TickActionItem> _updateList = new TickItemList<TickActionItem>();
    static TickItemList<TickActionItem> _lateUpdateList = new TickItemList<TickActionItem>();
    static TickItemList<TickActionItem> _fixUpdateList = new TickItemList<TickActionItem>();
    static TickItemList<TickTweenFloatItem> _tweenFloatList = new TickItemList<TickTweenFloatItem>();

    /// <summary>
    /// 刷帧
    /// 需要外部主动调用
    /// </summary>
    static public void tUpdate()
    {
        //记录下时间
        _realDeltaTime = Time.realtimeSinceStartup - _realtimeSinceStartup;
        _realtimeSinceStartup = Time.realtimeSinceStartup;

        _tickDoFlag++;
        _updateList.Do();
        _cbdList.Do();
        _tweenFloatList.Do();
    }

    /// <summary>
    /// 刷帧
    /// 需要外部主动调用
    /// </summary>
    static public void tLateUpdate()
    {
        _tickDoFlag++;
        _lateUpdateList.Do();
    }

    /// <summary>
    /// 固定频率刷帧
    /// 需要外部主动调用
    /// </summary>
    static public void FixedUpdate()
    {
        _tickDoFlag++;
        _fixUpdateList.Do();
    }


    //Update & LateUpdate & CallBack
    //======================================================================================================
    #region Update & LateUpdate & CallBack

    class CallBackDataComparer : IComparer<CallBackData>
    {
        public static CallBackDataComparer Default = new CallBackDataComparer();
        public int Compare(CallBackData x, CallBackData y)
        {
            return x.id - y.id;
        }
    }
    class ActionComparer : IComparer<Action>
    {
        public static ActionComparer Default = new ActionComparer();
        public int Compare(Action x, Action y)
        {
            var hashcode = x.GetHashCode() - y.GetHashCode();

            if (x == y && hashcode != 0)
                Debug.LogError("Error hashcode 重要错误.");

            return hashcode;
        }
    }
    /// <summary>
    /// 延时回调数据
    /// 这个class有个坑：被泛型类继承了，导致这个类不可以如池！！！这里以后是个优化点。@cmy 20180606.
    /// </summary>
    class CallBackData
    {
        protected static int ID;
        public int id;

        public Action fun;
        public float interval = 0;//延时或间隔多少秒（0，则每帧都刷，但不会立即执行，而是等到下一帧）
        public uint repeat = 0;//执行次数（0，则无限循环）

        public float lastTime = 0;//上一次执行的时间点
        public uint curRepeat = 0;//当前已经执行的次数

        public bool ignoreTimeScale = true;

        public CallBackData()
        {
            id = ID++;
        }

        virtual internal void Execute()
        {
            fun();
        }
    }

    /// <summary>
    /// 延迟回调带参数版本 避免闭包
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class CallBackData<T> : CallBackData
    {
        public new Action<T> fun;

        public T arg1;

        public CallBackData()
        {
            id = ID++;
        }

        internal override void Execute()
        {
            if (fun != null)
                fun(arg1);
        }
    }
    /// <summary>
    /// 延迟回调带参数版本 避免闭包
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="U"></typeparam>
    class CallBackData<T, U> : CallBackData
    {
        public new Action<T, U> fun;

        public T arg1;

        public U arg2;

        public CallBackData()
        {
            id = ID++;
        }

        internal override void Execute()
        {
            if (fun != null)
                fun(arg1, arg2);
        }

    }

    /// <summary>
    /// 延迟回调带参数版本 避免闭包
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="U"></typeparam>
    /// <typeparam name="V"></typeparam>
    class CallBackData<T, U, V> : CallBackData
    {
        public new Action<T, U, V> fun;

        public T arg1;

        public U arg2;

        public V arg3;

        public CallBackData()
        {
            id = ID++;
        }

        internal override void Execute()
        {
            if (fun != null)
                fun(arg1, arg2, arg3);
        }
    }

    /// <summary>
    /// 添加每帧更新回调
    /// </summary>
    /// <param name="__fun">回调</param>
    static public void AddUpdate(Action __fun)
    {
        for (int i = 0; i < _updateList.Count; i++)
        {
            if (_updateList[i].Fun == __fun)
            {
                return;
            }
        }

        TickActionItem item = _updateList.GetFreeItem();
        item.Fun = __fun;
    }
    /// <summary>
    /// 添加每帧更新回调
    /// </summary>
    /// <param name="__fun">回调</param>
    static public void AddLateUpdate(Action __fun)
    {
        for (int i = 0; i < _lateUpdateList.Count; i++)
        {
            if (_lateUpdateList[i].Fun == __fun)
            {
                return;
            }
        }

        TickActionItem item = _lateUpdateList.GetFreeItem();
        item.Fun = __fun;
    }
    /// <summary>
    /// 添加固定频率刷帧更新回调
    /// </summary>
    /// <param name="__fun">回调</param>
    static public void AddFixUpdate(Action __fun)
    {
        for (int i = 0; i < _fixUpdateList.Count; i++)
        {
            if (_fixUpdateList[i].Fun == __fun)
            {
                return;
            }
        }

        TickActionItem item = _fixUpdateList.GetFreeItem();
        item.Fun = __fun;
    }

    /// <summary>
    /// 移除 每帧更新回调
    /// </summary>
    /// <param name="__fun"></param>
    static public void RemoveUpdate(Action __fun)
    {
        for (int i = 0; i < _updateList.Count; i++)
        {
            if (_updateList[i].Fun == __fun)
            {
                _updateList.RecycleAt(i);
                return;
            }
        }
    }

    /// <summary>
    /// 移除 每帧更新回调
    /// </summary>
    /// <param name="__fun"></param>
    static public void RemoveLateUpdate(Action __fun)
    {
        for (int i = 0; i < _lateUpdateList.Count; i++)
        {
            if (_lateUpdateList[i].Fun == __fun)
            {
                _lateUpdateList.RecycleAt(i);
                return;
            }
        }
    }
    /// <summary>
    /// 移除 固定刷帧更新回调
    /// </summary>
    /// <param name="__fun"></param>
    static public void RemoveFixUpdate(Action __fun)
    {
        for (int i = 0; i < _fixUpdateList.Count; i++)
        {
            if (_fixUpdateList[i].Fun == __fun)
            {
                _fixUpdateList.RecycleAt(i);
                return;
            }
        }
    }

    /// <summary>
    /// 添加延时回调回调（不检查重复）
    /// </summary>
    /// <param name="__interval">延时多少秒(0，则每帧执行)</param>
    /// <param name="__fun">回调</param>
    /// <param name="ignoreTimeScale">是否忽略时间变换</param>
    static public int AddDelayCallback(float __interval, Action __fun, bool ignoreTimeScale = true)
    {
        return AddCallback(__fun, __interval, 1, ignoreTimeScale);
    }

    /// <summary>
    /// 添加延时回调回调（不检查重复）
    /// </summary>
    /// <param name="__fun">回调</param>
    /// <param name="__interval">延时多少秒(0，则每帧执行)</param>
    /// <param name="ignoreTimeScale">是否忽略时间变换</param>
    static public int AddDelayCallback(Action<System.Object> __fun, float __interval = 0, System.Object __theParams = null, bool ignoreTimeScale = true)
    {
        return AddCallback(__fun, __theParams, __interval, 1, ignoreTimeScale);
    }
    /// <summary>
    /// 添加回调（不检查重复）
    /// </summary>
    /// <param name="__fun">回调</param>
    /// <param name="__interval">延时多少秒(0，则每帧执行)</param>
    /// <param name="__repeat">执行次数（0，则无限循环）</param>
    /// <param name="ignoreTimeScale">是否忽略时间变换</param>
    static public int AddCallback(Action __fun, float __interval = 0, uint __repeat = 1, bool ignoreTimeScale = true)
    {
        TickCallBackItem item = _cbdList.GetFreeItem();
        item.Data = new CallBackData();
        CallBackData cbd = item.Data;

        cbd.fun = __fun;
        cbd.interval = __interval;
        cbd.repeat = __repeat;
        cbd.ignoreTimeScale = ignoreTimeScale;

        cbd.lastTime = cbd.ignoreTimeScale ? Time.realtimeSinceStartup : Time.time;//有误差，但也只能暂时这么做了
        //cbd.curRepeat = 0;
        return cbd.id;
    }

    /// <summary>
    /// 带参数的回调 避免出现闭包现象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="__fun"></param>
    /// <param name="arg1"></param>
    /// <param name="__interval"></param>
    /// <param name="__repeat"></param>
    /// <param name="ignoreTimeScale">是否忽略时间变换</param>
    /// <returns></returns>
    static public int AddCallback<T>(Action<T> __fun, T arg1, float __interval = 0, uint __repeat = 1, bool ignoreTimeScale = true)
    {
        var cbd = new CallBackData<T>();
        cbd.arg1 = arg1;
        cbd.fun = __fun;
        cbd.interval = __interval;
        cbd.repeat = __repeat;
        cbd.ignoreTimeScale = ignoreTimeScale;

        cbd.lastTime = ignoreTimeScale ? Time.realtimeSinceStartup : Time.time;//有误差，但也只能暂时这么做了

        TickCallBackItem item = _cbdList.GetFreeItem();
        item.Data = cbd;

        return cbd.id;
    }

    /// <summary>
    /// 带参数的回调 避免出现闭包现象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="U"></typeparam>
    /// <param name="__fun"></param>
    /// <param name="arg1"></param>
    /// <param name="arg2"></param>
    /// <param name="__interval"></param>
    /// <param name="__repeat"></param>
    /// <param name="ignoreTimeScale">是否忽略时间变换</param>
    /// <returns></returns>
    static public int AddCallback<T, U>(Action<T, U> __fun, T arg1, U arg2, float __interval = 0, uint __repeat = 1, bool ignoreTimeScale = true)
    {
        var cbd = new CallBackData<T, U>();
        cbd.arg1 = arg1;

        cbd.arg2 = arg2;
        cbd.fun = __fun;
        cbd.interval = __interval;
        cbd.repeat = __repeat;
        cbd.ignoreTimeScale = ignoreTimeScale;

        cbd.lastTime = ignoreTimeScale ? Time.realtimeSinceStartup : Time.time;//有误差，但也只能暂时这么做了
        //cbd.curRepeat = 0;
        TickCallBackItem item = _cbdList.GetFreeItem();
        item.Data = cbd;

        return cbd.id;
    }

    /// <summary>
    /// 带参数的回调 避免出现闭包现象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="U"></typeparam>
    /// <typeparam name="V"></typeparam>
    /// <param name="__fun"></param>
    /// <param name="arg1"></param>
    /// <param name="arg2"></param>
    /// <param name="arg3"></param>
    /// <param name="__interval"></param>
    /// <param name="__repeat"></param>
    /// <param name="ignoreTimeScale">是否忽略时间变换</param>
    /// <returns></returns>
    static public int AddCallback<T, U, V>(Action<T, U, V> __fun, T arg1, U arg2, V arg3, float __interval = 0, uint __repeat = 1, bool ignoreTimeScale = true)
    {
        var cbd = new CallBackData<T, U, V>();
        cbd.arg1 = arg1;
        cbd.arg2 = arg2;
        cbd.arg3 = arg3;
        cbd.fun = __fun;
        cbd.interval = __interval;
        cbd.repeat = __repeat;
        cbd.ignoreTimeScale = ignoreTimeScale;

        cbd.lastTime = ignoreTimeScale ? Time.realtimeSinceStartup : Time.time;//有误差，但也只能暂时这么做了
        //cbd.curRepeat = 0;
        TickCallBackItem item = _cbdList.GetFreeItem();
        item.Data = cbd;

        return cbd.id;
    }

    /// <summary>
    /// 移除回调（会移除关于此函数的所有回调）
    /// </summary>
    /// <param name="__fun"></param>
    static public void RemoveCallback(Action __fun)
    {
        for (int i = _cbdList.Count - 1; i >= 0; i--)
        {
            var tickCallBackItem = _cbdList[i];
            if (tickCallBackItem.Data != null && tickCallBackItem.Data.fun == __fun)
            {
                _cbdList.Recycle(tickCallBackItem);
                //break; 此处不执行break。
            }
        }
    }

    /// <summary>
    /// 移除回调
    /// </summary>
    /// <param name="__id"></param>
    static public void RemoveCallback(int __id)
    {
        for (int i = _cbdList.Count - 1; i >= 0; i--)
        {
            var tickCallBackItem = _cbdList[i];
            if (tickCallBackItem.Data != null && tickCallBackItem.Data.id == __id)
            {
                _cbdList.Recycle(tickCallBackItem);
            }
        }
    }
    #endregion

    #region Tween
    /// <summary>
    /// 数字滚动数据
    /// </summary>
    class TweenFloatData
    {
        static public int TWEEN_ID = 0;//自增唯一ID

        //public int tweenID = ++TWEEN_ID;//自增唯一ID
        public int tweenID;

        public float from;
        public float to;
        public float duration;
        public Action<float> onUpdate;
        public Action onComplete;
        public float interval;

        public float startTime = 0;//添加的时间
        public float lastTime = 0;//上一次执行的时间点
        public float lastValue = 0;//上一次的值
        public float curValue = 0;//当前值
        public bool ignoreTimeScale = true;

        public void Init()
        {
            tweenID = ++TWEEN_ID;//自增唯一ID

            from = 0;
            to = 0;
            duration = 0;

            onUpdate = null;
            onComplete = null;
            interval = 0;

            startTime = 0;
            lastTime = 0;
            lastValue = 0;
            curValue = 0;
            ignoreTimeScale = true;
        }
    }

    /// <summary>
    /// 开始数字滚动
    /// </summary>
    /// <param name="__from">开始的值</param>
    /// <param name="__to">结束的值</param>
    /// <param name="__duration">时长 单位：秒</param>
    /// <param name="__onUpdate">进度回调 格式为onUpdate($per),$per为从$from到$to的过程中当前进行到的值</param>
    /// <param name="__onComplete">完成回调 格式为onComplete()</param>
    /// <param name="__interval">回调步伐(从$from到$to的过程中,每个多少回调一次，0则每帧都回调,注意不是时间间隔，而是从from到to的值间隔)</param>
    /// <param name="ignoreTimeScale">是否忽略时间变换</param>
    /// <returns>返回一个数字， 可以用这个数字来取消滚动</returns>
    static public int StartTweenFloat(float __from, float __to, float __duration,
        Action<float> __onUpdate = null, Action __onComplete = null, float __interval = 0, bool ignoreTimeScale = true)
    {
        TickTweenFloatItem item = _tweenFloatList.GetFreeItem();
        TweenFloatData tfd = item.Data;
        if (tfd == null)
        {
            tfd = new TweenFloatData();
            item.Data = tfd;
        }

        tfd.Init();

        tfd.from = __from;
        tfd.to = __to;
        tfd.duration = __duration;
        tfd.onUpdate = __onUpdate;
        tfd.onComplete = __onComplete;
        tfd.interval = __interval;
        tfd.ignoreTimeScale = ignoreTimeScale;

        tfd.startTime = TTicker.realTime;//有误差，但也只能暂时这么做了
        //tfd.lastTime = 0;//这个和CallBackData不同
        tfd.lastValue = __from;
        tfd.curValue = __from;

        return tfd.tweenID;
    }

    /// <summary>
    /// 移除数字滚动
    /// </summary>
    /// <param name="__tweenID"></param>
    static public void KillTweenFloat(int __tweenID)
    {
        for (int i = _tweenFloatList.Count - 1; i >= 0; i--)
        {
            var item = _tweenFloatList[i];
            if (item.Data != null && item.Data.tweenID == __tweenID)
            {
                _tweenFloatList.Recycle(item);
                break;
            }
        }
    }
    #endregion
}
