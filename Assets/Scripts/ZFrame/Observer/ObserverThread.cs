using System.Collections.Generic;

namespace ZLib
{
    /// <summary>
    /// 观察者线程（密封类； 因比较难理解，仅供ZFrame框架使用）
    /// @author zcp
    /// </summary>
    sealed  class ObserverThread
    {

        /// <summary>
        /// 观察者集
        /// </summary>
        private Dictionary<string, List<Observer>> _observerMap = new Dictionary<string, List<Observer>>();

        private Dictionary<int, List<Observer>> _observerIntMap = new Dictionary<int, List<Observer>>();
        public ObserverThread()
        {

        }

        /// <summary>
        /// 清除所有观察者
        /// </summary>
        public void clear()
        {
            _observerMap.Clear();
        }


        /// <summary>
        /// 注册观察者
        /// </summary>
        /// <param name="__notificationName"></param>
        /// <param name="__observer"></param>
        public void registerObserver(string __notificationName, Observer __observer)
        {
            List<Observer> obList;
            if (_observerMap.TryGetValue(__notificationName, out obList))
            {
                obList.Add(__observer);
            }
            else
            {
                obList = new List<Observer> { __observer };
                _observerMap.Add(__notificationName, obList);
            }
        }
        public void registerObserver(int __notificationName, Observer __observer)
        {
            List<Observer> obList;
            if (_observerIntMap.TryGetValue(__notificationName, out obList))
            {
                obList.Add(__observer);
            }
            else
            {
                obList = new List<Observer> { __observer };
                _observerIntMap.Add(__notificationName, obList);
            }
        }
        /// <summary>
        /// 移除观察者
        /// </summary>
        /// <param name="__notificationName"></param>
        /// <param name="__notifyContext"></param>
        public void removeObserver(string __notificationName, string __notifyContext)
        {
            List<Observer> obList;
            if (_observerMap.TryGetValue(__notificationName, out obList))
            {
                //查找并移除
                for (int i = obList.Count - 1; i >= 0; i--)
                {
                    if (obList[i].compareNotifyContext(__notifyContext))
                    {
                        obList.RemoveAt(i);
                    }
                }

                //如果没有观察者再注意此通知，则移除对于这个通知的观察数组
                if (obList.Count == 0)
                {
                    _observerMap.Remove(__notificationName);
                }
            }
        }
        /// <summary>
        /// 移除观察者
        /// </summary>
        /// <param name="__notificationName"></param>
        /// <param name="__notifyContext"></param>
        public void removeObserver(int __notificationName, string __notifyContext)
        {
            List<Observer> obList;
            if (_observerIntMap.TryGetValue(__notificationName, out obList))
            {
                //查找并移除
                for (int i = obList.Count - 1; i >= 0; i--)
                {
                    if (obList[i].compareNotifyContext(__notifyContext))
                    {
                        obList.RemoveAt(i);
                    }
                }

                //如果没有观察者再注意此通知，则移除对于这个通知的观察数组
                if (obList.Count == 0)
                {
                    _observerIntMap.Remove(__notificationName);
                }
            }
        }
        /// <summary>
        /// 移除观察者(通过notificationName)
        /// </summary>
        /// <param name="__notificationName"></param>
        public void removeObserverByNotificationName(string __notificationName)
        {
            if (_observerMap.ContainsKey(__notificationName))
            {
                _observerMap.Remove(__notificationName);
            }
        }
        /// <summary>
        /// 移除观察者(通过notificationName)
        /// </summary>
        /// <param name="__notificationName"></param>
        public void removeObserverByNotificationName(int __notificationName)
        {
            if (_observerIntMap.ContainsKey(__notificationName))
            {
                _observerIntMap.Remove(__notificationName);
            }
        }
        /// <summary>
        /// 移除观察者(通过notifyContext)
        /// </summary>
        /// <param name="__notifyContext"></param>
        public void removeObserverByNotifyContext(string __notifyContext)
        {
            List<string> keyList = new List<string>(_observerMap.Keys);
            foreach (string key in keyList)
            {
                removeObserver(key, __notifyContext);
            }
            List<int> intkeyList = new List<int>(_observerIntMap.Keys);
            foreach (int key in intkeyList)
            {
                removeObserver(key, __notifyContext);
            }
        }
       
        /// <summary>
        /// 将数据通知观察者
        /// </summary>
        /// <param name="__notification"></param>
        public void notifyObservers(Notification __notification)
        {
            List<Observer> obList_ref;
            if (!string.IsNullOrEmpty(__notification.name) )
            {
                if (_observerMap.TryGetValue(__notification.name, out obList_ref))
                {
                    List<Observer> obList = new List<Observer>();
                    Observer observer;
                    int i;
                    int len = obList_ref.Count;

                    //检索出所有观察此消息号的回调函数,因为在执行回调的过程中observers_ref这个数组可能发生改变，所以需要这样操作
                    for (i = 0; i < len; i++)
                    {
                        observer = obList_ref[i];
                        obList.Add(observer);
                    }
                    //执行所有观察此消息号的回调函数
                    for (i = 0; i < len; i++)
                    {
                        observer = obList[i];
                        observer.notifyObserver(__notification);
                    }
                }
            }
            else if(__notification.iname!=null)
            {
                if (_observerIntMap.TryGetValue((int)__notification.iname, out obList_ref))
                {
                    List<Observer> obList = new List<Observer>();
                    Observer observer;
                    int i;
                    int len = obList_ref.Count;

                    //检索出所有观察此消息号的回调函数,因为在执行回调的过程中observers_ref这个数组可能发生改变，所以需要这样操作
                    for (i = 0; i < len; i++)
                    {
                        observer = obList_ref[i];
                        obList.Add(observer);
                    }
                    //执行所有观察此消息号的回调函数
                    for (i = 0; i < len; i++)
                    {
                        observer = obList[i];
                        observer.notifyObserver(__notification);
                    }
                }
            }
          
        }
    }
}