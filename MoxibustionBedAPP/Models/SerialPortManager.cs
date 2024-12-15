using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Configuration;
using MoxibustionBedAPP.Views;
using System.Windows.Threading;
using MoxibustionBedAPP.ViewModes;

namespace MoxibustionBedAPP.Models
{
    public class SerialPortManager : IDisposable
    {
        /// <summary>
        /// 串口变量
        /// </summary>
        private static SerialPortManager _instance;
        public static SerialPortManager Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new SerialPortManager();
                }
                return _instance;
            }
        }

        private SerialPort _serialPort;
        private CancellationTokenSource _cancellationTokenSource;
        private bool _isDisposed = false;

        private SerialPortManager()
        {
            _serialPort = new SerialPort()
            {
                PortName = ConfigurationManager.AppSettings["SerialPortName"],//"COM1"
                BaudRate = int.Parse(ConfigurationManager.AppSettings["BaudRate"]),//9600
                Parity = Parity.None,
                DataBits = 8,
                StopBits = StopBits.One,
            };
            _cancellationTokenSource = new CancellationTokenSource();
        }

        /// <summary>
        /// 打开串口
        /// </summary>
        public void OpenPort()
        {
            if (_serialPort != null && !_serialPort.IsOpen)
            {
                try
                {
                    _serialPort.Open();//打开串口
                    Console.WriteLine("串口已打开");
                    //读写超时设置
                    _serialPort.WriteTimeout = 3000;
                    _serialPort.ReadTimeout = 3000;
                    _serialPort.DataReceived += new SerialDataReceivedEventHandler(ReceiveData);
                    //StartReceivingData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"打开串口失败: {ex.Message}");
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 关闭串口
        /// </summary>
        public void ClosePort()
        {
            if(_serialPort != null && _serialPort.IsOpen)
            {

                _serialPort.Close();
                Console.WriteLine("串口已关闭");
                _cancellationTokenSource.Cancel();
            }
        }

        public async void StartReceivingData()
        {
            var token=_cancellationTokenSource.Token;
            while(!token.IsCancellationRequested)
            {
                try
                {
                    if(_serialPort.IsOpen)
                    {
                        byte[] buffer = new byte[_serialPort.BytesToRead];
                        if (buffer.Length > 0)
                        {
                            _serialPort.Read(buffer, 0, buffer.Length);
                            //string data = _serialPort.Read(data,0,)
                            //处理接收到的数据
                            OnDataReceived(buffer);
                        }
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                await Task.Delay(5, token);
            }
        }

        public event EventHandler<byte[]> DataReceived;

        /// <summary>
        /// 处理接收到的数据
        /// </summary>
        /// <param name="data"></param>
        protected virtual void OnDataReceived(byte[] data)
        {
            //DataReceived?.Invoke(this, data);
            // 将ASCII字节数组转换为字符串
            string asciiString = Encoding.ASCII.GetString(data);

            // 分割字符串以获取每个字节的十六进制表示
            string[] hexValues = asciiString.Split(' ');

            // 创建字节数组来存储转换后的值
            byte[] bytes = new byte[hexValues.Length];
            byte[] b = new byte[hexValues.Length];

            // 将每个十六进制字符串转换为字节
            for (int i = 0; i < hexValues.Length; i++)
            {
                bytes[i] = Convert.ToByte(hexValues[i], 16);
                b[i] = Convert.ToByte(hexValues[i], 16);
            }
            //数据验证
            b[b.Count() - 3] = 0x00;
            b[b.Count() - 4] = 0x00;
            b = CRC16(b);
            if (b[b.Count() - 3] == bytes[bytes.Count() - 3]&& b[b.Count() - 4] == bytes[bytes.Count() - 4])
            {
                if(bytes[5]==0x12)//监控数据
                {
                    App.PropertyModelInstance.Upper_CabinTemperatureNow = Convert.ToInt16(bytes[6]);//上舱温度
                    App.PropertyModelInstance.BackTemperatureNow= Convert.ToInt16(bytes[7]);//背部温度
                    App.PropertyModelInstance.LegTemperatureNow= Convert.ToInt16(bytes[8]);//腿部温度
                    App.PropertyModelInstance.BackMoxibustionColumn_Height=Convert.ToInt16(bytes[9]);//背部灸柱高度
                    App.PropertyModelInstance.LegMoxibustionColumn_Height = Convert.ToInt16(bytes[10]);//腿部灸柱高度
                    App.PropertyModelInstance.BatteryLevel= Convert.ToInt16(bytes[11]);//电池电量
                    App.PropertyModelInstance.InfraredLamp= Convert.ToInt16(bytes[12]);//红外灯
                    App.PropertyModelInstance.SmokeExhaustSystem = Convert.ToInt16(bytes[13]);//排烟系统
                    App.PropertyModelInstance.SmokePurificationSystem=bytes[14] == 0x02 ? false : true;//净烟系统
                    App.PropertyModelInstance.SwingSystem=bytes[15] == 0x02 ? false :true;//摇摆系统
                    App.PropertyModelInstance.Hatch=bytes[16] == 0x02 ? false :true;//舱盖


                    if(App.PropertyModelInstance.Upper_CabinTemperatureNow >= App.PropertyModelInstance.UpperAlarmCabinTemperature)
                    {
                        App.PropertyModelInstance.IsUpperAlarm = true;
                    }
                    else
                    {
                        App.PropertyModelInstance.IsUpperAlarm= false;
                    }

                    if(App.PropertyModelInstance.BackTemperatureNow>=App.PropertyModelInstance.BackAlarmTemperature)
                    {
                        App.PropertyModelInstance.IsBackAlarm = true;
                    }
                    else
                    {
                        App.PropertyModelInstance.IsBackAlarm = false;
                    }

                    if(App.PropertyModelInstance.LegTemperatureNow>=App.PropertyModelInstance.LegAlarmTemperature)
                    {
                        App.PropertyModelInstance.IsLegAlarm = true;
                    }
                    else
                    {
                        App.PropertyModelInstance.IsLegAlarm= false;
                    }


                }
                else
                {
                    App.IsReceive = true;
                    if (bytes[6]==0x01)
                    {
                        Console.WriteLine("操作成功");
                    }
                    else if(bytes[6] == 0x02)
                    {
                        PopupBoxViewModel.ShowPopupBox("操作失败，请重新操作");
                    }
                    //switch(bytes[5])
                    //{
                    //    case 0x01://治疗参数设置
                    //        break;
                    //    case 0x02://设置预热
                    //        break;
                    //    case 0x03://点火选择
                    //        break;
                    //    case 0x04://背部点升
                    //        break;
                    //    case 0x05://背部点降
                    //        break;
                    //    case 0x06://腿部点升
                    //        break;
                    //    case 0x07://腿部点降
                    //        break;
                    //    case 0x08://舱盖点开
                    //        break;
                    //    case 0x09://舱盖点关
                    //        break;
                    //    case 0x0A://一键开舱
                    //        break;
                    //    case 0x0B://一键关舱
                    //        break;
                    //    case 0x0C://排烟系统
                    //        break;
                    //    case 0x0D://净烟系统
                    //        break;
                    //    case 0x0E://摇摆系统
                    //        break;
                    //    case 0x0F://红外灯
                    //        break;
                    //}
                }
            }
            else
            {
                PopupBoxViewModel.ShowPopupBox($"数据错误！");
            }
        }

        /// <summary>
        /// 发送数据byte[]
        /// </summary>
        /// <param name="data"></param>
        public void SendData(byte[] data)
        {
            if (_serialPort.IsOpen)
            {
                try
                {
                    _serialPort.Write(data, 0, data.Length);
                    Thread.Sleep(500);
                    if (App.IsReceive == false)
                    {
                        PopupBoxViewModel.ShowPopupBox($"串口错误，无应答数据");
                    }
                }
                catch (Exception ex)
                {
                    PopupBoxViewModel.ShowPopupBox($"数据发送失败: {ex.Message}");
                }
            }
            else
            {
                PopupBoxViewModel.ShowPopupBox($"串口未打开");
            }
        }

        /// <summary>
        /// 发送数据string
        /// </summary>
        /// <param name="data"></param>
        public void SendData(string data)
        {
            if (_serialPort.IsOpen)
            {
                try
                {
                    _serialPort.Write(data);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"数据发送失败: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show($"串口未打开");
            }
        }

        /// <summary>
        /// 接受下位机数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ReceiveData(object sender,SerialDataReceivedEventArgs e)
        {
            if (_serialPort.IsOpen)
            {
                _serialPort.ReadTimeout = 3000;//读取超时

                if (_serialPort.BytesToRead > 0)
                {
                    try
                    {
                        byte[] buffer = new byte[_serialPort.BytesToRead];
                        if (buffer.Length <= 0)
                            return;
                        _serialPort.Read(buffer, 0, buffer.Length);
                        OnDataReceived(buffer);//处理接收到的数据
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("串口数据接受失败："+ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show($"串口未打开");
            }
        }

        /// <summary>
        /// CRC校验
        /// 从帧长度开始到数据帧结束，不包括帧头帧尾
        /// for (int n = 0; n < len; n++)
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static byte[] CRC16(byte[] bytes)
        {
            //计算并填写CRC校验码
            int crc = 0xffff;
            int len = bytes.Length;
            for (int n = 2; n < len-2; n++)
            {
                byte i;
                crc ^= bytes[n];
                for (i = 0; i < 8; i++)
                {
                    int TT;
                    TT = crc & 1;
                    crc = crc >> 1;
                    crc = crc & 0x7fff;
                    if (TT == 1)
                    {
                        crc = crc ^ 0xa001;
                    }
                    crc = crc & 0xffff;
                }

            }
            //var nl = bytes.Length + 2;
            //生成的两位校验码
            byte[] redata = new byte[2];
            redata[0] = (byte)((crc & 0xff));
            redata[1] = (byte)((crc >> 8) & 0xff);

            //重新组织字节数组
            var newByte = new byte[len];
            for (int i = 0; i < bytes.Length; i++)
            {
                newByte[i] = bytes[i];
            }
            newByte[len - 4] = (byte)redata[0];
            newByte[len - 3] = redata[1];
            // HelperTypeConversion.concat(bytes, newByte)
            return newByte;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    // 如果托管资源需要释放
                    _serialPort?.Dispose();
                }

                // 释放非托管资源（如果有）

                _isDisposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
