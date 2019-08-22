using System;
using System.Collections.Generic;

/// <summary>
/// @author Lpy
/// 关心每一步的指令，可以接受的情况：1.这一步的启动需要多个指令的完成才能开始，或者需要某一个指令的完成
/// 即可开始，在CheckStepCanStart()的完成
/// 不负责派发
/// </summary>
public abstract class BaseStep
{
    /// <summary>
    /// 需要接受的上一个节点的完成指令
    /// </summary>
    public List<string> m_BeforeStepEndCommandList { get; private set; }

    /// <summary>
    /// 需要开启的后续指令
    /// </summary>
    public List<string> m_NextStepsStartCommandList { get; private set; }

    private object m_startData;

    public BaseStep(List<string> p_BeforeStepEndCommandList, List<string> p_NextStepsStartCommandList)
    {
        m_BeforeStepEndCommandList = p_BeforeStepEndCommandList;
        m_NextStepsStartCommandList = p_NextStepsStartCommandList;
    }

    public virtual void InitData()
    {
    }

    public object GetData()
    {
        return m_startData;
    }
    /// <summary>
    /// 检测当前步骤是否可以开始
    /// </summary>
    /// <returns></returns>
    public abstract bool CheckStepCanStart();
}