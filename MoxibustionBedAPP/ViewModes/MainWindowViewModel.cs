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
        #endregion

        public MainWindowViewModel()
        {
            //PlayMusicViewModel.playMusic.ReadFileNamesFromFolder(@".\Resources\Music");
            //ReadFileNamesFromFolder(@".\Resources\Music");
            //SerialPortManager.Instance.SendData(new byte[] { 0x55, 0xaa, 0x11 });
            //SerialPortManager.Instance.ReceiveData();
            //SerialPortManager.Instance.ReceiveData();
            //SerialPortManager.Instance.ReceiveData();
            //SerialPortManager.Instance.ReceiveData();
            //SerialPortManager.Instance.ReceiveData();
            CurrentUserControl = new FunctionControlView();//初始界面为功能控制界面
            PlayMusicView= new PlayMusicView();//定义音乐播放界面
            FunctionControlView = new FunctionControlView();//定义功能控制界面
            DataMonitoringView= new DataMonitoringView();//定义数据监控界面
            ParameterSettingView= new ParameterSettingView();//定义参数设置界面
            
            //显示时间，每隔一秒刷新
            UpdateProgressTimer.Elapsed += (sender, e) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    CurrentTime = DateTime.Now.ToString();
                });
            };
            UpdateProgressTimer.Start();
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
                });
            }
        }
        #endregion

    }
}
