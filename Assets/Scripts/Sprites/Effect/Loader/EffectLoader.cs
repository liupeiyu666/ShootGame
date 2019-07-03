using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Engine.Effect
{
    /// <summary>
    /// 特效加载器，与外部对接，
    /// 完成资源的异步加载的管理，如果项目中已经有对应的管理了，直接在这里进行改装 
    /// </summary>
    public class EffectLoader:IEffectLoader
    {
        Dictionary<int,string> m_resDic=new Dictionary<int, string>()
        {
            { 0,"Bullets/jian"},
            {1,"Bullets/luoxuan"},
            {1001,"Bullets/yuandan"},
            {2001,"Bullets/bossemit" },
        };

        private GameObject t_go;
        /// <summary>
        /// 根据地质加载一个特效，并执行这个特效的回调
        /// </summary>
        /// <param name="url"></param>
        /// <param name="callback"></param>
        public void Load(int p_id, Action<GameObject,object> callback,object p_param=null)
        {
            //根据id获取url
            var t_res = Resources.Load<GameObject>(m_resDic[p_id]);
            t_go = GameObject.Instantiate(t_res);
            callback(t_go, p_param);
        }
        /// <summary>
        /// 卸载
        /// </summary>
        public void Unload()
        {
            GameObject.Destroy(t_go);
        }
    }
}
