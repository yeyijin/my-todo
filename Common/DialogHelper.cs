using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ToDoList.Common
{
    public static class DialogHelper
    {

        public static void Open(this IDialogService dialogService
            , string viewName
            , DialogParameters parameter
            , Action<IDialogResult> success
            , Action<IDialogResult> cancel)
        {
            ///弹窗，传递参数

            //触发弹窗打开
            dialogService.ShowDialog(viewName, parameter, callback =>
            {

                if (callback.Result == ButtonResult.OK)
                {
                    //获取弹窗返回的数据
                    if (success != null)
                        success(callback);

                }
                else if (callback.Result == ButtonResult.Cancel)
                {
                    cancel(callback);
                }
            });
        }

        public static void Open(this IDialogService dialogService
         , string viewName
         , string title
         , string content
         , Action<IDialogResult> success
         , Action<IDialogResult> cancel)
        {
            ///弹窗，传递参数
            DialogParameters parameter=new DialogParameters();
            parameter.Add("Title", title);
            parameter.Add("Content", content);
            //触发弹窗打开
            dialogService.ShowDialog(viewName, parameter, callback =>
            {

                if (callback.Result == ButtonResult.OK)
                {
                    //获取弹窗返回的数据
                    if (success != null)
                        success(callback);

                }
                else if (callback.Result == ButtonResult.Cancel)
                {
                    cancel(callback);
                }
            });
        }
    }
}
