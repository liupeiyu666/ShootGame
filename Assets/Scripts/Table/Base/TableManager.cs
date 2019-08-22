using LumenWorks.Framework.IO.Csv;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using Tgame.Game.Table;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ZTool.Table
{

    /// <summary>
    /// 管理所有 table（配置表数据信息）
    /// </summary>
    public class TableManager
    {
        #region 单例

        private TableManager()
        {
        }

        private static TableManager _instance;

        public static TableManager instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TableManager();
                }

                return _instance;
            }
        }

        #endregion

        public Action OnLoadComplete;
        #region 开始加载，对外发布

       static Object locker = new Object();

    //已经加载的配置资源
       private  int m_iCount;

        private int m_loadCount;
        //所有的资源配置数量
        private int m_iAllConfigCount;

        static public string ShowProgress = "0/0";
        public   void StartLoad()
        {
            var classes = typeof(TableManager).Assembly.GetTypes();
            var att = typeof(TableNameAttribute);
            List<string> tableNames = new List<string>();
            for (int i = 0; i < classes.Length; i++)
            {
                if (!string.IsNullOrEmpty(classes[i].Namespace) && classes[i].Namespace.Equals("Tgame.Game.Table"))
                {
                    Type type = classes[i];
                    object[] objs = type.GetCustomAttributes(att, false);
                    if (objs.Length > 0)
                    {
                        string tName = ((TableNameAttribute)objs[0]).tableName;
                        tableNames.Add(tName);
                    }
                }
            }
            string[] pre_load_tables = tableNames.ToArray();
            OnLoadConfig(pre_load_tables);
        }

        private void OnLoadConfig(string[] config)
        {
            if (config == null || config.Length == 0)
                return;

            m_iCount = 0;

            m_loadCount = 0;
            m_iAllConfigCount = config.Length;
            for (int i = 0; i < config.Length; i++)
            {
                //这个try在配置文件加载本地（Resources.Load）的时候是同步执行的，包括回调也是同步执行
                //如果出现异常回调和catch都会吧计数+1，
                //这就造成了m_iCount >= m_iAllConfigCount这个表达式会早于预期成立
                try
                {
                    string name = config[i].TrimEnd(new char[] { '\r' });
                    //string name = config[i];

                    if (OnCheckConfig(name))
                        continue;

                    string url = string.Format("table/{0}.csv", name);//"table/gamebasedata.bin" 
                    System.Text.Encoding enco = null;
                    enco = System.Text.Encoding.UTF8;
                    //加载配置资源--不同的工程修改这里的加载方式即可
                    LoadManager.instance.LoadBin(Path.Combine(LoadManager.instance.m_streamingPath,url),OnLoadConfigOver,name);

                    //TextAsset m_assets = Resources.Load<TextAsset>("table/bag_row");
                    
                    //Debug.LogError("    :"+ m_assets + "   " + url);
                    //if (m_assets!=null)
                    //{
                    //    OnLoadConfigOver(m_assets.bytes, name);
                    //}
                }
                catch (Exception ex)
                {
                    Debug.LogError("配置资源 " + config[i] + " 加载出问题\n " + ex.ToString() + "\n" + ex.StackTrace);

                    //跳过此配置的加载
                    m_iCount++;
                    
                    OnAllConfigLoadOver();
                }
            }
        }

        private void OnLoadConfigOver(byte[] p_content, object p_name)
        {
            if (p_content == null)
            {
                m_iCount += 1;
             
                m_loadCount++;
                OnAllConfigLoadOver();
                return;
            }
            m_loadCount++;

            //  CodeBridgeTool.instance.SignTaskProgress(TaskID.LoadStaticConfig, m_loadCount / (float)m_iAllConfigCount);

            var name = (string)p_name;

            var type = Type.GetType(string.Format("Tgame.Game.Table.Table_{0}", name));
            if (type != null)
            {
                var objs = new object[]
                {
                p_content,name,type
                };
                HandlerDataInWorkItem(objs);
                //启动线程池 操作数据解析存储
            //    ThreadPool.QueueUserWorkItem(HandlerDataInWorkItem, objs);
            }
            else
            {
                OnAllConfigLoadOver();
                throw new Exception("table没有找到" + name);
            }

        }
        //object lockOOO = new object();
        /// <summary>
        /// 非主线程解析数据
        /// </summary>
        /// <param name="parmsArr"></param>
        private void HandlerDataInWorkItem(object parmsArr)
        {
            //注意，这里写法为了安全降低了执行效率，如需优化需关闭lock，代码需要进一步验证 ，
            //已发现问题 多线程会出现问题，问题暂未定位
            //lock (lockOOO)
            //{

            var parms = parmsArr as object[];

            if (parms != null && parms.Length >= 3)
            {
                var obj = parms[0] as byte[];
                var name = parms[1] as string;
                var type = parms[2] as Type;
                try
                {
                    TableManager.LoadTable(obj, name, type);
                }
                catch (Exception e)
                {
                    Debug.LogError(e.ToString() + " " + name);
                }
            }

            lock (locker)
            {
                Interlocked.Increment(ref m_iCount);
                OnAllConfigLoadOver();
            }
           
            //}
        }

        private void OnAllConfigLoadOver()
        {
            float pro = (float)m_iCount / m_iAllConfigCount * 0.95f;
            ShowProgress = string.Format("{0}/{1}", m_iCount, m_iAllConfigCount);
            if (m_iCount >= m_iAllConfigCount)
            {
                pro = 1.0f;
                InitOtherModelTable();
                if (OnLoadComplete!=null)
                {
                    OnLoadComplete();
                }
            }

           
        }

        /// <summary>
        /// 检测配置数据
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private bool OnCheckConfig(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                m_iAllConfigCount--;
                OnAllConfigLoadOver();
                return true;
            }
            return false;
        }
        /// <summary>
        /// 初始化其他模块的数据
        /// </summary>
        private void InitOtherModelTable()
        {
            //引擎配表 地图  avatar 跳点
        }
        #endregion
        #region 加载配置数据

        //当前程序集
        static Assembly assembly;
        //tabel 表所在的命名空间
        static string nameSpace = "Tgame.Game.Table";

        /// <summary>
        /// 设置Table的程序集和命名空间
        /// </summary>
        /// <param name="ass"></param>
        /// <param name="tableNameSpace"></param>
        public static void SetTableAssemblyAndNameSpace(Assembly ass, string tableNameSpace)
        {
            assembly = ass;
            nameSpace = tableNameSpace;
        }

        /// <summary>
        /// 加载Table
        /// </summary>
        /// <param name="_bys">需要解析的字符内容</param>
        /// <param name="_tableName">Table 名字</param>
        /// <param name="_callback">解析完成之后的回调</param>
        /// <param name="_parameter">回调参数</param>
        /// <returns></returns>
        public static bool LoadTable(byte[] _bys, string _tableName, Type _tableType, Action<object> _callback = null, object _parameter = null)
        {
            //检测数据内容和配表名字
            if (_bys == null || _bys.Length == 0 || string.IsNullOrEmpty(_tableName))
            {
                Debug.LogError(string.Format("!!!数据内容，或者配表名字为空:{0}", _tableName));
                return false;
            }

            if (_tableType != null)
            {
                return ReadCsvbyDll(_bys, _tableType, 1);
                //return ReadCsvbyDll(_bys, _tableType, true);
            }

            //检测当前程序集
            if (assembly == null)
                assembly = Assembly.GetExecutingAssembly();

            //表名如果首字母不是大写的要转换成大写的才能找到
            _tableName = ToTitleCase(_tableName);

            //获得配表的类名
            _tableName = string.Format("Table_{0}", _tableName);

            _tableType = assembly.GetType(string.Format("{0}.{1}", nameSpace, _tableName));

            if (_tableType == null)
                Debug.LogError("Get Table Type Failure! " + assembly + " dd " + nameSpace + "  " + _tableName);

            return ReadCsvbyDll(_bys, _tableType, 1);
            //return ReadCsvbyDll(_bys, _tableType, true);
        }

        /// <summary>
        /// 首字母转换成大写
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToTitleCase(string str)
        {
            str = str.Replace("_", " ");
            str = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(str);
            str = str.Replace(" ", "_");
            return str;
        }

        #endregion

        #region 解析数据

        //计数
        public static int HowManyTableTotal = 0;


        public static object CreateGeneric(Type generic, Type innerType, params object[] args)
        {
            Type specificType = generic.MakeGenericType(new System.Type[] { innerType });
            return Activator.CreateInstance(specificType, args);
        }

        /// <summary>
        /// 使用第三方CSV库解析我们不是很规范的CSV文件。。。。
        /// </summary>
        /// <param name="_bys"></param>
        /// <param name="_type"></param>
        /// <param name="_modify"></param>
        /// <returns></returns>
        private static bool ReadCsvbyDll(byte[] _bys, Type _type, object _modify)
        {
            //TableKeyType keyType = TableKeyType.IntKey;
            
            using (var ms = new MemoryStream(_bys))
            {
                using (var csv = new CsvReader(new StreamReader(ms, Encoding.GetEncoding("GBK")), false))
                {
                    csv.SupportsMultiline = true;
                    string s;
                    //单行数据，
                    var dataLine = new List<string>();
                    //反射使用的文件内字段名
                    string[] keys = null;
                    //反射使用的文件类型，因为csv文件中有类型表示，所以直接字符串判断
                    string[] types = null;
                    var index = -1;
                    IList genericList = CreateGeneric(typeof(List<>), _type) as IList;

                    //var instance = Activator.CreateInstance(_type, true) as TableContent;


                    Dictionary<string, string> tab = new Dictionary<string, string>();
                    var cache = new TypeFieldInfoCache { type = _type };

                    while (csv.ReadNextRecord())
                    {
                        for (var i = 0; i < csv.FieldCount; i++)
                        {
                            s = csv[i];
                            dataLine.Add(s);
                            if(index>1)
                            {
                                tab.Add(keys[i], s);
                            }
                        }
                        index++;

                        //初始化字段和类型
                        switch (index)
                        {
                            case 0:
                                keys = dataLine.ToArray();
                                cache.Init(keys);
                                break;
                            case 1:
                                types = dataLine.ToArray();
                                break;
                            default:
                                if (index > 2)
                                {
                                    TableContent obj = Activator.CreateInstance(_type, true) as TableContent;
                                    //obj.ParseFrom(dataLine.ToArray());
                                    obj.ParseFrom(tab);
                                    obj.Init();
                                    Interlocked.Increment(ref HowManyTableTotal);
                                    genericList.Add(obj);
                                }
                                break;
                        }
                        tab.Clear();
                        dataLine.Clear();
                    }
                    //end Add
                    MethodInfo mf = _type.GetMethod("InitPool");
                    if (mf != null)
                    {
                        mf.Invoke(null, new object[] { genericList });
                    }
                    else
                    {
                        Debug.Log("没有方法。。。。" + _type);
                    }
                }
            }
            return true;
        }


        #endregion

    }

}
