using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MoxibustionBedAPP.ViewModes;

namespace MoxibustionBedAPP.ViewModes
{

    public class MainWindowCopyViewModel : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 定时器用于更新时间
        /// </summary>
        public static System.Timers.Timer UpdateProgressTimer = new System.Timers.Timer(500);

        public PlayMusicViewModel SharedVM { get; set; }

        /// <summary>
        /// 当前时间
        /// </summary>
        private string currentTime;
        public string CurrentTime
        {
            get
            {
                return currentTime;
            }
            set
            {
                currentTime = value;
                OnPropertyChanged(nameof(CurrentTime));
            }
        }
        public MainWindowCopyViewModel()
        {
            //SharedVM = new PlayMusicViewModel();

            try
            {
                //显示时间，每隔一秒刷新
                UpdateProgressTimer.Elapsed += (sender, e) =>
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        CurrentTime = DateTime.Now.ToString("yyyy/MM/dd HH：mm：ss");
                    });
                };
                UpdateProgressTimer.Start();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
