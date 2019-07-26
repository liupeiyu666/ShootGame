using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;


public class LoadManager
{
    #region 单例

    private LoadManager()
    {
    }

    private static LoadManager _instance;

    public static LoadManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new LoadManager();
                _instance.m_streamingPath = Path.Combine("file://", Application.streamingAssetsPath);
            }

            return _instance;
        }
    }

    #endregion

    #region 路径

    public string m_streamingPath;


    #endregion
    List<TBinLoader> m_allBinLoader = new List<TBinLoader>();
    public void LoadBin(string p_url, Action<byte[], object> p_callBack, object p_params)
    {
        TBinLoader t_loader = new TBinLoader(p_url, p_callBack, p_params);
        m_allBinLoader.Add(t_loader);
    }

    public void Update()
    {
        //驱动检测
        for (int i = m_allBinLoader.Count - 1; i >= 0; i--)
        {
            if (!m_allBinLoader[i].m_finsh)
            {
                m_allBinLoader[i].Update();

            }
            else
            {
                m_allBinLoader.RemoveAt(i);
            }
        }
    }
}