using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZFrame;

namespace ZframeModel.ZEvent.Camera
{
    /// <summary>
    /// 设置摄像机的参数
    /// </summary>
    public class ME_SetCameraParam : ModuleEvent
    {
        public int m_cameraParmId;
        public override bool Init(object[] objs)
        {
            m_cameraParmId = (int)objs[0];
            return base.Init(objs);
        }

        public override void OnBeforePushToPool()
        {
            base.OnBeforePushToPool();
        }
    }
}
