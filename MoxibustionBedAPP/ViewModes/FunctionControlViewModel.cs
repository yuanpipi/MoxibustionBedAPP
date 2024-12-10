using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using MoxibustionBedAPP.Models;
using static System.Net.Mime.MediaTypeNames;

namespace MoxibustionBedAPP.ViewModes
{
    public class FunctionControlViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        ///// <summary>
        ///// 排烟系统
        ///// </summary>
        //public RelayCommand SmokeExhaust { get; set; }
        ///// <summary>
        ///// 净烟系统
        ///// </summary>
        //public RelayCommand SmokePurification { get; set; }
        ///// <summary>
        ///// 摇摆系统
        ///// </summary>
        //public RelayCommand Swing { get; set; }
        ///// <summary>
        ///// 红外灯关闭
        ///// </summary>
        //public RelayCommand InfraredLampClose { get; set; }
        ///// <summary>
        ///// 红外灯低档
        ///// </summary>
        //public RelayCommand InfraredLampLow { get; set; }
        ///// <summary>
        ///// 红外灯中档
        ///// </summary>
        //public RelayCommand InfraredLampMedium { get; set; }
        ///// <summary>
        ///// 红外灯高档
        ///// </summary>
        //public RelayCommand InfraredLampHigh { get; set; }
        /// <summary>
        /// 公共指令发送方法
        /// </summary>
        public ICommand PublicFunction { get; set; }

        public ICommand Prehead { get; set; }

        public ICommand Inignition { get; set; }

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



        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public FunctionControlViewModel()
        {
            //SmokeExhaust = new RelayCommand(SmokeExhaustSystemMethod);
            //SmokePurification = new RelayCommand(SmokePurificationSystemMethod);
            //Swing = new RelayCommand(SwingSystemMethod);
            //InfraredLampClose = new RelayCommand(InfraredLampCloseMethod);
            //InfraredLampLow = new RelayCommand(InfraredLampLowMethod);
            //InfraredLampMedium = new RelayCommand(InfraredLampMediumMethod);
            //InfraredLampHigh = new RelayCommand(InfraredLampHighMethod);
            //PublicFunction = new RelayCommand(FunctionMethod);
            PublicFunction = new RelayCommand(ExecuteFunctionMethod);
            Prehead = new RelayCommand(PreheadMethod);
            Inignition= new RelayCommand(InignitionMethod);
        }

        /// <summary>
        /// 操作指令公共方法
        /// </summary>
        /// <param name="parameter"></param>
        private void ExecuteFunctionMethod(object parameter)
        {
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
                        }
                        else
                        {
                            data[6] = 0x01;
                        }
                        break;
                    }
                case "Prehead"://设置预热
                    {

                        data[5] = 0x02;
                        if (App.PropertyModelInstance.PreheadMode)
                        {
                            data[6] = 0x02;
                        }
                        else
                        {
                            data[6] = 0x01;
                        }
                        break;
                    }
                case "Inignition"://点火选择
                    {
                        data[5] = 0x06;
                        if (App.PropertyModelInstance.InignitionStatus)
                        {
                            data[6] = 0x02;
                        }
                        else
                        {
                            data[6] = 0x01;
                        }
                        break;
                    }
                case "BackMoxibustionColumnUp"://背部点升
                    {
                        data[5] = 0x04;
                        data[6] = 0x01;
                        break;
                    }
                case "BackMoxibustionColumnDown"://背部点降
                    {
                        data[5] = 0x05;
                        data[6] = 0x01;
                        break;
                    }
                case "LegMoxibustionColumnUp"://腿部点升
                    {
                        data[5] = 0x06;
                        data[6] = 0x01;
                        break;
                    }
                case "LegMoxibustionColumnDown"://腿部点降
                    {
                        data[5] = 0x07;
                        data[6] = 0x01;
                        break;
                    }
                case "HatchClickUp"://舱盖点开
                    {
                        data[5] = 0x08;
                        data[6] = 0x01;
                        break;
                    }
                case "HatchClickDown"://舱盖点关
                    {
                        data[5] = 0x09;
                        data[6] = 0x01;
                        break;
                    }
                case "OpenHatch"://一键开舱
                    {
                        data[5] = 0x0A;
                        data[6] = 0x01;
                        break;
                    }
                case "CloseHatch"://一键关舱
                    {
                        data[5] = 0x0B;
                        data[6] = 0x01;
                        break;
                    }
                case "SmokeExhaustClose"://关闭排烟系统
                    {
                        data[5] = 0x0C;
                        data[6] = 0x00;
                        break;
                    }
                case "SmokeExhaustLow"://排烟系统低档
                    {
                        data[5] = 0x0C;
                        data[6] = 0x01;
                        break;
                    }
                case "SmokeExhaustMedium"://排烟系统中档
                    {
                        data[5] = 0x0C;
                        data[6] = 0x02;
                        break;
                    }
                case "SmokeExhaustHigh"://排烟系统高档
                    {
                        data[5] = 0x0C;
                        data[6] = 0x03;
                        break;
                    }
                case "SmokePurificationSystem"://净烟系统
                    {
                        data[5] = 0x0D;
                        if (App.PropertyModelInstance.SmokePurificationSystem)
                        {
                            data[6] = 0x01;
                        }
                        else
                        {
                            data[6] = 0x02;
                        }
                        break;
                    }
                case "SwingSystem"://摇摆系统
                    {
                        data[5] = 0x0E;
                        if (App.PropertyModelInstance.SmokePurificationSystem)
                        {
                            data[6] = 0x01;
                        }
                        else
                        {
                            data[6] = 0x02;
                        }
                        break;
                    }
                case "InfraredLampClose"://红外线关
                    {
                        data[5] = 0x0F;
                        data[6] = 0x00;
                        break;
                    }
                case "InfraredLampLow"://红外线低档
                    {
                        data[5] = 0x0F;
                        data[6] = 0x01;
                        break;
                    }
                case "InfraredLampMedium"://红外线中档
                    {
                        data[5] = 0x0F;
                        data[6] = 0x02;
                        break;
                    }
                case "InfraredLampHigh"://红外线高档
                    {
                        data[5] = 0x0F;
                        data[6] = 0x03;
                        break;
                    }
            }
            data[9] = 0x55;
            data[10] = 0xAA;
            data = SerialPortManager.CRC16(data);
            SerialPortManager.Instance.SendData(data);
            //Thread.Sleep(1500);
            //if (!App.IsReceive)
            //{
            //    MessageBox.Show($"串口错误，无返回数据");
            //}
        }

        ///// <summary>
        ///// 排烟系统
        ///// </summary>
        //private void SmokeExhaustSystemMethod()
        //{
        //    byte[] data = new byte[11];
        //    data[0] = 0x55;
        //    data[1] = 0xAA;
        //    data[2] = 0x07;
        //    data[3] = 0x01;
        //    data[4] = 0x10;
        //    data[5] = 0x0C;
        //    if (App.PropertyModelInstance.SmokeExhaustSystem)
        //    {
        //        data[6] = 0x01;
        //    }
        //    else
        //    {
        //        data[6] = 0x02;
        //    }
        //    data[9] = 0x55;
        //    data[10] = 0xAA;
        //    data = SerialPortManager.CRC16(data);
        //    SerialPortManager.Instance.SendData(data);
        //}

        ///// <summary>
        ///// 净烟系统
        ///// </summary>
        //private void SmokePurificationSystemMethod()
        //{
        //    byte[] data = new byte[11];
        //    data[0] = 0x55;
        //    data[1] = 0xAA;
        //    data[2] = 0x07;
        //    data[3] = 0x01;
        //    data[4] = 0x10;
        //    data[5] = 0x0D;
        //    if (App.PropertyModelInstance.SmokePurificationSystem)
        //    {
        //        data[6] = 0x01;
        //    }
        //    else
        //    {
        //        data[6] = 0x02;
        //    }
        //    data[9] = 0x55;
        //    data[10] = 0xAA;
        //    data = SerialPortManager.CRC16(data);
        //    SerialPortManager.Instance.SendData(data);
        //}

        ///// <summary>
        ///// 摇摆系统
        ///// </summary>
        //private void SwingSystemMethod()
        //{
        //    byte[] data = new byte[11];
        //    data[0] = 0x55;
        //    data[1] = 0xAA;
        //    data[2] = 0x07;
        //    data[3] = 0x01;
        //    data[4] = 0x10;
        //    data[5] = 0x0E;
        //    if (App.PropertyModelInstance.SmokePurificationSystem)
        //    {
        //        data[6] = 0x01;
        //    }
        //    else
        //    {
        //        data[6] = 0x02;
        //    }
        //    data[9] = 0x55;
        //    data[10] = 0xAA;
        //    data = SerialPortManager.CRC16(data);
        //    SerialPortManager.Instance.SendData(data);
        //}

        ///// <summary>
        ///// 红外灯关
        ///// </summary>
        //private void InfraredLampCloseMethod()
        //{
        //    byte[] data = new byte[11];
        //    data[0] = 0x55;
        //    data[1] = 0xAA;
        //    data[2] = 0x07;
        //    data[3] = 0x01;
        //    data[4] = 0x10;
        //    data[5] = 0x0F;
        //    data[6] = 0x00;
        //    data[9] = 0x55;
        //    data[10] = 0xAA;
        //    data = SerialPortManager.CRC16(data);
        //    SerialPortManager.Instance.SendData(data);
        //}

        ///// <summary>
        ///// 红外灯低档
        ///// </summary>
        //private void InfraredLampLowMethod()
        //{
        //    byte[] data = new byte[11];
        //    data[0] = 0x55;
        //    data[1] = 0xAA;
        //    data[2] = 0x07;
        //    data[3] = 0x01;
        //    data[4] = 0x10;
        //    data[5] = 0x0F;
        //    data[6] = 0x01;
        //    data[9] = 0x55;
        //    data[10] = 0xAA;
        //    data = SerialPortManager.CRC16(data);
        //    SerialPortManager.Instance.SendData(data);
        //}

        ///// <summary>
        ///// 红外灯中档
        ///// </summary>
        //private void InfraredLampMediumMethod()
        //{
        //    byte[] data = new byte[11];
        //    data[0] = 0x55;
        //    data[1] = 0xAA;
        //    data[2] = 0x07;
        //    data[3] = 0x01;
        //    data[4] = 0x10;
        //    data[5] = 0x0F;
        //    data[6] = 0x02;
        //    data[9] = 0x55;
        //    data[10] = 0xAA;
        //    data = SerialPortManager.CRC16(data);
        //    SerialPortManager.Instance.SendData(data);
        //}

        ///// <summary>
        ///// 红外灯高档
        ///// </summary>
        //private void InfraredLampHighMethod()
        //{
        //    byte[] data = new byte[11];
        //    data[0] = 0x55;
        //    data[1] = 0xAA;
        //    data[2] = 0x07;
        //    data[3] = 0x01;
        //    data[4] = 0x10;
        //    data[5] = 0x0F;
        //    data[6] = 0x03;
        //    data[9] = 0x55;
        //    data[10] = 0xAA;
        //    data = SerialPortManager.CRC16(data);
        //    SerialPortManager.Instance.SendData(data);
        //}

        ///// <summary>
        ///// 背部点升
        ///// </summary>
        //private void BackMoxibustionColumnUpMethod()
        //{
        //    byte[] data = new byte[11];
        //    data[0] = 0x55;
        //    data[1] = 0xAA;
        //    data[2] = 0x07;
        //    data[3] = 0x01;
        //    data[4] = 0x10;
        //    data[5] = 0x04;
        //    data[6] = 0x01;
        //    data[9] = 0x55;
        //    data[10] = 0xAA;
        //    data = SerialPortManager.CRC16(data);
        //    SerialPortManager.Instance.SendData(data);
        //}

        ///// <summary>
        ///// 背部点降
        ///// </summary>
        //private void BackMoxibustionColumnDownMethod()
        //{
        //    byte[] data = new byte[11];
        //    data[0] = 0x55;
        //    data[1] = 0xAA;
        //    data[2] = 0x07;
        //    data[3] = 0x01;
        //    data[4] = 0x10;
        //    data[5] = 0x05;
        //    data[6] = 0x01;
        //    data[9] = 0x55;
        //    data[10] = 0xAA;
        //    data = SerialPortManager.CRC16(data);
        //    SerialPortManager.Instance.SendData(data);
        //}

        ///// <summary>
        ///// 腿部点升
        ///// </summary>
        //private void LegMoxibustionColumnUpMethod()
        //{
        //    byte[] data = new byte[11];
        //    data[0] = 0x55;
        //    data[1] = 0xAA;
        //    data[2] = 0x07;
        //    data[3] = 0x01;
        //    data[4] = 0x10;
        //    data[5] = 0x06;
        //    data[6] = 0x01;
        //    data[9] = 0x55;
        //    data[10] = 0xAA;
        //    data = SerialPortManager.CRC16(data);
        //    SerialPortManager.Instance.SendData(data);
        //}

        ///// <summary>
        ///// 腿部点降
        ///// </summary>
        //private void LegMoxibustionColumnDownMethod()
        //{
        //    byte[] data = new byte[11];
        //    data[0] = 0x55;
        //    data[1] = 0xAA;
        //    data[2] = 0x07;
        //    data[3] = 0x01;
        //    data[4] = 0x10;
        //    data[5] = 0x07;
        //    data[6] = 0x01;
        //    data[9] = 0x55;
        //    data[10] = 0xAA;
        //    data = SerialPortManager.CRC16(data);
        //    SerialPortManager.Instance.SendData(data);
        //}

        ///// <summary>
        ///// 舱盖点升
        ///// </summary>
        //private void HatchClickUpMethod()
        //{
        //    byte[] data = new byte[11];
        //    data[0] = 0x55;
        //    data[1] = 0xAA;
        //    data[2] = 0x07;
        //    data[3] = 0x01;
        //    data[4] = 0x10;
        //    data[5] = 0x08;
        //    data[6] = 0x01;
        //    data[9] = 0x55;
        //    data[10] = 0xAA;
        //    data = SerialPortManager.CRC16(data);
        //    SerialPortManager.Instance.SendData(data);
        //}

        ///// <summary>
        ///// 舱盖点降
        ///// </summary>
        //private void HatchClickDownMethod()
        //{
        //    byte[] data = new byte[11];
        //    data[0] = 0x55;
        //    data[1] = 0xAA;
        //    data[2] = 0x07;
        //    data[3] = 0x01;
        //    data[4] = 0x10;
        //    data[5] = 0x09;
        //    data[6] = 0x01;
        //    data[9] = 0x55;
        //    data[10] = 0xAA;
        //    data = SerialPortManager.CRC16(data);
        //    SerialPortManager.Instance.SendData(data);
        //}

        ///// <summary>
        ///// 一键开舱
        ///// </summary>
        //private void OpenHatchMethod()
        //{
        //    byte[] data = new byte[11];
        //    data[0] = 0x55;
        //    data[1] = 0xAA;
        //    data[2] = 0x07;
        //    data[3] = 0x01;
        //    data[4] = 0x10;
        //    data[5] = 0x0A;
        //    data[6] = 0x01;
        //    data[9] = 0x55;
        //    data[10] = 0xAA;
        //    data = SerialPortManager.CRC16(data);
        //    SerialPortManager.Instance.SendData(data);
        //}

        ///// <summary>
        ///// 一键关舱
        ///// </summary>
        //private void CloseHatchMethod()
        //{
        //    byte[] data = new byte[11];
        //    data[0] = 0x55;
        //    data[1] = 0xAA;
        //    data[2] = 0x07;
        //    data[3] = 0x01;
        //    data[4] = 0x10;
        //    data[5] = 0x0B;
        //    data[6] = 0x01;
        //    data[9] = 0x55;
        //    data[10] = 0xAA;
        //    data = SerialPortManager.CRC16(data);
        //    SerialPortManager.Instance.SendData(data);
        //}

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


        private void StartCountdown()
        {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            //App.PropertyModelInstance.CountdownSeconds--;
            //if (App.PropertyModelInstance.CountdownSeconds < 0)
            //{
            //    _timer.Stop();
            //    App.PropertyModelInstance.CountdownSeconds = 0;
            //}

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
                if(App.PropertyModelInstance.PreheadMode == true)
                {
                    App.PropertyModelInstance.PreheadMode = false;
                }
                else if(App.PropertyModelInstance.InignitionStatus == true)
                {
                    App.PropertyModelInstance.InignitionStatus = false;
                }
            }
        }

        //private void Instance_DataReceived(object sender,string data)
        //{
        //    System.Windows.Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
        //    {
        //        ReceivedData = data;
        //    }));
        //}

        //private string _receivedData;
        //public string ReceivedData
        //{
        //    get
        //    {
        //        return _receivedData;
        //    }
        //    set
        //    {
        //        _receivedData = value;
        //        OnPropertyChanged("ReceivedData");
        //    }
        //}

        //public void OpenSerialPort(string portName,int baudRate)
        //{
        //    SerialPortManager.Instance.OpenPort(portName, baudRate);
        //}

        //public void CloseSerialPort()
        //{
        //    SerialPortManager.Instance.ClosePort();
        //}

        //public void SendSerialData(string data)
        //{
        //   // SerialPortManager.Instance.SendData(data);
        //}
    }
}
