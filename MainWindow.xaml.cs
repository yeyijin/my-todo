using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ToDoList
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //初始化操作
            WinodowInit();
        }

        public void WinodowInit()
        {
            #region 初始化一部分窗口绑定

            MaxBtn.Click += MaxBtn_Click;
            MinBtn.Click += MinBtn_Click;
            CloseBtn.Click += CloseBtn_Click;
            //窗口拖动设置
            TopDock.MouseMove += TopDock_MouseMove;

            #endregion 初始化一部分窗口绑定

            MenuItemsListBox.SelectionChanged += MenuItemsListBox_SelectionChanged;
        }

        private void MenuItemsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MenuToggleButton.IsChecked = false;
        }

        private void TopDock_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                this.DragMove();
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MinBtn_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void MaxBtn_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = (WindowState)(WindowState.Maximized - this.WindowState);
        }
    }
}