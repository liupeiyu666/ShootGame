using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Engine.Effect
{
    public interface IEffectLoader
    {
        void Load(int p_id, Action<GameObject, object> callback, object p_param = null);
        void Unload();
    }
}
