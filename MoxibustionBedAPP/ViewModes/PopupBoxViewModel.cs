using MoxibustionBedAPP.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;

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


        public static void ShowPopupBox(string message)
        {
            // 获取主显示器信息
            Screen primaryScreen = Screen.PrimaryScreen;
            System.Drawing.Rectangle workingArea = primaryScreen.WorkingArea;

            PopupBox popupBox = new PopupBox();
            popupBox.PopupBoxViewModel.Message = message;
            popupBox.Topmost = true;

            // 计算居中位置 (可选)
            popupBox.Loaded += (sender, e) =>
            {
                // 确保窗口完全加载后再计算位置
                popupBox.Left = workingArea.Left + (workingArea.Width - popupBox.ActualWidth) / 2;
                popupBox.Top = workingArea.Top + (workingArea.Height - popupBox.ActualHeight) / 2;
            };

            popupBox.Show();
            //count++;
            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(2)
            };
            timer.Tick += (sender, args) =>
            {
                popupBox.Close();
                //count--;
                ((DispatcherTimer)sender).Stop();
            };
            timer.Start();
        }

    }
}
