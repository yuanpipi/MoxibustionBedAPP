using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoxibustionBedAPP.Models;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.ComponentModel;
//using System.Windows.Media.Animation;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace MoxibustionBedAPP.ViewModes
{
    public class LoadWindowViewModel : INotifyPropertyChanged
    {


        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        //private BitmapImage _currentFrameImage;
        //private DispatcherTimer _timer;
        //private int _interval = 100; // 初始播放间隔（毫秒），可调节播放速度
        //private Image _systemDrawingGif;
        //private List<BitmapImage> _wpfFrames;

        //// 用于绑定到视图中Image控件显示当前帧的属性
        //public BitmapImage CurrentFrameImage
        //{
        //    get { return _currentFrameImage; }
        //    set
        //    {
        //        _currentFrameImage = value;
        //        RaisePropertyChanged(nameof(CurrentFrameImage));
        //    }
        //}

        //// 控制播放速度的间隔属性，可在视图中进行调整
        //public int Interval
        //{
        //    get { return _interval; }
        //    set
        //    {
        //        _interval = value;
        //        RaisePropertyChanged(nameof(Interval));
        //        if (_timer != null)
        //        {
        //            _timer.Interval = TimeSpan.FromMilliseconds(_interval);
        //        }
        //    }
        //}

        //// 加载GIF的命令
        //public RelayCommand LoadGifCommand { get; private set; }

        //public LoadWindowViewModel()
        //{
        //    LoadGifCommand = new RelayCommand(LoadGif);
        //}

        //private void LoadGif()
        //{
        //    // 使用System.Drawing加载GIF图像
        //    _systemDrawingGif = Image.FromFile("your_gif_path.gif");
        //    FrameDimension frameDimension = new FrameDimension(_systemDrawingGif.FrameDimensionsList[0]);
        //    int frameCount = _systemDrawingGif.GetFrameCount(frameDimension);

        //    // 将System.Drawing的Image帧转换为WPF的BitmapImage帧
        //    _wpfFrames = new List<BitmapImage>();
        //    for (int i = 0; i < frameCount; i++)
        //    {
        //        _systemDrawingGif.SelectActiveFrame(frameDimension, i);
        //        using (var bitmap = new System.Drawing.Bitmap(_systemDrawingGif))
        //        {
        //            using (var memoryStream = new MemoryStream())
        //            {
        //                bitmap.Save(memoryStream, ImageFormat.Png);
        //                memoryStream.Seek(0, SeekOrigin.Begin);
        //                var bitmapImage = new BitmapImage();
        //                bitmapImage.BeginInit();
        //                bitmapImage.StreamSource = memoryStream;
        //                bitmapImage.EndInit();
        //                _wpfFrames.Add(bitmapImage);
        //            }
        //        }
        //    }

        //    // 初始化定时器，用于控制帧更新频率（播放速度）
        //    _timer = new DispatcherTimer(DispatcherPriority.Render);
        //    _timer.Interval = TimeSpan.FromMilliseconds(Interval);
        //    _timer.Tick += Timer_Tick;

        //    // 启动System.Drawing.ImageAnimator动画，设置回调以更新当前显示帧
        //    System.Drawing.ImageAnimator.Animate(_systemDrawingGif, (sender, EventArgs) =>
        //    {
        //        int frameIndex = System.Drawing.ImageAnimator.CurrentFrameIndex;
        //        CurrentFrameImage = _wpfFrames[frameIndex];
        //    });

        //    _timer.Start();
        //}

        //private void Timer_Tick(object sender, EventArgs e)
        //{
        //    System.Drawing.ImageAnimator.UpdateFrames(_systemDrawingGif);
        //}
    }
}
