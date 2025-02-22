using DryIoc;
using ImTools;
using MaterialDesignThemes.Wpf;
using Prism.Commands;
using Prism.Common;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ToDoList.Common;
using ToDoList.Models;
using ToDoList.Resposity;
using static ImTools.ImMap;

namespace ToDoList.ViewModels
{
    internal class HomePageViewModel : BindableBase
    {
        private System.Timers.Timer timer = null;



        private bool _bkDataSyncRun = false;
        //后台同步任务时钟
        private System.Timers.Timer _bkDataSyncTimer = null;

        private ToDoInfo _todoInfo;

        /// <summary>
        /// 新增或者删除的类
        /// </summary>
        public ToDoInfo TodoInfo
        {
            get { return _todoInfo; }
            set { SetProperty(ref _todoInfo, value); }
        }

        private ToDoInfo _currentTodo;

        /// <summary>
        /// 当前番茄闹钟类
        /// </summary>
        public ToDoInfo CurrentTodo
        {
            get { return _currentTodo; }
            set { SetProperty(ref _currentTodo, value); }
        }


        #region MyRegion
        /// <summary>
        /// 今日聚焦
        /// </summary>
        private StatisticsInfo _todayFocus;

        public StatisticsInfo TodayFocus
        {
            get { return _todayFocus; }
            set { SetProperty(ref _todayFocus, value); }
        }

        /// <summary>
        /// 今日目标数
        /// </summary>
        private StatisticsInfo _todayGoal;

        public StatisticsInfo TodayGoal
        {
            get { return _todayGoal; }
            set { SetProperty(ref _todayGoal, value); }
        }

        /// <summary>
        /// 今日完成
        /// </summary>
        private StatisticsInfo _todaySuccess;

        public StatisticsInfo TodaySuccess
        {
            get { return _todaySuccess; }
            set { SetProperty(ref _todaySuccess, value); }
        }

        /// <summary>
        /// 今日逾期
        /// </summary>
        private StatisticsInfo _todayOverdue;

        public StatisticsInfo TodayOverdue
        {
            get { return _todayOverdue; }
            set { SetProperty(ref _todayOverdue, value); }
        }

        /// <summary>
        /// 今日进行中
        /// </summary>
        private StatisticsInfo _todayOnGoing;

        public StatisticsInfo TodayOnGoing
        {
            get { return _todayOnGoing; }
            set { SetProperty(ref _todayOnGoing, value); }
        }
        #endregion


        //最上面那部分
        private int _statisticsInfoCount;

        public int StatisticsInfoCount
        {
            get { return _statisticsInfoCount; }
            set { SetProperty(ref _statisticsInfoCount, value); }
        }


        private ObservableCollection<StatisticsInfo> _statisticsInfos;
        public ObservableCollection<StatisticsInfo> StatisticsInfos
        {
            get { return _statisticsInfos; }
            set { SetProperty(ref _statisticsInfos, value); }
        }


        /// <summary>
        /// 进行中列表
        /// </summary>
        private ObservableCollection<ToDoInfo> _onGoingToDoList;
        public ObservableCollection<ToDoInfo> OnGoingToDoList
        {
            get { return _onGoingToDoList; }
            set { SetProperty(ref _onGoingToDoList, value); }
        }

        /// <summary>
        /// 成功完成列表
        /// </summary>
        private ObservableCollection<ToDoInfo> _successToDoList;
        public ObservableCollection<ToDoInfo> SuccessToDoList
        {
            get { return _successToDoList; }
            set { SetProperty(ref _successToDoList, value); }
        }

        /// <summary>
        /// dialog
        /// </summary>
        private bool _isDialogOpen;
        public bool IsDialogOpen
        {
            get { return _isDialogOpen; }
            set { SetProperty(ref _isDialogOpen, value); }
        }
        private string _clockTime;
        public string ClockTime
        {
            get { return _clockTime; }
            set { SetProperty(ref _clockTime, value); }
        }
        /// <summary>
        /// 统计秒速
        /// </summary>
        private int _clockSecond;
        public int ClockSecond
        {
            get { return _clockSecond; }
            set { SetProperty(ref _clockSecond, value); ClockTime = FomartClockTime(); }
        }

        public bool AddOrEditBtnEnable
        {
            get
            {
                return TodoInfo != null && TodoInfo.Title.IsNotNullOrEmpty()
                    && TodoInfo.Description.IsNotNullOrEmpty()
                    && TodoInfo.StartHour.IsNotNullOrEmpty()
                    && TodoInfo.EndHour.IsNotNullOrEmpty()
                    && TodoInfo.StartTime < TodoInfo.EndTime
                    ;
            }

        }



        //是否右边窗口show
        private bool _isRightShow;
        public bool IsRightShow
        {
            get { return _isRightShow; }
            set { SetProperty(ref _isRightShow, value); }
        }

        public DelegateCommand AddClickCommond { get; private set; }

        public DelegateCommand AddOrEditSubmitCommond { get; private set; }


        public DelegateCommand<ToDoInfo> TodoStatusChangeCommand { get; private set; }




        //
        public DelegateCommand<ToDoInfo> TodoDeleteCommand { get; private set; }

        public DelegateCommand<ToDoInfo> TodoUpdateCommand { get; private set; }

        public DelegateCommand<ToDoInfo> SetCurrentTodoCommand { get; private set; }

        private DateTime _nowDate;

        private ResposityManager _resposityManager;
        public HomePageViewModel(IDialogService dialogService)
        {
            _nowDate = DateTime.Now.Date;
            _resposityManager = ContainerLocator.Container.Resolve<ResposityManager>();

            AddClickCommond = new DelegateCommand(AddClick);
            AddOrEditSubmitCommond = new DelegateCommand(AddOrEditSubmit);
            TodoStatusChangeCommand = new DelegateCommand<ToDoInfo>(TodoStatusChange);
            TodoDeleteCommand = new DelegateCommand<ToDoInfo>(TodoDeleteSubmit);
            TodoUpdateCommand = new DelegateCommand<ToDoInfo>(TodoUpdateSubmit);
            SetCurrentTodoCommand = new DelegateCommand<ToDoInfo>(SetCurrentTodo);
            TomatoExcuteCommand = new DelegateCommand(TomatoExcute);
            TomatoResetCommand = new DelegateCommand(TomatoReset);
            InitData().GetAwaiter().GetResult();
            _dialogService = dialogService;

        }

        private void SetCurrentTodo(ToDoInfo info)
        {
            IsDialogOpen = true;
            //设置当前todo 为当前
            //会清空当前闹钟，有一个确定按钮
            DialogHelper.Open(_dialogService, Common.ConstInfo.DialogTipPage, "确定设置为当前任务吗", "确定设置为当前任务吗", u =>
            {
                //初始化闹钟状态
                InitClockTime();
                //设置当前任务
                CurrentTodo = info;

            }, cancel: u => { });
            IsDialogOpen = false;
        }

        private void TodoUpdateSubmit(ToDoInfo info)
        {
            if (info == null)
                return;
            TodoInfo = info.CopyTo<ToDoInfo>();
            IsRightShow = true;
            StatictistInfo();
        }

        private void TodoDeleteSubmit(ToDoInfo info)
        {
            IsDialogOpen = true;
            if (info == null)
                return;
            var pa = new DialogParameters();
            pa.Add("Title", "删除计划清单");
            pa.Add("Content", $"确定删除该计划清单嘛:{info.Title}。");
            _dialogService.Open(ConstInfo.DialogTipPage, pa, async success =>
            {
                //执行成功操作
                await _resposityManager.DeleteAsync(info);
                //已出队列
                if (info.IsComplete)
                {
                    var item = SuccessToDoList.FindFirst(u => u.Id == info.Id);
                    SuccessToDoList.Remove(item);
                }
                else
                {
                    var item = OnGoingToDoList.FindFirst(u => u.Id == info.Id);
                    OnGoingToDoList.Remove(item);
                }
            }, cancel =>
            {

            });
            IsDialogOpen = false;

            StatictistInfo();
        }

        private async void TodoStatusChange(ToDoInfo info)
        {
            if (info == null)
                return;
            if (info.Id > 0)
                await _resposityManager.UpdateAsync(info);
            if (!info.IsComplete)
            {
                //原本是成功的
                var item = SuccessToDoList.FindFirst(u => u.Id == info.Id);
                SuccessToDoList.Remove(item);
                //往失败里面加
                OnGoingToDoList.Add(item);
            }
            else
            {
                //原本是进行中
                var item = OnGoingToDoList.FindFirst(u => u.Id == info.Id);
                OnGoingToDoList.Remove(item);
                //往失败里面加
                SuccessToDoList.Add(item);

                //如果设置的是当前状态，那么重置
                if (CurrentTodo != null && CurrentTodo.Id == info.Id)
                {
                    //重置
                    InitClockTime();
                    CurrentTodo = null;
                }

            }
            StatictistInfo();
        }

        private async void AddOrEditSubmit()
        {
            string day = $"{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day}";
            TodoInfo.StartTime = DateTime.Parse($"{day} {TodoInfo.StartHour}");
            TodoInfo.EndTime = DateTime.Parse($"{day} {TodoInfo.EndHour}");

            //数据校验
            //展示dialog
            if (AddOrEditBtnEnable == false)
            {
                IsDialogOpen = true;



                return;
            }


            //对象处理，默认时间
            if (TodoInfo.Id <= 0)
            {
              var id=  await _resposityManager.AddAsync(TodoInfo);
                TodoInfo.Id = id;
                //新增todo list
                OnGoingToDoList.Add(TodoInfo);
            }
            else
            {
                await _resposityManager.UpdateAsync(TodoInfo);

                //查找对应的数据更新
                if (TodoInfo.IsComplete)
                {
                    var item = SuccessToDoList.FindFirst(U => U.Id == TodoInfo.Id);
                    int index = SuccessToDoList.IndexOf(item);
                    SuccessToDoList.Remove(item);
                    SuccessToDoList.Insert(index, TodoInfo);
                }
                else
                {
                    var item = OnGoingToDoList.FindFirst(U => U.Id == TodoInfo.Id);
                    int index = OnGoingToDoList.IndexOf(item);
                    OnGoingToDoList.Remove(item);
                    OnGoingToDoList.Insert(index, TodoInfo);
                }
                //当前的也更新
                if (CurrentTodo != null && CurrentTodo.Id == TodoInfo.Id)
                {
                    CurrentTodo = TodoInfo;
                }
            }
            TodoInfo = new ToDoInfo(); //重新设置绑定对象
            //关闭右弹窗
            IsRightShow = false;

            StatictistInfo();
        }

        private void AddClick()
        {
            IsRightShow = true;
            TodoInfo = new ToDoInfo();
        }

        /// <summary>
        /// 防止跨天的出问题
        /// </summary>
        private async Task LoadStatisInfoData()
        {
            _statisticsInfos.Clear();
            var statictisInfos = await _resposityManager.AsQueryable<StatisticsInfo>().Where(u => u.Date == DateTime.Now.Date).ToListAsync();
            statictisInfos = new List<StatisticsInfo>();
            if (statictisInfos.Count <= 0)
            {
                _statisticsInfos.Add(new StatisticsInfo() { BgColor = "#F08080", Key = Common.ConstInfo.ToadyFocusKey, Title = "今日专注时长", Icon = "ImageFilterCenterFocus", Val = 0, Description = "分钟", Date = DateTime.Now });
                _statisticsInfos.Add(new StatisticsInfo() { BgColor = "#E08080", Key = Common.ConstInfo.ToadyTomatoKey, Title = "今日番茄闹钟", Icon = "ImageFilterCenterFocus", Val = 0, Description = "个", Date = DateTime.Now });
                _statisticsInfos.Add(new StatisticsInfo() { BgColor = "#2EB7FC", Key = Common.ConstInfo.ToadyGoalKey, Title = "今日目标", Icon = "Basketball", Val = 0, Description = "", Date = DateTime.Now });
                _statisticsInfos.Add(new StatisticsInfo() { BgColor = "#32CD32", Key = Common.ConstInfo.ToadySuccessedKey, Title = "今日成功", Icon = "CheckCircleOutline", Val = 0, Description = "", Date = DateTime.Now });
                _statisticsInfos.Add(new StatisticsInfo() { BgColor = "#DC143C", Key = Common.ConstInfo.ToadyOverdueKey, Title = "今日逾期", Icon = "RobotVacuumAlert", Val = 0, Description = "", Date = DateTime.Now });
                _statisticsInfos.Add(new StatisticsInfo() { BgColor = "#DAA520", Key = Common.ConstInfo.ToadyOnGoingKey, Title = "今日进行中", Icon = "AirplaneTakeoff", Val = 0, Description = "", Date = DateTime.Now });
                //存储数据库中
                //新增
                //  await _resposityManager.AddAsync(statictisInfos!.ToList());
            }
            else
            {
                _statisticsInfos.AddRange(statictisInfos);
            }

            StatisticsInfoCount = _statisticsInfos.Count;
        }

        private async Task LoadInitTodoList()
        {
            //加载任务列表
            var list = await _resposityManager.AsQueryable<ToDoInfo>().Where(u => u.StartTime >= DateTime.Now.Date && u.StartTime < DateTime.Now.Date.AddDays(1))
            .ToListAsync();

            foreach (var item in list)
            {
                item.StartHour = item.StartTime.ToString("HH:mm");
                item.EndHour = item.EndTime.ToString("HH:mm");

                if (item.IsComplete)
                {
                    SuccessToDoList.Add(item);
                }
                else
                {
                    OnGoingToDoList.Add(item);
                }
            }
        }




        private async Task InitData()
        {
            TodoInfo = null;
            CurrentTodo = null;
            _statisticsInfos = new ObservableCollection<StatisticsInfo>();
            OnGoingToDoList = new ObservableCollection<ToDoInfo>();
            SuccessToDoList = new ObservableCollection<ToDoInfo>();
            await LoadStatisInfoData();
            await LoadInitTodoList();
            //初始化统计
            StatictistInfo();
            timer = new System.Timers.Timer();
            timer.Interval = 1000;
            //  timer.SynchronizingObject = this;

            timer.Elapsed += Timer_Elapsed;
            InitClockTime();

            //开启后台同步任务
            _bkDataSyncTimer = new System.Timers.Timer() { Interval = ConstInfo.DataSyncTimeSecond };
            _bkDataSyncTimer.Elapsed += _bkDataSyncTimer_Elapsed;
            _bkDataSyncTimer.Interval = 1000 * 60;
            _bkDataSyncTimer.Start();
        }



        private readonly IDialogService _dialogService;

        #region 番茄闹钟逻辑


        //tomato 图标
        private string _tomatoRunIcon;
        public string TomatoRunIcon
        {
            get { return _tomatoRunIcon; }
            set
            {
                SetProperty(ref _tomatoRunIcon, value);
            }
        }
        public DelegateCommand TomatoExcuteCommand { get; private set; }
        public DelegateCommand TomatoResetCommand { get; private set; }



        /// <summary>
        /// 是否运行 。有两种状态。 运行和休息
        /// </summary>
        private bool _isTomatoStart;
        /// <summary>
        /// 是否运行 。有两种状态。 运行中，和不运行中
        /// </summary>
        public bool IsTomatoStart
        {
            get { return _isTomatoStart; }
            set
            {
                SetProperty(ref _isTomatoStart, value);
                if (value)
                    TomatoRunIcon = ConstInfo.ClockStopIcon;
                else
                    TomatoRunIcon = ConstInfo.ClockStartIcon;
            }
        }




        private bool _isTomatoWorking;
        /// <summary>
        ///   //闹钟工作中。包含休息和工作中
        /// </summary>
        public bool IsTomatoWorking
        {
            get { return _isTomatoWorking; }
            set
            {

                SetProperty(ref _isTomatoWorking, value);

            }
        }

        private string FomartClockTime()
        {
            return ((int)(ClockSecond / 60)).ToString().PadLeft(2, '0') + ":" + (ClockSecond % 60).ToString().PadLeft(2, '0');
        }

        private void Timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            ClockSecond--;
            Task.Delay(TimeSpan.FromSeconds(5)).GetAwaiter().GetResult();


            ///成功设置停止
            if (ClockSecond <= 0)
            {
                if (IsTomatoWorking)
                {
                    //记录当前时间的番茄闹钟数
                    CurrentTodo.TomatoCount += 1;
                   
                }
                //设置运行状态为停止
                SetTimerStatus(false);
                //跟进工作状态设置想应的时长
                IsTomatoWorking = !IsTomatoWorking;
                ClockSecond = GetTomatoClockTotalSecond();
                StatictistInfo();
                //跨窗体线程不能直接执行，需要这种写法
                App.Current.Dispatcher.Invoke(new Action(delegate
                {
                    //设置窗体最大化
                    App.Current.MainWindow.WindowState = System.Windows.WindowState.Maximized;
                    //设置窗口置顶
                    App.Current.MainWindow.Topmost = true;

                }));
            }
            // 修改当前的
        }

        private int GetTomatoClockTotalSecond()
        {
            return IsTomatoWorking ? ConstInfo.TomatoClockSuccess : ConstInfo.TomateClockTest;
        }

        private void SetTimerStatus(bool isStart)
        {
            if (isStart)
                timer.Start();
            else
                timer.Stop();
            IsTomatoStart = isStart;
        }

        //重置
        private void TomatoReset()
        {
            if (CurrentTodo == null)
                return;

            //重新赋值
            //设置相反的
            IsTomatoWorking = !IsTomatoWorking;
            ClockSecond = GetTomatoClockTotalSecond();
            SetTimerStatus(false);
            StatictistInfo();
        }
        /// <summary>
        /// 初始化闹钟样子
        /// </summary>
        private void InitClockTime()
        {
            SetTimerStatus(false);
            //初始化时间
            IsTomatoWorking = true;

            ClockSecond = GetTomatoClockTotalSecond();
        }
        //开启或者暂停
        private void TomatoExcute()
        {
            if (CurrentTodo == null)
                return;
            //开启
            SetTimerStatus(!IsTomatoStart);
        }
        #endregion

        #region 定时同步任务

        //定时同步任务
        private async void _bkDataSyncTimer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            if (DateTime.Now.Date != _nowDate)
            {
                //重置
                InitData().GetAwaiter().GetResult();
            }

            if (CurrentTodo != null)
            {
                if (IsTomatoWorking && IsTomatoStart)
                    //设置当前有效时间++
                    CurrentTodo.CompleteMin++;
            }
            //数据的存储
            //最新的专注时长更新

            if (OnGoingToDoList.Count > 0)
                await _resposityManager.UpdateAsync<ToDoInfo>(OnGoingToDoList.ToList());
            if (SuccessToDoList.Count > 0)
                await _resposityManager.UpdateAsync<ToDoInfo>(SuccessToDoList.ToList());

            StatictistInfo();
        }


        private void StatictistInfo()
        {
            //今日专注时长
            //从A中读取统计，以及当前转动的闹钟
            int totalFocus = (int)(OnGoingToDoList.Sum(u => u.CompleteMin) + SuccessToDoList.Sum(u => u.CompleteMin));
            int totalTomato = (int)(OnGoingToDoList.Sum(u => u.TomatoCount) + SuccessToDoList.Sum(u => u.TomatoCount));
            //今日目标数
            int goalCount = OnGoingToDoList.Count + SuccessToDoList.Count;
            //今日成功数
            int successCont = SuccessToDoList.Count;
            //今日逾期数
            int overdueCount = OnGoingToDoList.Count(u => u.Overdue == true) + SuccessToDoList.Count(u => u.Overdue == true);
            //今日进行中数
            int ongoing = OnGoingToDoList.Count;

            /*
             
                   _statisticsInfos = new ObservableCollection<StatisticsInfo>();
            _statisticsInfos.Add(new StatisticsInfo() { BgColor = "#F08080", Key = Common.ConstInfo.ToadyFocusKey, Title = "今日专注时长", Icon = "ImageFilterCenterFocus", Val = 0, Description = "分钟" });
            _statisticsInfos.Add(new StatisticsInfo() { BgColor = "#2EB7FC", Key = Common.ConstInfo.ToadyGoalKey, Title = "今日目标", Icon = "Basketball", Val = 0, Description = "" });
            _statisticsInfos.Add(new StatisticsInfo() { BgColor = "#32CD32", Key = Common.ConstInfo.ToadySuccessedKey, Title = "今日成功", Icon = "CheckCircleOutline", Val = 0, Description = "" });
            _statisticsInfos.Add(new StatisticsInfo() { BgColor = "#DC143C", Key = Common.ConstInfo.ToadyOverdueKey, Title = "今日逾期", Icon = "RobotVacuumAlert", Val = 0, Description = "" });
            _statisticsInfos.Add(new StatisticsInfo() { BgColor = "#DAA520", Key = Common.ConstInfo.ToadyOnGoingKey, Title = "今日进行中", Icon = "AirplaneTakeoff", Val = 0, Description = "" });
            StatisticsInfoCount = _statisticsInfos.Count;
             
             */
            _statisticsInfos.FindFirst(u => u.Key == Common.ConstInfo.ToadyFocusKey).Val = totalFocus;
            _statisticsInfos.FindFirst(u => u.Key == Common.ConstInfo.ToadyTomatoKey).Val = totalTomato;

            _statisticsInfos.FindFirst(u => u.Key == Common.ConstInfo.ToadyGoalKey).Val = goalCount;
            _statisticsInfos.FindFirst(u => u.Key == Common.ConstInfo.ToadySuccessedKey).Val = successCont;
            _statisticsInfos.FindFirst(u => u.Key == Common.ConstInfo.ToadyOverdueKey).Val = overdueCount;
            _statisticsInfos.FindFirst(u => u.Key == Common.ConstInfo.ToadyOnGoingKey).Val = ongoing;
        }
        #endregion
    }
}
