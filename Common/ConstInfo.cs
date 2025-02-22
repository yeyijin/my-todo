namespace ToDoList.Common
{
    public static class ConstInfo
    {
        public const string MainControlRegion = "MainControlRegion";

        public const string DialogTipPage = "DialogTipPage";

        //今日专注
        public const string ToadyFocusKey = "ToadyFocusKey";

        //今日番茄闹钟
        public const string ToadyTomatoKey = "ToadyTomatoKey";

        //今日成功
        public const string ToadySuccessedKey = "ToadySuccessedKey";

        //今日逾期
        public const string ToadyOverdueKey = "ToadyOverdueKey";

        //今日目标数
        public const string ToadyGoalKey = "ToadyGoalKey";

        //今日进行中
        public const string ToadyOnGoingKey = "ToadyOnGoingKey";

        //时钟开始图标
        public const string ClockStartIcon = "PlayOutline";

        //时钟结束图标
        public const string ClockStopIcon = "PlayPause";

        ///番茄闹钟成功时间
        public const int TomatoClockSuccess = 60 * 25;

        //番茄闹钟休息时间
        public const int TomateClockTest = 60 * 5;

        /// <summary>
        /// 同步时间。秒数
        /// </summary>
        public const int DataSyncTimeSecond = 60;
    }
}