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
using System.Windows.Forms.Integration;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Windows.Markup.Localizer;
using System.Windows.Automation.Peers;

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

        public PropertyModel propertyModel = new PropertyModel();
        public static DispatcherTimer _timer;

        /// <summary>
        /// 下位机相关串口连接
        /// </summary>
        private SerialPort _serialPort;
        private CancellationTokenSource _cancellationTokenSource;

        /// <summary>
        /// 语音控制模块相关串口连接
        /// </summary>
        private SerialPort _serialPortVoice;
        private CancellationTokenSource _cancellationTokenSourceVoice;
        private bool _isDisposed = false;
        private byte[] frameHeader = { 0x55, 0xAA };
        private byte[] frameEnd = { 0x55, 0xAA };
        private List<byte> dataBuffer = new List<byte>();
        private int bufferLength = 0; 
        /// <summary>
        /// 开关舱倒计时
        /// </summary>
        private DispatcherTimer timerOpen = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(20)
        };
        /// <summary>
        /// 开关舱倒计时
        /// </summary>
        private DispatcherTimer timerClose = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(20)
        };

        private SerialPortManager()
        {
            _serialPort = new SerialPort()
            {
                PortName = ConfigurationManager.AppSettings["SerialPortName"],//"COM1"
                BaudRate = int.Parse(ConfigurationManager.AppSettings["BaudRate"]),//115200
                Parity = Parity.None,
                DataBits = 8,
                StopBits = StopBits.One,
                ReadBufferSize = 4096
            };
            _cancellationTokenSource = new CancellationTokenSource();

            _serialPortVoice = new SerialPort()
            {
                PortName = "COM3",
                BaudRate = 115200,
                Parity = Parity.None,
                DataBits = 8,
                StopBits = StopBits.One
            };
            _cancellationTokenSourceVoice = new CancellationTokenSource();
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
            App.PropertyModelInstance.Upper_CabinTemperatureNow = propertyModel.Upper_CabinTemperatureNow;//上舱温度
            App.PropertyModelInstance.BackTemperatureNow = propertyModel.BackTemperatureNow;//背部温度
            App.PropertyModelInstance.LegTemperatureNow = propertyModel.LegTemperatureNow;//腿部温度
            App.PropertyModelInstance.BackMoxibustionColumn_Height = propertyModel.BackMoxibustionColumn_Height;//背部灸柱高度
            App.PropertyModelInstance.LegMoxibustionColumn_Height = propertyModel.LegMoxibustionColumn_Height;//腿部灸柱高度
            App.PropertyModelInstance.BatteryLevel = propertyModel.BatteryLevel;//电池电量
            App.PropertyModelInstance.InfraredLamp = propertyModel.InfraredLamp;//红外灯
            App.PropertyModelInstance.SmokeExhaustSystem = propertyModel.SmokeExhaustSystem;//排烟系统
            App.PropertyModelInstance.SmokePurificationSystem = propertyModel.SmokePurificationSystem;//净烟系统
            App.PropertyModelInstance.SwingSystem = propertyModel.SwingSystem;//摇摆系统
            App.PropertyModelInstance.Hatch = propertyModel.Hatch;//舱盖
            App.PropertyModelInstance.IsUpperAlarm = propertyModel.IsUpperAlarm;
            App.PropertyModelInstance.IsBackAlarm = propertyModel.IsBackAlarm;
            App.PropertyModelInstance.IsLegAlarm = propertyModel.IsLegAlarm;
            App.PropertyModelInstance.IsSmokeSystemOn = propertyModel.IsSmokeSystemOn;
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
                    //throw ex;
                    return;
                }
            }
            if(_serialPortVoice!=null&&!_serialPortVoice.IsOpen)
            {
                try
                {
                    _serialPortVoice.Open();
                    Console.WriteLine("语音串口已打开");
                    _serialPortVoice.DataReceived += new SerialDataReceivedEventHandler(ReceiveDataByVoice);
                }
                catch(Exception e)
                {
                    Console.WriteLine($"串口打开失败:{e.Message}");
                    return;
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
            //string asciiString = Encoding.ASCII.GetString(data);

            // 分割字符串以获取每个字节的十六进制表示
            //string[] hexValues = asciiString.Split(' ');

            // 创建字节数组来存储转换后的值
            //byte[] bytes = new byte[hexValues.Length];
            //byte[] b = new byte[hexValues.Length];
            byte[] bytes=new byte[data.Length];
            Array.Copy(data, bytes, data.Length);
            byte[] b = new byte[data.Length];
            Array.Copy(data, b, data.Length);

            // 将每个十六进制字符串转换为字节
            //for (int i = 0; i < hexValues.Length; i++)
            //{
            //    bytes[i] = Convert.ToByte(hexValues[i], 16);
            //    b[i] = Convert.ToByte(hexValues[i], 16);
            //}
            //数据验证
            b[b.Count() - 3] = 0x00;
            b[b.Count() - 4] = 0x00;
            b = CRC16(b);
            if (b[b.Count() - 3] == bytes[bytes.Count() - 3]&& b[b.Count() - 4] == bytes[bytes.Count() - 4])
            {
                if(bytes[5]==0x12)//监控数据
                {
                    propertyModel.Upper_CabinTemperatureNow = Convert.ToInt16(bytes[6]);//上舱温度
                    propertyModel.BackTemperatureNow= Convert.ToInt16(bytes[7]);//背部温度
                    propertyModel.LegTemperatureNow= Convert.ToInt16(bytes[8]);//腿部温度
                    propertyModel.BackMoxibustionColumn_Height=Convert.ToInt16(bytes[9]);//背部灸柱高度
                    propertyModel.LegMoxibustionColumn_Height = Convert.ToInt16(bytes[10]);//腿部灸柱高度
                    propertyModel.BatteryLevel= Convert.ToInt16(bytes[11]);//电池电量
                    propertyModel.InfraredLamp= Convert.ToInt16(bytes[12]);//红外灯
                    propertyModel.SmokeExhaustSystem = Convert.ToInt16(bytes[13]);//排烟系统
                    propertyModel.SmokePurificationSystem=bytes[14] == 0x02 ? false : true;//净烟系统
                    propertyModel.SwingSystem=bytes[15] == 0x02 ? false :true;//摇摆系统
                    propertyModel.Hatch=bytes[16] == 0x02 ? false :true;//舱盖


                    if(propertyModel.Upper_CabinTemperatureNow >= App.PropertyModelInstance.UpperAlarmCabinTemperature)//上舱温度
                    {
                        propertyModel.IsUpperAlarm = true;
                    }
                    else
                    {
                        propertyModel.IsUpperAlarm= false;
                    }

                    if(propertyModel.BackTemperatureNow>=App.PropertyModelInstance.BackAlarmTemperature)//背部温度
                    {
                        propertyModel.IsBackAlarm = true;
                    }
                    else
                    {
                        propertyModel.IsBackAlarm = false;
                    }

                    if(propertyModel.LegTemperatureNow>=App.PropertyModelInstance.LegAlarmTemperature)//腿部温度
                    {
                        propertyModel.IsLegAlarm = true;
                    }
                    else
                    {
                        propertyModel.IsLegAlarm= false;
                    }

                    if (propertyModel.SmokePurificationSystem == false && propertyModel.SmokeExhaustSystem == 0)//查看是否开启排烟系统或者净烟系统
                    {
                        propertyModel.IsSmokeSystemOn = false;
                    }
                    else
                    {
                        propertyModel.IsSmokeSystemOn = true;
                    }
                    
                    //App.PropertyModelInstance.Upper_CabinTemperatureNow = Convert.ToInt16(bytes[6]);//上舱温度
                    //App.PropertyModelInstance.BackTemperatureNow= Convert.ToInt16(bytes[7]);//背部温度
                    //App.PropertyModelInstance.LegTemperatureNow= Convert.ToInt16(bytes[8]);//腿部温度
                    //App.PropertyModelInstance.BackMoxibustionColumn_Height=Convert.ToInt16(bytes[9]);//背部灸柱高度
                    //App.PropertyModelInstance.LegMoxibustionColumn_Height = Convert.ToInt16(bytes[10]);//腿部灸柱高度
                    //App.PropertyModelInstance.BatteryLevel= Convert.ToInt16(bytes[11]);//电池电量
                    //App.PropertyModelInstance.InfraredLamp= Convert.ToInt16(bytes[12]);//红外灯
                    //App.PropertyModelInstance.SmokeExhaustSystem = Convert.ToInt16(bytes[13]);//排烟系统
                    //App.PropertyModelInstance.SmokePurificationSystem=bytes[14] == 0x02 ? false : true;//净烟系统
                    //App.PropertyModelInstance.SwingSystem=bytes[15] == 0x02 ? false :true;//摇摆系统
                    //App.PropertyModelInstance.Hatch=bytes[16] == 0x02 ? false :true;//舱盖


                    //if(App.PropertyModelInstance.Upper_CabinTemperatureNow >= App.PropertyModelInstance.UpperAlarmCabinTemperature)//上舱温度
                    //{
                    //    App.PropertyModelInstance.IsUpperAlarm = true;
                    //}
                    //else
                    //{
                    //    App.PropertyModelInstance.IsUpperAlarm= false;
                    //}

                    //if(App.PropertyModelInstance.BackTemperatureNow>=App.PropertyModelInstance.BackAlarmTemperature)//背部温度
                    //{
                    //    App.PropertyModelInstance.IsBackAlarm = true;
                    //}
                    //else
                    //{
                    //    App.PropertyModelInstance.IsBackAlarm = false;
                    //}

                    //if(App.PropertyModelInstance.LegTemperatureNow>=App.PropertyModelInstance.LegAlarmTemperature)//腿部温度
                    //{
                    //    App.PropertyModelInstance.IsLegAlarm = true;
                    //}
                    //else
                    //{
                    //    App.PropertyModelInstance.IsLegAlarm= false;
                    //}

                    //if (App.PropertyModelInstance.SmokePurificationSystem == false && App.PropertyModelInstance.SmokeExhaustSystem == 0)//查看是否开启排烟系统或者净烟系统
                    //{
                    //    App.PropertyModelInstance.IsSmokeSystemOn = false;
                    //}
                    //else
                    //{
                    //    App.PropertyModelInstance.IsSmokeSystemOn = true;
                    //}

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
            //else
            //{
            //    PopupBoxViewModel.ShowPopupBox($"数据错误！");
            //}
        }
        protected virtual void OnDataReceived(List<byte> data)
        {
            //DataReceived?.Invoke(this, data);
            // 将ASCII字节数组转换为字符串
            //string asciiString = Encoding.ASCII.GetString(data);

            // 分割字符串以获取每个字节的十六进制表示
            //string[] hexValues = asciiString.Split(' ');

            // 创建字节数组来存储转换后的值
            //byte[] bytes = new byte[hexValues.Length];
            //byte[] b = new byte[hexValues.Length];
            List<byte> bytes = data;
            //Array.Copy(data, bytes, data.Count);
            List<byte> b = data;
            //byte[] b = new byte[data.Length];
            //Array.Copy(data, b, data.Length);

            // 将每个十六进制字符串转换为字节
            //for (int i = 0; i < hexValues.Length; i++)
            //{
            //    bytes[i] = Convert.ToByte(hexValues[i], 16);
            //    b[i] = Convert.ToByte(hexValues[i], 16);
            //}
            //数据验证
            b[b.Count() - 3] = 0x00;
            b[b.Count() - 4] = 0x00;
            //b = CRC16(b);
            if (b[b.Count() - 3] == bytes[bytes.Count() - 3]&& b[b.Count() - 4] == bytes[bytes.Count() - 4])
            {
                if(bytes[5]==0x12)//监控数据
                {
                    propertyModel.Upper_CabinTemperatureNow = Convert.ToInt16(bytes[6]);//上舱温度
                    propertyModel.BackTemperatureNow= Convert.ToInt16(bytes[7]);//背部温度
                    propertyModel.LegTemperatureNow= Convert.ToInt16(bytes[8]);//腿部温度
                    propertyModel.BackMoxibustionColumn_Height=Convert.ToInt16(bytes[9]);//背部灸柱高度
                    propertyModel.LegMoxibustionColumn_Height = Convert.ToInt16(bytes[10]);//腿部灸柱高度
                    propertyModel.BatteryLevel= Convert.ToInt16(bytes[11]);//电池电量
                    propertyModel.InfraredLamp= Convert.ToInt16(bytes[12]);//红外灯
                    propertyModel.SmokeExhaustSystem = Convert.ToInt16(bytes[13]);//排烟系统
                    propertyModel.SmokePurificationSystem=bytes[14] == 0x02 ? false : true;//净烟系统
                    propertyModel.SwingSystem=bytes[15] == 0x02 ? false :true;//摇摆系统
                    propertyModel.Hatch=bytes[16] == 0x02 ? false :true;//舱盖


                    if(propertyModel.Upper_CabinTemperatureNow >= App.PropertyModelInstance.UpperAlarmCabinTemperature)//上舱温度
                    {
                        propertyModel.IsUpperAlarm = true;
                    }
                    else
                    {
                        propertyModel.IsUpperAlarm= false;
                    }

                    if(propertyModel.BackTemperatureNow>=App.PropertyModelInstance.BackAlarmTemperature)//背部温度
                    {
                        propertyModel.IsBackAlarm = true;
                    }
                    else
                    {
                        propertyModel.IsBackAlarm = false;
                    }

                    if(propertyModel.LegTemperatureNow>=App.PropertyModelInstance.LegAlarmTemperature)//腿部温度
                    {
                        propertyModel.IsLegAlarm = true;
                    }
                    else
                    {
                        propertyModel.IsLegAlarm= false;
                    }

                    if (propertyModel.SmokePurificationSystem == false && App.PropertyModelInstance.SmokeExhaustSystem == 0)//查看是否开启排烟系统或者净烟系统
                    {
                        propertyModel.IsSmokeSystemOn = false;
                    }
                    else
                    {
                        propertyModel.IsSmokeSystemOn = true;
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
            //else
            //{
            //    PopupBoxViewModel.ShowPopupBox($"数据错误！");
            //}
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
                    Thread.Sleep(700);
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
            //Application.Current.Dispatcher.Invoke(() =>
            //{
                if (_serialPort.IsOpen)
                {
                    //_serialPort.ReadTimeout = 3000;//读取超时

                    if (_serialPort.BytesToRead > 0)
                    {
                        try
                        {
                            byte[] buffer = new byte[_serialPort.BytesToRead];
                            if (buffer.Length <= 0)
                            {
                                return;
                            }
                                
                            //_serialPort.Read(buffer, 0, buffer.Length);
                            _serialPort.Read(buffer, 0, buffer.Length);
                        //bufferLength += buffer.Length;
                        if (buffer[2] + 4 != buffer.Length)
                        {
                            return;
                        }
                        WriteLog(buffer);
                        //dataBuffer.AddRange(buffer);
                        //int index = FindFrameHeader(dataBuffer);
                        //if (index != -1)
                        //{
                        //    // 找到了帧头，提取完整的数据帧
                        //    List<byte> frameData = ExtractFrameData(dataBuffer, index);
                        //    WriteLog(frameData);
                        //    // 处理接收到的完整数据帧
                        //    OnDataReceived(frameData);
                        //    // 清除已处理的数据
                        //    dataBuffer.RemoveRange(0, index + frameData.Count);
                        //}
                        //while(TryToReadFrame(buffer,out byte[] data))
                        //{
                        //WriteLog(buffer);
                        OnDataReceived(buffer);//处理接收到的数据
                        //}
                            //OnDataReceived(buffer);//处理接收到的数据
                        }
                        catch (Exception ex)
                        {
                        //MessageBox.Show("串口数据接受失败：" + ex.Message);
                            return;
                        }
                    }
                }
                else
                {
                    MessageBox.Show($"串口未打开");
                }
            //});
            
        }

        public void SendDataByVoice(byte[] data)
        {
            if (_serialPortVoice.IsOpen)
            {
                try
                {
                    _serialPortVoice.Write(data, 0, data.Length);
                }
                catch (Exception ex)
                {
                    PopupBoxViewModel.ShowPopupBox($"语音模块指令发送失败：{ex.Message}");
                    return;
                }
            }
            else
            {
                PopupBoxViewModel.ShowPopupBox($"串口未打开");
            }
        }

        public void ReceiveDataByVoice(object sender,SerialDataReceivedEventArgs e)
        {
            if (_serialPortVoice.IsOpen)
            {
                if(_serialPortVoice.BytesToRead > 0)
                {
                    try
                    {
                        byte[] bytes = new byte[_serialPortVoice.BytesToRead];
                        if(bytes.Length <= 0)
                        {
                            return;
                        }
                        _serialPortVoice.Read(bytes, 0, _serialPortVoice.BytesToRead);

                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            OnDataReceivedByVoice(bytes);

                        });

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"串口问题：{ex.Message}");
                        return;
                    }
                }
            }
        }

        private void OnDataReceivedByVoice(byte[] datas)
        {
            bool isNeedSendData = false;
            byte[] returnDatas = new byte[6];
            returnDatas[0] = 0xAA;
            returnDatas[1] = 0x55;
            byte[] bytes = new byte[11];
            bytes[0] = 0x55;
            bytes[1] = 0xAA;
            bytes[2] = 0x07;
            bytes[3] = 0x01;
            bytes[4] = 0x10;
            switch(datas[3])
            {
                case 0x01://开舱
                    isNeedSendData = true;
                    bytes[5] = 0x0A;
                    bytes[6] = 0x01;
                    break;
                case 0x02://关舱
                    isNeedSendData = true;
                    bytes[5] = 0x0B;
                    bytes[6] = 0x01;
                    break;
                case 0x03://上一曲
                    isNeedSendData = false;
                    App.sharedPlayMusicModel.LastSong();
                    returnDatas[2] = 0x03;
                    break;
                case 0x04://下一曲
                    isNeedSendData = false;
                    App.sharedPlayMusicModel.NextSong();
                    returnDatas[2] = 0x04;
                    break;
                case 0x05://停止播放/暂停播放
                    isNeedSendData = false;
                    App.sharedPlayMusicModel.PlayOrPauseSong();
                    returnDatas[2] = 0x05;
                    break;
                case 0x06://播放音乐
                    isNeedSendData = false;
                    App.sharedPlayMusicModel.NextSong();
                    returnDatas[2] = 0x06;
                    break;
                case 0x07://继续播放
                    isNeedSendData = false;
                    App.sharedPlayMusicModel.PlayOrPauseSong();
                    returnDatas[2] = 0x07;
                    break;
                case 0x08://背部灸盘抬升
                    isNeedSendData = true;
                    bytes[5] = 0x04;
                    bytes[6] = 0x01;
                    returnDatas[2] = 0x08;
                    break;
                case 0x09://背部灸盘降低
                    isNeedSendData = true;
                    bytes[5] = 0x05;
                    bytes[6] = 0x01;
                    returnDatas[2] = 0x09;
                    break;
                case 0x0A://腿部灸盘抬升
                    isNeedSendData = true;
                    bytes[5] = 0x06;
                    bytes[6] = 0x01;
                    returnDatas[2] = 0x0A;
                    break;
                case 0x0B://腿部灸盘降低
                    isNeedSendData = true;
                    bytes[5] = 0x07;
                    bytes[6] = 0x01;
                    returnDatas[2] = 0x0B;
                    break;
            }
            if (isNeedSendData)
            {
                bytes[9] = 0x55;
                bytes[10] = 0xAA;
                bytes = CRC16(bytes);
                SendData(bytes);//发送数据到下位机
                if (datas[3] == 0x01)
                {
                    App.PropertyModelInstance.IsOpen = true;//舱门开启
                    App.PropertyModelInstance.OpenHatch = "../Resources/Pictures/HatchBtnBackSelected.png";//切换背景图片
                    timerOpen.Tick += (sender, args) =>
                    {
                        App.PropertyModelInstance.IsOpen = false;
                        App.PropertyModelInstance.OpenHatch = "../Resources/Pictures/HatchBtnBack.png";
                        byte[] b = new byte[6];
                        b[0] = 0xAA;
                        b[1] = 0x55;
                        b[2] = 0x01;
                        b[3] = 0x01;
                        b[4] = 0x55;
                        b[5] = 0xAA;
                        SendDataByVoice(b);//返回数据给语音模块

                        b[0] = 0xAA;
                        b[1] = 0x55;
                        b[2] = 0x0F;
                        b[3] = 0x01;
                        b[4] = 0x55;
                        b[5] = 0xAA;
                        SendDataByVoice(b);//返回舱门状态给语音模块
                        ((DispatcherTimer)sender).Stop();
                    };
                    timerOpen.Start();

                }
                else if (datas[3] == 0x02)
                {
                    App.PropertyModelInstance.IsClose = true;//舱门关闭
                    App.PropertyModelInstance.CloseHatch = "../Resources/Pictures/HatchBtnBackSelected.png";//切换背景图片
                    timerClose.Tick += (sender, args) =>
                    {
                        App.PropertyModelInstance.IsClose = false;
                        App.PropertyModelInstance.CloseHatch = "../Resources/Pictures/HatchBtnBack.png";
                        byte[] b = new byte[6];
                        b[0] = 0xAA;
                        b[1] = 0x55;
                        b[2] = 0x02;
                        b[3] = 0x01;
                        b[4] = 0x55;
                        b[5] = 0xAA;
                        SendDataByVoice(b);//返回数据给语音模块

                        b[0] = 0xAA;
                        b[1] = 0x55;
                        b[2] = 0x0F;
                        b[3] = 0x00;
                        b[4] = 0x55;
                        b[5] = 0xAA;
                        SendDataByVoice(b);//返回舱门状态给语音模块

                        ((DispatcherTimer)sender).Stop();
                    };
                    timerClose.Start();
                }
            }
            if (datas[3] != 0x01 && datas[3] != 0x02)
            {
                returnDatas[3] = 0x01;
                returnDatas[4] = 0x55;
                returnDatas[5] = 0xAA;
                SendDataByVoice(returnDatas);//返回数据给语音模块
            }
        }

        private bool TryToReadFrame(byte[] buffer,out byte[] data)
        {
            data = null;
            if (bufferLength < 2) return false;

            int startIndex = -1;
            for(int i=0;i< bufferLength; i++)
            {
                if (buffer[i] == 0x55 && buffer[i + 1] == 0xAA)
                {
                    startIndex = i;
                    break;
                }
                
            }

            if (startIndex == -1)
            {
                return false;
            }

            int endIndex = -1;
            for(int i = startIndex + 2; i < bufferLength; i++)
            {
                if (buffer[i] == 0x55 && buffer[i+1]==0xAA)
                {
                    endIndex = i; 
                    break;
                }
            }
            if (endIndex == -1)
            {
                return false;
            }

            data = new byte[endIndex - startIndex + 2];
            Array.Copy(buffer, startIndex, data, 0, endIndex - startIndex + 2);

            int bytesToMove = bufferLength - endIndex - 2;
            if(bytesToMove > 0)
            {
                Array.Copy(buffer, endIndex + 2, buffer, 0, bytesToMove);
            }
            bufferLength = bytesToMove;
            return true;
        }

        private void WriteLog(byte[] data)
        {
            string txt = "ReceivedData(" + DateTime.Now.ToString("HH:mm:ss") + "):   ";
            foreach(byte b in data)
            {
                txt += b.ToString();
                txt += " ";
            }
            txt += "\r\n";
            File.AppendAllText("ReceiveLog.txt", txt);
        }

        private void WriteLog(List<byte> data)
        {
            string txt="ReceivedData处理后:  ";
            if (data != null)
            {
                for (int i = 0; i < data.Count; i++)
                {
                    txt += data[i].ToString();
                    txt += " ";
                }
                txt += "\r\n";
            }
            File.AppendAllText("ReceiveLog.txt", txt);

        }

        private int FindFrameHeader(List<byte> buffer)
        {
            for (int i = 0; i < buffer.Count - frameHeader.Length + 1; i++)
            {
                bool found = true;
                for (int j = 0; j < frameHeader.Length; j++)
                {
                    if (buffer[i + j] != frameHeader[j])
                    {
                        found = false;
                        break;
                    }
                }
                if (found) return i;
            }
            return -1;
        }

        private List<byte> ExtractFrameData(List<byte> buffer, int startIndex)
        {
            // 假设数据帧长度为10字节（不包括帧头），根据实际情况修改
            int frameLength = buffer[startIndex+2] +4;
            if (frameLength > buffer.Count)
            {
                return null;
            }
            List<byte> frameData = buffer.GetRange(startIndex, frameLength);
            return frameData;
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
            for (int n = 2; n < len - 4; n++)
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
        //public static List<byte> CRC16(byte[] bytes)
        //{
        //    //计算并填写CRC校验码
        //    int crc = 0xffff;
        //    int len = bytes.Length;
        //    for (int n = 2; n < len-2; n++)
        //    {
        //        byte i;
        //        crc ^= bytes[n];
        //        for (i = 0; i < 8; i++)
        //        {
        //            int TT;
        //            TT = crc & 1;
        //            crc = crc >> 1;
        //            crc = crc & 0x7fff;
        //            if (TT == 1)
        //            {
        //                crc = crc ^ 0xa001;
        //            }
        //            crc = crc & 0xffff;
        //        }

        //    }
        //    //var nl = bytes.Length + 2;
        //    //生成的两位校验码
        //    byte[] redata = new byte[2];
        //    redata[0] = (byte)((crc & 0xff));
        //    redata[1] = (byte)((crc >> 8) & 0xff);

        //    //重新组织字节数组
        //    var newByte = new byte[len];
        //    for (int i = 0; i < bytes.Length; i++)
        //    {
        //        newByte[i] = bytes[i];
        //    }
        //    newByte[len - 4] = (byte)redata[0];
        //    newByte[len - 3] = redata[1];
        //    // HelperTypeConversion.concat(bytes, newByte)
        //    return newByte;
        //}

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
