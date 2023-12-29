using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoList.ViewModels
{
    public class DialogTipPageViewModel : BindableBase, IDialogAware
    {
        private readonly IRegionManager _regionManager;

        /// <summary>
        /// 处理结果关键
        /// </summary>
        public event Action<IDialogResult> RequestClose;

        public DialogTipPageViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;

            SureCommand = new DelegateCommand(sure);
            CancelCommand = new DelegateCommand(cancel);
        }


        private string  _contentTitle;

        public string  ContentTitle
        {
            get { return _contentTitle; }
            set { SetProperty(ref _contentTitle, value); }
        }

        private string _content;

        public string Content
        {
            get { return _content; }
            set { SetProperty(ref _content, value); }
        }




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

        public DelegateCommand SureCommand { get; private set; }

        public DelegateCommand CancelCommand { get; private set; }

        /// <summary>
        /// true 表示页面不引用还是会存在，false 表示会重新实例 ，优先级大于IsNavigationTarget
        /// </summary>
        public bool KeepAlive => true;

        public string Title => "这是弹窗啊";
    }
}
