using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class GoLoader<T> where T:UnityEngine.Object
{
    string fullUrl;//这个是外部已经拼接好的url
    private T m_go;
    public bool m_finsh = false;
    private Action<T, object> m_callBack;
    private object m_params;
    internal GoLoader(string p_fullUrl, Action<T, object> p_callBack, object p_params)
    {
        //Wade 9.8+
        fullUrl = p_fullUrl;
        m_callBack = p_callBack;
        m_finsh = false;
        m_params = p_params;
        DoStart();
    }

    internal void DoStart()
    {
        m_go= Resources.Load<T>(fullUrl);
        if (m_callBack != null)
        {
            m_callBack(m_go, m_params);
        }
    }
    internal void Update()
    {
        if (m_finsh)
        {
            if (m_callBack != null)
            {
                m_callBack(m_go, m_params);
            }
        }
    }
}
