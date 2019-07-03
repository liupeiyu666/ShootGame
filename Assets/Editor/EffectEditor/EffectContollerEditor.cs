using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Engine.Effect;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;


[CustomEditor(typeof(EffectController))]
public class EffectContollerEditor : Editor
{
    //整个面板只有四个BOX和一个横排的BOX,
    //  出生规则   生存规则  死亡规则  运动规则
    //  
    private EffectController m_target;

    private SerializedObject m_object;

    private Texture2D m_lable;
    /// <summary>
    /// 游戏中所有的规则
    /// </summary>
    private Dictionary<Type,List<Type>> m_allRulesDic=new Dictionary<Type, List<Type>>()
    {
        {typeof(BornRule),new List<Type>() },
        {typeof(ServiveRule),new List<Type>() },
        {typeof(DeadRule),new List<Type>() },
        {typeof(MoveRule),new List<Type>() },
        {typeof(ExpandRule),new List<Type>()  }
    };
    /// <summary>
    /// 已经添加上去的规则
    /// </summary>
    private Dictionary<Type, List<BaseRule>> m_addedRulesDic = new Dictionary<Type, List<BaseRule>>()
    {
        {typeof(BornRule),new List<BaseRule>() },
        {typeof(ServiveRule),new List<BaseRule>() },
        {typeof(DeadRule),new List<BaseRule>() },
        {typeof(MoveRule),new List<BaseRule>() },
        {typeof(ExpandRule),new List<BaseRule>() },
    };
    Dictionary<Type, Vector2> m_scrollIndexDic = new Dictionary<Type, Vector2>()
    {
        {typeof(BornRule),new Vector2() },
        {typeof(ServiveRule),new Vector2()  },
        {typeof(DeadRule),new Vector2()  },
        {typeof(MoveRule),new Vector2()  },
        {typeof(ExpandRule),new Vector2() },
    };
    void OnEnable()
    {
        if (target != null&&m_target==null)
        {
            m_target = target as EffectController;
            m_lable = Resources.Load<Texture2D>("EffectLable");
           // target.name = "我靠";
            InitAllRules();
            GetAddedRules();
            RecordEffectTemp.CheckChangeTarget(m_target.gameObject,target,ref m_scrollIndexDic,ref m_currentShowType);
        }
      
    }

    void OnDisable()
    {
        if (m_target!=null)
        {
            RecordEffectTemp.SaveData(m_target.gameObject, target, m_scrollIndexDic, m_currentShowType);
        }
       
    }

    void InitAllRules()
    {
        var types = from t in Assembly.GetAssembly(typeof(BaseRule)).GetTypes()
                    where
                        t.IsClass
                        && t.Namespace == "Engine.Effect"
                        && t.IsSubclassOf(typeof(BaseRule))
                    select t;
        foreach (var t_temp in types)
        {
            ClassifyRules(m_allRulesDic, t_temp);
        }
    }

    void GetAddedRules()
    {
        BaseRule[] t_ruleArry= m_target.gameObject.GetComponents<BaseRule>();
        for (int i = 0; i < t_ruleArry.Length; i++)
        {
            if (t_ruleArry[i] is BornRule)
            {
                m_addedRulesDic[typeof(BornRule)].Add(t_ruleArry[i]);
            }
            if (t_ruleArry[i] is ServiveRule)
            {
                m_addedRulesDic[typeof(ServiveRule)].Add(t_ruleArry[i]);
            }
            if (t_ruleArry[i] is DeadRule)
            {
                m_addedRulesDic[typeof(DeadRule)].Add(t_ruleArry[i]);
            }
            if (t_ruleArry[i] is MoveRule)
            {
                m_addedRulesDic[typeof(MoveRule)].Add(t_ruleArry[i]);
            }

        }
    }

    void ClassifyRules(Dictionary<Type, List<Type>> p_dic,Type p_type)
    {
        if (p_type.IsSubclassOf(typeof(BornRule)))
        {
            p_dic[typeof(BornRule)].Add(p_type);
        }
        else if (p_type.IsSubclassOf(typeof(ServiveRule)))
        {
            p_dic[typeof(ServiveRule)].Add(p_type);
        }
        else if (p_type.IsSubclassOf(typeof(DeadRule)))
        {
            p_dic[typeof(DeadRule)].Add(p_type);
        }
        else if (p_type.IsSubclassOf(typeof(MoveRule)))
        {
            p_dic[typeof(MoveRule)].Add(p_type);
        }
        else if (p_type.IsSubclassOf(typeof(ExpandRule)))
        {
            p_dic[typeof(ExpandRule)].Add(p_type);
        }
    }

    /// <summary>
    /// 是否处于编辑状态
    /// </summary>
    private bool m_isEditor;

    private Type m_currentShowType = typeof(BornRule);

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        using (EditorGUILayout.VerticalScope vScope = new EditorGUILayout.VerticalScope(GUILayout.Width(EditorGUIUtility.currentViewWidth)))
        {
            DrawHead();
            DrawBornBody(m_currentShowType);
        }
    }

    void DrawHead()
    {
        GUILayout.Label(m_lable,GUILayout.Height(EditorGUIUtility.singleLineHeight * 1.5f),GUILayout.Width(EditorGUIUtility.currentViewWidth));
        using (var scope = new EditorGUILayout.HorizontalScope(GUILayout.Height(EditorGUIUtility.singleLineHeight * 1.5f)))
        {
            GUI.backgroundColor = Color.green;

            GUI.Box(scope.rect, new GUIContent());
            GUI.backgroundColor = Color.white;
            GUI.backgroundColor = Color.blue;

            GUI.backgroundColor = m_currentShowType == typeof(BornRule) ? Color.gray : Color.blue;
            if (GUILayout.Button("出生规则+"))
            {
                m_currentShowType = typeof(BornRule);
            }
            GUI.backgroundColor = Color.white;
            GUI.backgroundColor = m_currentShowType == typeof(ServiveRule) ? Color.gray : Color.blue;
            if (GUILayout.Button("生存规则+"))
            {
                m_currentShowType = typeof(ServiveRule);
            }
            GUI.backgroundColor = Color.white;
            GUI.backgroundColor = m_currentShowType == typeof(DeadRule) ? Color.gray : Color.blue;
            if (GUILayout.Button("死亡规则+"))
            {
                m_currentShowType = typeof(DeadRule);
            }
            GUI.backgroundColor = Color.white;
            GUI.backgroundColor = m_currentShowType == typeof(MoveRule) ? Color.gray : Color.blue;
            if (GUILayout.Button("运动规则+"))
            {
                m_currentShowType = typeof(MoveRule);
            }
            GUI.backgroundColor = Color.white;
            GUI.backgroundColor = m_currentShowType == typeof(ExpandRule) ? Color.gray : Color.blue;
            if (GUILayout.Button("扩展规则+"))
            {
                m_currentShowType = typeof(ExpandRule);
            }
            GUI.backgroundColor = Color.white;

            GUI.backgroundColor = Color.blue;
            if (GUILayout.Button("重置"))
            {
                foreach (var t_ruleList in m_addedRulesDic.Values)
                {
                    for (int i = 0; i < t_ruleList.Count; i++)
                    {
                        DestroyImmediate(t_ruleList[i]);
                    }
                    t_ruleList.Clear();
                }
            }
            GUI.backgroundColor = Color.white;
        }
    }


    void DrawBornBody(Type p_type)
    {
        using (EditorGUILayout.VerticalScope vScope = new EditorGUILayout.VerticalScope(GUILayout.Width(EditorGUIUtility.currentViewWidth)))
        {
            GUI.backgroundColor = Color.red;
            GUI.Box(vScope.rect, new GUIContent());
            GUI.backgroundColor = Color.white;
            
            //列表展示所已经添加上的信息
            using (var scroll=new EditorGUILayout.ScrollViewScope(m_scrollIndexDic[p_type], GUILayout.Height(EditorGUIUtility.singleLineHeight * 8)))
            {
               
                m_scrollIndexDic[p_type] = scroll.scrollPosition;
                //一行3个，
                for (int i = 0; i < m_allRulesDic[p_type].Count; i++)
                {
                 
                    bool t_isContain = CheckContainType(m_allRulesDic[p_type][i]);
                    if (t_isContain)
                    {
                        GUI.backgroundColor = Color.yellow;
                    }
                    else
                    {
                        GUI.backgroundColor = Color.magenta;
                    }
                   
                    if (i%3==0)
                    {
                        GUILayout.BeginHorizontal();
                    }
                    //获取这个描述
                    Type t_myType = m_allRulesDic[p_type][i];

                    System.Attribute[] attrs = System.Attribute.GetCustomAttributes(t_myType);
                    string t_desName = "我是属性描述";
                    foreach (System.Attribute attr in attrs)
                    {
                        if (attr is Des)
                        {
                            Des a = (Des)attr;
                            t_desName=(a.m_des);
                        }
                    }

                    //object[] t_desArry= t_myType.GetCustomAttributes(typeof(Des), false);
                    //if (t_desArry.Length>=1)
                    //{
                    //    Debug.LogError(((Des)t_desArry[0]).m_des);
                    //}
                    if (GUILayout.Button(m_allRulesDic[p_type][i].Name + "\n"+ t_desName, GUILayout.Width(EditorGUIUtility.currentViewWidth / 3.3f), GUILayout.Height(EditorGUIUtility.singleLineHeight * 2)))
                    {
                        //如果存在，点击说明删除，如果不存在点击说明需要添加
                        if (t_isContain)
                        {
                            RemoveOneType(m_allRulesDic[p_type][i]);
                        }
                        else
                        {
                            AddOneType(m_allRulesDic[p_type][i]);
                        }
                    }
                    if (i % 3 == 2||i== m_allRulesDic[p_type].Count-1)
                    {
                        GUILayout.EndHorizontal();
                    }
                    GUI.backgroundColor = Color.white;
                }
               

            }
        }
    }
    /// <summary>
    /// 检测已经添加的类型中是否存在该类型
    /// </summary>
    /// <param name="p_type"></param>
    /// <returns></returns>
    bool CheckContainType(Type p_type)
    {
        foreach (var t_ruleList in m_addedRulesDic.Values)
        {
            for (int i = 0; i < t_ruleList.Count; i++)
            {
                if (p_type.IsInstanceOfType(t_ruleList[i]))
                {
                    return true;
                }
            }
        }
        return false;
    }
    /// <summary>
    /// 从已经添加的脚本中删除这个类型
    /// </summary>
    /// <param name="p_type"></param>
    void RemoveOneType(Type p_type)
    {
        foreach (var t_ruleList in m_addedRulesDic.Values)
        {
            for (int i = t_ruleList.Count-1; i >=0; i--)
            {
                if (p_type.IsInstanceOfType(t_ruleList[i]))
                {
                   // Object.Destroy();
                    DestroyImmediate(t_ruleList[i],true);
                    t_ruleList.RemoveAt(i);
                }
            }
        }
    }

    void AddOneType(Type p_type)
    {
        foreach (var t_ruleType in m_addedRulesDic.Keys)
        {
            if (p_type.IsSubclassOf(t_ruleType))
            {
               
                var t_oneRule = m_target.gameObject.AddComponent(p_type);
                m_addedRulesDic[t_ruleType].Add((BaseRule)t_oneRule);
            }
        }
    }
}
