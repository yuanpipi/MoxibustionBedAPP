using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Threading;
using MoxibustionBedAPP.Models;
using MoxibustionBedAPP.Views;
using static System.Net.Mime.MediaTypeNames;

namespace MoxibustionBedAPP.ViewModes
{
    public class FunctionControlViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
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
        /// 开舱背景图
        /// </summary>
        private string _openHatch;
        public string OpenHatch
        {
            get
            {
                return _openHatch;
            }
            set
            {
                _openHatch = value;
                OnPropertyChanged(nameof(OpenHatch));
            }
        }

        /// <summary>
        /// 关舱背景图
        /// </summary>
        private string _closeHatch;
        public string CloseHatch
        {
            get
            {
                return _closeHatch;
            }
            set
            {
                _closeHatch = value;
                OnPropertyChanged(nameof(CloseHatch));
            }
        }

        /// <summary>
        /// 是否处于开舱状态
        /// </summary>
        private bool isOpen;
        public bool IsOpen
        {
            get
            {
                return isOpen;
            }
            set
            {
                isOpen = value;
                OnPropertyChanged(nameof(IsOpen));
            }
        }

        /// <summary>
        /// 是否处于关舱状态
        /// </summary>
        private bool isClose;
        public bool IsClose
        {
            get
            {
                return isClose;
            }
            set
            {
                isClose = value;
                OnPropertyChanged(nameof(IsClose));
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
        /// 开关舱倒计时，5s
        /// </summary>
        private DispatcherTimer timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(5)
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

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public FunctionControlViewModel()
        {
            PublicFunction = new RelayCommand(ExecuteFunctionMethod);
            Prehead = new RelayCommand(PreheadMethod);
            Inignition= new RelayCommand(InignitionMethod);
            OpenHatch = "../Resources/Pictures/HatchBtnBack.png";
            CloseHatch = "../Resources/Pictures/HatchBtnBack.png";
            RadioBtnOfSmoke = new RelayCommand(CloseSmokeSystem);
            RadioBtnCanUse = !App.PropertyModelInstance.IsMoxibustionTherapyMode;
            StopCommand = new RelayCommand(StopMethod);
            IsOpen = false;
            IsClose=false;
            Smoke = false;
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

        /// <summary>
        /// 操作指令公共方法
        /// </summary>
        /// <param name="parameter"></param>
        private void ExecuteFunctionMethod(object parameter)
        {
            if(isClose&& (string)parameter== "OpenHatch")
            {
                PopupBoxViewModel.ShowPopupBox("关舱过程中不能够开舱");
                return;
            }
            if (IsOpen && (string)parameter == "CloseHatch")
            {
                PopupBoxViewModel.ShowPopupBox("开舱过程中不能够关舱");
                return;
            }
            if (App.PropertyModelInstance.BackMoxibustionColumn_Height == 0 && (string)parameter == "BackMoxibustionColumnDown")
            {
                PopupBoxViewModel.ShowPopupBox("灸盘已降至最低位置");
                return;
            }
            if (App.PropertyModelInstance.BackMoxibustionColumn_Height == 4 && (string)parameter == "BackMoxibustionColumnUp")
            {
                PopupBoxViewModel.ShowPopupBox("灸盘已升至最高位置");
                return;
            }
            if (App.PropertyModelInstance.LegMoxibustionColumn_Height == 0 && (string)parameter == "LegMoxibustionColumnDown")
            {
                PopupBoxViewModel.ShowPopupBox("灸盘已降至最低位置");
                return;
            }
            if (App.PropertyModelInstance.LegMoxibustionColumn_Height == 4 && (string)parameter == "LegMoxibustionColumnUp")
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
                        if (IsOpen == false)
                        {
                            IsOpen = true;
                            data[5] = 0x0A;
                            data[6] = 0x01;
                            OpenHatch = "../Resources/Pictures/HatchBtnBackSelected.png";//切换背景图片
                        }
                        else
                        {
                            IsOpen = false;
                            timer.Stop();
                            data[5] = 0x0A;
                            data[6] = 0x02;
                            OpenHatch = "../Resources/Pictures/HatchBtnBack.png";
                        }
                        break;
                    }
                case "CloseHatch"://一键关舱
                    {
                        if (IsClose == false)
                        {
                            IsClose = true;
                            data[5] = 0x0B;
                            data[6] = 0x01;
                            CloseHatch = "../Resources/Pictures/HatchBtnBackSelected.png";//切换背景图片
                        }
                        else
                        {
                            IsClose = false;
                            timer.Stop();
                            data[5] = 0x0B;
                            data[6] = 0x02;
                            CloseHatch = "../Resources/Pictures/HatchBtnBack.png";
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
                        if (App.PropertyModelInstance.SmokePurificationSystem)
                        {
                            data[6] = 0x02;
                            App.PropertyModelInstance.SmokePurificationSystem = false;
                        }
                        else
                        {
                            data[6] = 0x01;
                            App.PropertyModelInstance.SmokePurificationSystem= true;
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
            data[9] = 0x55;
            data[10] = 0xAA;
            data = SerialPortManager.CRC16(data);
            SerialPortManager.Instance.SendData(data);

            if(IsOpen)
            {
                timer.Tick += (sender, args) =>
                {
                    IsOpen = false;
                    OpenHatch = "../Resources/Pictures/HatchBtnBack.png";
                    ((DispatcherTimer)sender).Stop();
                };
                timer.Start();
            }
            if(IsClose)
            {
                timer.Tick += (sender, args) =>
                {
                    IsClose = false;
                    CloseHatch = "../Resources/Pictures/HatchBtnBack.png";
                    ((DispatcherTimer)sender).Stop();
                };
                timer.Start();
            }
        }

        /// <summary>
        /// 预热选择
        /// </summary>
        private void PreheadMethod()
        {
            byte[] data = new byte[11];
            data[0] = 0x55;
            data[1] = 0xAA;
            data[2] = 0x07;
            data[3] = 0x01;
            data[4] = 0x10;
            data[5] = 0x0B;
            data[6] = 0x01;
            data[9] = 0x55;
            data[10] = 0xAA;
            data = SerialPortManager.CRC16(data);
            SerialPortManager.Instance.SendData(data);



            data[5] = 0x02;
            data[6] = 0x01;
            data[9] = 0x55;
            data[10] = 0xAA;
            data = SerialPortManager.CRC16(data);
            SerialPortManager.Instance.SendData(data);
            App.PropertyModelInstance.PreheadMode = true;
            IsCountingDown = true;
            App.PropertyModelInstance.CountdownMinutes = App.PropertyModelInstance.PreheadTime;
            App.PropertyModelInstance.CountdownSeconds = 0;
            StartCountdown();
        }

        /// <summary>
        /// 点火选择
        /// </summary>
        private void InignitionMethod()
        {
            byte[] data = new byte[11];
            data[0] = 0x55;
            data[1] = 0xAA;
            data[2] = 0x07;
            data[3] = 0x01;
            data[4] = 0x10;
            data[5] = 0x0B;
            data[6] = 0x01;
            data[9] = 0x55;
            data[10] = 0xAA;
            data = SerialPortManager.CRC16(data);
            SerialPortManager.Instance.SendData(data);
            App.PropertyModelInstance.Hatch = false;


            data[5] = 0x03;
            data[6] = 0x01;
            data[9] = 0x55;
            data[10] = 0xAA;
            data = SerialPortManager.CRC16(data);
            SerialPortManager.Instance.SendData(data);
            App.PropertyModelInstance.InignitionStatus = true;
            IsCountingDown = true;
            App.PropertyModelInstance.CountdownSeconds = App.PropertyModelInstance.InignitionTime;
            App.PropertyModelInstance.CountdownMinutes = 0;
            StartCountdown();
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
            data[9] = 0x55;
            data[10] = 0xAA;
            data = SerialPortManager.CRC16(data);
            SerialPortManager.Instance.SendData(data);
        }

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
            data[9] = 0x55;
            data[10] = 0xAA;
            data = SerialPortManager.CRC16(data);
            SerialPortManager.Instance.SendData(data);
            if((string)parameter== "StopMoxibustionTherapy")
            {
                _timer.Stop();
                App.PropertyModelInstance.IsMoxibustionTherapyMode = false;
                RadioBtnCanUse = !App.PropertyModelInstance.IsMoxibustionTherapyMode;
            }
            if((string)parameter== "StopSmoke")
            {
                _timer.Stop();
                App.PropertyModelInstance.IsSmokeSystemOn = false;
                Smoke = false;
            }
        }

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

            data[9] = 0x55;
            data[10] = 0xAA;
            data = SerialPortManager.CRC16(data);
            SerialPortManager.Instance.SendData(data);
        }

        private void StartCountdown()
        {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (App.PropertyModelInstance.CountdownSeconds > 0)
            {
                App.PropertyModelInstance.CountdownSeconds--;
            }
            else if (App.PropertyModelInstance.CountdownMinutes > 0)
            {
                App.PropertyModelInstance.CountdownMinutes--;
                App.PropertyModelInstance.CountdownSeconds = 59;
            }
            else
            {
                _timer.Stop();
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
                    StartCountdown();//开启治疗倒计时
                    if(App.PropertyModelInstance.AutoMusic==true)
                    {
                        App.PropertyModelInstance.CurrentUserControl = MainWindowViewModel.PlayMusicView;
                    }
                }
                else if (App.PropertyModelInstance.IsMoxibustionTherapyMode == true)
                {
                    //治疗结束
                    App.PropertyModelInstance.IsMoxibustionTherapyMode = false;
                    byte[] data = new byte[11];
                    //发送结束治疗指令
                    StopMethods("StopMoxibustionTherapy");

                    //RadioBtnCanUse = !App.PropertyModelInstance.IsMoxibustionTherapyMode;
                    if (App.PropertyModelInstance.IsSmokeSystemOn == false)//如果排烟和净烟都未开启
                    {

                        //if (IsSmokeExhaust == false)//若选择净烟，开启净烟
                        //{
                        //    data[0] = 0x55;
                        //    data[1] = 0xAA;
                        //    data[2] = 0x07;
                        //    data[3] = 0x01;
                        //    data[4] = 0x10;
                        //    data[5] = 0x0D;
                        //    data[6] = 0x01;
                        //    data[9] = 0x55;
                        //    data[10] = 0xAA;
                        //}
                        //else//若选择排烟，开启排烟中档
                        //{
                        //    data[0] = 0x55;
                        //    data[1] = 0xAA;
                        //    data[2] = 0x07;
                        //    data[3] = 0x01;
                        //    data[4] = 0x10;
                        //    data[5] = 0x0C;
                        //    data[6] = 0x02;
                        //    data[9] = 0x55;
                        //    data[10] = 0xAA;
                        //}
                        //data = SerialPortManager.CRC16(data);
                        //SerialPortManager.Instance.SendData(data);
                        //开启排烟
                        StopMethods("OpenSmoke");
                    }
                    App.PropertyModelInstance.IsSmokeSystemOn = true;
                    Smoke = true;
                    IsCountingDown = true;
                    App.PropertyModelInstance.CountdownSeconds = 0;
                    App.PropertyModelInstance.CountdownMinutes = 5;
                    StartCountdown();//开始排烟倒计时，五分钟
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
    }
}
