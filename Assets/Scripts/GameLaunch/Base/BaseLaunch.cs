using  System;
using  System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ZFrame;

/// <summary>
/// @author Lpy
/// 启动流程的基类，主要负责游戏的各个模块之间的启动顺序
/// </summary>
public abstract class BaseLaunch
{
    public BaseLaunch()
    {
        AddLaunchStep();
    }

    public  List<string> ListenModuleEvents()
    {
        List<string> t_listenList=new List<string>();
        if (m_launchStepList.Count!=0)
        {

            for (int i = 0; i < m_launchStepList.Count; i++)
            {
                for (int j = 0; j < m_launchStepList[i].m_BeforeStepEndCommandList.Count; j++)
                {
                  
                    t_listenList.Add(m_launchStepList[i].m_BeforeStepEndCommandList[j]);
                    m_listendStepCommandDic.Add(m_launchStepList[i].m_BeforeStepEndCommandList[j], m_launchStepList[i]);
                }
            }

        }
       
        return t_listenList;
    }

    public void ReceivedModuleEvent(string __key, object __data)
    {
       //接受到监听的指令后发布下一步的内容，并且附带初始的数据,找到含有这个指令的BaseStep
       if (m_listendStepCommandDic.ContainsKey(__key))
       {
           BaseStep t_temp = m_listendStepCommandDic[__key];
           if (t_temp.CheckStepCanStart())
           {
               //发布后续
               for (int i = 0; i < t_temp.m_NextStepsStartCommandList.Count; i++)
               {
                   Frame.DispatchEvent(t_temp.m_NextStepsStartCommandList[i],t_temp.GetData());
                }
           }
       }

    }
    #region 启动流程数据
    /// <summary>
    /// 启动的步骤列表
    /// </summary>
    protected List<BaseStep> m_launchStepList = new List<BaseStep>();
    /// <summary>
    /// 存储监听的指令，和对应的step
    /// </summary>
    private Dictionary<string,BaseStep> m_listendStepCommandDic=new Dictionary<string, BaseStep>();
    #endregion

    /// <summary>
    /// 添加命令
    /// </summary>
    /// <param name="p_StepStartCommand">启动命令</param>
    /// <param name="p_StepEndCommand">结束时的指令</param>
    public abstract void AddLaunchStep();

    public abstract void Start();

}