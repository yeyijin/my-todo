using SqlSugar;

namespace ToDoList.Models
{
    /// <summary>
    /// 菜单信息
    /// </summary>
    [SugarTable(nameof(MenuInfo))]
    public class MenuInfo
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        public string Icon { get; set; }

        public string Title { get; set; }

        public string Navigation { get; set; }
    }
}