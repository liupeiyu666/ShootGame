using System;
using UnityEngine;
using System.Collections;

/// <summary>
/// 简单的用来加载二进制数据的loader,不需要FileVer信息来工作，用来加载all.ver.txt或者block_island_jumppoing_water.bin等
/// 注意，如果是加载bin,则是要传入全路径url
/// 
/// Wade Add 6.5+只用于all.ver.txt
/// 	其它的采用TBinRemoteLoader，先下载资源，再从本地读取 - 
/// 	比如地图的bin数据，在网络。 首包和第一次下载都不下载，运行时下载使用。
/// 		这样就不用每次都去服务器请求了，只是第一次去趟服务器
/// </summary>
public class TBinLoader 
{

    string fullUrl;//这个是外部已经拼接好的url

    WWW www;

    public bool m_finsh = false;
    private Action<byte[],object> m_callBack;
    private object m_params;
    internal TBinLoader(string p_fullUrl,Action<byte[],object> p_callBack, object p_params)
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
      //  Debug.LogError("开始加载："+ fullUrl);
        www = new WWW(fullUrl);
    }

  

    internal  void Update()
    {
        if (www != null)
        {
            if (www.isDone)
            {
             //   Debug.LogError("加载成功：" + fullUrl+"    "+ www.bytes.Length);
                if (m_callBack!=null)
                {
                    m_callBack(www.bytes,m_params);
                    Dispose();
                }
            }
        }
    }

    //真正释放
    private  void Dispose()
    {
        m_finsh = true;
        www.Dispose();
        www = null;
    }

}
