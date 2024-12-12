using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using MoxibustionBedAPP.Views;

namespace MoxibustionBedAPP.ViewModes
{
    public class PopupBoxViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private static int count;

        private string _title;
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                OnPropertyChanged(nameof(Title));
            }
        }

        private string _message;
        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                OnPropertyChanged(nameof(Message));
            }
        }


        public PopupBoxViewModel()
        {
            count = 0;
        }


        public static void ShowPopupBox(string title,string message)
        {
            PopupBox popupBox = new PopupBox();

            popupBox.Show();
            count++;
            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(3)
            };
            timer.Tick += (sender, args) =>
            {
                popupBox.Close();
                count--;
                ((DispatcherTimer)sender).Stop();
            };
            timer.Start();
        }

    }
}
