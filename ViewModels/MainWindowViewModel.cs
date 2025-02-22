using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using ToDoList.Common;
using ToDoList.IService;
using ToDoList.Models;

namespace ToDoList.ViewModels
{
    public class MainWindowViewModel : BindableBase, IConfigutaionInitWindow
    {
        private ObservableCollection<MenuInfo> _menuItemsList;

        public ObservableCollection<MenuInfo> MenuItemsList
        {
            get { return _menuItemsList; }
            set { SetProperty(ref _menuItemsList, value); }
        }

        private MenuInfo _selectedItem;

        public MenuInfo SelectedItem
        {
            get { return _selectedItem; }
            set { SetProperty(ref _selectedItem, value); }
        }


        public DelegateCommand SelectMenuChangedCommand { get; private set; }
        public DelegateCommand GoBackCommand { get; private set; }

        public DelegateCommand GoPreCommand { get; private set; }

        public DelegateCommand GoHomeCommand { get; private set; }

        private IRegionNavigationJournal _regionNavigationJournal;
        private readonly IRegionManager _regionManager;

        public MainWindowViewModel(IRegionManager regionManager)
        {
            _menuItemsList = new ObservableCollection<MenuInfo>();
            InitMenuList();

            SelectMenuChangedCommand = new DelegateCommand(SelectMenuChanged);
            this._regionManager = regionManager;
            GoBackCommand = new DelegateCommand(GoBack);
            GoPreCommand = new DelegateCommand(GoPre);
            GoHomeCommand = new DelegateCommand(GoHome);
        }

        private void SelectMenuChanged()
        {
            //执行跳转
            if (_selectedItem == null)
                return;
            //实现跳转
            JumpNavigate(Common.ConstInfo.MainControlRegion, _selectedItem.Navigation, null);
        }

        private void InitMenuList()
        {
            _menuItemsList.Add(new MenuInfo()
            {
                Icon = "Home",
                Title = "主页",
                Navigation = "HomePage"
            });
            _menuItemsList.Add(new MenuInfo()
            {
                Icon = "Home",
                Title = "主页",
                Navigation = "HomePage"
            });

        }

        /// <summary>
        /// 导航跳转
        /// </summary>
        /// <param name="contentResion"></param>
        /// <param name="page"></param>
        /// <param name="navigationParameters"></param>
        private void JumpNavigate(string contentResion, string page, NavigationParameters navigationParameters = null)
        {
            _regionManager.RequestNavigate(
           contentResion,
            new Uri(page, UriKind.Relative)
              , navigationCallback: aa =>
              {
                  if (aa.Result ?? false)
                  {
                      //只需要跳转的地方赋值，后续其他页面跳转，调用会自动有信息
                      _regionNavigationJournal = aa.Context.NavigationService.Journal;
                  }
              }, navigationParameters
              );
        }

        //主页
        private void GoHome()
        {
            //跳转
            JumpNavigate(Common.ConstInfo.MainControlRegion, "HomePage");
        }
        //前
        private void GoPre()
        {
            if (_regionNavigationJournal != null && _regionNavigationJournal.CanGoForward)
                _regionNavigationJournal.GoForward();
        }
        //后退
        private void GoBack()
        {
            if (_regionNavigationJournal != null && _regionNavigationJournal.CanGoBack)
                _regionNavigationJournal.GoBack();
        }

        /// <summary>
        /// 初始化配置
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void InitConfiguration()
        {
            Grid grid = new Grid();
            GoHome();
        }
    }
}
