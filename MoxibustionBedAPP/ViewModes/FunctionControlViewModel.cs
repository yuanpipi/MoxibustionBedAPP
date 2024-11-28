using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using MoxibustionBedAPP.Models;

namespace MoxibustionBedAPP.ViewModes
{
    public class FunctionControlViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        //public FunctionControlViewModel()
        //{
        //    SerialPortManager.Instance.DataReceived += Instance_DataReceived;
        //}

        //private void Instance_DataReceived(object sender,string data)
        //{
        //    System.Windows.Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
        //    {
        //        ReceivedData = data;
        //    }));
        //}

        //private string _receivedData;
        //public string ReceivedData
        //{
        //    get
        //    {
        //        return _receivedData;
        //    }
        //    set
        //    {
        //        _receivedData = value;
        //        OnPropertyChanged("ReceivedData");
        //    }
        //}

        //public void OpenSerialPort(string portName,int baudRate)
        //{
        //    SerialPortManager.Instance.OpenPort(portName, baudRate);
        //}

        //public void CloseSerialPort()
        //{
        //    SerialPortManager.Instance.ClosePort();
        //}

        //public void SendSerialData(string data)
        //{
        //   // SerialPortManager.Instance.SendData(data);
        //}



    }
}
