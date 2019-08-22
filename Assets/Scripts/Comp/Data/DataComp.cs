using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 数据存储的容器，这里也是做容器的方式。不同的数据结构会对应不同的数据组装的方式
/// </summary>
public class DataComp : SpriteComp {

	#region 基本
	public override void OnAdded(){
		//创建的时候被赋值
	}

	public override void OnRemoved(){
	
	}
    #endregion

    #region 数据容器

    /// <summary>
    /// 所有的组件容器，这里我们限定同一类型只能加一个，因为目前来看，加多个的需求暂时没有
    /// 仿照unity的组件式开发的方法
    /// </summary>
    private Dictionary<Type, BaseData> m_allData = new Dictionary<Type, BaseData>();
    /// <summary>
    /// 添加comp，当有这个组件的时候，直接返回这个组件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T AddData<T>(T p_data=null) where T : BaseData, new()
    {
        Type t_type = typeof(T);
        if (p_data!=null)
        {
            if (m_allData.ContainsKey(t_type))
            {
                m_allData[t_type]= p_data;
            }
            else
            {
                m_allData.Add(t_type,p_data);
            }
            return p_data;
        }
        else
        {
            if (m_allData.ContainsKey(t_type))
            {
                return (T)m_allData[t_type];
            }
            T t_temp = new T();
            m_allData.Add(t_type, t_temp);
            return t_temp;
        }
       
    }
    public void RemoveData<T>() where T : BaseData, new()
    {
        if (m_allData.ContainsKey(typeof(T)))
        {
            m_allData.Remove(typeof(T));
        }
    }
    /// <summary>
    /// 获取组件，如果没有是否需创建
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="p_noneCreat"></param>
    /// <returns></returns>
    public T GetData<T>(bool p_noneCreat = false) where T : BaseData, new()
    {
        if (m_allData.ContainsKey(typeof(T)))
        {
            return (T)m_allData[typeof(T)];
        }
        else
        {
            if (p_noneCreat)
            {
                return AddData<T>();
            }
            return null;
        }
    }

    #endregion
}
