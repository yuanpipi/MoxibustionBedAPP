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
    public class TestWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public RelayCommand CloseWindow { get; set; }
        public ICommand PublicFunction { get; set; }
        public TestWindowViewModel()
        {
            CloseWindow = new RelayCommand(CloseWindowMethod);
            PublicFunction = new RelayCommand(ExecuteFunctionMethod);
        }
        private void CloseWindowMethod()
        {
            App.Test.Close();
        }

        /// <summary>
        /// 操作指令公共方法
        /// </summary>
        /// <param name="parameter"></param>
        private void ExecuteFunctionMethod(object parameter)
        {
            App.IsReceive = false;
            byte[] data = new byte[11];
            data[0] = 0x55;
            data[1] = 0xAA;
            data[2] = 0x07;
            data[3] = 0x01;
            data[4] = 0x10;
            switch (parameter)
            {
                case "TreatmentMode"://设置治疗模式
                    {

                        data[5] = 0x00;
                        if (App.PropertyModelInstance.MoxibustionTherapyMode)
                        {
                            data[6] = 0x02;
                        }
                        else
                        {
                            data[6] = 0x01;
                        }
                        break;
                    }
                case "Prehead"://设置预热
                    {

                        data[5] = 0x02;
                        if (App.PropertyModelInstance.PreheadMode)
                        {
                            data[6] = 0x02;
                        }
                        else
                        {
                            data[6] = 0x01;
                        }
                        break;
                    }
                case "Inignition"://点火选择
                    {
                        data[5] = 0x06;
                        if (App.PropertyModelInstance.InignitionStatus)
                        {
                            data[6] = 0x02;
                        }
                        else
                        {
                            data[6] = 0x01;
                        }
                        break;
                    }
                case "BackMoxibustionColumnUp"://背部点升
                    {
                        data[5] = 0x04;
                        data[6] = 0x01;
                        break;
                    }
                case "BackMoxibustionColumnDown"://背部点降
                    {
                        data[5] = 0x05;
                        data[6] = 0x01;
                        break;
                    }
                case "LegMoxibustionColumnUp"://腿部点升
                    {
                        data[5] = 0x06;
                        data[6] = 0x01;
                        break;
                    }
                case "LegMoxibustionColumnDown"://腿部点降
                    {
                        data[5] = 0x07;
                        data[6] = 0x01;
                        break;
                    }
                case "HatchClickUp"://舱盖点开
                    {
                        data[5] = 0x08;
                        data[6] = 0x01;
                        break;
                    }
                case "HatchClickDown"://舱盖点关
                    {
                        data[5] = 0x09;
                        data[6] = 0x01;
                        break;
                    }
                case "OpenHatch"://一键开舱
                    {
                        data[5] = 0x0A;
                        data[6] = 0x01;
                        break;
                    }
                case "CloseHatch"://一键关舱
                    {
                        data[5] = 0x0B;
                        data[6] = 0x01;
                        break;
                    }
                case "SmokeExhaustClose"://关闭排烟系统
                    {
                        data[5] = 0x0C;
                        data[6] = 0x00;
                        break;
                    }
                case "SmokeExhaustLow"://排烟系统低档
                    {
                        data[5] = 0x0C;
                        data[6] = 0x01;
                        break;
                    }
                case "SmokeExhaustMedium"://排烟系统中档
                    {
                        data[5] = 0x0C;
                        data[6] = 0x02;
                        break;
                    }
                case "SmokeExhaustHigh"://排烟系统高档
                    {
                        data[5] = 0x0C;
                        data[6] = 0x03;
                        break;
                    }
                case "SmokePurificationSystem"://净烟系统
                    {
                        data[5] = 0x0D;
                        if (App.PropertyModelInstance.SmokePurificationSystem)
                        {
                            data[6] = 0x01;
                        }
                        else
                        {
                            data[6] = 0x02;
                        }
                        break;
                    }
                case "SwingSystem"://摇摆系统
                    {
                        data[5] = 0x0E;
                        if (App.PropertyModelInstance.SmokePurificationSystem)
                        {
                            data[6] = 0x01;
                        }
                        else
                        {
                            data[6] = 0x02;
                        }
                        break;
                    }
                case "InfraredLampClose"://红外线关
                    {
                        data[5] = 0x0F;
                        data[6] = 0x00;
                        break;
                    }
                case "InfraredLampLow"://红外线低档
                    {
                        data[5] = 0x0F;
                        data[6] = 0x01;
                        break;
                    }
                case "InfraredLampMedium"://红外线中档
                    {
                        data[5] = 0x0F;
                        data[6] = 0x02;
                        break;
                    }
                case "InfraredLampHigh"://红外线高档
                    {
                        data[5] = 0x0F;
                        data[6] = 0x03;
                        break;
                    }
            }
            data[9] = 0x55;
            data[10] = 0xAA;
            data = SerialPortManager.CRC16(data);
            SerialPortManager.Instance.SendData(data);
            Thread.Sleep(1500);
            if (!App.IsReceive)
            {
                MessageBox.Show($"串口错误，无返回数据");
            }
        }

    }
}
