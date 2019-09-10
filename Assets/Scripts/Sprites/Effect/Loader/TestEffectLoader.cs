using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ZTool.Table;

namespace Engine.Effect
{
    /// <summary>
    /// 特效加载器，与外部对接，
    /// 完成资源的异步加载的管理，如果项目中已经有对应的管理了，直接在这里进行改装 
    /// </summary>
    public class TestEffectLoader : IEffectLoader
    {
        public static Dictionary<int, GameObject> m_resDic = new Dictionary<int, GameObject>();
        /// <summary>
        /// 容器
        /// </summary>
        public static Transform m_ContainerTransform;

        private GameObject m_go;
        /// <summary>
        /// 路径
        /// </summary>
        private string m_url;

        public static int m_num;
        /// <summary>
        /// 根据地质加载一个特效，并执行这个特效的回调
        /// </summary>
        /// <param name="url"></param>
        /// <param name="callback"></param>
        public void Load(int p_id, Action<GameObject, object> callback, object p_param = null)
        {
            //根据id获取url
            m_go =GameObject.Instantiate(m_resDic[p_id]);
            m_go.name = "Bullet" + m_num++;
         //  Debug.LogError(m_go+"   "+ m_go.transform.position+"   "+Time.frameCount);
            //
            if (!m_ContainerTransform)
            {
                m_ContainerTransform = new GameObject("Effect").transform;
            }
            m_go.transform.SetParent(m_ContainerTransform);
            callback(m_go, p_param);
        }
        /// <summary>
        /// 卸载
        /// </summary>
        public void Unload()
        {
            //BaseEffectController t_controller = m_go.GetComponent<BaseEffectController>();
            //if (t_controller != null)
            //{
            //    t_controller.BeforePushPool();
            //}
            //GOPoolV3.instance.RecycleItem(m_url, "", m_go);
            GameObject.Destroy(m_go);
        }
    }
}
