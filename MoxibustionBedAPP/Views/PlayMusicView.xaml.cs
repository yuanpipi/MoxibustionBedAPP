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
using MoxibustionBedAPP.Models;
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

        private void MusicList_ManipulationBoundaryFeedback(object sender, ManipulationBoundaryFeedbackEventArgs e)
        {
            e.Handled = true;
        }

        private void MusicList_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            // 获取 ListBox 的 ScrollViewer
            ScrollViewer scrollViewer = FindVisualChild<ScrollViewer>(MusicList);

            if (scrollViewer != null)
            {
                // 计算滚动偏移量
                double deltaY = e.DeltaManipulation.Translation.Y;

                // 滚动 ScrollViewer
                scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - deltaY);

                // 阻止事件冒泡，避免影响其他元素的触摸事件
                e.Handled = true;
            }
        }

        // 辅助方法，用于在可视化树中查找指定类型的子元素
        private T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (child is T)
                    return (T)child;
                else
                {
                    T foundChild = FindVisualChild<T>(child);
                    if (foundChild != null)
                        return foundChild;
                }
            }
            return null;
        }
    }
}
