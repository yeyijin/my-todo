using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;

namespace ToDoList.ViewModels
{
    /// <summary>
    /// 提示框
    /// </summary>
    public class DialogTipPageViewModel : BindableBase, IDialogAware
    {
        #region Fields

        private readonly IRegionManager _regionManager;

        private string _content;

        private string _contentTitle;

        #endregion Fields

        #region Public Constructors

        public DialogTipPageViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;

            SureCommand = new DelegateCommand(sure);
            CancelCommand = new DelegateCommand(cancel);
        }

        #endregion Public Constructors

        #region Events

        /// <summary>
        /// 处理结果关键
        /// </summary>
        public event Action<IDialogResult> RequestClose;

        #endregion Events

        #region Properties

        public DelegateCommand CancelCommand { get; private set; }

        public string Content
        {
            get { return _content; }
            set { SetProperty(ref _content, value); }
        }

        public string ContentTitle
        {
            get { return _contentTitle; }
            set { SetProperty(ref _contentTitle, value); }
        }
        /// <summary>
        /// true 表示页面不引用还是会存在，false 表示会重新实例 ，优先级大于IsNavigationTarget
        /// </summary>
        public bool KeepAlive => true;

        public DelegateCommand SureCommand { get; private set; }

        public string Title => "这是弹窗啊";

        #endregion Properties

        #region Public Methods

        public bool CanCloseDialog()
        {
            return true;
        }

        /// <summary>
        /// 关闭是触发
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void OnDialogClosed()
        {
        }

        /// <summary>
        /// 打开时触发
        /// </summary>
        /// <param name="parameters"></param>
        public void OnDialogOpened(IDialogParameters parameters)
        {
            _contentTitle = parameters.GetValue<string>("Tilte");
            _content = parameters.GetValue<string>("Content");
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// 取消命令
        /// </summary>
        private void cancel()
        {
            //取消，设置
            RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel));
        }

        /// <summary>
        /// 成功确定命令
        /// </summary>
        private void sure()
        {
            //设置回调给主窗体的参数
            DialogParameters pa = new DialogParameters();
            pa.Add("Id", "456");
            //确定，设置参数回传到主窗体
            RequestClose?.Invoke(new DialogResult(ButtonResult.OK, pa));
        }

        #endregion Private Methods
    }
}