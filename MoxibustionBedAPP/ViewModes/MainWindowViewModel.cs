using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using MoxibustionBedAPP.Models;
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

        //public enum UserControlType
        //{
        //    FunctionControl,
        //    DataMonitoring,
        //    PlayMusic
        //}

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

        public ICommand ShowPlayMusicCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    CurrentUserControl = new PlayMusicView();
                });
            }
        }

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
