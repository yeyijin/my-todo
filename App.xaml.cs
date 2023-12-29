using Prism.DryIoc;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Services.Dialogs;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using ToDoList.IService;
using ToDoList.Resposity;
using ToDoList.ViewModels;
using ToDoList.Views;

namespace ToDoList
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            //注册启动窗体
            return Container.Resolve<MainWindow>();
        }

        protected override void OnInitialized()
        {
 
            var mainWC= this.MainWindow.DataContext as IConfigutaionInitWindow;
            mainWC?.InitConfiguration();    
            base.OnInitialized();
        }

        /// <summary>
        /// 依赖注入
        /// </summary>
        /// <param name="containerRegistry"></param>
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            ////直接注册导航页面
            ///
            containerRegistry.RegisterForNavigation<MainWindow, MainWindowViewModel>();
            containerRegistry.RegisterForNavigation<HomePage, HomePageViewModel>();
            containerRegistry.RegisterForNavigation<DialogTipPage, DialogTipPageViewModel>();

            //注册db服务
            string filePath = System.Environment.CurrentDirectory + "/TodoDb.db";
            ResposityManager resposityManager = new ResposityManager("data source = " + filePath);
            containerRegistry.RegisterSingleton<ResposityManager>(u => resposityManager);
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            /////模块引用的方式注册
            //moduleCatalog.AddModule<CommandAndNoticeWindowsModule>();

            //base.ConfigureModuleCatalog(moduleCatalog);
        }
    }
}
