using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Engine.Effect
{
    /// <summary>
    /// 死亡规则，
    /// 普通死亡就直接消息即可
    /// 特殊的需要额外添加，比如，播放下一个特效等等的连串反应
    /// </summary>
    [Serializable]
    [Des("普通死亡")]
    public class NormalDead:DeadRule
    {
        public override void DoDead()
        {
            //直接调用销毁就行了
            //Debug.LogError("我被销毁了----");
            Unload();
        }
    }
}
