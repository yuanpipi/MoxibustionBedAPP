using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using MoxibustionBedAPP.Models;

namespace MoxibustionBedAPP.ViewModes
{
    public class ParameterSettingViewModel : INotifyPropertyChanged
    {
        public ICommand SaveSetting { get; set; }
        public ICommand ResetSettings { get; set; }

        public ParameterSettingViewModel()
        {
            SaveSetting = new RelayCommand(SaveSettingMethod);
            ResetSettings= new RelayCommand(ResetSettingsMethod);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 保存设置
        /// </summary>
        private void SaveSettingMethod()
        {
            byte[] data = new byte[18];
            data[0] = 0x55;
            data[1] = 0xAA;
            data[2] = 0x0F;
            data[3] = 0x01;
            data[4] = 0x10;
            data[5] = 0x01;
            data[6] = Convert.ToByte(App.PropertyModelInstance.Upper_CabinTemperature.ToString("X"),16);
            data[7] = Convert.ToByte(App.PropertyModelInstance.BackTemperature.ToString("X"),16);
            data[8] = Convert.ToByte(App.PropertyModelInstance.LegTemperature.ToString("X"),16);
            data[9] = Convert.ToByte(App.PropertyModelInstance.PreheadTemperature.ToString("X"),16);
            data[10] = Convert.ToByte(App.PropertyModelInstance.UpperAlarmCabinTemperature.ToString("X"),16);
            data[11] = Convert.ToByte(App.PropertyModelInstance.BackAlarmTemperature.ToString("X"),16);
            data[12] = Convert.ToByte(App.PropertyModelInstance.LegAlarmTemperature.ToString("X"),16);
            data[13] = Convert.ToByte(App.PropertyModelInstance.AutomaticLidOpening==true ? 0x00 : 0x01);
            data[16] = 0x55;
            data[17] = 0xAA;
            data = SerialPortManager.CRC16(data);
            SerialPortManager.Instance.SendData(data);
            //Thread.Sleep(1500);
            //if (!App.IsReceive)
            //{
            //    MessageBox.Show($"串口错误，无返回数据");
            //}
        }

        /// <summary>
        /// 恢复初始值并重置仪器
        /// </summary>
        private void ResetSettingsMethod()
        {
            App.PropertyModelInstance.Upper_CabinTemperature = 40;
            App.PropertyModelInstance.BackTemperature = 40;
            App.PropertyModelInstance.LegTemperature = 40;
            App.PropertyModelInstance.PreheadTemperature = 40;
            App.PropertyModelInstance.UpperAlarmCabinTemperature = 67;
            App.PropertyModelInstance.BackAlarmTemperature = 77;
            App.PropertyModelInstance.LegAlarmTemperature = 77;
            App.PropertyModelInstance.PreheadTime = 30;
            App.PropertyModelInstance.InignitionTime = 120;
            App.PropertyModelInstance.MoxibustionTherapyTime = 90;
            App.PropertyModelInstance.AutomaticLidOpening = true;

            byte[] data = new byte[18];
            data[0] = 0x55;
            data[1] = 0xAA;
            data[2] = 0x0F;
            data[3] = 0x01;
            data[4] = 0x10;
            data[5] = 0x01;
            data[6] = Convert.ToByte(App.PropertyModelInstance.Upper_CabinTemperature.ToString("X"), 16);
            data[7] = Convert.ToByte(App.PropertyModelInstance.BackTemperature.ToString("X"), 16);
            data[8] = Convert.ToByte(App.PropertyModelInstance.LegTemperature.ToString("X"), 16);
            data[9] = Convert.ToByte(App.PropertyModelInstance.PreheadTemperature.ToString("X"), 16);
            data[10] = Convert.ToByte(App.PropertyModelInstance.UpperAlarmCabinTemperature.ToString("X"), 16);
            data[11] = Convert.ToByte(App.PropertyModelInstance.BackAlarmTemperature.ToString("X"), 16);
            data[12] = Convert.ToByte(App.PropertyModelInstance.LegAlarmTemperature.ToString("X"), 16);
            data[13] = Convert.ToByte(App.PropertyModelInstance.AutomaticLidOpening == true ? 0x00 : 0x01);
            data[16] = 0x55;
            data[17] = 0xAA;
            data = SerialPortManager.CRC16(data);
            SerialPortManager.Instance.SendData(data);
            //Thread.Sleep(1500);
            //if (!App.IsReceive)
            //{
            //    MessageBox.Show($"串口错误，无返回数据");
            //}
        }
    }

}
