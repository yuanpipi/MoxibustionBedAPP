using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;
using MoxibustionBedAPP.Models;

namespace MoxibustionBedAPP.ViewModes
{
    public class ParameterSettingViewModel : INotifyPropertyChanged
    {
        public ICommand SaveSetting { get; set; }
        public ICommand ResetSettings { get; set; }

        public ICommand AutoCommand { get;private set; }

        public ICommand AddOrSubCommand { get;private set; }

        private bool _isSomeControlVisible;
        public bool IsSomeControlVisible
        {
            get { return _isSomeControlVisible; }
            set
            {
                _isSomeControlVisible = value;
                OnPropertyChanged(nameof(IsSomeControlVisible));
            }
        }

        public ParameterSettingViewModel()
        {
            SaveSetting = new RelayCommand(SaveSettingMethod);
            ResetSettings= new RelayCommand(ResetSettingsMethod);
            AutoCommand = new RelayCommand(AutoOrOnhandMethod);
            AddOrSubCommand=new RelayCommand(AddOrSubMethod);
            IsSomeControlVisible = true;
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
            //发送至下位机
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
            data[16] = 0xAA;
            data[17] = 0x5C;
            data = SerialPortManager.CRC16(data);
            SerialPortManager.Instance.SendData(data);

            //保存进文件
            PublicMethods.SaveToJson();
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
            data[16] = 0xAA;
            data[17] = 0x5C;
            data = SerialPortManager.CRC16(data);
            SerialPortManager.Instance.SendData(data);

            //保存进文件
            PublicMethods.SaveToJson();
        }

        /// <summary>
        /// 结束后自动开盖
        /// </summary>
        /// <param name="title"></param>
        private void AutoOrOnhandMethod(object title)
        {
            switch (title)
            {
                case "Auto":
                    App.PropertyModelInstance.AutomaticLidOpening = true;
                    break;
                case "Onhand":
                    App.PropertyModelInstance.AutomaticLidOpening = false;
                    break;
            }
        }

        /// <summary>
        /// 加、减按钮事件
        /// </summary>
        /// <param name="title"></param>
        private void AddOrSubMethod(object title)
        {
            switch(title)
            {
                case "PreheadTimeSub"://预热时间 减
                    App.PropertyModelInstance.PreheadTime--;
                    if (App.PropertyModelInstance.PreheadTime < 5)
                    {
                        App.PropertyModelInstance.PreheadTime = 5;
                    }
                    break;
                case "PreheadTimeAdd"://预热时间  加
                    App.PropertyModelInstance.PreheadTime++;
                    if (App.PropertyModelInstance.PreheadTime > 90)
                    {
                        App.PropertyModelInstance.PreheadTime = 90;
                    }
                    break;
                case "PreheadTemperatureSub"://预热温度  减
                    App.PropertyModelInstance.PreheadTemperature--;
                    if (App.PropertyModelInstance.PreheadTemperature < 20)
                    {
                        App.PropertyModelInstance.PreheadTemperature = 20;
                    }
                    break;
                case "PreheadTemperatureAdd"://预热温度  加
                    App.PropertyModelInstance.PreheadTemperature++;
                    if (App.PropertyModelInstance.PreheadTemperature > 50)
                    {
                        App.PropertyModelInstance.PreheadTemperature = 50;
                    }
                    break;
                case "Upper_CabinTemperatureSub"://上舱温度  减
                    App.PropertyModelInstance.Upper_CabinTemperature--;
                    if (App.PropertyModelInstance.Upper_CabinTemperature < 30)
                    {
                        App.PropertyModelInstance.Upper_CabinTemperature = 30;
                    }
                    break;
                case "Upper_CabinTemperatureAdd"://上舱温度  加
                    App.PropertyModelInstance.Upper_CabinTemperature++;
                    if (App.PropertyModelInstance.Upper_CabinTemperature > 60)
                    {
                        App.PropertyModelInstance.Upper_CabinTemperature = 60;
                    }
                    break;
                case "UpperAlarmCabinTemperatureSub"://上舱报警温度  减
                    App.PropertyModelInstance.UpperAlarmCabinTemperature--;
                    if (App.PropertyModelInstance.UpperAlarmCabinTemperature < 30)
                    {
                        App.PropertyModelInstance.UpperAlarmCabinTemperature = 30;
                    }
                    break;
                case "UpperAlarmCabinTemperatureAdd"://上舱报警温度  加
                    App.PropertyModelInstance.UpperAlarmCabinTemperature++;
                    if (App.PropertyModelInstance.UpperAlarmCabinTemperature > 80)
                    {
                        App.PropertyModelInstance.UpperAlarmCabinTemperature = 80;
                    }
                    break;
                case "BackTemperatureSub"://背部温度  减
                    App.PropertyModelInstance.BackTemperature--;
                    if (App.PropertyModelInstance.BackTemperature < 30)
                    {
                        App.PropertyModelInstance.BackTemperature = 30;
                    }
                    break;
                case "BackTemperatureAdd"://背部温度  加
                    App.PropertyModelInstance.BackTemperature++;
                    if (App.PropertyModelInstance.BackTemperature > 70)
                    {
                        App.PropertyModelInstance.BackTemperature = 70;
                    }
                    break;
                case "BackAlarmTemperatureSub"://背部报警温度  减
                    App.PropertyModelInstance.BackAlarmTemperature--;
                    if (App.PropertyModelInstance.BackAlarmTemperature < 30)
                    {
                        App.PropertyModelInstance.BackAlarmTemperature = 30;
                    }
                    break;
                case "BackAlarmTemperatureAdd"://背部报警温度  加
                    App.PropertyModelInstance.BackAlarmTemperature++;
                    if (App.PropertyModelInstance.BackAlarmTemperature > 90)
                    {
                        App.PropertyModelInstance.BackAlarmTemperature = 90;
                    }
                    break;
                case "LegTemperatureSub"://腿部温度  减
                    App.PropertyModelInstance.LegTemperature--;
                    if (App.PropertyModelInstance.LegTemperature < 30)
                    {
                        App.PropertyModelInstance.LegTemperature = 30;
                    }
                    break;
                case "LegTemperatureAdd"://腿部温度  加
                    App.PropertyModelInstance.LegTemperature++;
                    if (App.PropertyModelInstance.LegTemperature > 70)
                    {
                        App.PropertyModelInstance.LegTemperature = 70;
                    }
                    break;
                case "LegAlarmTemperatureSub"://腿部报警温度  减
                    App.PropertyModelInstance.LegAlarmTemperature--;
                    if (App.PropertyModelInstance.LegAlarmTemperature < 30)
                    {
                        App.PropertyModelInstance.LegAlarmTemperature = 30;
                    }
                    break;
                case "LegAlarmTemperatureAdd"://腿部报警温度  加
                    App.PropertyModelInstance.LegAlarmTemperature++;
                    if (App.PropertyModelInstance.LegAlarmTemperature > 90)
                    {
                        App.PropertyModelInstance.LegAlarmTemperature = 90;
                    }
                    break;
                case "InignitionTimeSub"://点火时间  减
                    App.PropertyModelInstance.InignitionTime--;
                    if (App.PropertyModelInstance.InignitionTime < 60)
                    {
                        App.PropertyModelInstance.InignitionTime = 60;
                    }
                    break;
                case "InignitionTimeAdd"://点火时间  加
                    App.PropertyModelInstance.InignitionTime++;
                    if (App.PropertyModelInstance.InignitionTime > 270)
                    {
                        App.PropertyModelInstance.InignitionTime = 270;
                    }
                    break;
                case "MoxibustionTherapyTimeSub"://灸疗时间  减
                    App.PropertyModelInstance.MoxibustionTherapyTime--;
                    if (App.PropertyModelInstance.MoxibustionTherapyTime < 25)
                    {
                        App.PropertyModelInstance.MoxibustionTherapyTime = 25;
                    }
                    break;
                case "MoxibustionTherapyTimeAdd"://灸疗时间  加
                    App.PropertyModelInstance.MoxibustionTherapyTime++;
                    if (App.PropertyModelInstance.MoxibustionTherapyTime > 90)
                    {
                        App.PropertyModelInstance.MoxibustionTherapyTime = 90;
                    }
                    break;
            }
        }
    }

    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool boolValue = (bool)value;
            return boolValue ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility visibility = (Visibility)value;
            return visibility == Visibility.Visible;
        }
    }
}
