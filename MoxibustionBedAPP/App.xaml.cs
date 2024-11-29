using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
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
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            //打开串口并且接受来自底层的数据
            SerialPortManager.Instance.OpenPort();

            //定义一个共享的PlayMusicViewModel
            PlayMusicViewModel sharedPlayMusicModel= new PlayMusicViewModel();
            //sharedPlayMusicModel.ReadFileNamesFromFolder(@".\Resources\Music");
            //显示在两块屏幕上
            var screen1 = System.Windows.Forms.Screen.AllScreens[0];
            var screen2 = System.Windows.Forms.Screen.AllScreens[1];

            //创建window1，并将sharedPlayMusicModel传入
            var window1 = new MainWindowView(sharedPlayMusicModel)
            {
                WindowStartupLocation = WindowStartupLocation.Manual,
                WindowState = WindowState.Normal,
                WindowStyle = WindowStyle.None,
                Title = screen1.DeviceName,
                Width = screen1.Bounds.Width,
                Height = screen1.Bounds.Height,
                Left = screen1.Bounds.Left,
                Top = screen1.Bounds.Top
            };

            //创建window2，并将sharedPlayMusicModel传入
            var window2 = new MainWindowCopyView(sharedPlayMusicModel)
            {
                WindowStartupLocation = WindowStartupLocation.Manual,
                WindowState = WindowState.Normal,
                WindowStyle = WindowStyle.None,
                Title = screen2.DeviceName,
                Width = screen2.Bounds.Width,
                Height = screen2.Bounds.Height,
                Left = screen2.Bounds.Left,
                Top = screen2.Bounds.Top,
                IsEnabled = false
            };
            window1.Show();
            window2.Show();

            ////现在在两块屏幕上
            //foreach (var screen in System.Windows.Forms.Screen.AllScreens)
            //{
            //    var window = new MainWindowView
            //    {
            //        WindowStartupLocation = WindowStartupLocation.Manual,
            //        WindowState = WindowState.Normal,
            //        WindowStyle = WindowStyle.None,
            //        Title = screen.DeviceName,
            //        Width = screen.Bounds.Width,
            //        Height = screen.Bounds.Height,
            //        Left = screen.Bounds.Left,
            //        Top = screen.Bounds.Top
            //    };
            //    window.Show();
            //}
            
        }
    }
}
