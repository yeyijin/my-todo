using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace ToDoList.Models
{

    [SugarTable(nameof(MenuInfo))]
    public class MenuInfo
    {

        [SugarColumn(IsPrimaryKey = true, IsIdentity =true)]
        public int Id { get; set; }

        public string Icon { get; set; }

        public string Title { get; set; }

        public string Navigation { get; set; }
    }
}
