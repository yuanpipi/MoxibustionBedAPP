using Microsoft.Xaml.Behaviors;
using MoxibustionBedAPP.Models;
using MoxibustionBedAPP.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Markup.Localizer;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using static System.Net.Mime.MediaTypeNames;

namespace MoxibustionBedAPP.ViewModes
{
    public class FunctionControlViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        #region 自定义变量
        /// <summary>
        /// 公共指令发送方法
        /// </summary>
        public ICommand PublicFunction { get; set; }

        /// <summary>
        /// 预热
        /// </summary>
        public ICommand Prehead { get; set; }

        /// <summary>
        /// 点火
        /// </summary>
        public ICommand Inignition { get; set; }

        /// <summary>
        /// 倒计时
        /// </summary>
        public static DispatcherTimer _timer; 
        public static bool _isCountingDown;
        public bool IsCountingDown
        {
            get { return _isCountingDown; }
            set
            {
                _isCountingDown = value;
                OnPropertyChanged(nameof(IsCountingDown));
            }
        }

        /// <summary>
        /// 是否选中排烟系统
        /// </summary>
        private bool isSmokeExhaust;
        public bool IsSmokeExhaust
        {
            get
            {
                return isSmokeExhaust;
            }
            set
            {
                isSmokeExhaust = value;
                OnPropertyChanged(nameof(IsSmokeExhaust));
            }
        }

        /// <summary>
        /// 排烟系统选中-》关闭净烟系统
        /// </summary>
        public ICommand RadioBtnOfSmoke { get; }

        /// <summary>
        /// radiobutton是否可选
        /// </summary>
        private bool _radioBtnCanUse;
        public bool RadioBtnCanUse
        {
            get
            {
                return _radioBtnCanUse;
            }
            set
            {
                _radioBtnCanUse = value;
                OnPropertyChanged(nameof(RadioBtnCanUse));
            }
        }

        /// <summary>
        /// 是否选中净烟系统
        /// </summary>
        private bool isSmokePurification;
        public bool IsSmokePurification
        {
            get
            {
                return isSmokePurification;
            }
            set
            {
                isSmokePurification = value;
                OnPropertyChanged(nameof(IsSmokePurification));
            }
        }

        /// <summary>
        /// 开关舱倒计时，10s
        /// </summary>
        private DispatcherTimer timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(30)
        };

        /// <summary>
        /// 开关舱倒计时，10s
        /// </summary>
        private DispatcherTimer timer1 = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(30)
        };

        public ICommand StopCommand { get; private set; }

        private bool _smoke;
        public bool Smoke
        {
            get
            {
                return _smoke;
            }
            set
            {
                _smoke = value;
                OnPropertyChanged(nameof(Smoke));
            }
        }

        public static DateTime StartTime;
        public static int seconds;


        private bool isPreheadClicked;//预热
        private bool isInignitionClicked;//点火
        private bool isCloseClick;
        #endregion

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public FunctionControlViewModel()
        {
            PublicFunction = new RelayCommand(ExecuteFunctionMethod);
            Prehead = new RelayCommand(PreheadMethod);
            Inignition= new RelayCommand(InignitionMethod);
            App.PropertyModelInstance.OpenHatch = "pack://application:,,,/Resources/Pictures/HatchBtnBack.png";
            App.PropertyModelInstance.CloseHatch = "pack://application:,,,/Resources/Pictures/HatchBtnBack.png";
            RadioBtnOfSmoke = new RelayCommand(CloseSmokeSystem);
            RadioBtnCanUse = !App.PropertyModelInstance.IsMoxibustionTherapyMode;
            StopCommand = new RelayCommand(StopMethod);
            App.PropertyModelInstance.IsOpen = false;
            App.PropertyModelInstance.IsClose=false;
            isInignitionClicked  = false;
            isPreheadClicked = false;
            isCloseClick = false;
            Smoke = false;
            StartCountdown();
            if (App.PropertyModelInstance.IsSmokePurificationSystem)
            {
                IsSmokeExhaust = false;
                IsSmokePurification = true;
            }
            else
            {
                IsSmokeExhaust = true;
                IsSmokePurification = false;
            }
        }

        #region 自定义方法

        /// <summary>
        /// 操作指令公共方法
        /// </summary>
        /// <param name="parameter"></param>
        private void ExecuteFunctionMethod(object parameter)
        {
            if(App.PropertyModelInstance.IsClose&& (string)parameter== "OpenHatch")
            {
                PopupBoxViewModel.ShowPopupBox("关舱过程中不能够开舱");
                return;
            }
            if (App.PropertyModelInstance.IsOpen && (string)parameter == "CloseHatch")
            {
                PopupBoxViewModel.ShowPopupBox("开舱过程中不能够关舱");
                return;
            }
            if (App.PropertyModelInstance.BackMoxibustionColumn_Height == 1 && (string)parameter == "BackMoxibustionColumnDown")
            {
                PopupBoxViewModel.ShowPopupBox("灸盘已降至最低位置");
                return;
            }
            if (App.PropertyModelInstance.BackMoxibustionColumn_Height == 3 && (string)parameter == "BackMoxibustionColumnUp")
            {
                PopupBoxViewModel.ShowPopupBox("灸盘已升至最高位置");
                return;
            }
            if (App.PropertyModelInstance.LegMoxibustionColumn_Height == 1 && (string)parameter == "LegMoxibustionColumnDown")
            {
                PopupBoxViewModel.ShowPopupBox("灸盘已降至最低位置");
                return;
            }
            if (App.PropertyModelInstance.LegMoxibustionColumn_Height == 3 && (string)parameter == "LegMoxibustionColumnUp")
            {
                PopupBoxViewModel.ShowPopupBox("灸盘已升至最高位置");
                return;
            }

            byte[] data = new byte[11];
            data[0] = 0x55;
            data[1] = 0xAA;
            data[2] = 0x07;
            data[3] = 0x01;
            data[4] = 0x10;
            switch(parameter)
            {
                case "TreatmentMode"://设置治疗模式
                    {

                        data[5] = 0x00;
                        if (App.PropertyModelInstance.MoxibustionTherapyMode)
                        {
                            data[6] = 0x02;
                            App.PropertyModelInstance.MoxibustionTherapyMode = false;
                        }
                        else
                        {
                            data[6] = 0x01;
                            App.PropertyModelInstance.MoxibustionTherapyMode = true;
                        }
                        break;
                    }
                case "Prehead"://设置预热
                    {

                        data[5] = 0x02;
                        data[6] = 0x01;
                        break;
                    }
                case "Inignition"://点火选择
                    {
                        data[5] = 0x03;
                        data[6] = 0x01;
                        break;
                    }
                case "BackMoxibustionColumnUp"://背部点升
                    {
                        data[5] = 0x04;
                        data[6] = 0x01;
                        App.PropertyModelInstance.BackMoxibustionColumn_Height++;
                        break;
                    }
                case "BackMoxibustionColumnDown"://背部点降
                    {
                        data[5] = 0x05;
                        data[6] = 0x01;
                        App.PropertyModelInstance.BackMoxibustionColumn_Height--;
                        break;
                    }
                case "LegMoxibustionColumnUp"://腿部点升
                    {
                        data[5] = 0x06;
                        data[6] = 0x01;
                        App.PropertyModelInstance.LegMoxibustionColumn_Height++;
                        break;
                    }
                case "LegMoxibustionColumnDown"://腿部点降
                    {
                        data[5] = 0x07;
                        data[6] = 0x01;
                        App.PropertyModelInstance.LegMoxibustionColumn_Height--;
                        break;
                    }
                case "OpenHatch"://一键开舱
                    {
                        if (App.PropertyModelInstance.IsOpen == false)
                        {
                            App.PropertyModelInstance.IsOpen = true;
                            data[5] = 0x0A;
                            data[6] = 0x01;
                            App.PropertyModelInstance.OpenHatch = "pack://application:,,,/Resources/Pictures/HatchBtnBackSelected.png";//切换背景图片
                        }
                        else
                        {
                            App.PropertyModelInstance.IsOpen = false;
                            timer.Stop();
                            SerialPortManager.Instance.timerOpen.Stop();
                            App.PropertyModelInstance.IsOpenOrClose = false;
                            data[5] = 0x0A;
                            data[6] = 0x02;
                            App.PropertyModelInstance.OpenHatch = "pack://application:,,,/Resources/Pictures/HatchBtnBack.png";
                            VoiceMethods("OnAndClose");//发送半开舱状态到语音模块
                        }
                        break;
                    }
                case "CloseHatch"://一键关舱
                    {
                        isCloseClick = true;
                        if (App.PropertyModelInstance.IsClose == false)
                        {
                            App.PropertyModelInstance.IsClose = true;
                            data[5] = 0x0B;
                            data[6] = 0x01;
                            App.PropertyModelInstance.CloseHatch = "pack://application:,,,/Resources/Pictures/HatchBtnBackSelected.png";//切换背景图片
                        }
                        else
                        {
                            App.PropertyModelInstance.IsClose = false;
                            timer.Stop();
                            SerialPortManager.Instance.timerClose.Stop();
                            timer1.Stop();
                            App.PropertyModelInstance.IsOpenOrClose = false;
                            data[5] = 0x0B;
                            data[6] = 0x02;
                            App.PropertyModelInstance.CloseHatch = "pack://application:,,,/Resources/Pictures/HatchBtnBack.png";
                            VoiceMethods("OnAndClose");//发送半开舱状态到语音模块
                        }
                        break;
                    }
                case "SmokeExhaustClose"://关闭排烟系统
                    {
                        data[5] = 0x0C;
                        data[6] = 0x00;
                        App.PropertyModelInstance.IsSmokeSystemOn = false;
                        App.PropertyModelInstance.SmokeExhaustSystem = 0;
                        break;
                    }
                case "SmokeExhaustLow"://排烟系统低档
                    {
                        data[5] = 0x0C;
                        data[6] = 0x01;
                        App.PropertyModelInstance.IsSmokeSystemOn = true;
                        App.PropertyModelInstance.SmokeExhaustSystem = 1;
                        break;
                    }
                case "SmokeExhaustMedium"://排烟系统中档
                    {
                        data[5] = 0x0C;
                        data[6] = 0x02;
                        App.PropertyModelInstance.IsSmokeSystemOn = true;
                        App.PropertyModelInstance.SmokeExhaustSystem = 2;
                        break;
                    }
                case "SmokeExhaustHigh"://排烟系统高档
                    {
                        data[5] = 0x0C;
                        data[6] = 0x03;
                        App.PropertyModelInstance.IsSmokeSystemOn = true;
                        App.PropertyModelInstance.SmokeExhaustSystem = 3;
                        break;
                    }
                case "SmokePurificationSystem"://净烟系统
                    {
                        data[5] = 0x0D;
                        if (App.PropertyModelInstance.SmokePurificationSystem)
                        {
                            data[6] = 0x02;
                            App.PropertyModelInstance.IsSmokeSystemOn = false;
                            App.PropertyModelInstance.SmokePurificationSystem = false;
                        }
                        else
                        {
                            data[6] = 0x01;
                            App.PropertyModelInstance.IsSmokeSystemOn = true;
                            App.PropertyModelInstance.SmokePurificationSystem = true;
                        }
                        break;
                    }
                case "SwingSystem"://摇摆系统
                    {
                        data[5] = 0x0E;
                        if (App.PropertyModelInstance.SwingSystem)
                        {
                            data[6] = 0x02;
                            App.PropertyModelInstance.SwingSystem = false;
                        }
                        else
                        {
                            data[6] = 0x01;
                            App.PropertyModelInstance.SwingSystem = true;
                        }
                        break;
                    }
                case "InfraredLampClose"://红外线关
                    {
                        data[5] = 0x0F;
                        data[6] = 0x00;
                        App.PropertyModelInstance.InfraredLamp = 0;
                        break;
                    }
                case "InfraredLampLow"://红外线低档
                    {
                        data[5] = 0x0F;
                        data[6] = 0x01; 
                        App.PropertyModelInstance.InfraredLamp = 1;
                        break;
                    }
                case "InfraredLampMedium"://红外线中档
                    {
                        data[5] = 0x0F;
                        data[6] = 0x02;
                        App.PropertyModelInstance.InfraredLamp = 2;
                        break;
                    }
                case "InfraredLampHigh"://红外线高档
                    {
                        data[5] = 0x0F;
                        data[6] = 0x03;
                        App.PropertyModelInstance.InfraredLamp = 3;
                        break;
                    }
            }
            data[9] = 0xAA;
            data[10] = 0x5C;
            data = SerialPortManager.CRC16(data);
            SerialPortManager.Instance.SendData(data);
            if (isCloseClick)
            {
                isCloseClick = false;
                byte[] data1 = new byte[11];
                data1[0] = 0x55;
                data1[1] = 0xAA;
                data1[2] = 0x07;
                data1[3] = 0x01;
                data1[4] = 0x10;
                if (isInignitionClicked)
                {
                    isInignitionClicked = false;
                    data[5] = 0x03;
                    data[6] = 0x01;
                    data[9] = 0xAA;
                    data[10] = 0x5C;
                    data = SerialPortManager.CRC16(data);
                    SerialPortManager.Instance.SendData(data);
                    App.PropertyModelInstance.InignitionStatus = true;
                    IsCountingDown = true;
                    App.PropertyModelInstance.CountdownSeconds = App.PropertyModelInstance.InignitionTime;
                    App.PropertyModelInstance.CountdownMinutes = 0;
                    //StartCountdown();
                    seconds = App.PropertyModelInstance.CountdownSeconds + App.PropertyModelInstance.CountdownMinutes * 60;
                    StartTime = DateTime.Now;
                    _timer.Start();
                }
                else if (isPreheadClicked)
                {
                    isPreheadClicked = false;
                    data[5] = 0x02;
                    data[6] = 0x01;
                    data[9] = 0xAA;
                    data[10] = 0x5C;
                    data = SerialPortManager.CRC16(data);
                    SerialPortManager.Instance.SendData(data);
                    App.PropertyModelInstance.PreheadMode = true;
                    IsCountingDown = true;
                    App.PropertyModelInstance.CountdownMinutes = App.PropertyModelInstance.PreheadTime;
                    App.PropertyModelInstance.CountdownSeconds = 0;
                    //StartCountdown();
                    seconds = App.PropertyModelInstance.CountdownSeconds + App.PropertyModelInstance.CountdownMinutes * 60;
                    StartTime = DateTime.Now;
                    _timer.Start();
                    //App.PropertyModelInstance.CloseHatch = "pack://application:,,,/Resources/Pictures/HatchBtnBack.png";
                    //App.PropertyModelInstance.IsClose = false;
                    //((DispatcherTimer)sender).Stop();
                }
            }

            if(App.PropertyModelInstance.IsOpen)
            {
                timer.Tick -= TimerOpenHandler;
                timer.Tick -= TimerCloseHandler;
                timer.Tick += TimerOpenHandler;
                timer.Start();
                App.PropertyModelInstance.IsOpenOrClose = true;
            }
            else if(App.PropertyModelInstance.IsClose)
            {
                timer.Tick -= TimerOpenHandler;
                timer.Tick -= TimerCloseHandler;
                timer.Tick += TimerCloseHandler;
                timer.Start();
                App.PropertyModelInstance.IsOpenOrClose = true;
            }
        }

        private void TimerOpenHandler(object sender, EventArgs e)
        {
            ((DispatcherTimer)sender).Stop();
            App.PropertyModelInstance.IsOpenOrClose = false;
            App.PropertyModelInstance.IsOpen = false;
            byte[] c = new byte[11];
            c[0] = 0x55;
            c[1] = 0xAA;
            c[2] = 0x07;
            c[3] = 0x01;
            c[4] = 0x10;
            c[5] = 0x0A;
            c[6] = 0x02;
            c[9] = 0xAA;
            c[10] = 0x5C;
            c = SerialPortManager.CRC16(c);
            SerialPortManager.Instance.SendData(c);
            App.PropertyModelInstance.OpenHatch = "pack://application:,,,/Resources/Pictures/HatchBtnBack.png";
            //((DispatcherTimer)sender).Stop();
            VoiceMethods("HatchOn");//发送开舱状态到语音模块
        }

        private void TimerCloseHandler(object sender, EventArgs e)
        {
            ((DispatcherTimer)sender).Stop();
            App.PropertyModelInstance.IsOpenOrClose = false;
            App.PropertyModelInstance.IsClose = false;
            byte[] c = new byte[11];
            c[0] = 0x55;
            c[1] = 0xAA;
            c[2] = 0x07;
            c[3] = 0x01;
            c[4] = 0x10;
            c[5] = 0x0B;
            c[6] = 0x02;
            c[9] = 0xAA;
            c[10] = 0x5C;
            c = SerialPortManager.CRC16(c);
            SerialPortManager.Instance.SendData(c);
            App.PropertyModelInstance.CloseHatch = "pack://application:,,,/Resources/Pictures/HatchBtnBack.png";
            //((DispatcherTimer)sender).Stop();
            VoiceMethods("HatchClose");//发送关舱状态到语音模块
        }


        /// <summary>
        /// 预热选择
        /// </summary>
        private void PreheadMethod()
        {
            isPreheadClicked = true;
            byte[] data = new byte[11];
            data[0] = 0x55;
            data[1] = 0xAA;
            data[2] = 0x07;
            data[3] = 0x01;
            data[4] = 0x10;
            data[5] = 0x0B;
            data[6] = 0x01;
            data[9] = 0xAA;
            data[10] = 0x5C;
            data = SerialPortManager.CRC16(data);
            SerialPortManager.Instance.SendData(data);
            App.PropertyModelInstance.CloseHatch = "pack://application:,,,/Resources/Pictures/HatchBtnBackSelected.png";//切换背景图片
            App.PropertyModelInstance.IsClose = true;
            App.PropertyModelInstance.Hatch = false;
            App.PropertyModelInstance.IsOpenOrClose = true;
            VoiceMethods("HatchClose");//发送关舱状态到语音模块

            timer1.Tick -= Timer1InignitionHandler;
            timer1.Tick -= Timer1PreheadHandler;
            timer1.Tick += Timer1PreheadHandler;
            timer1.Start();
        }

        /// <summary>
        /// 点火选择
        /// </summary>
        private void InignitionMethod()
        {
            isInignitionClicked = true;
            byte[] data = new byte[11];
            data[0] = 0x55;
            data[1] = 0xAA;
            data[2] = 0x07;
            data[3] = 0x01;
            data[4] = 0x10;
            data[5] = 0x0B;
            data[6] = 0x01;
            data[9] = 0xAA;
            data[10] = 0x5C;
            data = SerialPortManager.CRC16(data);
            SerialPortManager.Instance.SendData(data);
            App.PropertyModelInstance.CloseHatch = "pack://application:,,,/Resources/Pictures/HatchBtnBackSelected.png";//切换背景图片
            App.PropertyModelInstance.IsClose = true;
            App.PropertyModelInstance.Hatch = false;
            App.PropertyModelInstance.IsOpenOrClose = true;
            VoiceMethods("HatchClose");//发送关舱状态到

            timer1.Tick -= Timer1InignitionHandler;
            timer1.Tick -= Timer1PreheadHandler;
            timer1.Tick += Timer1InignitionHandler;
            timer1.Start();
        }

        /// <summary>
        /// 预热timer事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer1PreheadHandler(object sender, EventArgs e)
        {
            byte[] data = new byte[11];
            data[0] = 0x55;
            data[1] = 0xAA;
            data[2] = 0x07;
            data[3] = 0x01;
            data[4] = 0x10;
            ((DispatcherTimer)sender).Stop();
            App.PropertyModelInstance.IsOpenOrClose = false;
            data[5] = 0x0B;
            data[6] = 0x02;
            data[9] = 0xAA;
            data[10] = 0x5C;
            data = SerialPortManager.CRC16(data);
            SerialPortManager.Instance.SendData(data);
            data[5] = 0x02;
            data[6] = 0x01;
            data[9] = 0xAA;
            data[10] = 0x5C;
            data = SerialPortManager.CRC16(data);
            SerialPortManager.Instance.SendData(data);
            App.PropertyModelInstance.PreheadMode = true;
            IsCountingDown = true;
            App.PropertyModelInstance.CountdownMinutes = App.PropertyModelInstance.PreheadTime;
            App.PropertyModelInstance.CountdownSeconds = 0;
            //StartCountdown();
            seconds = App.PropertyModelInstance.CountdownSeconds + App.PropertyModelInstance.CountdownMinutes * 60;
            StartTime = DateTime.Now;
            _timer.Start();
            //App.PropertyModelInstance.IsOpen = false;
            App.PropertyModelInstance.CloseHatch = "pack://application:,,,/Resources/Pictures/HatchBtnBack.png";
            App.PropertyModelInstance.IsClose = false;
            ((DispatcherTimer)sender).Stop();
            isPreheadClicked = false;
        }

        /// <summary>
        /// 点火timer事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer1InignitionHandler(object sender, EventArgs e)
        {
            byte[] data = new byte[11];
            data[0] = 0x55;
            data[1] = 0xAA;
            data[2] = 0x07;
            data[3] = 0x01;
            data[4] = 0x10;
            ((DispatcherTimer)sender).Stop();
            App.PropertyModelInstance.IsOpenOrClose = false;
            data[5] = 0x0B;
            data[6] = 0x02;
            data[9] = 0xAA;
            data[10] = 0x5C;
            data = SerialPortManager.CRC16(data);
            SerialPortManager.Instance.SendData(data);
            data[5] = 0x03;
            data[6] = 0x01;
            data[9] = 0xAA;
            data[10] = 0x5C;
            data = SerialPortManager.CRC16(data);
            SerialPortManager.Instance.SendData(data);
            App.PropertyModelInstance.InignitionStatus = true;
            IsCountingDown = true;
            App.PropertyModelInstance.CountdownSeconds = App.PropertyModelInstance.InignitionTime;
            App.PropertyModelInstance.CountdownMinutes = 0;
            //StartCountdown();
            seconds = App.PropertyModelInstance.CountdownSeconds + App.PropertyModelInstance.CountdownMinutes * 60;
            StartTime = DateTime.Now;
            _timer.Start();
            App.PropertyModelInstance.CloseHatch = "pack://application:,,,/Resources/Pictures/HatchBtnBack.png";
            App.PropertyModelInstance.IsClose = false;
            ((DispatcherTimer)sender).Stop();
            isInignitionClicked = false;
        }

        /// <summary>
        /// RadioButton切换事件
        /// </summary>
        /// <param name="parameter"></param>
        private void CloseSmokeSystem(object parameter)
        {
            byte[] data = new byte[11];
            data[0] = 0x55;
            data[1] = 0xAA;
            data[2] = 0x07;
            data[3] = 0x01;
            data[4] = 0x10;
            
            if ((string)parameter== "SmokeExhaust")
            {
                data[5] = 0x0D;
                data[6] = 0x02;
            }
            else if((string)parameter== "SmokePurification")
            {
                data[5] = 0x0C;
                data[6] = 0x00;
            }
            data[9] = 0xAA;
            data[10] = 0x5C;
            data = SerialPortManager.CRC16(data);
            SerialPortManager.Instance.SendData(data);

            if ((string)parameter == "SmokeExhaust")
            {
                App.PropertyModelInstance.SmokeExhaustSystem = 2;
            }
            else if ((string)parameter == "SmokePurification")
            {
                App.PropertyModelInstance.SmokePurificationSystem = false;
            }
        }

        /// <summary>
        /// 停止治疗、停止排烟方法
        /// </summary>
        /// <param name="parameter"></param>
        private void StopMethod(object parameter)
        {
            byte[] data = new byte[11];
            data[0] = 0x55;
            data[1] = 0xAA;
            data[2] = 0x07;
            data[3] = 0x01;
            data[4] = 0x10;

            switch(parameter)
            {
                case "StopMoxibustionTherapy"://停止治疗
                    data[5] = 0x08;
                    data[6] = 0x02;
                    break;
                case "StopSmoke":
                    if (IsSmokeExhaust == false)//若选择净烟，关闭净烟
                    {
                        data[5] = 0x0D;
                        data[6] = 0x02;
                    }
                    else//若选择排烟，关闭排烟
                    {
                        data[5] = 0x0C;
                        data[6] = 0x00;
                    }
                    break;
            }
            data[9] = 0xAA;
            data[10] = 0x5C;
            data = SerialPortManager.CRC16(data);
            SerialPortManager.Instance.SendData(data);
            if((string)parameter== "StopMoxibustionTherapy")
            {
                _timer.Stop();
                App.PropertyModelInstance.CountdownMinutes = 0;
                App.PropertyModelInstance.CountdownSeconds = 0;
                App.PropertyModelInstance.IsMoxibustionTherapyMode = false;
                RadioBtnCanUse = !App.PropertyModelInstance.IsMoxibustionTherapyMode;
                VoiceMethods("StopMoxibustion");//结束治疗发送给语音模块
            }
            if((string)parameter== "StopSmoke")
            {
                _timer.Stop();
                App.PropertyModelInstance.CountdownMinutes = 0;
                App.PropertyModelInstance.CountdownSeconds = 0;
                App.PropertyModelInstance.IsSmokeSystemOn = false;
                Smoke = false;
            }
        }

        /// <summary>
        /// 停止点火、停止治疗、开始治疗、开启排烟、关闭排烟、开启舱门等方法
        /// </summary>
        /// <param name="parameter"></param>
        private void StopMethods(object parameter)
        {
            byte[] data = new byte[11];
            data[0] = 0x55;
            data[1] = 0xAA;
            data[2] = 0x07;
            data[3] = 0x01;
            data[4] = 0x10;

            switch (parameter)
            {
                case "StopInignition"://点火选择
                    {
                        data[5] = 0x03;
                        data[6] = 0x02;
                        break;
                    }
                case "StopMoxibustionTherapy"://停止治疗
                    data[5] = 0x08;
                    data[6] = 0x02;
                    break;
                case "StartMoxibustionTherapy"://开始治疗
                    data[5] = 0x08;
                    data[6] = 0x01;
                    break;
                case "StopSmoke":
                    if (IsSmokeExhaust == false)//若选择净烟，关闭净烟
                    {
                        data[5] = 0x0D;
                        data[6] = 0x02;
                    }
                    else//若选择排烟，关闭排烟
                    {
                        data[5] = 0x0C;
                        data[6] = 0x00;
                    }
                    break;
                case "OpenSmoke":
                    if (IsSmokeExhaust == false)//若选择净烟，开启净烟
                    {
                        data[5] = 0x0D;
                        data[6] = 0x01;
                    }
                    else//若选择排烟，开启排烟中档
                    {
                        data[5] = 0x0C;
                        data[6] = 0x02;
                    }
                    break;
                case "OpenHatch"://开启舱门
                    data[5] = 0x0A;
                    data[6] = 0x01;
                    break;
            }

            data[9] = 0xAA;
            data[10] = 0x5C;
            data = SerialPortManager.CRC16(data);
            SerialPortManager.Instance.SendData(data);
            if ((string)parameter == "OpenHatch")//发送开舱状态到语音模块
            {
                VoiceMethods("OpenHatch");
            }
        }

        private void VoiceMethods(object parameter)
        {
            byte[] bytes = new byte[6];
            bytes[0] = 0xAA;
            bytes[1] = 0x55;
            switch(parameter)
            {
                case "StartMoxibustion"://开始治疗
                    bytes[2] = 0x0E;
                    bytes[3] = 0x01;
                    break;
                case "StopMoxibustion"://结束治疗
                    bytes[2] = 0x0E;
                    bytes[3] = 0x00;
                    break;
                case "HatchClose"://舱门关闭状态
                    bytes[2] = 0x0F;
                    bytes[3] = 0x00;
                    break;
                case "HatchOn"://舱门开启状态
                    bytes[2] = 0x0F;
                    bytes[3] = 0x01;
                    break;
                case "OnAndClose"://舱门半开启状态
                    bytes[2] = 0x0F;
                    bytes[3] = 0x02;
                    break;
            }
            bytes[4] = 0xAA;
            bytes[5] = 0x5C;
            SerialPortManager.Instance.SendDataByVoice(bytes);
        }

        /// <summary>
        /// 开始计时器
        /// </summary>
        private void StartCountdown()
        {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(200);
            _timer.Tick += Timer_Tick;
            //_timer.Start();
        }

        /// <summary>
        /// 计时器方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Tick(object sender, EventArgs e)
        {
            TimeSpan t = DateTime.Now.Subtract(StartTime);
            if(App.PropertyModelInstance.CountdownMinutes > 0 || App.PropertyModelInstance.CountdownSeconds > 0)
            {
                int s = (int)(seconds - t.TotalSeconds);
                App.PropertyModelInstance.CountdownMinutes = s / 60;
                App.PropertyModelInstance.CountdownSeconds = s % 60;
            }
            //if (App.PropertyModelInstance.CountdownSeconds > 0)
            //{
            //    App.PropertyModelInstance.CountdownSeconds = seconds - t.Seconds;
            //}
            //else if (App.PropertyModelInstance.CountdownMinutes > 0)
            //{
            //    App.PropertyModelInstance.CountdownMinutes--;
            //    App.PropertyModelInstance.CountdownSeconds = 60;
            //    seconds = 59;


            //}
            else
            {
                _timer.Stop();
                App.PropertyModelInstance.CountdownMinutes = 0;
                App.PropertyModelInstance.CountdownSeconds = 0;
                IsCountingDown = false;
                if (App.PropertyModelInstance.PreheadMode == true)
                {
                    App.PropertyModelInstance.PreheadMode = false;
                }
                else if (App.PropertyModelInstance.InignitionStatus == true)
                {
                    App.PropertyModelInstance.InignitionStatus = false;
                    App.PropertyModelInstance.IsInignitionStatus = true;//点火状态设为已点火
                    //停止点火
                    StopMethods("StopInignition");

                    //开始治疗后，舱盖控制模块不能控制
                    App.PropertyModelInstance.IsMoxibustionTherapyMode = true;
                    RadioBtnCanUse = !App.PropertyModelInstance.IsMoxibustionTherapyMode;
                    IsCountingDown = true;
                    App.PropertyModelInstance.CountdownSeconds = 0;
                    App.PropertyModelInstance.CountdownMinutes = App.PropertyModelInstance.MoxibustionTherapyTime;
                    //发送开始治疗指令
                    StopMethods("StartMoxibustionTherapy");
                    //StartCountdown();//开启治疗倒计时
                    seconds = App.PropertyModelInstance.CountdownSeconds + App.PropertyModelInstance.CountdownMinutes * 60;
                    StartTime = DateTime.Now;
                    _timer.Start();
                    if (App.PropertyModelInstance.AutoMusic==true)
                    {
                        App.PropertyModelInstance.CurrentUserControl = MainWindowViewModel.PlayMusicView;
                    }
                    VoiceMethods("StartMoxibustion");//开始治疗发送给语音模块
                }
                else if (App.PropertyModelInstance.IsMoxibustionTherapyMode == true)
                {
                    //治疗结束
                    App.PropertyModelInstance.IsMoxibustionTherapyMode = false;
                    byte[] data = new byte[11];
                    //发送结束治疗指令
                    StopMethods("StopMoxibustionTherapy");

                    if (App.PropertyModelInstance.IsSmokeSystemOn == false)//如果排烟和净烟都未开启
                    {
                        //开启排烟
                        StopMethods("OpenSmoke");
                    }
                    App.PropertyModelInstance.IsSmokeSystemOn = true;
                    Smoke = true;
                    IsCountingDown = true;
                    App.PropertyModelInstance.CountdownSeconds = 0;
                    App.PropertyModelInstance.CountdownMinutes = 5;
                    //StartCountdown();//开始排烟倒计时，五分钟
                    seconds = App.PropertyModelInstance.CountdownSeconds + App.PropertyModelInstance.CountdownMinutes * 60;
                    StartTime = DateTime.Now;
                    _timer.Start();
                    VoiceMethods("StopMoxibustion");//结束治疗发送给语音模块
                }
                else if (App.PropertyModelInstance.IsSmokeSystemOn == true)
                {
                    //排烟结束
                    App.PropertyModelInstance.IsSmokeSystemOn = false;
                    //发送停止排烟指令
                    StopMethods("StopSmoke");
                    Smoke = false;

                    //结束后自动开盖
                    if (App.PropertyModelInstance.AutomaticLidOpening)
                    {
                        StopMethods("OpenHatch");
                    }
                    RadioBtnCanUse = !App.PropertyModelInstance.IsMoxibustionTherapyMode;
                    if (IsSmokeExhaust)
                    {
                        App.PropertyModelInstance.IsSmokePurificationSystem = false;
                    }
                    else if (IsSmokePurification)
                    {
                        App.PropertyModelInstance.IsSmokePurificationSystem = true;
                    }

                    PublicMethods.SaveToJson();
                }
            }
        }
        #endregion
    }

    public class UpTouchFeedbackBehavior : Behavior<Button>
    {
        private Brush _originalBrush;
        private static readonly ImageBrush PressedBrush = new ImageBrush
        {
            ImageSource = new BitmapImage(new Uri("pack://application:,,,/MoxibustionBedAPP;component/Resources/Pictures/BtnUpSelected.png")),
            Stretch = Stretch.None
        };

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PreviewTouchDown += OnTouchDown;
            AssociatedObject.PreviewTouchUp += OnTouchUp;
        }

        private void OnTouchDown(object sender, TouchEventArgs e)
        {
            if (AssociatedObject.Template?.FindName("border", AssociatedObject) is Border border)
            {
                _originalBrush = border.Background;
                border.Background = PressedBrush;
            }
        }

        private void OnTouchUp(object sender, TouchEventArgs e)
        {
            if (AssociatedObject.Template?.FindName("border", AssociatedObject) is Border border)
            {
                border.Background = _originalBrush;
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PreviewTouchDown -= OnTouchDown;
            AssociatedObject.PreviewTouchUp -= OnTouchUp;
        }
    }

    public class DownTouchFeedbackBehavior : Behavior<Button>
    {
        private Brush _originalBrush;
        private static readonly ImageBrush PressedBrush = new ImageBrush
        {
            ImageSource = new BitmapImage(new Uri("pack://application:,,,/MoxibustionBedAPP;component/Resources/Pictures/BtnDownSelected.png")),
            Stretch = Stretch.None
        };

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PreviewTouchDown += OnTouchDown;
            AssociatedObject.PreviewTouchUp += OnTouchUp;
        }

        private void OnTouchDown(object sender, TouchEventArgs e)
        {
            if (AssociatedObject.Template?.FindName("border", AssociatedObject) is Border border)
            {
                _originalBrush = border.Background;
                border.Background = PressedBrush;
            }
        }

        private void OnTouchUp(object sender, TouchEventArgs e)
        {
            if (AssociatedObject.Template?.FindName("border", AssociatedObject) is Border border)
            {
                border.Background = _originalBrush;
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PreviewTouchDown -= OnTouchDown;
            AssociatedObject.PreviewTouchUp -= OnTouchUp;
        }
    }
}
