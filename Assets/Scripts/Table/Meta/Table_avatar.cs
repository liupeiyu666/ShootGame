using System.Collections;
using System.Collections.Generic;
using System;
using ZTool.Table;
namespace Tgame.Game.Table
{
    ///<summary>
    /// AutoCreat
    ///</summary>
    [Serializable]
    [TableName("avatar")]
    public partial class Table_avatar : TableContent
    {

        private static List<Table_avatar> all_Table_avatar_List = new List<Table_avatar>();
        //primary | 主键
        public static Dictionary<int, Table_avatar> pool_primary = new Dictionary<int, Table_avatar>();

        #region 字段
        ///<summary>
        /// 
        ///</summary>
        private int _id;
        ///<summary>
        /// 
        ///</summary>
        public int id
        {
            get
            {
               return this._id;
            }
        }
        ///<summary>
        /// 
        ///</summary>
        private string _url;
        ///<summary>
        /// 
        ///</summary>
        public string url
        {
            get
            {
               return this._url;
            }
        }
        ///<summary>
        /// 
        ///</summary>
        private string _des;
        ///<summary>
        /// 
        ///</summary>
        public string des
        {
            get
            {
               return this._des;
            }
        }

        #endregion

        #region 数据填充
        ///<summary>
        /// 通过字典初始化对象值
        ///</summary>
        public override void ParseFrom(Dictionary<string, string> _itemData)
        {
            string _currValue = "";
            if(_itemData.TryGetValue("id", out _currValue))
			{
			   UGEUtils.GetValueFromString(_currValue, out this._id);
			}
            if(_itemData.TryGetValue("url", out _currValue))
			{
			   UGEUtils.GetValueFromString(_currValue, out this._url);
			}
            if(_itemData.TryGetValue("des", out _currValue))
			{
			   UGEUtils.GetValueFromString(_currValue, out this._des);
			}

        }

        ///<summary>
        /// 获取table表名
        ///</summary>
        public override string Table()
        {
            return "avatar";
        }
        #endregion
        ///<summary>
        /// 主键
        /// 查询数据
        ///</summary>
        ///	<param id> 主键：ID</param>
        ///
        public static Table_avatar GetPrimary(int _id)
        {
            Table_avatar _map0 = null;
            pool_primary.TryGetValue(_id, out _map0);
            return _map0;
        }
        ///<summary>
        ///主键
        ///查询所有数据
        ///</summary>
        public static Dictionary<int, Table_avatar> GetAllPrimary()
        {
            return pool_primary;
        }


        ///查询出所有的数据
        public static List<Table_avatar> GetAllPrimaryList()
        {
            return all_Table_avatar_List;
        }



        ///<summary>
        ///根据column获取值
        ///</summary>
        public override object GetValue(string column)
        {
            switch (column)
            {
                case "id":
                    return this._id;
                default:
                    return null;
            }
        }

        ///<summary>
        /// 初始化Pool
        ///</summary>
        public static void InitPool(IList _rows)
        {
            List<Table_avatar> rows = _rows as List<Table_avatar>;
            pool_primary = TableContent.ListToPool<int, Table_avatar>(rows, "map", "id");
            all_Table_avatar_List = rows;
        }

        ///<summary>
        /// 清理静态数据
        ///</summary>
        public static void Clear()
        {
            pool_primary.Clear();
            all_Table_avatar_List.Clear();
        }

    }
}
