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
    [TableName("role")]
    public partial class Table_role : TableContent
    {

        private static List<Table_role> all_Table_role_List = new List<Table_role>();
        //primary | 主键
        public static Dictionary<int, Table_role> pool_primary = new Dictionary<int, Table_role>();

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
        /// 移动速度
        ///</summary>
        private float _move_speed;
        ///<summary>
        /// 移动速度
        ///</summary>
        public float move_speed
        {
            get
            {
               return this._move_speed;
            }
        }
        ///<summary>
        /// 转身速度
        ///</summary>
        private float _rotate_speed;
        ///<summary>
        /// 转身速度
        ///</summary>
        public float rotate_speed
        {
            get
            {
               return this._rotate_speed;
            }
        }
        ///<summary>
        /// 模板id
        ///</summary>
        private int _avatar_id;
        ///<summary>
        /// 模板id
        ///</summary>
        public int avatar_id
        {
            get
            {
               return this._avatar_id;
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
            if(_itemData.TryGetValue("move_speed", out _currValue))
			{
			   UGEUtils.GetValueFromString(_currValue, out this._move_speed);
			}
            if(_itemData.TryGetValue("rotate_speed", out _currValue))
			{
			   UGEUtils.GetValueFromString(_currValue, out this._rotate_speed);
			}
            if(_itemData.TryGetValue("avatar_id", out _currValue))
			{
			   UGEUtils.GetValueFromString(_currValue, out this._avatar_id);
			}

        }

        ///<summary>
        /// 获取table表名
        ///</summary>
        public override string Table()
        {
            return "role";
        }
        #endregion
        ///<summary>
        /// 主键
        /// 查询数据
        ///</summary>
        ///	<param id> 主键：ID</param>
        ///
        public static Table_role GetPrimary(int _id)
        {
            Table_role _map0 = null;
            pool_primary.TryGetValue(_id, out _map0);
            return _map0;
        }
        ///<summary>
        ///主键
        ///查询所有数据
        ///</summary>
        public static Dictionary<int, Table_role> GetAllPrimary()
        {
            return pool_primary;
        }


        ///查询出所有的数据
        public static List<Table_role> GetAllPrimaryList()
        {
            return all_Table_role_List;
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
            List<Table_role> rows = _rows as List<Table_role>;
            pool_primary = TableContent.ListToPool<int, Table_role>(rows, "map", "id");
            all_Table_role_List = rows;
        }

        ///<summary>
        /// 清理静态数据
        ///</summary>
        public static void Clear()
        {
            pool_primary.Clear();
            all_Table_role_List.Clear();
        }

    }
}
