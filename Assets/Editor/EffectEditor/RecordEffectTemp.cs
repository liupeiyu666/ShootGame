using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Effect;
using UnityEngine;
using Object = System.Object;

/// <summary>
/// 由于每次添加或者删除一个compnent的时候，editor脚本都会重新激活，导致数据刷新
/// </summary>
public class RecordEffectTemp
{
    public static Object m_currentObject;

    public static Type m_currentShowType;

    public static GameObject m_go;

    public static Dictionary<Type, Vector2> m_scrollIndexDic = new Dictionary<Type, Vector2>()
    {
        {typeof(BornRule), new Vector2()},
        {typeof(ServiveRule), new Vector2()},
        {typeof(DeadRule), new Vector2()},
        {typeof(MoveRule), new Vector2()},
    };

    public static void CheckChangeTarget(GameObject p_obj,Object p_currentObject, ref Dictionary<Type, Vector2> p_scrollIndexDic, ref Type p_currentShowType)
    {
        if (p_currentObject==m_currentObject)
        {
            p_scrollIndexDic = m_scrollIndexDic;
            p_currentShowType = m_currentShowType;
            p_scrollIndexDic[typeof(BornRule)]=Vector2.one;
        }
    }

    public static void SaveData(GameObject p_obj,Object p_currentObject,Dictionary<Type, Vector2> p_scrollIndexDic, Type p_currentShowType)
    {
        m_go = p_obj;
        m_currentObject = p_currentObject;
        m_currentShowType = p_currentShowType;
        m_scrollIndexDic = p_scrollIndexDic;
    }
}