using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
        public static PlayMusicViewModel sharedPlayMusicModel;
        public static PropertyModel PropertyModelInstance { get; set; }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            //显示加载界面
            LoadWindowView loadWindow = new LoadWindowView();
            loadWindow.Show();
            System.Threading.Thread.Sleep(3000);

            //将PropertyModel注册为资源
            PropertyModelInstance = new PropertyModel();
            PropertyModelInstance.Upper_CabinTemperature = "25";
            PropertyModelInstance.BackTemperature = "26";
            PropertyModelInstance.LegTemperature = "28";
            PropertyModelInstance.PreheadTemperature = "10";
            //Resources.Add("PropertyModelKey", PropertyModelInstance);

            //打开串口并且接受来自底层的数据
            SerialPortManager.Instance.OpenPort();

            //定义一个共享的PlayMusicViewModel
            sharedPlayMusicModel = new PlayMusicViewModel();
            //显示在两块屏幕上
            var screen1 = System.Windows.Forms.Screen.AllScreens[0];
            //var screen2 = System.Windows.Forms.Screen.AllScreens[1];

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
           //创建window2，并将sharedPlayMusicModel传入
            //var window2 = new MainWindowCopyView(sharedPlayMusicModel)
            //{
            //    WindowState = WindowState.Normal,
            //    WindowStyle = WindowStyle.None,
            //    Title = "华伟医疗 - 患者",
            //    Width = screen2.Bounds.Width,
            //    Height = screen2.Bounds.Height,
            //    Left = screen2.Bounds.Left,
            //    Top = screen2.Bounds.Top,
            //    //IsEnabled = false
            //};
            window1.Show();
            //window2.Show();
            loadWindow.Close();
        }
    }
}
