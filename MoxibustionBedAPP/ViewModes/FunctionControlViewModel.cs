using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using MoxibustionBedAPP.Models;
using static System.Net.Mime.MediaTypeNames;

namespace MoxibustionBedAPP.ViewModes
{
    public class FunctionControlViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 排烟系统
        /// </summary>
        public RelayCommand SmokeExhaust { get; set; }
        /// <summary>
        /// 净烟系统
        /// </summary>
        public RelayCommand SmokePurification { get; set; }
        /// <summary>
        /// 摇摆系统
        /// </summary>
        public RelayCommand Swing { get; set; }
        /// <summary>
        /// 红外灯关闭
        /// </summary>
        public RelayCommand InfraredLampClose { get; set; }
        /// <summary>
        /// 红外灯低档
        /// </summary>
        public RelayCommand InfraredLampLow { get; set; }
        /// <summary>
        /// 红外灯中档
        /// </summary>
        public RelayCommand InfraredLampMedium { get; set; }
        /// <summary>
        /// 红外灯高档
        /// </summary>
        public RelayCommand InfraredLampHigh { get; set; }







        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public FunctionControlViewModel()
        {
            SmokeExhaust = new RelayCommand(SmokeExhaustSystemMethod);
            SmokePurification = new RelayCommand(SmokePurificationSystemMethod);
            Swing = new RelayCommand(SwingSystemMethod);
            InfraredLampClose = new RelayCommand(InfraredLampCloseMethod);
            InfraredLampLow = new RelayCommand(InfraredLampLowMethod);
            InfraredLampMedium = new RelayCommand(InfraredLampMediumMethod);
            InfraredLampHigh = new RelayCommand(InfraredLampHighMethod);
        }

        /// <summary>
        /// 排烟系统
        /// </summary>
        private void SmokeExhaustSystemMethod()
        {
            byte[] data = new byte[11];
            data[0] = 0x55;
            data[1] = 0xAA;
            data[2] = 0x07;
            data[3] = 0x01;
            data[4] = 0x10;
            data[5] = 0x0C;
            if (App.PropertyModelInstance.SmokeExhaustSystem)
            {
                data[6] = 0x01;
            }
            else
            {
                data[6] = 0x02;
            }
            data[9] = 0x55;
            data[10] = 0xAA;
            data=SerialPortManager.CRC16(data);
            SerialPortManager.Instance.SendData(data);
        }

        /// <summary>
        /// 净烟系统
        /// </summary>
        private void SmokePurificationSystemMethod()
        {
            byte[] data = new byte[11];
            data[0] = 0x55;
            data[1] = 0xAA;
            data[2] = 0x07;
            data[3] = 0x01;
            data[4] = 0x10;
            data[5] = 0x0D;
            if (App.PropertyModelInstance.SmokePurificationSystem)
            {
                data[6] = 0x01;
            }
            else
            {
                data[6] = 0x02;
            }
            data[9] = 0x55;
            data[10] = 0xAA;
            data = SerialPortManager.CRC16(data);
            SerialPortManager.Instance.SendData(data);
        }

        /// <summary>
        /// 摇摆系统
        /// </summary>
        private void SwingSystemMethod()
        {
            byte[] data = new byte[11];
            data[0] = 0x55;
            data[1] = 0xAA;
            data[2] = 0x07;
            data[3] = 0x01;
            data[4] = 0x10;
            data[5] = 0x0E;
            if (App.PropertyModelInstance.SmokePurificationSystem)
            {
                data[6] = 0x01;
            }
            else
            {
                data[6] = 0x02;
            }
            data[9] = 0x55;
            data[10] = 0xAA;
            data = SerialPortManager.CRC16(data);
            SerialPortManager.Instance.SendData(data);
        }

        /// <summary>
        /// 红外灯关
        /// </summary>
        private void InfraredLampCloseMethod()
        {
            byte[] data = new byte[11];
            data[0] = 0x55;
            data[1] = 0xAA;
            data[2] = 0x07;
            data[3] = 0x01;
            data[4] = 0x10;
            data[5] = 0x0F;
            data[6] = 0x00;
            data[9] = 0x55;
            data[10] = 0xAA;
            data = SerialPortManager.CRC16(data);
            SerialPortManager.Instance.SendData(data);
        }

        /// <summary>
        /// 红外灯低档
        /// </summary>
        private void InfraredLampLowMethod()
        {
            byte[] data = new byte[11];
            data[0] = 0x55;
            data[1] = 0xAA;
            data[2] = 0x07;
            data[3] = 0x01;
            data[4] = 0x10;
            data[5] = 0x0F;
            data[6] = 0x01;
            data[9] = 0x55;
            data[10] = 0xAA;
            data = SerialPortManager.CRC16(data);
            SerialPortManager.Instance.SendData(data);
        }

        /// <summary>
        /// 红外灯中档
        /// </summary>
        private void InfraredLampMediumMethod()
        {
            byte[] data = new byte[11];
            data[0] = 0x55;
            data[1] = 0xAA;
            data[2] = 0x07;
            data[3] = 0x01;
            data[4] = 0x10;
            data[5] = 0x0F;
            data[6] = 0x02;
            data[9] = 0x55;
            data[10] = 0xAA;
            data = SerialPortManager.CRC16(data);
            SerialPortManager.Instance.SendData(data);
        }

        /// <summary>
        /// 红外灯高档
        /// </summary>
        private void InfraredLampHighMethod()
        {
            byte[] data = new byte[11];
            data[0] = 0x55;
            data[1] = 0xAA;
            data[2] = 0x07;
            data[3] = 0x01;
            data[4] = 0x10;
            data[5] = 0x0F;
            data[6] = 0x03;
            data[9] = 0x55;
            data[10] = 0xAA;
            data = SerialPortManager.CRC16(data);
            SerialPortManager.Instance.SendData(data);
        }

        /// <summary>
        /// 背部点升
        /// </summary>
        private void BackMoxibustionColumnUpMethod()
        {
            byte[] data = new byte[11];
            data[0] = 0x55;
            data[1] = 0xAA;
            data[2] = 0x07;
            data[3] = 0x01;
            data[4] = 0x10;
            data[5] = 0x04;
            data[6] = 0x01;
            data[9] = 0x55;
            data[10] = 0xAA;
            data = SerialPortManager.CRC16(data);
            SerialPortManager.Instance.SendData(data);
        }

        /// <summary>
        /// 背部点降
        /// </summary>
        private void BackMoxibustionColumnDownMethod()
        {
            byte[] data = new byte[11];
            data[0] = 0x55;
            data[1] = 0xAA;
            data[2] = 0x07;
            data[3] = 0x01;
            data[4] = 0x10;
            data[5] = 0x05;
            data[6] = 0x01;
            data[9] = 0x55;
            data[10] = 0xAA;
            data = SerialPortManager.CRC16(data);
            SerialPortManager.Instance.SendData(data);
        }

        /// <summary>
        /// 腿部点升
        /// </summary>
        private void LegMoxibustionColumnUpMethod()
        {
            byte[] data = new byte[11];
            data[0] = 0x55;
            data[1] = 0xAA;
            data[2] = 0x07;
            data[3] = 0x01;
            data[4] = 0x10;
            data[5] = 0x06;
            data[6] = 0x01;
            data[9] = 0x55;
            data[10] = 0xAA;
            data = SerialPortManager.CRC16(data);
            SerialPortManager.Instance.SendData(data);
        }

        /// <summary>
        /// 腿部点降
        /// </summary>
        private void LegMoxibustionColumnDownMethod()
        {
            byte[] data = new byte[11];
            data[0] = 0x55;
            data[1] = 0xAA;
            data[2] = 0x07;
            data[3] = 0x01;
            data[4] = 0x10;
            data[5] = 0x07;
            data[6] = 0x01;
            data[9] = 0x55;
            data[10] = 0xAA;
            data = SerialPortManager.CRC16(data);
            SerialPortManager.Instance.SendData(data);
        }

        /// <summary>
        /// 舱盖点升
        /// </summary>
        private void HatchClickUpMethod()
        {
            byte[] data = new byte[11];
            data[0] = 0x55;
            data[1] = 0xAA;
            data[2] = 0x07;
            data[3] = 0x01;
            data[4] = 0x10;
            data[5] = 0x08;
            data[6] = 0x01;
            data[9] = 0x55;
            data[10] = 0xAA;
            data = SerialPortManager.CRC16(data);
            SerialPortManager.Instance.SendData(data);
        }

        /// <summary>
        /// 舱盖点降
        /// </summary>
        private void HatchClickDownMethod()
        {
            byte[] data = new byte[11];
            data[0] = 0x55;
            data[1] = 0xAA;
            data[2] = 0x07;
            data[3] = 0x01;
            data[4] = 0x10;
            data[5] = 0x09;
            data[6] = 0x01;
            data[9] = 0x55;
            data[10] = 0xAA;
            data = SerialPortManager.CRC16(data);
            SerialPortManager.Instance.SendData(data);
        }

        /// <summary>
        /// 一键开舱
        /// </summary>
        private void OpenHatchMethod()
        {
            byte[] data = new byte[11];
            data[0] = 0x55;
            data[1] = 0xAA;
            data[2] = 0x07;
            data[3] = 0x01;
            data[4] = 0x10;
            data[5] = 0x0A;
            data[6] = 0x01;
            data[9] = 0x55;
            data[10] = 0xAA;
            data = SerialPortManager.CRC16(data);
            SerialPortManager.Instance.SendData(data);
        }

        /// <summary>
        /// 一键关舱
        /// </summary>
        private void CloseHatchMethod()
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
            data[5] = 0x02;
            if(App.PropertyModelInstance.PreheadMode)
            {
                data[6] = 0x02;
            }
            else
            {
                data[6] = 0x01;
            }
            data[9] = 0x55;
            data[10] = 0xAA;
            data = SerialPortManager.CRC16(data);
            SerialPortManager.Instance.SendData(data);
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
            data[5] = 0x03;
            if (App.PropertyModelInstance.InignitionStatus)
            {
                data[6] = 0x02;
            }
            else
            {
                data[6] = 0x01;
            }
            data[9] = 0x55;
            data[10] = 0xAA;
            data = SerialPortManager.CRC16(data);
            SerialPortManager.Instance.SendData(data);
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
