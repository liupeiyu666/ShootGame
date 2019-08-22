using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZLib
{
    /// <summary>
    /// 单例(抽象类)
    /// 需要单例的类只需要继承自此类即可, 不支持子类单例
        ////类似这样使用:
        //Class1继承自Singleton<Class1>
        //Class1.instance
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    abstract public class Singleton<T>
    {
        private static T _instance;

        /// <summary>
        /// 单例
        /// </summary>
        public static T instance
        {
            get
            {
                if (Singleton<T>._instance == null)
                {
                    Singleton<T>._instance = Activator.CreateInstance<T>();
                }
                return Singleton<T>._instance;
            }
        }

        public Singleton()
        {
            //防止创建多个实例
            if (Singleton<T>._instance != null)
            {
                throw new Exception(typeof(T).ToString()+"单例！");
            }

            Singleton<T>._instance = (T)(System.Object)this;
        }
    }
}
