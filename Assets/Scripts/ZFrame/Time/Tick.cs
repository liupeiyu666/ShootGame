using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using UnityEngine;


namespace ZLib
{
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
    /// </summary>
    static public class Tick
    {
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
        #endregion

        /// <summary>
        /// 刷帧
        /// 需要外部主动调用
        /// </summary>
        static public void Update()
        {
            //记录下时间
            _realDeltaTime = Time.realtimeSinceStartup - _realtimeSinceStartup;
            _realtimeSinceStartup = Time.realtimeSinceStartup;

            //更新Update
            //-------------------------------------------------------
            //ZLog.Log("_updateList:" + _updateList.Count);
            if (_updateList.Count > 0)
            {
                Action[] funArr = _updateList.ToArray();//使用数组副本，防止在下面的fun()回调中修改_updateList
                for (int i = 0; i < funArr.Length; i++)
                {
                    Action fun = funArr[i];
                    //if (!_updateList.Contains(fun)) continue;//检测存在性！！！ 这个消耗比较大，如果没必要后面去掉111111111111111111

                    //执行回调
			if(fun!=null){
                    		fun();
			}
                }
            }
            //-------------------------------------------------------

            //更新CallBack
            //-------------------------------------------------------
            //ZLog.Log("_cbdList:" + _cbdList.Count);
            if (_cbdList.Count > 0)
            {
                CallBackData[] cbdArr = _cbdList.ToArray();//使用数组副本，防止在下面的fun()回调中修改_cbdList
                for (int i = 0; i < cbdArr.Length; i++)
                {
                    CallBackData cbd = cbdArr[i];
                    if (!_cbdList.Contains(cbd)) continue;//检测存在性！！！ 这个消耗比较大，如果没必要后面去掉111111111111111111

                    //如果不需要延时或已达到时间间隔
                    if (cbd.interval == 0 || Tick.realTime - cbd.lastTime >= cbd.interval)
                    {
                        //执行次数加1
                        cbd.curRepeat++;
                        //如果达到执行次数要求则移除
                        if (cbd.repeat != 0 && cbd.curRepeat >= cbd.repeat)
                        {
                            //从字典中移除
                            _cbdList.Remove(cbd);
                        }
                        else
                        {
                            //更新执行时间
                            cbd.lastTime = Tick.realTime;
                        }

                        //执行回调
                        cbd.fun();
                    }
                }
            }
            //-------------------------------------------------------

            //更新TweenFloat
            //-------------------------------------------------------
            //ZLog.Log("_tfdList:" + _tfdList.Count);
            if (_tfdList.Count > 0)
            {
                TweenFloatData[] tfdArr = _tfdList.ToArray();//使用数组副本，防止在下面的fun()回调中修改_tfdList
                for (int i = 0; i < tfdArr.Length; i++)
                {
                    TweenFloatData tfd = tfdArr[i];
                    if (!_tfdList.Contains(tfd)) continue;//检测存在性！！！

                    //如果总时间已经达到
                    if (Tick.realTime - tfd.startTime >= tfd.duration)
                    {
                        //执行回调
                        if (tfd.onUpdate != null) tfd.onUpdate(tfd.to);
                        if (tfd.onComplete != null) tfd.onComplete();
                        //移除
                        _tfdList.Remove(tfd);
                    }
                    else
                    {
                        //更新from
                        tfd.curValue = tfd.from + (Tick.realTime - tfd.startTime) / tfd.duration * (tfd.to - tfd.from);

                        //如果不需要延时或已达到间隔
                        if (tfd.interval == 0 || tfd.curValue - tfd.lastValue >= tfd.interval)
                        {
                            //更新执行时间
                            tfd.lastTime = Tick.realTime;
                            tfd.lastValue = tfd.curValue;

                            //执行回调
                            if (tfd.onUpdate != null) tfd.onUpdate(tfd.curValue);
                        }
                    }
                }
            }
            //-------------------------------------------------------
        }
        /// <summary>
        /// 刷帧
        /// 需要外部主动调用
        /// </summary>
        static public void LateUpdate()
        {
            //更新LateUpdate
            //-------------------------------------------------------
            //ZLog.Log("_lateUpdateList:" + _lateUpdateList.Count);
            if (_lateUpdateList.Count > 0)
            {
                Action[] funArr = _lateUpdateList.ToArray();//使用数组副本，防止在下面的fun()回调中修改_lateUpdateList
                for (int i = 0; i < funArr.Length; i++)
                {
                    Action fun = funArr[i];
                    if (!_lateUpdateList.Contains(fun)) continue;//检测存在性！！！ 这个消耗比较大，如果没必要后面去掉111111111111111111

                    //执行回调
                    fun();
                }
            }
            //-------------------------------------------------------
        }


        //Update & LateUpdate & CallBack
        //======================================================================================================
        #region Update & LateUpdate & CallBack
        /// <summary>
        /// 延时回调数据
        /// </summary>
        class CallBackData
        {
            private static int ID;
            public int id;

            public Action fun;
            public float interval = 0;//延时或间隔多少秒（0，则每帧都刷，但不会立即执行，而是等到下一帧）
            public uint repeat = 0;//执行次数（0，则无限循环）

            public float lastTime = 0;//上一次执行的时间点
            public uint curRepeat = 0;//当前已经执行的次数

            public CallBackData()
            {
                id = ID++;
            }
        }

        //CallBackData列表
        static private List<CallBackData> _cbdList = new List<CallBackData>();
        //Update列表
        static private List<Action> _updateList = new List<Action>();
        //LateUpdate列表
        static private List<Action> _lateUpdateList = new List<Action>();
        /// <summary>
        /// 添加每帧更新回调
        /// </summary>
        /// <param name="__fun">回调</param>
        static public void AddUpdate(Action __fun)
        {
            if (_updateList.Contains(__fun)) return;

            _updateList.Add(__fun);
        }
        /// <summary>
        /// 添加每帧更新回调
        /// </summary>
        /// <param name="__fun">回调</param>
        static public void AddLateUpdate(Action __fun)
        {
            if (_lateUpdateList.Contains(__fun)) return;

            _lateUpdateList.Add(__fun);
        }
        /// <summary>
        /// 移除 每帧更新回调
        /// </summary>
        /// <param name="__fun"></param>
        static public void RemoveUpdate(Action __fun)
        {
            for (int i = _updateList.Count - 1; i >= 0; i--)
            {
                if (_updateList[i] == __fun)
                {
                    _updateList.RemoveAt(i);
                    break;
                }
            }
        }
        /// <summary>
        /// 移除 每帧更新回调
        /// </summary>
        /// <param name="__fun"></param>
        static public void RemoveLateUpdate(Action __fun)
        {
            for (int i = _lateUpdateList.Count - 1; i >= 0; i--)
            {
                if (_lateUpdateList[i] == __fun)
                {
                    _lateUpdateList.RemoveAt(i);
                    break;
                }
            }
        }
        /// <summary>
        /// 添加延时回调回调（不检查重复）
        /// </summary>
        /// <param name="__interval">延时多少秒(0，则每帧执行)</param>
        /// <param name="__fun">回调</param>
        static public int AddDelayCallback(float __interval, Action __fun)
        {
            return AddCallback(__fun, __interval, 1);
        }
        /// <summary>
        /// 添加延时回调回调（不检查重复）
        /// </summary>
        /// <param name="__fun">回调</param>
        /// <param name="__interval">延时多少秒(0，则每帧执行)</param>
        static public int AddDelayCallback(Action __fun, float __interval = 0)
        {
            return AddCallback(__fun, __interval, 1);
        }
        /// <summary>
        /// 添加回调（不检查重复）
        /// </summary>
        /// <param name="__fun">回调</param>
        /// <param name="__interval">延时多少秒(0，则每帧执行)</param>
        /// <param name="__repeat">执行次数（0，则无限循环）</param>
        static public int AddCallback(Action __fun, float __interval = 0, uint __repeat = 1)
        {
            CallBackData cbd = new CallBackData();
            cbd.fun = __fun;
            cbd.interval = __interval;
            cbd.repeat = __repeat;

            cbd.lastTime = Tick.realTime;//有误差，但也只能暂时这么做了
            //cbd.curRepeat = 0;

            _cbdList.Add(cbd);

            return cbd.id;
        }
        /// <summary>
        /// 移除回调（会移除关于此函数的所有回调）
        /// </summary>
        /// <param name="__fun"></param>
        static public void RemoveCallback(Action __fun)
        {
            for (int i = _cbdList.Count-1; i >=0; i--)
			{
                CallBackData cbd = _cbdList[i];
                if (cbd.fun == __fun)
                {
                    _cbdList.RemoveAt(i);
                    //break; 此处不执行break
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
                CallBackData cbd = _cbdList[i];
                if (cbd.id == __id)
                {
                    _cbdList.RemoveAt(i);
                    return;
                }
            }
        }
        #endregion
        //======================================================================================================

        //Tween
        //======================================================================================================
        #region Tween
        /// <summary>
        /// 数字滚动数据
        /// </summary>
        class TweenFloatData
        {
            static public int TWEEN_ID = 0;//自增唯一ID

            public int tweenID = ++TWEEN_ID;//自增唯一ID

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
        }

        //TweenFloatData列表
        static private List<TweenFloatData> _tfdList = new List<TweenFloatData>();
        /// <summary>
        /// 开始数字滚动
        /// </summary>
        /// <param name="__from">开始的值</param>
        /// <param name="__to">结束的值</param>
        /// <param name="__duration">时长 单位：秒</param>
        /// <param name="__onUpdate">进度回调 格式为onUpdate($per),$per为从$from到$to的过程中当前进行到的值</param>
        /// <param name="__onComplete">完成回调 格式为onComplete()</param>
        /// <param name="__interval">回调步伐(从$from到$to的过程中,每个多少回调一次，0则每帧都回调,注意不是时间间隔，而是从from到to的值间隔)</param>
        /// <returns>返回一个数字， 可以用这个数字来取消滚动</returns>
        static public int StartTweenFloat(float __from, float __to, float __duration,
            Action<float> __onUpdate = null, Action __onComplete = null, float __interval = 0)
        {
            TweenFloatData tfd = new TweenFloatData();
            tfd.from = __from;
            tfd.to = __to;
            tfd.duration = __duration;
            tfd.onUpdate = __onUpdate;
            tfd.onComplete = __onComplete;
            tfd.interval = __interval;

            tfd.startTime = Tick.realTime;//有误差，但也只能暂时这么做了
            //tfd.lastTime = 0;//这个和CallBackData不同
            tfd.lastValue = __from;
            tfd.curValue = __from;

            _tfdList.Add(tfd);

            return tfd.tweenID;
        }
        /// <summary>
        /// 移除数字滚动
        /// </summary>
        /// <param name="__tweenID"></param>
        static public void KillTweenFloat(int __tweenID)
        {
            for (int i = _tfdList.Count - 1; i >= 0; i--)
            {
                TweenFloatData tfd = _tfdList[i];
                if (tfd.tweenID == __tweenID)
                {
                    _tfdList.RemoveAt(i);
                    break;
                }
            }
        }
        #endregion
        //======================================================================================================

    }
}
