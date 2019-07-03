using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Effect
{
    public class Des:Attribute
    {
        public string m_des;
        public Des(string p_des)
        {
            m_des = p_des;
        }
    }
}
