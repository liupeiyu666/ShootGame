using System.Collections;
using System.Collections.Generic;
using System;
using ZTool.Table;
namespace  Tgame.Game.Table
{
    ///<summary>
    /// 背包扩展行
    ///</summary>
    [Serializable]
    [TableName("bag_row")]
    public partial class Table_bag_row : TableContent
    {

        private static List<Table_bag_row> all_Table_Bag_Row_List = new List<Table_bag_row>();
        //primary | 主键
        public static Dictionary<int, Table_bag_row > pool_primary = new Dictionary<int, Table_bag_row > ();
        
        
        ///<summary>
        /// 主键：ID
        ///</summary>
        private int _id;

        
        ///<summary>
        /// 开启条件
        ///</summary>
        private int _condition_id;

        
        ///<summary>
        /// 开启消耗道具
        ///</summary>
        private int _cost_item_id;

        
        ///<summary>
        /// 消耗道具数量
        ///</summary>
        private int _cost_item_num;


        ///<summary>
        /// 主键
        /// 查询数据
        ///</summary>
        ///	<param id> 主键：ID</param>
        ///
        public static Table_bag_row GetPrimary ( int _id ){        
            Table_bag_row _map0=null;        
            pool_primary. TryGetValue(_id,out _map0);        
            return  _map0;
        }
         ///<summary>
        ///主键
        ///查询所有数据
        ///</summary>
        public static Dictionary<int, Table_bag_row > GetAllPrimary()
        {
            return pool_primary;
        }


        ///查询出所有的数据
        public static List<Table_bag_row> GetAllPrimaryList()
        {
            return all_Table_Bag_Row_List;
        }

        ///<summary>
        /// 通过字典初始化对象值
        ///</summary>
        public override void ParseFrom(Dictionary<string, string> _itemData) 
        {
            string _currValue = "";
            if(_itemData.TryGetValue("id", out _currValue))
            {
                UGEUtils.GetValueFromString(_currValue,out this._id);
            }
            if(_itemData.TryGetValue("condition_id", out _currValue))
            {
                UGEUtils.GetValueFromString(_currValue,out this._condition_id);
            }
            if(_itemData.TryGetValue("cost_item_id", out _currValue))
            {
                UGEUtils.GetValueFromString(_currValue, out this._cost_item_id);
            }
            if(_itemData.TryGetValue("cost_item_num", out _currValue))
            {
                UGEUtils.GetValueFromString(_currValue, out this._cost_item_num);
            }
        }
        
        ///<summary>
        /// 获取table表名
        ///</summary>
        public override string Table()
        {
           return "bag_row";
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
                case "condition_id":
                    return this._condition_id;
                case "cost_item_id":
                    return this._cost_item_id;
                case "cost_item_num":
                    return this._cost_item_num;
                default:
                    return null;
            }
        }
        
        ///<summary>
        /// 初始化Pool
        ///</summary>
        public static void InitPool(IList _rows){
            List<Table_bag_row> rows = _rows as List<Table_bag_row>;
            pool_primary=TableContent.ListToPool < int, Table_bag_row > ( rows, "map", "id" );
            all_Table_Bag_Row_List=rows;
        }
        
        ///<summary>
        /// 清理静态数据
        ///</summary>
        public static void Clear()
        {
            pool_primary.Clear();
            all_Table_Bag_Row_List.Clear();
        }


        ///<summary>
        /// 主键：ID
        ///</summary>
        public int id
        {
            get
            {
                return this._id;
            }
        }

        ///<summary>
        /// 开启条件
        ///</summary>
        public int condition_id
        {
            get
            {
                return this._condition_id;
            }
        }

        ///<summary>
        /// 开启消耗道具
        ///</summary>
        public int cost_item_id
        {
            get
            {
                return this._cost_item_id;
            }
        }

        ///<summary>
        /// 消耗道具数量
        ///</summary>
        public int cost_item_num
        {
            get
            {
                return this._cost_item_num;
            }
        }
    }
}
