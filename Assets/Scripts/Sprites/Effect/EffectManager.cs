using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditorInternal;
using UnityEngine;

namespace Engine.Effect
{
    /// <summary>
    /// 特效管理类
    /// 负责特效的生产，并标识唯一ID，同时返回特效本身。
    /// </summary>
    public class EffectManager
    {
        #region 单例

        private EffectManager()
        {
        }

        private static EffectManager _instance;

        public static EffectManager instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new EffectManager();
                }

                return _instance;
            }
        }

        #endregion
        /// <summary>
        /// 存储所有特效的加载器和控制器
        /// </summary>
        Dictionary<IEffectLoader,BaseEffectController> m_allEffectDic=new Dictionary<IEffectLoader, BaseEffectController>();
        #region 对外事件
        private struct LoadStruct
        {
            public LoadStruct(EffectShareData p_shareData, IEffectLoader p_loader)
            {
                m_shareData = p_shareData;
                m_loader = p_loader;
            }

            public EffectShareData m_shareData;
            public IEffectLoader m_loader;
        }
        /// <summary>
        /// 一个特效的最基本的单元
        /// </summary>
        /// <returns></returns>
        public IEffectLoader CreatEffectByController(int p_id,EffectShareData p_shareData)
        {
            IEffectLoader t_loader =new EffectLoader();
            m_allEffectDic.Add(t_loader, null);
            t_loader.Load(p_id, OnLoadOK,new LoadStruct(p_shareData, t_loader));
            return null;
        }

        void OnLoadOK(GameObject p_go,object p_params)
        {
            LoadStruct t_Data = (LoadStruct)p_params;

            //获取控制器
            BaseEffectController t_controller = p_go.GetComponent<BaseEffectController>();
            if (t_controller!=null)
            {
                m_allEffectDic[t_Data.m_loader] = t_controller;
                t_controller.m_shareDate = t_Data.m_shareData;
            }
        }

        public void Unload(IEffectLoader p_loader)
        {
            if (m_allEffectDic.ContainsKey(p_loader))
            {
                p_loader.Unload();
                m_allEffectDic.Remove(p_loader);
            }
            else
            {
                Debug.LogError("卸载失败了,没有找到对应key~~~" );
            }
        }
        public void Unload(BaseEffectController p_controller)
        {
            var t_enumerator = m_allEffectDic.GetEnumerator();
            IEffectLoader t_key = null;
            while (t_enumerator.MoveNext())
            {
                if (t_enumerator.Current.Value== p_controller)
                {
                    t_key = t_enumerator.Current.Key;
                    break;
                }
            }

            if (t_key!=null)
            {
                t_key.Unload();
                m_allEffectDic.Remove(t_key);
            }
            else
            {
                Debug.LogError("卸载失败了~~~："+ p_controller.name);
            }
        }
        #endregion

    }

    
}
