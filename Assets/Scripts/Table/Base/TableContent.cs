using System;
using System.Collections;
using System.Collections.Generic;

namespace ZTool.Table
{
    public abstract class TableContent
    {
        public virtual object[] getKeyValues()
        {
            return new object[] { };
        }
        /// <summary>
        /// 解析数据
        /// </summary>
        /// <param name="_itemDate"></param>
        //virtual public void ParseFrom(string[] _itemDate){}
        abstract public void ParseFrom(Dictionary<string, string> _itemDate);

        /// <summary>
        /// 表名
        /// </summary>
        /// <returns></returns>
        abstract public string Table();


        abstract public object GetValue(string column);


        virtual public void Init() {
        }

        /// <summary>
        /// 三层key/value...
        /// </summary>
        /// <typeparam name="TD"></typeparam>
        /// <typeparam name="TS"></typeparam>
        /// <typeparam name="TF"></typeparam>
        /// <typeparam name="Tvalue"></typeparam>
        /// <param name="rows"></param>
        /// <param name="indexType"></param>
        /// <param name="indexes"></param>
        /// <returns></returns>
        static public Dictionary<TD, Dictionary<TS, Dictionary<TF, Tvalue>>> ListToPool<TD, TS, TF, Tvalue>(IList _rows, string indexType, params string[] indexes) where Tvalue : TableContent
        {
            List<Tvalue> rows = _rows as List<Tvalue>;
            var pool = new Dictionary<TD, Dictionary<TS, Dictionary<TF, Tvalue>>>();

            for (int i = 0; i < rows.Count; i++)
            {
                Tvalue data = rows[i];
                IDictionary temp = pool;
                for (int j = 0; j < indexes.Length; j++)
                {
                    string keyField = indexes[j];
                    object keyValue = data.GetValue(keyField);
                    if (j < indexes.Length - 1)
                    {
                        IDictionary subMap = temp[keyValue] as IDictionary;
                        if (subMap == null)
                        {
                            switch (j)
                            {
                                //case 2:
                                //    subMap = new Dictionary<TF, Tvalue>();
                                //    break;
                                case 1:
                                    subMap = new Dictionary<TF, Tvalue>();
                                    break;
                                case 0:
                                    subMap = new Dictionary<TS, Dictionary<TF, Tvalue>>();
                                    break;
                            }
                            temp.Add(keyValue, subMap);
                        }
                        temp = subMap;
                    }
                    else
                    {
                        if ("map".Equals(indexType))
                        {
                            //最后一层是单个数据
                            if (!temp.Contains(keyValue))
                            {
                                temp.Add(keyValue, data);
                            }
                            else
                            {
                                throw new Exception(string.Format("数据主键重复 {0} | {1} | {2}", data.Table(), data, keyValue));
                            }

                        }
                        else
                        {
                            //最后一层是集合
                            //List<TableContent> finalList = (List<TableContent>)temp[keyValue];
                            //if (finalList == null)
                            //{
                            //    finalList = new List<TableContent>();
                            //    temp.Add(keyValue, finalList);
                            //}
                            //finalList.Add(data);
                        }

                    }
                }
            }

            return pool;
        }

        /// <summary>
        /// 二层key/value...
        /// </summary>
        /// <typeparam name="TD"></typeparam>
        /// <typeparam name="TS"></typeparam>
        /// <typeparam name="TF"></typeparam>
        /// <typeparam name="Tvalue"></typeparam>
        /// <param name="rows"></param>
        /// <param name="indexType"></param>
        /// <param name="indexes"></param>
        /// <returns></returns>
        static public Dictionary<TS, Dictionary<TF, Tvalue>> ListToPool<TS, TF, Tvalue>(List<Tvalue> rows, string indexType, params string[] indexes) where Tvalue:TableContent
        {
            var pool = new Dictionary<TS, Dictionary<TF, Tvalue>>();

            for (int i = 0; i < rows.Count; i++)
            {
                Tvalue data = rows[i];
                IDictionary temp = pool;
                for (int j = 0; j < indexes.Length; j++)
                {
                    string keyField = indexes[j];
                    object keyValue = data.GetValue(keyField);
                    if (j < indexes.Length - 1)
                    {
                        IDictionary subMap = temp[keyValue] as IDictionary;
                        if (subMap == null)
                        {
                            switch (j)
                            {
                               
                                case 0:
                                    subMap = new Dictionary<TF, Tvalue>();
                                    break;
                            }
                            temp.Add(keyValue, subMap);
                        }
                        temp = subMap;
                    }
                    else
                    {
                        if ("map".Equals(indexType))
                        {
                            //最后一层是单个数据
                            if (!temp.Contains(keyValue))
                            {
                                temp.Add(keyValue, data);
                            }
                            else
                            {
                                throw new Exception(string.Format("数据主键重复 {0} | {1} | {2}", data.Table(), data, keyValue));
                            }

                        }
                        else
                        {
                            //最后一层是集合
                            //List<TableContent> finalList = (List<TableContent>)temp[keyValue];
                            //if (finalList == null)
                            //{
                            //    finalList = new List<TableContent>();
                            //    temp.Add(keyValue, finalList);
                            //}
                            //finalList.Add(data);
                        }

                    }
                }
            }

            return pool;
        }

        /// <summary>
        /// 一层key/value...
        /// </summary>
        /// <typeparam name="TD"></typeparam>
        /// <typeparam name="TS"></typeparam>
        /// <typeparam name="TF"></typeparam>
        /// <typeparam name="Tvalue"></typeparam>
        /// <param name="rows"></param>
        /// <param name="indexType"></param>
        /// <param name="indexes"></param>
        /// <returns></returns>
        static public Dictionary<TF, Tvalue> ListToPool<TF, Tvalue>(List<Tvalue> rows, string indexType, params string[] indexes) where Tvalue : TableContent
        {
            var pool = new Dictionary<TF, Tvalue>();

            for (int i = 0; i < rows.Count; i++)
            {
                Tvalue data = rows[i];
                IDictionary temp = pool;
                for (int j = 0; j < indexes.Length; j++)
                {
                    string keyField = indexes[j];
                    object keyValue = data.GetValue(keyField);
                    //一层结构，这里应该不会进去
                    if (j < indexes.Length - 1)
                    {
                        
                    }
                    else
                    {
                        if ("map".Equals(indexType))
                        {
                            //最后一层是单个数据
                            if (!temp.Contains(keyValue))
                            {
                                temp.Add(keyValue, data);
                            }
                            else
                            {
                                throw new Exception(string.Format("数据主键重复 {0} | {1} | {2}", data.Table(), data, keyValue));
                            }

                        }
                        else
                        {
                            //最后一层是集合
                            //List<TableContent> finalList = (List<TableContent>)temp[keyValue];
                            //if (finalList == null)
                            //{
                            //    finalList = new List<TableContent>();
                            //    temp.Add(keyValue, finalList);
                            //}
                            //finalList.Add(data);
                        }

                    }
                }
            }

            return pool;
        }

////////////////////key-value-List/////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// 双层key/value...List
        /// </summary>
        /// <typeparam name="TD"></typeparam>
        /// <typeparam name="TS"></typeparam>
        /// <typeparam name="Tvalue"></typeparam>
        /// <param name="rows"></param>
        /// <param name="indexType"></param>
        /// <param name="indexes"></param>
        /// <returns></returns>
        static public Dictionary<TD, Dictionary<TS, List<Tvalue>>> ListToPoolList<TD, TS, Tvalue>(List<Tvalue> rows, string indexType, params string[] indexes) where Tvalue : TableContent
        {
            var pool = new Dictionary<TD, Dictionary<TS, List<Tvalue>>>();

            for (int i = 0; i < rows.Count; i++)
            {
                Tvalue data = rows[i];
                IDictionary temp = pool;
                for (int j = 0; j < indexes.Length; j++)
                {
                    string keyField = indexes[j];
                    object keyValue = data.GetValue(keyField);
                    if (j < indexes.Length - 1)
                    {
                        IDictionary subMap = temp[keyValue] as IDictionary;
                        if (subMap == null)
                        {
                            switch (j)
                            {
                                case 0:
                                    subMap = new Dictionary<TS, List<Tvalue>>();
                                    break;
                            }
                            temp.Add(keyValue, subMap);
                        }
                        temp = subMap;
                    }
                    else
                    {
                        if ("map".Equals(indexType))
                        {
                            //最后一层是单个数据
                            if (!temp.Contains(keyValue))
                            {
                                temp.Add(keyValue, data);
                            }
                            else
                            {
                                throw new Exception(string.Format("数据主键重复 {0} | {1} | {2}", data.Table(), data, keyValue));
                            }

                        }
                        else
                        {
                            //最后一层是集合
                            List<Tvalue> finalList = (List<Tvalue>)temp[keyValue];
                            if (finalList == null)
                            {
                                finalList = new List<Tvalue>();
                                temp.Add(keyValue, finalList);
                            }
                            finalList.Add(data);
                        }

                    }
                }
            }

            return pool;
        }

        /// <summary>
        /// 单层key/value...List
        /// </summary>
        /// <typeparam name="TD"></typeparam>
        /// <typeparam name="TS"></typeparam>
        /// <typeparam name="TF"></typeparam>
        /// <typeparam name="Tvalue"></typeparam>
        /// <param name="rows"></param>
        /// <param name="indexType"></param>
        /// <param name="indexes"></param>
        /// <returns></returns>
        static public Dictionary<TS, List<Tvalue>> ListToPoolList<TS, Tvalue>(List<Tvalue> rows, string indexType, params string[] indexes) where Tvalue : TableContent
        {
            var pool = new Dictionary<TS, List<Tvalue>>();

            for (int i = 0; i < rows.Count; i++)
            {
                Tvalue data = rows[i];
                IDictionary temp = pool;
                for (int j = 0; j < indexes.Length; j++)
                {
                    string keyField = indexes[j];
                    object keyValue = data.GetValue(keyField);
                    //这里应该无法进入
                    if (j < indexes.Length - 1)
                    {
                        IDictionary subMap = temp[keyValue] as IDictionary;
                        if (subMap == null)
                        {
                            temp.Add(keyValue, subMap);
                        }
                        temp = subMap;
                    }
                    else
                    {
                        if ("map".Equals(indexType))
                        {
                            //最后一层是单个数据
                            if (!temp.Contains(keyValue))
                            {
                                temp.Add(keyValue, data);
                            }
                            else
                            {
                                throw new Exception(string.Format("数据主键重复 {0} | {1} | {2}", data.Table(), data, keyValue));
                            }

                        }
                        else
                        {
                            //最后一层是集合
                            List<Tvalue> finalList = (List<Tvalue>)temp[keyValue];
                            if (finalList == null)
                            {
                                finalList = new List<Tvalue>();
                                temp.Add(keyValue, finalList);
                            }
                            finalList.Add(data);
                        }

                    }
                }
            }

            return pool;
        }

    }
}
