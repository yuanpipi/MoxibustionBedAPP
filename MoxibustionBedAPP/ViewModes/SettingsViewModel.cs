using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using MoxibustionBedAPP.Models;

namespace MoxibustionBedAPP.ViewModes
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ICommand ReturnClick { get; private set; }

        public ICommand AutoMusicCommand { get; private set; }

        public ICommand ShowDetail { get; private set; }

        private Visibility _isShow;
        public Visibility IsShow
        {
            get { return _isShow; }
            set
            {
                _isShow = value;
                OnPropertyChanged("IsShow");
            }
        }

        public SettingsViewModel()
        {
            ReturnClick = new RelayCommand(ReturnClickCommand);
            AutoMusicCommand = new RelayCommand(AutoMusicMethod);
            ShowDetail = new RelayCommand(ShowDetailMethod);
            IsShow = Visibility.Hidden;
        }

        private void ReturnClickCommand()
        {
            App.PropertyModelInstance.IsClickS = !App.PropertyModelInstance.IsClickS;
        }

        /// <summary>
        /// 点火后自动跳到音乐播放界面
        /// </summary>
        /// <param name="title"></param>
        private void AutoMusicMethod(object title)
        {
            switch (title)
            {
                case "Yes":
                    App.PropertyModelInstance.AutoMusic = true;
                    break;
                case "No":
                    App.PropertyModelInstance.AutoMusic = false;
                    break;
            }
        }

        private void ShowDetailMethod()
        {
            if (IsShow == Visibility.Visible)
            {
                IsShow = Visibility.Hidden;
            }
            else
            {
                IsShow = Visibility.Visible;
            }
        }
    }
}
