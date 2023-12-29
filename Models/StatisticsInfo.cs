using Prism.Mvvm;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoList.Models
{
    [SugarTable(nameof(StatisticsInfo))]
    public class StatisticsInfo:BindableBase
    {

        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        /// <summary>
        /// 唯一标识
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description{ get; set; }

        /// <summary>
        /// 数值
        /// </summary>
        private double _val;

        public double Val
        {
            get { return _val; }
            set { SetProperty(ref _val,value); }
        }



        /// <summary>
        /// 背景颜色
        /// </summary>
        public string BgColor { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Icon { get; set; }

        [SugarColumn(IsIgnore=true)]
        public string Content { get => Val + Description; }

        public DateTime Date { get; set; }  
    }
}
