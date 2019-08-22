using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZFrame;

namespace ZframeModel.ZEvent.Sprites
{
    public class ME_Hero_Creat : ModuleEvent
    {
        public int m_data;
        public override bool Init(object[] objs)
        {
            m_data = (int)objs[0];
            return base.Init(objs);
        }

        public override void OnBeforePushToPool()
        {
            base.OnBeforePushToPool();
        }
    }
}
