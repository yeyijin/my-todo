using Prism.Mvvm;
using SqlSugar;
using System;

namespace ToDoList.Models
{
    /// <summary>
    /// todo任务
    /// </summary>
    [SugarTable(nameof(ToDoInfo))]
    public class ToDoInfo : BindableBase
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        [SugarColumn(IsIgnore = true)]
        public string StartHour { get; set; }

        public DateTime StartTime { get; set; }

        [SugarColumn(IsIgnore = true)]
        public string EndHour { get; set; }

        public DateTime EndTime { get; set; }

        /// <summary>
        /// 是否逾期
        /// </summary>
        public bool Overdue { get; set; }

        /// <summary>
        /// 完成时间
        /// </summary>
        public DateTime CompleteTime { get; set; }

        /// <summary>
        /// 总完成时间 单位秒
        /// </summary>
        private double _completeMin;

        public double CompleteMin
        {
            get { return _completeMin; }
            set { SetProperty(ref _completeMin, value); }
        }

        /// <summary>
        /// 番茄时钟数量
        /// </summary>
        public int TomatoCount { get; set; }

        /// <summary>
        /// 是否已经完成
        /// </summary>
        public bool IsComplete { get; set; }
    }
}