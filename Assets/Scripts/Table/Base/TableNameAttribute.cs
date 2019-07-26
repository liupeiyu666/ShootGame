using System;
namespace ZTool.Table
{
    /// <summary>
    /// Table预制表头，读取加载配置时使用
    /// </summary>
    public class TableNameAttribute : Attribute
    {
        public string tableName;
        public TableNameAttribute(string _tableName)
        {
            this.tableName = _tableName;
        }
    }
}