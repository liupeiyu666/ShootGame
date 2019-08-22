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
    [TableName("skill")]
    public partial class Table_skill : TableContent
    {

        private static List<Table_skill> all_Table_skill_List = new List<Table_skill>();
        //primary | 主键
        public static Dictionary<int, Table_skill> pool_primary = new Dictionary<int, Table_skill>();

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
        private int _effectid;
        ///<summary>
        /// 
        ///</summary>
        public int effectid
        {
            get
            {
               return this._effectid;
            }
        }
        ///<summary>
        /// 
        ///</summary>
        private string _ani_name;
        ///<summary>
        /// 
        ///</summary>
        public string ani_name
        {
            get
            {
               return this._ani_name;
            }
        }
        ///<summary>
        /// 绑定的位置
        ///</summary>
        private int _bind_pos;
        ///<summary>
        /// 绑定的位置
        ///</summary>
        public int bind_pos
        {
            get
            {
               return this._bind_pos;
            }
        }
        ///<summary>
        /// 绑定的描述，用于偏移设置
        ///</summary>
        private string _bind_des;
        ///<summary>
        /// 绑定的描述，用于偏移设置
        ///</summary>
        public string bind_des
        {
            get
            {
               return this._bind_des;
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
            if(_itemData.TryGetValue("effectid", out _currValue))
			{
			   UGEUtils.GetValueFromString(_currValue, out this._effectid);
			}
            if(_itemData.TryGetValue("ani_name", out _currValue))
			{
			   UGEUtils.GetValueFromString(_currValue, out this._ani_name);
			}
            if(_itemData.TryGetValue("bind_pos", out _currValue))
			{
			   UGEUtils.GetValueFromString(_currValue, out this._bind_pos);
			}
            if(_itemData.TryGetValue("bind_des", out _currValue))
			{
			   UGEUtils.GetValueFromString(_currValue, out this._bind_des);
			}

        }

        ///<summary>
        /// 获取table表名
        ///</summary>
        public override string Table()
        {
            return "skill";
        }
        #endregion
        ///<summary>
        /// 主键
        /// 查询数据
        ///</summary>
        ///	<param id> 主键：ID</param>
        ///
        public static Table_skill GetPrimary(int _id)
        {
            Table_skill _map0 = null;
            pool_primary.TryGetValue(_id, out _map0);
            return _map0;
        }
        ///<summary>
        ///主键
        ///查询所有数据
        ///</summary>
        public static Dictionary<int, Table_skill> GetAllPrimary()
        {
            return pool_primary;
        }


        ///查询出所有的数据
        public static List<Table_skill> GetAllPrimaryList()
        {
            return all_Table_skill_List;
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
            List<Table_skill> rows = _rows as List<Table_skill>;
            pool_primary = TableContent.ListToPool<int, Table_skill>(rows, "map", "id");
            all_Table_skill_List = rows;
        }

        ///<summary>
        /// 清理静态数据
        ///</summary>
        public static void Clear()
        {
            pool_primary.Clear();
            all_Table_skill_List.Clear();
        }

    }
}
