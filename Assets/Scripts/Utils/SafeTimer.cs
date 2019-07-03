using System; 
using UnityEngine;
using System.Collections.Generic;

///<summary>
/// 每帧执行，时间不是非常精确
/// 一个安全的计时器，被每帧更新,定时结束则返回true.
/// 用法:
/// SafeTimer tt=new SafeTimer();
/// tt.Start(2);
/// 
/// if(tt.IsOK()){//注意这个是每帧调用
/// }
/// Fiexd lpy 2017.10.11 将计时器中使用实时的时间去做，因为使用统一的计时会导致时间上的不一致 UGEUtilsV2.realtimeSinceStartup代替为Time.realtimeSinceStartup
/// </summary>
public class SafeTimer {

    public class TimePauseParams
    {
        public float timeScaleChangeTime;

        public  float timeScale = 1;
    }
    /// <summary>
    /// 存储所有的关于timescale变化的数据
    /// </summary>
    public static List<TimePauseParams> timePauseList = new List<TimePauseParams>() { new TimePauseParams() { timeScaleChangeTime=0 } };
    /// <summary>
    /// 获取从计时开始到现在已经持续了多长时间,会根据时间系数作出对应的转换
    /// </summary>
    /// <returns></returns>
    private float GetDuringTime()
    {
        //如果没有暂停过，直接返回差值即可
        if (!isUseTimeScale)
        {
            return Time.realtimeSinceStartup - startTimeSec;
        }
        //返回的总的时间长度
        float totalTime=0;
        //用于记录上一次的时间变化的时间
        float recordChangeTimeScale=Time.realtimeSinceStartup;
        for (int i = timePauseList.Count-1; i >= 0; i--)
        {
            TimePauseParams tp = timePauseList[i];
            //1.查找出开始计时以后所有时间系数的变化，包括开始计数时的时间
            if (startTimeSec <= tp.timeScaleChangeTime)
            {
                //获取此次系数改变相对的时间---比如，改变timescal为0.5时，过了2s，按照正常的转换应该为1s，如果为2，过了两秒，相当于过了4秒
                totalTime += (recordChangeTimeScale - tp.timeScaleChangeTime) * tp.timeScale;
                //记录此次计算的值
                recordChangeTimeScale = tp.timeScaleChangeTime;
            }
            else
            {
                //因为使用倒叙计时，所以如果第一次到了这里就是这个timer开始计时的时候的timescale，在计算一次即可
                totalTime += (recordChangeTimeScale - startTimeSec) * tp.timeScale;
                break;
            }
        }
        return totalTime;
    }

	float durSec;//倒计时持续时间
	
	float startTimeSec;//开始的时间
	
	bool isRunning = false;
    /// <summary>
    /// 用于标识是否伴随timescale来改变时间
    /// </summary>
    bool isUseTimeScale = true;

    public SafeTimer(bool p_isUseTimeScale = true)
    {
		isRunning = true;
        isUseTimeScale = p_isUseTimeScale;
    }
    /// <summary>
    /// Add Lpy 用于创建一个timer，如果不开始计时则认为计时器没有开始生效
    /// </summary>
    /// <param name="isRun"></param>
    public SafeTimer(bool p_isRun,bool p_isUseTimeScale=true)
    {
        isRunning = p_isRun;
        isUseTimeScale = p_isUseTimeScale;
    }
    public SafeTimer(float durSec, bool p_isUseTimeScale = true)
	{
        isUseTimeScale = p_isUseTimeScale;
        Start(durSec);
	}
	
	public void Start(float durSec){
		this.durSec=durSec;
        //startTimeSec = UGEUtilsV2.realtimeSinceStartup;//0307 KK改
        startTimeSec = Time.realtimeSinceStartup;
		isRunning=true;
	}

    /// <summary>
    /// 用的时候检查
    /// </summary>
    /// <returns></returns>
	public bool IsOK(){
        if (!isRunning)
        {
            return false;
        }

        //if (UGEUtilsV2.realtimeSinceStartup - startTimeSec >= durSec)
        //{
        //    return true;
        //}
        if (GetDuringTime() >= durSec)
        {
            return true;
        }
        //if (Time.realtimeSinceStartup - startTimeSec >= durSec)
        //{
        //    return true;
        //}
		return false;
	}

    /// <summary>
    /// 获得剩余的冷却时间
    /// </summary>
    /// <returns></returns>
    public float SurplusTime()
    {
      //  return startTimeSec + durSec - UGEUtilsV2.realtimeSinceStartup;
        return startTimeSec-GetDuringTime();
      //  return startTimeSec + durSec - Time.realtimeSinceStartup;
    }

	/// <summary>
	/// 剩余时间百分比
	/// </summary>
	/// <returns>The time percentage.</returns>
	public float Percentage()
	{
        // float passedTime = UGEUtilsV2.realtimeSinceStartup - startTimeSec;
        float passedTime = GetDuringTime();
       // float passedTime = Time.realtimeSinceStartup - startTimeSec;
		return UGEUtils.SafeDivide(passedTime,durSec);
	}

	/// <summary>
	/// 重置时间
	/// </summary>
	public void Reset()
	{
		Start(durSec);
	}

    /// <summary>
    /// 停止计时
    /// </summary>
    public void Stop()
    {
        isRunning = false;
    }

	/// <summary>
	/// 设置计时完成,苍手用的，奇怪的用法，为了兼容而保利 kk+
	/// </summary>
	public void SetTimeOK()
	{
		isRunning = true;
		startTimeSec = 0;
	}
    /// <summary>
    /// Add Lpy 这里获取当前计时器是否在进行计时
    /// </summary>
    /// <returns></returns>
    public bool IsRunning()
    {
        return isRunning;
    }
}//end of class