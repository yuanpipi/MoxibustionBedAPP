using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
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

            //将PropertyModel注册为资源
            PropertyModelInstance = new PropertyModel();
            PropertyModelInstance.Upper_CabinTemperature = 40;//上舱温度
            PropertyModelInstance.BackTemperature = 40;//背部温度
            PropertyModelInstance.LegTemperature = 40;//腿部温度
            PropertyModelInstance.PreheadTemperature = 40;//预热温度
            PropertyModelInstance.UpperAlarmCabinTemperature = 67;//上舱报警温度
            PropertyModelInstance.BackAlarmTemperature = 77;//背部报警温度
            PropertyModelInstance.LegAlarmTemperature = 77;//腿部报警温度
            PropertyModelInstance.PreheadTime = 30;//预热时间
            PropertyModelInstance.MoxibustionTherapyTime = 90;//灸疗时间
            PropertyModelInstance.InignitionTime = 20;//点火时间
            PropertyModelInstance.AutomaticLidOpening = true;//自动开盖


            PropertyModelInstance.MoxibustionTherapyMode = false;//灸疗模式
            PropertyModelInstance.InfraredLamp = 0;//红外灯
            PropertyModelInstance.SmokeExhaustSystem = 0;//排烟系统
            PropertyModelInstance.SmokePurificationSystem=false;//净烟系统
            PropertyModelInstance.SwingSystem = false;//摇摆系统
            PropertyModelInstance.InignitionStatus = false;//点火模式
            PropertyModelInstance.PreheadMode = false;//预热模式
            PropertyModelInstance.Hatch = false;//舱盖
            PropertyModelInstance.BackMoxibustionColumn_Height = 3;//背部灸柱高度
            PropertyModelInstance.LegMoxibustionColumn_Height = 2;//腿部灸柱高度
            PropertyModelInstance.BatteryLevel = 1;//电池电量
            PropertyModelInstance.IsMoxibustionTherapyMode = false;//治疗状态

            //打开串口并且接受来自底层的数据
            SerialPortManager.Instance.OpenPort();

            //定义一个共享的PlayMusicViewModel
            sharedPlayMusicModel = new PlayMusicViewModel();
            //显示在两块屏幕上
            var screen1 = System.Windows.Forms.Screen.AllScreens[0];
            var screen2 = System.Windows.Forms.Screen.AllScreens[1];

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


            //显示加载界面

            LoadWindowView loadWindow = new LoadWindowView();
            loadWindow.Show();

            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(3)
            };
            timer.Tick += (sender, args) =>
            {
                window1.Show();
                window2.Show();
                loadWindow.Close();
                ((DispatcherTimer)sender).Stop();
            };
            timer.Start();

        }
    }
}
