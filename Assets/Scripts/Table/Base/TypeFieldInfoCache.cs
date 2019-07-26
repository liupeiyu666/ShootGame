using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace ZTool.Table
{
    /// <summary>
    /// 根据csv的第一行的数据缓存FieldInfo
    /// </summary>
    internal class TypeFieldInfoCache
    {
        /// <summary>
        /// 类型
        /// </summary>
        public Type type;

        /// <summary>
        /// 缓存的FieldInfo
        /// </summary>
        public Dictionary<string, FieldInfo> cachedField;

        /// <summary>
        /// 初始化filedList
        /// </summary>
        /// <param name="_filedList"></param>
        public void Init(string[] _filedList)
        {
            cachedField = new Dictionary<string, FieldInfo>();

            var len = _filedList.Length;

            for (int i = 0; i < len; i++)
            {
                var fieldstr = _filedList[i];

                FieldInfo field;

                if (!cachedField.TryGetValue(_filedList[i], out field))
                {
                    field = type.GetField(_filedList[i]);

                    cachedField.Add(fieldstr, field);
                }
            }
        }

        /// <summary>
        /// 加速获取字段
        /// </summary>
        /// <param name="_fieldString"></param>
        /// <returns></returns>
        public FieldInfo GetFieldInfo(string _fieldString)
        {
            FieldInfo field = null;

            if (cachedField == null) UnityEngine.Debug.Log("cache" + type + this.GetHashCode() + "id:" + Thread.CurrentThread.ManagedThreadId);

            if (cachedField.TryGetValue(_fieldString, out field))
            {
                return field;
            }

            throw new Exception(string.Format("并未找到{0}的字段", _fieldString));
        }
    }
}