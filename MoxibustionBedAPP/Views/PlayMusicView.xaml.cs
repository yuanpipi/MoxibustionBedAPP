using System;
using System.Collections.Generic;
using System.IO;
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
using WpfAnimatedGif;

namespace MoxibustionBedAPP.Views
{
    /// <summary>
    /// PlayMusicView.xaml 的交互逻辑
    /// </summary>
    public partial class PlayMusicView : UserControl
    {
        //public static ImageAnimationController controller;
        public PlayMusicView()
        {
            InitializeComponent();
            DataContext = App.sharedPlayMusicModel;
            progressSlider.ValueChanged += ProgressSlider_ValueChanged;

            //App.sharedPlayMusicModel.IsAutoPlay = false;
            //string imagePath = "pack://application:,,,/Resources/Pictures/Musicplayer.gif";
            //BitmapImage bitmapImage = new BitmapImage();
            //bitmapImage.BeginInit();
            //bitmapImage.UriSource = new Uri(imagePath, UriKind.RelativeOrAbsolute);
            //bitmapImage.EndInit();
            //imageControl.Source = bitmapImage;
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

        //private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        //{
        //    var grid = (sender as FrameworkElement);

        //    // 获取绑定上下文
        //    object item = grid.DataContext;

        //    int index = itemsControl.Items.IndexOf(item);
        //    App.sharedPlayMusicModel.SelectIndex = index;
        //    App.sharedPlayMusicModel.PlayMusic();
        //    SelectItemByIndex(index);
        //}

        private void ScrollViewer_ManipulationBoundaryFeedback(object sender, ManipulationBoundaryFeedbackEventArgs e)
        {
            e.Handled= true;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //string url = AppDomain.CurrentDomain.BaseDirectory + @"Resources\Pictures\Musicplayer.gif";
            //string url = "pack://application:,,,/Resources/Pictures/Musicplayer.gif";
            //string url = AppDomain.CurrentDomain.BaseDirectory + @"Resources\Pictures\02.gif";
            //this.pictureBox.Image = System.Drawing.Image.FromFile("pack://application:,,,/Resources/Pictures/Musicplayer.gif");
            //this.pictureBox.ImageLocation = url;



            //using (FileStream fs = new FileStream(AppDomain.CurrentDomain.BaseDirectory + @"Resources/Pictures/Musicplayer.gif", FileMode.Open, FileAccess.Read))
            //{
            //    pictureBox.Image = System.Drawing.Image.FromStream(fs);
            //}
            //this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //if(App.sharedPlayMusicModel.controller == null)
            //{
            //    App.sharedPlayMusicModel.controller = ImageBehavior.GetAnimationController(imageControl);
            //}

            //if (App.sharedPlayMusicModel.IsPlaying)
            //{
            //    //mediaElement.Pause();
            //    controller.Pause();
            //    App.sharedPlayMusicModel.IsAutoPlay = false;
            //}
            //else
            //{
            //    //mediaElement.Play();
            //    controller.Play();
            //    App.sharedPlayMusicModel.IsAutoPlay = true;
            //}
            //if(!App.sharedPlayMusicModel.IsAutoPlay)
            //{
            //    App.sharedPlayMusicModel.IsAutoPlay = true;
            //}
            
        }

        private void MusicList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //if (App.sharedPlayMusicModel.controller == null)
            //{
            //    App.sharedPlayMusicModel.controller = ImageBehavior.GetAnimationController(imageControl);
            //}
            //controller.Play();
            //if (!App.sharedPlayMusicModel.IsAutoPlay)
            //{
            //    App.sharedPlayMusicModel.IsAutoPlay = true;
            //}
        }

        //public void SelectItemByIndex(int index)
        //{
        //    // 获取 ItemsControl 的实例
        //    ItemsControl itemsControl = FindName("itemsControl") as ItemsControl;
        //    if (itemsControl != null && index >= 0 && index < itemsControl.Items.Count)
        //    {
        //        // 通过 index 获取 item
        //        object item = itemsControl.Items[index];
        //        // 假设 item 是一个 FrameworkElement，设置其 IsSelected 属性
        //        if (item is Border border)
        //        {
        //            border.SetValue(Border.TagProperty, true);
        //        }
        //    }
        //    else
        //    {
        //    }
        //}
    }
}
