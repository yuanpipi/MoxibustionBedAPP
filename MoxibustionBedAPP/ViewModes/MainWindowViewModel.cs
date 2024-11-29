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
using MoxibustionBedAPP.Models;
using MoxibustionBedAPP.Properties;
using MoxibustionBedAPP.Views;

namespace MoxibustionBedAPP.ViewModes
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private PlayMusicView PlayMusicView { get; set; }
        private FunctionControlView FunctionControlView { get; set; }
        private DataMonitoringView DataMonitoringView {  get; set; }
        private ParameterSettingView ParameterSettingView {  get; set; }

        public PlayMusicViewModel SharedVM { get; set; }
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
            CurrentUserControl = new FunctionControlView();
            //SharedVM = new PlayMusicViewModel();
            PlayMusicView= new PlayMusicView();
            FunctionControlView = new FunctionControlView();
            DataMonitoringView= new DataMonitoringView();
            ParameterSettingView= new ParameterSettingView();
        }

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

    }
}
