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

namespace MoxibustionBedAPP.Models
{
    public class SerialPortManager
    {
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
        protected virtual void OnDataReceived(byte[] data)
        {
            //string message = "";
            DataReceived?.Invoke(this, data);
            //for(int i=0;i<data.Count();i++)
            //{
            //    message+= data[i].ToString();
            //}
            //MessageBox.Show(message);
        }

        /// <summary>
        /// 发送数据byte[]
        /// </summary>
        /// <param name="data"></param>
        public void SendData(byte[] data)
        {
            if (_serialPort.IsOpen)
            {
                _serialPort.Write(data,0,data.Length);
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
                _serialPort.Write(data);
            }
        }

        public void ReceiveData(object sender,SerialDataReceivedEventArgs e)
        {
            if (_serialPort.IsOpen)
            {
                _serialPort.ReadTimeout = 10000;

                if (_serialPort.BytesToRead > 0)
                {
                    try
                    {
                        byte[] buffer = new byte[_serialPort.BytesToRead];
                        if (buffer.Length <= 0)
                            return;
                        _serialPort.Read(buffer, 0, buffer.Length);
                        OnDataReceived(buffer);
                            //MessageBox.Show(buffer);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("串口数据接受失败："+ex.Message);
                    }
                }
            }
        }
    }
}
