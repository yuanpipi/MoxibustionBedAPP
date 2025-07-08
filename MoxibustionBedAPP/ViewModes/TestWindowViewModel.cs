using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using MoxibustionBedAPP.Models;

namespace MoxibustionBedAPP.ViewModes
{
    public class TestWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public RelayCommand CloseWindow { get; set; }
        public ICommand PublicFunction { get; set; }
        public ICommand Prehead { get; set; }
        public ICommand Inignition { get; set; }


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
        /// 开关舱倒计时，30s
        /// </summary>
        private DispatcherTimer timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(30)
        };

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

        public static DateTime StartTime;
        public static int seconds;

        public TestWindowViewModel()
        {
            CloseWindow = new RelayCommand(CloseWindowMethod);
            PublicFunction = new RelayCommand(ExecuteFunctionMethod);
            Prehead = new RelayCommand(PreheadMethod);
            Inignition = new RelayCommand(InignitionMethod);
            IsOpen = false; 
            IsClose=false;
        }

        private void CloseWindowMethod()
        {
            //发送指令，关闭调试界面
            //byte[] data = new byte[11];
            //data[0] = 0x55;
            //data[1] = 0xAA;
            //data[2] = 0x07;
            //data[3] = 0x01;
            //data[4] = 0x10;
            //data[5] = 0x10;
            //data[6] = 0x02;
            //data[9] = 0xAA;
            //data[10] = 0x5C;
            //data = SerialPortManager.CRC16(data);
            //SerialPortManager.Instance.SendData(data);

            App.Test.Close();
        }

        /// <summary>
        /// 操作指令公共方法
        /// </summary>
        /// <param name="parameter"></param>
        private void ExecuteFunctionMethod(object parameter)
        {
            if (App.PropertyModelInstance.IsClose && (string)parameter == "OpenHatch")
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
            switch (parameter)
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
                        }
                        else
                        {
                            IsOpen = false;
                            timer.Stop();
                            App.PropertyModelInstance.IsOpenOrClose = false;
                            data[5] = 0x0A;
                            data[6] = 0x02;
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
                        }
                        else
                        {
                            IsClose = false;
                            timer.Stop();
                            App.PropertyModelInstance.IsOpenOrClose = false;
                            data[5] = 0x0B;
                            data[6] = 0x02;
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

            if (IsOpen)
            {
                timer.Tick += (sender, args) =>
                {
                    IsOpen = false;
                    ((DispatcherTimer)sender).Stop();
                };
                timer.Start();
            }
            if (IsClose)
            {
                timer.Tick += (sender, args) =>
                {
                    IsClose = false;
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
            if (App.PropertyModelInstance.PreheadMode == false)
            {
                byte[] data = new byte[11];
                data[0] = 0x55;
                data[1] = 0xAA;
                data[2] = 0x07;
                data[3] = 0x01;
                data[4] = 0x10;
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

                seconds = App.PropertyModelInstance.CountdownSeconds + App.PropertyModelInstance.CountdownMinutes * 60;
                StartTime = DateTime.Now;
                StartCountdown();
            }
            else
            {
                _timer.Stop();
                StopMethod("StopPrehead");
                App.PropertyModelInstance.PreheadMode = false;
            }
        }

        /// <summary>
        /// 点火选择
        /// </summary>
        private void InignitionMethod()
        {
            if (App.PropertyModelInstance.InignitionStatus == false)
            {
                byte[] data = new byte[11];
                data[0] = 0x55;
                data[1] = 0xAA;
                data[2] = 0x07;
                data[3] = 0x01;
                data[4] = 0x10;
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

                seconds = App.PropertyModelInstance.CountdownSeconds + App.PropertyModelInstance.CountdownMinutes * 60;
                StartTime = DateTime.Now;
                StartCountdown();
            }
            else
            {
                _timer.Stop();
                StopMethod("StopInignition");//停止点火
                App.PropertyModelInstance.InignitionStatus = false;
            }
        }


        private void StopMethod(object parameter)
        {
            byte[] data = new byte[11];
            data[0] = 0x55;
            data[1] = 0xAA;
            data[2] = 0x07;
            data[3] = 0x01;
            data[4] = 0x10;

            switch (parameter)
            {
                case "StopInignition"://停止点火
                    {
                        data[5] = 0x03;
                        data[6] = 0x02;
                        break;
                    }
                case "StopPrehead"://停止预热
                    {
                        data[5] = 0x02;
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
        }
        private void StartCountdown()
        {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(200);
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            //if (App.PropertyModelInstance.CountdownSeconds > 0)
            //{
            //    App.PropertyModelInstance.CountdownSeconds--;
            //}
            //else if (App.PropertyModelInstance.CountdownMinutes > 0)
            //{
            //    App.PropertyModelInstance.CountdownMinutes--;
            //    App.PropertyModelInstance.CountdownSeconds = 59;
            //}
            TimeSpan t = DateTime.Now.Subtract(StartTime);
            if (App.PropertyModelInstance.CountdownMinutes > 0 || App.PropertyModelInstance.CountdownSeconds > 0)
            {
                int s = (int)(seconds - t.TotalSeconds);
                App.PropertyModelInstance.CountdownMinutes = s / 60;
                App.PropertyModelInstance.CountdownSeconds = s % 60;
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
                    StopMethod("StopInignition");

                    //开始治疗后，舱盖控制模块不能控制
                    App.PropertyModelInstance.IsMoxibustionTherapyMode = true;
                    IsCountingDown = true;
                    App.PropertyModelInstance.CountdownSeconds = 0;
                    App.PropertyModelInstance.CountdownMinutes = App.PropertyModelInstance.MoxibustionTherapyTime;
                    //发送开始治疗指令
                    StopMethod("StartMoxibustionTherapy");

                    seconds = App.PropertyModelInstance.CountdownSeconds + App.PropertyModelInstance.CountdownMinutes * 60;
                    StartTime = DateTime.Now;
                    StartCountdown();//开启治疗倒计时
                }
                else if (App.PropertyModelInstance.IsMoxibustionTherapyMode == true)
                {
                    //治疗结束
                    App.PropertyModelInstance.IsMoxibustionTherapyMode = false;
                    byte[] data = new byte[11];
                    //发送结束治疗指令
                    StopMethod("StopMoxibustionTherapy");

                    if (App.PropertyModelInstance.IsSmokeSystemOn == false)//如果排烟和净烟都未开启
                    {
                        StopMethod("OpenSmoke");
                    }
                    App.PropertyModelInstance.IsSmokeSystemOn = true;
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
                    StopMethod("StopSmoke");

                    //结束后自动开盖
                    if (App.PropertyModelInstance.AutomaticLidOpening)
                    {
                        StopMethod("OpenHatch");
                    }
                }
            }
        }

    }
}
