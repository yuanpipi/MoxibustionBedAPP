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
using Newtonsoft.Json.Linq;
using System.IO;
using System.Windows.Markup.Localizer;
using System.Windows.Automation.Peers;
using System.Diagnostics;
using System.Windows.Ink;

namespace MoxibustionBedAPP.Models
{
    public class SerialPortManager : IDisposable
    {
        #region 自定义变脸
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

        //public PropertyModel propertyModel = new PropertyModel();
        //public static DispatcherTimer _timer;

        /// <summary>
        /// 下位机相关串口连接
        /// </summary>
        private SerialPort _serialPort;
        private CancellationTokenSource _cancellationTokenSource;
        private Stopwatch _stopWatch;

        /// <summary>
        /// 语音控制模块相关串口连接
        /// </summary>
        private SerialPort _serialPortVoice;
        private CancellationTokenSource _cancellationTokenSourceVoice;
        private bool _isDisposed = false;

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

        /// <summary>
        /// 语音倒计时
        /// </summary>
        public DispatcherTimer timerOfVoice = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(30)
        };

        public COMFailMessageBox comfail = new COMFailMessageBox();

        private bool isOpenMotherboardCOM = false;
        private bool isOpenAICOM = false;
        #endregion

        private SerialPortManager()
        {
            _serialPort = new SerialPort()
            {
                PortName = App.PropertyModelInstance.MotherboardCOM,
                BaudRate = 115200,
                Parity = Parity.None,
                DataBits = 8,
                StopBits = StopBits.One,
                ReadBufferSize = 4096
            };
            _cancellationTokenSource = new CancellationTokenSource();

            _serialPortVoice = new SerialPort()
            {
                PortName = App.PropertyModelInstance.AICOM,
                BaudRate = 115200,
                Parity = Parity.None,
                DataBits = 8,
                StopBits = StopBits.One
            };
            _cancellationTokenSourceVoice = new CancellationTokenSource();
            _stopWatch = new Stopwatch();
            //StartCountdown();
        }

        //private void StartCountdown()
        //{
        //    _timer = new DispatcherTimer();
        //    _timer.Interval = TimeSpan.FromSeconds(1);
        //    _timer.Tick += Timer_Tick;
        //    _timer.Start();
        //}
        //private void Timer_Tick(object sender, EventArgs e)
        //{
        //    App.PropertyModelInstance.Upper_CabinTemperatureNow = propertyModel.Upper_CabinTemperatureNow;//上舱温度
        //    App.PropertyModelInstance.BackTemperatureNow = propertyModel.BackTemperatureNow;//背部温度
        //    App.PropertyModelInstance.LegTemperatureNow = propertyModel.LegTemperatureNow;//腿部温度
        //    App.PropertyModelInstance.BackMoxibustionColumn_Height = propertyModel.BackMoxibustionColumn_Height;//背部灸柱高度
        //    App.PropertyModelInstance.LegMoxibustionColumn_Height = propertyModel.LegMoxibustionColumn_Height;//腿部灸柱高度
        //    App.PropertyModelInstance.BatteryLevel = propertyModel.BatteryLevel;//电池电量
        //    App.PropertyModelInstance.InfraredLamp = propertyModel.InfraredLamp;//红外灯
        //    App.PropertyModelInstance.SmokeExhaustSystem = propertyModel.SmokeExhaustSystem;//排烟系统
        //    App.PropertyModelInstance.SmokePurificationSystem = propertyModel.SmokePurificationSystem;//净烟系统
        //    App.PropertyModelInstance.SwingSystem = propertyModel.SwingSystem;//摇摆系统
        //    App.PropertyModelInstance.Hatch = propertyModel.Hatch;//舱盖
        //    App.PropertyModelInstance.IsUpperAlarm = propertyModel.IsUpperAlarm;
        //    App.PropertyModelInstance.IsBackAlarm = propertyModel.IsBackAlarm;
        //    App.PropertyModelInstance.IsLegAlarm = propertyModel.IsLegAlarm;
        //    App.PropertyModelInstance.IsSmokeSystemOn = propertyModel.IsSmokeSystemOn;
        //}

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
                    _stopWatch.Start();
                    isOpenMotherboardCOM = true;
                }
                catch (Exception ex)
                {
                    //MessageBox.Show($"打开串口失败: {ex.Message}");
                    //PopupBoxViewModel.ShowPopupBox($"串口打开失败：{ex.Message}");
                    //COMFailMessageBox comfail = new COMFailMessageBox();


                    //comfail.ShowDialog();

                    ////自动重连逻辑
                    //_serialPort.PortName = App.PropertyModelInstance.MotherboardCOM;
                    //_serialPort.Open();//打开串口
                    //_serialPort.WriteTimeout = 3000;
                    //_serialPort.ReadTimeout = 3000;
                    //byte[] data = { 0x55, 0xAA, 0x0F, 0x01, 0x00, 0x00, 0x01, 0xAA, 0X5C };
                    //_serialPort.Write(data, 0, data.Length);
                    //Thread.Sleep(1500);
                    //byte[] buffer = new byte[_serialPort.BytesToRead];
                    //_serialPort.Read(buffer, 0, buffer.Length);

                    isOpenMotherboardCOM = false;
                }
            }
            if (_serialPortVoice != null && !_serialPortVoice.IsOpen)
            {
                try
                {
                    _serialPortVoice.Open();
                    Console.WriteLine("语音串口已打开");
                    _serialPortVoice.DataReceived += new SerialDataReceivedEventHandler(ReceiveDataByVoice);
                    isOpenAICOM = true;
                }
                catch (Exception e)
                {
                    //Console.WriteLine($"串口打开失败:{e.Message}");

                    ////自动重连逻辑
                    //return;
                    isOpenAICOM = false;
                    //isOpenAICOM = true;
                }
            }


            while(!isOpenMotherboardCOM || !isOpenAICOM)
            {
                if (!isOpenMotherboardCOM && !isOpenAICOM)//主板串口和语音助手串口均为打开
                {
                    MessageBox.Show("主板串口和语音助手串口错误，请重新设置");
                    comfail.ShowDialog();

                    //主板自动重连逻辑
                    _serialPort.PortName = App.PropertyModelInstance.MotherboardCOM;
                    try
                    {
                        _serialPort.Open();//打开串口
                        _serialPort.WriteTimeout = 3000;
                        _serialPort.ReadTimeout = 3000;
                        byte[] data = { 0x55, 0xAA, 0x0F, 0x01, 0x00, 0x00, 0x01, 0xAA, 0X5C };
                        _serialPort.Write(data, 0, data.Length);
                        Thread.Sleep(1500);
                        byte[] buffer = new byte[_serialPort.BytesToRead];
                        _serialPort.Read(buffer, 0, buffer.Length);
                        if (buffer.Length > 0)
                        {
                            isOpenMotherboardCOM = true;
                            _serialPort.DataReceived += new SerialDataReceivedEventHandler(ReceiveData);
                            _stopWatch.Start();
                        }
                        else
                        {
                            isOpenMotherboardCOM = false;
                        }
                    }
                    catch
                    {
                        isOpenMotherboardCOM = false;
                        //isOpenMotherboardCOM = true;
                    }


                    //语音助手自动重连逻辑
                    _serialPortVoice.PortName = App.PropertyModelInstance.AICOM;
                    try
                    {
                        _serialPortVoice.Open();//打开串口
                        _serialPortVoice.WriteTimeout = 3000;
                        _serialPortVoice.ReadTimeout = 3000;
                        byte[] data1 = { 0xAA, 0x55, 0x10, 0x55, 0XAA };
                        _serialPortVoice.Write(data1, 0, data1.Length);
                        Thread.Sleep(1500);
                        byte[] buffer1 = new byte[_serialPort.BytesToRead];
                        _serialPortVoice.Read(buffer1, 0, buffer1.Length);
                        if (buffer1.Length > 0)
                        {
                            isOpenAICOM = true;
                            _serialPortVoice.DataReceived += new SerialDataReceivedEventHandler(ReceiveDataByVoice);
                        }
                        else
                        {
                            isOpenAICOM = false;
                        }
                    }
                    catch
                    {
                        isOpenAICOM = false;
                    }
                }
                else if (!isOpenMotherboardCOM && isOpenAICOM)//主板串口未打开，语音串口已打开
                {
                    MessageBox.Show("主板串口错误，请重新设置");
                    comfail.ShowDialog();

                    //主板串口自动重连逻辑
                    _serialPort.PortName = App.PropertyModelInstance.MotherboardCOM;
                    try
                    {
                        _serialPort.Open();//打开串口
                        _serialPort.WriteTimeout = 3000;
                        _serialPort.ReadTimeout = 3000;
                        byte[] data = { 0x55, 0xAA, 0x0F, 0x01, 0x00, 0x00, 0x01, 0xAA, 0X5C };
                        _serialPort.Write(data, 0, data.Length);
                        Thread.Sleep(1500);
                        byte[] buffer = new byte[_serialPort.BytesToRead];
                        _serialPort.Read(buffer, 0, buffer.Length);
                        if (buffer.Length > 0)
                        {
                            isOpenMotherboardCOM = true;
                            _serialPort.DataReceived += new SerialDataReceivedEventHandler(ReceiveData);
                            _stopWatch.Start();
                        }
                        else
                        {
                            isOpenMotherboardCOM = false;
                        }
                    }
                    catch
                    {
                        isOpenMotherboardCOM = false;
                        //isOpenMotherboardCOM = true;
                    }
                }
                else if (isOpenMotherboardCOM && !isOpenAICOM)//主板串口已打开，语音串口未打开
                {
                    MessageBox.Show("语音助手串口错误，请重新设置");
                    comfail.ShowDialog();

                    //语音助手串口自动重连逻辑
                    _serialPortVoice.PortName = App.PropertyModelInstance.AICOM;
                    try
                    {
                        _serialPortVoice.Open();//打开串口
                        _serialPortVoice.WriteTimeout = 3000;
                        _serialPortVoice.ReadTimeout = 3000;
                        byte[] data = { 0xAA, 0x55, 0x10, 0x55, 0XAA };
                        _serialPortVoice.Write(data, 0, data.Length);
                        Thread.Sleep(1500);
                        byte[] buffer = new byte[_serialPort.BytesToRead];
                        _serialPortVoice.Read(buffer, 0, buffer.Length);
                        if (buffer.Length > 0)
                        {
                            isOpenAICOM = true;
                            _serialPortVoice.DataReceived += new SerialDataReceivedEventHandler(ReceiveDataByVoice);
                        }
                        else
                        {
                            isOpenAICOM = false;
                        }
                    }
                    catch
                    {
                        isOpenAICOM = false;
                    }
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
                _serialPortVoice.Close();
                Console.WriteLine("串口已关闭");
                _cancellationTokenSource.Cancel();
                _cancellationTokenSourceVoice.Cancel();
                _stopWatch.Stop();
            }
        }

        /// <summary>
        /// 处理接收到的数据
        /// </summary>
        /// <param name="data"></param>
        protected void OnDataReceived(byte[] data)
        {
            byte[] bytes=new byte[data.Length];
            Array.Copy(data, bytes, data.Length);
            byte[] b = new byte[data.Length];
            Array.Copy(data, b, data.Length);

            //数据验证
            b[b.Count() - 3] = 0x00;
            b[b.Count() - 4] = 0x00;
            b = CRC16(b);
            if (b[b.Count() - 3] == bytes[bytes.Count() - 3]&& b[b.Count() - 4] == bytes[bytes.Count() - 4])
            {
                if (bytes[4]==0x12)
                {
                    App.PropertyModelInstance.CurrentUserControl= MainWindowViewModel.FunctionControlView;
                    switch (bytes[5])
                    {
                        case 0x00://紧急开舱
                            if (bytes[6] == 0x01)
                            {
                                App.PropertyModelInstance.IsOpen = true;
                                App.PropertyModelInstance.OpenHatch = "pack://application:,,,/Resources/Pictures/HatchBtnBackSelected.png";//切换背景图片
                                timerOpen.Start();
                            }
                            else if(bytes[6] == 0x02)
                            {
                                App.PropertyModelInstance.IsOpen = false;
                                App.PropertyModelInstance.OpenHatch = "pack://application:,,,/Resources/Pictures/HatchBtnBack.png";//切换背景图片
                                timerOpen.Stop();
                            }
                            break;
                        case 0x01://开舱
                            if (bytes[6] == 0x01)
                            {
                                App.PropertyModelInstance.IsOpen = true;
                                App.PropertyModelInstance.OpenHatch = "pack://application:,,,/Resources/Pictures/HatchBtnBackSelected.png";//切换背景图片
                                timerOpen.Start();
                            }
                            else if (bytes[6] == 0x02)
                            {
                                App.PropertyModelInstance.IsOpen = false;
                                App.PropertyModelInstance.OpenHatch = "pack://application:,,,/Resources/Pictures/HatchBtnBack.png";//切换背景图片
                                timerOpen.Stop();
                            }
                            break;
                        case 0x02://关舱
                            if (bytes[6] == 0x01)
                            {
                                App.PropertyModelInstance.IsClose = true;
                                App.PropertyModelInstance.CloseHatch = "pack://application:,,,/Resources/Pictures/HatchBtnBackSelected.png"; ;//切换背景图片
                                timerClose.Start();
                            }
                            else if (bytes[6] == 0x02)
                            {
                                App.PropertyModelInstance.IsClose = false;
                                App.PropertyModelInstance.CloseHatch = "pack://application:,,,/Resources/Pictures/HatchBtnBack.png";//切换背景图片
                                timerClose.Stop();
                            }
                            break;
                        case 0x03://点火
                            if (bytes[6] == 0x01)
                            {
                                App.PropertyModelInstance.InignitionStatus = true;
                                App.PropertyModelInstance.CountdownSeconds = App.PropertyModelInstance.InignitionTime;
                                App.PropertyModelInstance.CountdownMinutes = 0;
                                FunctionControlViewModel.seconds = App.PropertyModelInstance.CountdownSeconds + App.PropertyModelInstance.CountdownMinutes * 60;
                                FunctionControlViewModel.StartTime = DateTime.Now;
                                FunctionControlViewModel._timer.Start();
                            }
                            else if (bytes[6] == 0x02)
                            {
                                App.PropertyModelInstance.InignitionStatus = false;
                                FunctionControlViewModel._timer.Stop();
                            }
                            break;
                        case 0x04://摇摆
                            if (bytes[6] == 0x01)
                            {
                                App.PropertyModelInstance.SmokePurificationSystem = true;
                            }
                            else if (bytes[6] == 0x02)
                            {
                                App.PropertyModelInstance.SmokePurificationSystem = false;
                            }
                            break;
                        case 0x05://排烟
                            if (bytes[6] == 0x01)
                            {
                                App.PropertyModelInstance.IsSmokeSystemOn = true;
                                App.PropertyModelInstance.SmokeExhaustSystem = 3;
                            }
                            else if (bytes[6] == 0x02)
                            {
                                App.PropertyModelInstance.IsSmokeSystemOn = false;
                                App.PropertyModelInstance.SmokeExhaustSystem = 0;
                            }
                            break;
                        case 0x06://背部点升
                            if(App.PropertyModelInstance.BackMoxibustionColumn_Height < 3)
                            {
                                App.PropertyModelInstance.BackMoxibustionColumn_Height++;
                            }
                            break;
                        case 0x07://背部点降
                            if (App.PropertyModelInstance.BackMoxibustionColumn_Height > 1)
                            {
                                App.PropertyModelInstance.BackMoxibustionColumn_Height--;
                            }
                            break;
                        case 0x08://腿部点升
                            if (App.PropertyModelInstance.LegMoxibustionColumn_Height < 3)
                            {
                                App.PropertyModelInstance.LegMoxibustionColumn_Height++;
                            }                            
                            break;
                        case 0x09://腿部点降
                            if (App.PropertyModelInstance.LegMoxibustionColumn_Height > 1)
                            {
                                App.PropertyModelInstance.LegMoxibustionColumn_Height--;
                            }
                            break;
                    }
                }
                else
                {
                    if (bytes[5] == 0x12)//监控数据
                    {
                        App.PropertyModelInstance.Upper_CabinTemperatureNow = Convert.ToInt16(bytes[6]);//上舱温度
                        App.PropertyModelInstance.BackTemperatureNow = Convert.ToInt16(bytes[7]);//背部温度
                        App.PropertyModelInstance.LegTemperatureNow = Convert.ToInt16(bytes[8]);//腿部温度
                        App.PropertyModelInstance.BackMoxibustionColumn_Height = Convert.ToInt16(bytes[9]);//背部灸柱高度
                        App.PropertyModelInstance.LegMoxibustionColumn_Height = Convert.ToInt16(bytes[10]);//腿部灸柱高度
                        App.PropertyModelInstance.BatteryLevel = Convert.ToInt16(bytes[11]);//电池电量
                        App.PropertyModelInstance.InfraredLamp = Convert.ToInt16(bytes[12]);//红外灯
                        App.PropertyModelInstance.SmokeExhaustSystem = Convert.ToInt16(bytes[13]);//排烟系统
                        App.PropertyModelInstance.SmokePurificationSystem = bytes[14] == 0x00 ? false : true;//净烟系统
                        App.PropertyModelInstance.SwingSystem = bytes[15] == 0x00 ? false : true;//摇摆系统
                        //App.PropertyModelInstance.Hatch = bytes[16] == 0x00 ? false : true;//舱盖


                        if (App.PropertyModelInstance.Upper_CabinTemperatureNow >= App.PropertyModelInstance.UpperAlarmCabinTemperature)//上舱温度
                        {
                            App.PropertyModelInstance.IsUpperAlarm = true;
                        }
                        else
                        {
                            App.PropertyModelInstance.IsUpperAlarm = false;
                        }

                        if (App.PropertyModelInstance.BackTemperatureNow >= App.PropertyModelInstance.BackAlarmTemperature)//背部温度
                        {
                            App.PropertyModelInstance.IsBackAlarm = true;
                        }
                        else
                        {
                            App.PropertyModelInstance.IsBackAlarm = false;
                        }

                        if (App.PropertyModelInstance.LegTemperatureNow >= App.PropertyModelInstance.LegAlarmTemperature)//腿部温度
                        {
                            App.PropertyModelInstance.IsLegAlarm = true;
                        }
                        else
                        {
                            App.PropertyModelInstance.IsLegAlarm = false;
                        }

                        if (App.PropertyModelInstance.SmokePurificationSystem == false && App.PropertyModelInstance.SmokeExhaustSystem == 0)//查看是否开启排烟系统或者净烟系统
                        {
                            App.PropertyModelInstance.IsSmokeSystemOn = false;
                        }
                        else
                        {
                            App.PropertyModelInstance.IsSmokeSystemOn = true;
                        }
                    }
                    else
                    {
                        App.IsReceive = true;
                        if (bytes[6] == 0x01)
                        {
                            Console.WriteLine("操作成功");
                        }
                        else if (bytes[6] == 0x02)
                        {
                            PopupBoxViewModel.ShowPopupBox("操作失败，请重新操作");
                        }
                    }
                }
                
            }

            long timestamp = _stopWatch.ElapsedMilliseconds;
            Console.WriteLine($"[{timestamp} ms] Data processed.");
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
        /// 接受下位机数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ReceiveData(object sender, SerialDataReceivedEventArgs e)
        {
            if (_serialPort.IsOpen)
            {
                Thread.Sleep(30);

                if (_serialPort.BytesToRead > 0)
                {
                    int bytesToRead = _serialPort.BytesToRead;
                    try
                    {
                        byte[] buffer = new byte[_serialPort.BytesToRead];

                        _serialPort.Read(buffer, 0, buffer.Length);

                        long timestamp = _stopWatch.ElapsedMilliseconds;
                        Console.WriteLine($"[{timestamp} ms] Received {bytesToRead} bytes.");

                        if (buffer[2] + 4 != buffer.Length)
                        {
                            return;
                        }
                        Task.Run(() => OnDataReceived(buffer));
                    }
                    catch (Exception ex)
                    {
                        return;
                    }
                }
            }
            else
            {
                MessageBox.Show($"串口未打开");
            }
        }

        /// <summary>
        /// 发送数据给语音模块
        /// </summary>
        /// <param name="data"></param>
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

        /// <summary>
        /// 接收来自语音模块发送的数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ReceiveDataByVoice(object sender,SerialDataReceivedEventArgs e)
        {
            App.PropertyModelInstance.IsOnVoice = true;
            if (_serialPortVoice.IsOpen)
            {
                Thread.Sleep(30);
                if (_serialPortVoice.BytesToRead > 0)
                {
                    try
                    {
                        byte[] bytes = new byte[_serialPortVoice.BytesToRead];
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
            timerOfVoice.Tick += (sender1, args) =>
            {
                App.PropertyModelInstance.IsOnVoice = false;
                ((DispatcherTimer)sender1).Stop();
            };
            timerOfVoice.Start();
        }

        /// <summary>
        /// 处理语音模块发送过来的数据
        /// </summary>
        /// <param name="datas"></param>
        private async void OnDataReceivedByVoice(byte[] datas)
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
                bytes[9] = 0xAA;
                bytes[10] = 0x5C;
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
            await Task.Yield();
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
            newByte[len - 3] = (byte)redata[0];
            newByte[len - 4] = (byte)redata[1];
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
