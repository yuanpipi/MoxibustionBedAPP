using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using MoxibustionBedAPP.ViewModes;

namespace MoxibustionBedAPP.Views
{
    /// <summary>
    /// PlayMusicView.xaml 的交互逻辑
    /// </summary>
    public partial class PlayMusicView : UserControl
    {
        public PlayMusicView()
        {
            InitializeComponent();
            DataContext = App.sharedPlayMusicModel;
            progressSlider.ValueChanged += ProgressSlider_ValueChanged;
        }

        /// <summary>
        /// 进度条滑动事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProgressSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var viewModel = (PlayMusicViewModel)this.DataContext;
            if (viewModel != null && viewModel._mediaPlayer != null)
            {
                viewModel._mediaPlayer.Position = TimeSpan.FromSeconds(e.NewValue);
            }
        }

        public object ViewModel
        {
            get { return DataContext; }
            set { DataContext = value; }
        }
    }
}
