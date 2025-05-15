using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using MoxibustionBedAPP.Models;

namespace MoxibustionBedAPP.ViewModes
{
    public class COMFailMessageBoxViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region 自定义变量
        private static COMFailMessageBoxViewModel _instance;
        public static COMFailMessageBoxViewModel Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new COMFailMessageBoxViewModel();
                }
                return _instance;
            }
        }

        public ICommand ButtonClickCommon { get; set; }

        public ObservableCollection<string> ComPorts { get; } = new ObservableCollection<string>();


        private int _selectedItem;
        public int SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnPropertyChanged(nameof(SelectedItem));
            }
        }
        #endregion

        public COMFailMessageBoxViewModel()
        {
            ButtonClickCommon = new RelayCommand(ButtonClickMethod);
            GetAllCOM();
        }

        #region 自定义方法
        private void ButtonClickMethod()
        {
            if (App.PropertyModelInstance.MotherboardCOM == App.PropertyModelInstance.AICOM)
            {
                MessageBox.Show("主板串口和语音助手串口不能相同！");
                return;
            }
            PublicMethods.SaveToCOMJson();
            SerialPortManager.Instance.comfail.Hide();
        }

        public void GetAllCOM()
        {
            ComPorts.Clear();
            foreach(string port in SerialPort.GetPortNames())
            {
                ComPorts.Add(port);
            }
        }
        #endregion
    }
}
