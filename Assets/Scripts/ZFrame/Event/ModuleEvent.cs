using System;
namespace ZFrame
{ /// <summary>
  /// 框架使用的事件基类(继承自Object, 区别于EventArgs)
  /// </summary>
    public class ModuleEvent : ScriptPoolItem
    {
        /// <summary>
        /// 唯一标识
        /// </summary>
        public int key
        {
            get
            {
                if (_key == -1)
                {
                    _key = GetType().GetHashCode();
                }
                return _key;
            }
            set { _key = value; }
        }
        protected int _key=-1;

        /// <summary>
        /// 事件派发者(可为null)
        /// </summary>
        public object sender
        {
            get;
            protected set;
        }

        /// <summary>
        /// 框架使用的事件基类(继承自Object, 区别于EventArgs)
        /// </summary>
        public ModuleEvent() { sender = null; }

        /// <summary>
        /// 框架使用的事件基类(继承自Object, 区别于EventArgs)
        /// </summary>
        /// <param name="__sender">事件派发者</param>
        public ModuleEvent(object __sender) { sender = __sender; }

    }
}
