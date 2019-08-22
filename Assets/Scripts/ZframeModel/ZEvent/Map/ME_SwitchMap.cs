using ZFrame;

namespace ZframeModel.ZEvent.Sprites
{
    /// <summary>
    /// @author Lpy
    /// </summary>
    public class ME_SwitchMap : ModuleEvent
    {
        public int m_mapId;
        public override bool Init(object[] objs)
        {
            m_mapId = (int)objs[0];
            return base.Init(objs);
        }

        public override void OnBeforePushToPool()
        {
            base.OnBeforePushToPool();
        }
    }
}
