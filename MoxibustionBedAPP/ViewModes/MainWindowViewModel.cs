using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using MoxibustionBedAPP.Models;
using MoxibustionBedAPP.Properties;
using MoxibustionBedAPP.Views;

namespace MoxibustionBedAPP.ViewModes
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        #region 自定义变量
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        /// <summary>
        /// 音乐播放界面
        /// </summary>
        private PlayMusicView PlayMusicView { get; set; }
        /// <summary>
        /// 功能控制界面
        /// </summary>
        private FunctionControlView FunctionControlView { get; set; }
        /// <summary>
        /// 数据监控界面
        /// </summary>
        private DataMonitoringView DataMonitoringView {  get; set; }
        /// <summary>
        /// 参数设置界面
        /// </summary>
        private ParameterSettingView ParameterSettingView {  get; set; }
        /// <summary>
        /// 音乐控制模型
        /// </summary>
        public PlayMusicViewModel SharedVM { get; set; }
        /// <summary>
        /// 自定义控件
        /// </summary>
        private UserControl _currentUserControl;
        public UserControl CurrentUserControl
        {
            get
            {
                return _currentUserControl;
            }
            set
            {
                _currentUserControl = value;
                OnPropertyChanged(nameof(CurrentUserControl));
            }
        }

        /// <summary>
        /// 当前时间
        /// </summary>
        private string currentTime;
        public string CurrentTime
        {
            get
            {
                return currentTime;
            }
            set
            {
                currentTime = value;
                OnPropertyChanged(nameof(CurrentTime));
            }
        }
        /// <summary>
        /// 定时器用于更新时间
        /// </summary>
        private System.Timers.Timer UpdateProgressTimer = new System.Timers.Timer(500);

        private bool _isLoading;
        public bool IsLoading
        {
            get
            {
                return _isLoading;
            }
            set
            {
                _isLoading = value;
                OnPropertyChanged(nameof(IsLoading));
            }
        }

        //public ICommand LoadCompleteCommand { get; set; }

        /// <summary>
        /// 按钮一背景图
        /// </summary>
        private string _btnBack1;
        public string BtnBack1
        {
            get
            {
                return _btnBack1;
            }
            set
            {
                _btnBack1 = value;
                OnPropertyChanged(nameof(BtnBack1));
            }
        }

        /// <summary>
        /// 按钮二背景图
        /// </summary>
        private string _btnBack2;
        public string BtnBack2
        {
            get
            {
                return _btnBack2;
            }
            set
            {
                _btnBack2 = value;
                OnPropertyChanged(nameof(BtnBack2));
            }
        }

        /// <summary>
        /// 按钮三背景图
        /// </summary>
        private string _btnBack3;
        public string BtnBack3
        {
            get
            {
                return _btnBack3;
            }
            set
            {
                _btnBack3 = value;
                OnPropertyChanged(nameof(BtnBack3));
            }
        }

        /// <summary>
        /// 按钮四背景图
        /// </summary>
        private string _btnBack4;
        public string BtnBack4
        {
            get
            {
                return _btnBack4;
            }
            set
            {
                _btnBack4 = value;
                OnPropertyChanged(nameof(BtnBack4));
            }
        }

        /// <summary>
        /// 停止点火
        /// </summary>
        public ICommand StopInignitionCommon {  get; set; }

        /// <summary>
        /// 停止预热
        /// </summary>
        public ICommand StopPreheadCommon {  get; set; }



        public ICommand QuesClick { get;private set; }
        public ICommand SetClick { get;private set; }

        private static int c = 0;
        #endregion

        public MainWindowViewModel()
        {
            
            IsLoading = true;
            CurrentUserControl = new FunctionControlView();//初始界面为功能控制界面
            PlayMusicView = new PlayMusicView();//定义音乐播放界面
            FunctionControlView = new FunctionControlView();//定义功能控制界面
            DataMonitoringView = new DataMonitoringView();//定义数据监控界面
            ParameterSettingView = new ParameterSettingView();//定义参数设置界面
            StopInignitionCommon = new RelayCommand(StopInignitionMethod);
            StopPreheadCommon = new RelayCommand(StopPreheadMethod);
            QuesClick = new RelayCommand(QuesClickCommand);
            SetClick = new RelayCommand(SetClickCommand);
            App.PropertyModelInstance.IsClickQ = false;
            App.PropertyModelInstance.IsClickS = false;
            BtnBack1 = "../Resources/Pictures/BtnStyleSelected.png";
            BtnBack2 = "../Resources/Pictures/BtnStyleUnselect.png";
            BtnBack3 = "../Resources/Pictures/BtnStyleUnselect.png";
            BtnBack4 = "../Resources/Pictures/BtnStyleUnselect.png";
            try
            {
                //显示时间，每隔一秒刷新
                UpdateProgressTimer.Elapsed += (sender, e) =>
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        CurrentTime = DateTime.Now.ToString("yyyy/MM/dd HH：mm：ss");
                    });
                };
                UpdateProgressTimer.Start();

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #region 自定义方法
        /// <summary>
        /// 显示功能控制界面
        /// </summary>
        public ICommand ShowFunctionControlCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    CurrentUserControl = new FunctionControlView();
                    BtnBack1 = "../Resources/Pictures/BtnStyleSelected.png";
                    BtnBack2 = "../Resources/Pictures/BtnStyleUnselect.png";
                    BtnBack3 = "../Resources/Pictures/BtnStyleUnselect.png";
                    BtnBack4 = "../Resources/Pictures/BtnStyleUnselect.png";
                });
            }
        }

        /// <summary>
        /// 显示数据监控界面
        /// </summary>
        public ICommand ShowDataMonitoringCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    CurrentUserControl = new DataMonitoringView();
                    BtnBack1 = "../Resources/Pictures/BtnStyleUnselect.png";
                    BtnBack2 = "../Resources/Pictures/BtnStyleUnselect.png";
                    BtnBack3 = "../Resources/Pictures/BtnStyleSelected.png";
                    BtnBack4 = "../Resources/Pictures/BtnStyleUnselect.png";
                });
            }
        }

        /// <summary>
        /// 显示音乐播放界面
        /// </summary>
        public ICommand ShowPlayMusicCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    CurrentUserControl = PlayMusicView;
                    BtnBack1 = "../Resources/Pictures/BtnStyleUnselect.png";
                    BtnBack2 = "../Resources/Pictures/BtnStyleUnselect.png";
                    BtnBack3 = "../Resources/Pictures/BtnStyleUnselect.png";
                    BtnBack4 = "../Resources/Pictures/BtnStyleSelected.png";
                });
            }
        }

        /// <summary>
        /// 显示参数设置界面
        /// </summary>
        public ICommand ShowParameterSettingCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    CurrentUserControl = new ParameterSettingView();
                    BtnBack1 = "../Resources/Pictures/BtnStyleUnselect.png";
                    BtnBack2 = "../Resources/Pictures/BtnStyleSelected.png";
                    BtnBack3 = "../Resources/Pictures/BtnStyleUnselect.png";
                    BtnBack4 = "../Resources/Pictures/BtnStyleUnselect.png";
                });
            }
        }

        private void QuesClickCommand()
        {
            App.PropertyModelInstance.IsClickQ = !App.PropertyModelInstance.IsClickQ;
            App.PropertyModelInstance.IsClickS = false;
        }

        private void SetClickCommand()
        {
            App.PropertyModelInstance.IsClickS = !App.PropertyModelInstance.IsClickS;
            App.PropertyModelInstance.IsClickQ = false;
        }



        /// <summary>
        /// 显示测试功能界面
        /// </summary>
        public ICommand ShowTestWindowCommand
        {

            get
            {
                return new RelayCommand(() =>
                {
                    c++;
                    if (c == 3)
                    {
                        c = 0;
                        App.Test = new TestWindowView();

                        App.Test.Show();
                    }
                });
            }
        }



        /// <summary>
        /// 预热选择
        /// </summary>
        private void StopPreheadMethod()
        {
            byte[] data = new byte[11];
            data[0] = 0x55;
            data[1] = 0xAA;
            data[2] = 0x07;
            data[3] = 0x01;
            data[4] = 0x10;
            data[5] = 0x02;
            data[6] = 0x02;
            data[9] = 0x55;
            data[10] = 0xAA;
            data = SerialPortManager.CRC16(data);
            SerialPortManager.Instance.SendData(data);
            App.PropertyModelInstance.PreheadMode = false;
            FunctionControlViewModel._timer.Stop();
            FunctionControlViewModel._isCountingDown = false;
        }

        /// <summary>
        /// 点火选择
        /// </summary>
        private void StopInignitionMethod()
        {
            byte[] data = new byte[11];


            data[0] = 0x55;
            data[1] = 0xAA;
            data[2] = 0x07;
            data[3] = 0x01;
            data[4] = 0x10;
            data[5] = 0x03;
            data[6] = 0x02;
            data[9] = 0x55;
            data[10] = 0xAA;
            data = SerialPortManager.CRC16(data);
            SerialPortManager.Instance.SendData(data);
            App.PropertyModelInstance.InignitionStatus = false;
            FunctionControlViewModel._timer.Stop();
            FunctionControlViewModel._isCountingDown = false;
        }

        #endregion

    }
}
