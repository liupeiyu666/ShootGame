namespace ZLib
{
    /// <summary>
    /// 通知信息
    /// @author zcp
    /// </summary>
    public class Notification
    {

        /// <summary>
        /// 通知标识
        /// </summary>
        public string name;

        public int? iname;
        /// <summary>
        /// 通知数据体
        /// </summary>
        public object body;

        /// <summary>
        /// Notification
        /// </summary>
        /// <param name="__name">通知标识</param>
        /// <param name="__body">通知数据体</param>
        public Notification(string __name, object __body)
        {
            name = __name;
            body = __body;
        }
        public Notification(int __name, object __body)
        {
            iname = __name;
            body = __body;
        }
    }
}