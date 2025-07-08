using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Win32;
using MoxibustionBedAPP.Models;
using MoxibustionBedAPP.ViewModes;
using MoxibustionBedAPP.Views;

namespace MoxibustionBedAPP
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public static PlayMusicViewModel sharedPlayMusicModel;
        public static PropertyModel PropertyModelInstance { get; set; }

        public static TestWindowView Test { get;set; }

        public static bool IsReceive { get; set; }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;//获取当前程序的可执行文件的路径

            string keyName = "MoxibustionBedApp";//注册表键值名称

            RegistryKey key= Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);//打开或创建一个注册表
            if(key==null)
            {
                throw new Exception("无法打开注册表项");
            }
            key.SetValue(keyName, exePath);//设置注册表项的值，将程序添加到自启列表
            key.Close();//关闭注册表项
            Console.WriteLine("程序已添加到开机自启列表");




            //将PropertyModel注册为资源
            PropertyModelInstance = new PropertyModel();
            PublicMethods.ReadFromJson();//开机自启时自动获取保存的文件
            PublicMethods.ReadFromCOMJson();//开机自启时自动获取保存的COM文件

            //打开串口并且接受来自底层的数据
            SerialPortManager.Instance.OpenPort();

            //定义一个共享的PlayMusicViewModel
            sharedPlayMusicModel = new PlayMusicViewModel();
            //显示在两块屏幕上
            //var screen1 = System.Windows.Forms.Screen.AllScreens[0];


            //显示加载界面

            LoadWindowView loadWindow = new LoadWindowView();
            MainWindow = loadWindow;
            MainWindow.Show();
            LoadMainPageAsync();


            var screen2 = System.Windows.Forms.Screen.AllScreens[1];
            if (screen2 == System.Windows.Forms.Screen.PrimaryScreen)
            {
                screen2 = System.Windows.Forms.Screen.AllScreens[0];
            }
            //var screen2 = System.Windows.Forms.Screen.PrimaryScreen;

            //创建window1，并将sharedPlayMusicModel传入
            //var window1 = new MainWindowView(sharedPlayMusicModel)
            //{
            //    WindowState = WindowState.Normal,
            //    WindowStyle = WindowStyle.None,
            //    Title = "华伟医疗 - 医师",
            //    Width = screen1.Bounds.Width,
            //    Height = screen1.Bounds.Height,
            //    Left = screen1.Bounds.Left,
            //    Top = screen1.Bounds.Top
            //};
            //创建window2，并将sharedPlayMusicModel传入
            if (screen2 != null)

            {
                var window2 = new MainWindowCopyView(sharedPlayMusicModel)
                {
                    WindowState = WindowState.Normal,
                    WindowStyle = WindowStyle.None,
                    Title = "华伟医疗 - 患者",
                    Width = screen2.Bounds.Width,
                    Height = screen2.Bounds.Height,
                    Left = screen2.Bounds.Left,
                    Top = screen2.Bounds.Top,
                    //IsEnabled = false
                };
                window2.Show();
            }

            //loadWindow.Show();

            //Task.Run(async () =>
            //{
            //    await Task.Delay(3000);

            //    MainWindow.Dispatcher.Invoke(() =>
            //    {
            //        //MainWindow.Close();
            //        //MainWindow = window1;
            //        //MainWindow.Show();
            //        loadWindow.Close();
            //        window1.Show();

            //    });
            //});

            //var timer = new DispatcherTimer
            //{
            //    Interval = TimeSpan.FromSeconds(3)
            //};
            //timer.Tick += (sender, args) =>
            //{
            //    window1.Show();
            //    window2.Show();
            //    loadWindow.Close();
            //    ((DispatcherTimer)sender).Stop();
            //};
            //timer.Start();

            //window1.Show();
            //window2.Show();
        }

        private async void LoadMainPageAsync()
        {
            await Task.Delay(3000);

            //显示在两块屏幕上
            //var screen1 = System.Windows.Forms.Screen.AllScreens[0];
            var screen1 = System.Windows.Forms.Screen.PrimaryScreen;

            //创建window1，并将sharedPlayMusicModel传入
            var window1 = new MainWindowView(sharedPlayMusicModel)
            {
                WindowState = WindowState.Normal,
                WindowStyle = WindowStyle.None,
                Title = "华伟医疗 - 医师",
                Width = screen1.Bounds.Width,
                Height = screen1.Bounds.Height,
                Left = screen1.Bounds.Left,
                Top = screen1.Bounds.Top
            };

            MainWindow.Close();
            MainWindow = window1;
            MainWindow.Show();
            if(SerialPortManager.Instance.comfail.Visibility== Visibility.Collapsed)
            {
                SerialPortManager.Instance.comfail.Close();
            }
        }

        //private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        //{
        //    MessageBox.Show($"致命错误: {e.Exception.Message}");
        //    e.Handled = true; // 防止进程终止
        //                      // 记录日志
        //    File.WriteAllText("error.log", e.Exception.ToString());
        //}

    }
}
