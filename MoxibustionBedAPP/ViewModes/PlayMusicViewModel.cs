﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using MoxibustionBedAPP.Models;
using MoxibustionBedAPP.Properties;
using MoxibustionBedAPP.Views;
using NAudio.Wave;
using WpfAnimatedGif;

namespace MoxibustionBedAPP.ViewModes
{
    public class PlayMusicViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        #region 变量定义
        private int i = 0;
        private ObservableCollection<MusicModel> _fileNames;
        public MediaPlayer _mediaPlayer = new MediaPlayer();
        //private WaveOut waveOut;
        //private AudioFileReader audioFileReader;

        private double currentPosition;

        public double CurrentPosition
        {
            get { return currentPosition; }
            set
            {
                currentPosition = value;
                OnPropertyChanged(nameof(CurrentPosition));
            }
        }

        private double totalDuration;
        public double TotalDuration
        {
            get { return totalDuration; }
            set
            {
                totalDuration = value;
                OnPropertyChanged(nameof(TotalDuration));
            }
        }

        /// <summary>
        /// listbox双击事件
        /// </summary>
        public RelayCommand ItemSelectedCommand { get; set; }

        /// <summary>
        /// listbox selectedIndex变化事件
        /// </summary>
        public RelayCommand SelectedIndexChangedCommand { get; set; }

        /// <summary>
        /// 上一曲点击事件
        /// </summary>
        public RelayCommand Previous { get; set; }
        
        /// <summary>
        /// 下一曲点击事件
        /// </summary>
        public RelayCommand Next { get; set; }
        
        /// <summary>
        /// 播放/暂停点击事件
        /// </summary>
        public RelayCommand PlayOrPause { get; set; }

        /// <summary>
        /// 随机/暂停事件
        /// </summary>
        public RelayCommand RandomOrSequence {  get; set; }

        /// <summary>
        /// 歌曲名称
        /// </summary>
        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        /// <summary>
        /// 音乐时长
        /// </summary>
        private string duration;
        public string Duration
        {
            get 
            { 
                return duration; 
            }
            set
            {
                duration = value;
                OnPropertyChanged(nameof(Duration));
            }
        }

        /// <summary>
        /// 随机播放图片
        /// </summary>
        private string randomOrSequencePicture;
        public string RandomOrSequencePicture
        {
            get 
            { 
                return randomOrSequencePicture; 
            }
            set
            {
                randomOrSequencePicture = value;
                OnPropertyChanged(nameof(RandomOrSequencePicture));
            }
        }
        

        /// <summary>
        /// 播放暂停图片
        /// </summary>
        private string playOrPausePicture;
        public string PlayOrPausePicture
        {
            get 
            { 
                return playOrPausePicture; 
            }
            set
            {
                playOrPausePicture = value;
                OnPropertyChanged(nameof(PlayOrPausePicture));
            }
        }

        /// <summary>
        /// 是否正在播放音乐
        /// </summary>
        private bool _isPlaying;
        public bool IsPlaying
        {
            get { return _isPlaying; }
            set
            {
                _isPlaying = value;
                OnPropertyChanged("IsPlaying");
            }
        }

        /// <summary>
        /// 是否是随机
        /// </summary>
        private bool _isRandom;
        public bool IsRandom
        {
            get { return _isRandom; }
            set
            {
                _isRandom = value;
                OnPropertyChanged("IsRandom");
            }
        }

        /// <summary>
        /// listbox选择的index
        /// </summary>
        private int selectIndex;
        public int SelectIndex
        {
            get { return selectIndex; }
            set
            {
                selectIndex = value;
                OnPropertyChanged(nameof(SelectIndex));
            }
        }

        /// <summary>
        /// 播放的歌曲间隔
        /// </summary>
        private int num;

        /// <summary>
        /// 定时器用于更新播放进度
        /// </summary>
        public System.Timers.Timer UpdateProgressTimer = new System.Timers.Timer(1000);

        private int minutes;
        private int seconds;

        private ObservableCollection<LineModel> _lines = new ObservableCollection<LineModel>();
        public ObservableCollection<LineModel> Lines
        {
            get { return _lines; }
            set
            {
                _lines = value;
                OnPropertyChanged(nameof(Lines));
            }
        }

        public bool IsDoubleClick;

        private ObservableCollection<ItemViewModel> _items = new ObservableCollection<ItemViewModel>();
        public ObservableCollection<ItemViewModel> Items
        {
            get
            {
                return _items;
            }
            set
            {
                _items = value;
                OnPropertyChanged(nameof(Items));
            }
        }

        private int MusicIndex;

        //private bool _isAutoPlay;
        //public bool IsAutoPlay
        //{
        //    get
        //    {
        //        return _isAutoPlay;
        //    }
        //    set
        //    {
        //        _isAutoPlay = value;
        //        OnPropertyChanged(nameof(IsAutoPlay));
        //    }
        //}

        //public ImageAnimationController controller { get; set; }


        // 用于控制GIF播放的定时器
        private DispatcherTimer _timer;
        // 当前显示的帧索引
        private int _currentFrameIndex;
        // 存储GIF帧的列表
        private BitmapFrame[] _frames;

        private ImageSource _gifSource;
        public ImageSource GifSource
        {
            get 
            { 
                return _gifSource; 
            }
            set
            {
                _gifSource = value;
                OnPropertyChanged(nameof(GifSource));
            }
        }
        #endregion

        public PlayMusicViewModel()
        {
            string filepath=Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)+ "/Resources/Music";
            //ReadFileNamesFromFolder(@"/Resources/Music");
            ReadFileNamesFromFolder(filepath);
            ItemSelectedCommand =new RelayCommand(OnItemSelected);
            SelectedIndexChangedCommand = new RelayCommand(SelectedIndexChandedMethod);
            Name = "Song Name";
            SelectIndex = -1;
            MusicIndex = SelectIndex;
            Duration = "00:00";
            RandomOrSequencePicture = "pack://application:,,,/Resources/Pictures/Sequence.png";
            PlayOrPausePicture = "pack://application:,,,/Resources/Pictures/PlayMusic.png";
            Previous = new RelayCommand(LastSong);
            Next = new RelayCommand(NextSong);
            PlayOrPause = new RelayCommand(PlayOrPauseSong);
            num = 1;
            IsRandom = false;
            RandomOrSequence = new RelayCommand(RandomOrSequenceSong);
            // 初始化方法，用于加载GIF并设置定时器
            InitializeGifPlayer(@"pack://application:,,,/Resources/Pictures/Musicplayer.gif");
            //waveOut = new WaveOut();

            //音乐播放进度条+时间倒计时
            UpdateProgressTimer.Elapsed += (sender, e) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (_mediaPlayer.Source != null)
                    {
                        CurrentPosition = _mediaPlayer.Position.TotalSeconds;
                        TotalDuration = _mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;
                        //audioFileReader.CurrentTime=TimeSpan.FromSeconds(CurrentPosition);
                        minutes = (int)(TotalDuration - CurrentPosition) / 60;
                        seconds = (int)(TotalDuration - CurrentPosition) % 60;
                        if (seconds < 10)
                        {
                            Duration = $"{minutes}:0{seconds}";
                        }
                        else
                        {
                            Duration = $"{minutes}:{seconds}";
                        }
                    }
                    //自动播放下一首歌曲
                    if (CurrentPosition == TotalDuration)
                    {
                        NextSong();
                    }
                });
            };
        }

        #region 自定义方法
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public ObservableCollection<MusicModel> FileNames
        {
            get
            {
                return _fileNames;
            }
            set
            {
                _fileNames = value;
                OnPropertyChanged(nameof(FileNames));
            }
        }

        /// <summary>
        /// 读取文件中的音乐的信息
        /// </summary>
        /// <param name="folderPath"></param>
        public void ReadFileNamesFromFolder(string folderPath)
        {
            try
            {
                FileNames = new ObservableCollection<MusicModel>();
                string name = "";
                string[] strings;
                foreach (string file in Directory.GetFiles(folderPath))
                {
                    if (file.EndsWith(".mp3") || file.EndsWith(".wav"))
                    {
                        name = Path.GetFileNameWithoutExtension(file);
                        strings=name.Split('_');
                        name = name.Remove(name.IndexOf('_'), name.LastIndexOf("_") - name.IndexOf('_') + 1);
                        MusicModel music = new MusicModel
                        {
                            FilePath = Path.GetFullPath(file),
                            MusicName = name.Insert(name.LastIndexOfAny(new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' }) + 1, " - "),
                            Singer = strings[1]
                        };
                        ItemViewModel item = new ItemViewModel
                        {
                            ImageSource = "pack://application:,,,/Resources/Pictures/PlayIcon.png",
                            MusicName = music.MusicName,
                            Singer = music.Singer
                        };
                        FileNames.Add(music);//保存文件中的音乐信息
                        Items.Add(item);
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// listbox双击事件
        /// </summary>
        private void OnItemSelected()
        {
            MusicIndex = SelectIndex;
            //播放音乐
            PlayMusic();
        }

        /// <summary>
        /// listbox双击事件
        /// </summary>
        private void SelectedIndexChandedMethod(object parameter)
        {
            if(IsDoubleClick)
            {
                MusicModel ss = (MusicModel)FileNames[SelectIndex];
                //更换歌曲名称
                Name = ss.MusicName;

                //播放音乐
                PlayMusic();
            }
            IsDoubleClick = false;
        }

        /// <summary>
        /// 播放音乐
        /// </summary>
        public void PlayMusic()
        {
            i++;
            Name = FileNames[selectIndex].MusicName;
            _mediaPlayer.Open(new Uri(FileNames[selectIndex].FilePath));            
            //加载音乐时长
            while (!_mediaPlayer.NaturalDuration.HasTimeSpan)
            {
                Thread.Sleep(10);
            }
            
            _mediaPlayer.Play();
            IsPlaying = true;
            // 启动定时器
            _timer.Start();
            CurrentPosition = _mediaPlayer.Position.TotalSeconds;
            TotalDuration = _mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;
            minutes = (int)(TotalDuration - CurrentPosition) / 60;
            seconds = (int)(TotalDuration - CurrentPosition) % 60;
            if (seconds < 10)
            {
                Duration = $"{minutes}:0{seconds}";
            }
            else
            {
                Duration = $"{minutes}:{seconds}";
            }
            PlayOrPausePicture = "pack://application:,,,/Resources/Pictures/StopMusic.png";
            //启动定时器更新进度
            UpdateProgressTimer.Start();
        }

        /// <summary>
        /// 音乐暂停
        /// </summary>
        public void PauseMusic()
        {
            _mediaPlayer.Pause();
            // 停止定时器
            UpdateProgressTimer.Stop();
            IsPlaying = false;
            // 停止定时器
            _timer.Stop();
            PlayOrPausePicture = "pack://application:,,,/Resources/Pictures/PlayMusic.png";
        }

        /// <summary>
        /// 音乐停止
        /// </summary>
        public void StopMusic()
        {
            _mediaPlayer.Stop();
            Lines.Clear();
            // 停止定时器
            UpdateProgressTimer.Stop();
            IsPlaying = false;
            _timer.Stop();
        }
        
        /// <summary>
        /// 上一曲事件
        /// </summary>
        public void LastSong()
        {
            IsDoubleClick = true;
            int i;
            i = MusicIndex - num;
            if (i < 0)
            {
                i = FileNames.Count()+i;
            }
            MusicIndex = i;
            SelectIndex = MusicIndex;
            PlayMusic();
        }
        
        /// <summary>
        /// 下一曲事件
        /// </summary>
        public void NextSong()
        {
            IsDoubleClick = true;
            MusicIndex = (MusicIndex + num) % FileNames.Count;
            SelectIndex = MusicIndex;
            PlayMusic();
        }

        /// <summary>
        /// 播放或暂停
        /// </summary>
        public void PlayOrPauseSong()
        {
            if(MusicIndex != -1)
            {
                if (IsPlaying)
                {
                    PauseMusic();
                }
                else
                {
                    _mediaPlayer.Play();
                    IsPlaying = true;
                    // 启动定时器
                    _timer.Start();
                    PlayOrPausePicture = "pack://application:,,,/Resources/Pictures/StopMusic.png";
                    UpdateProgressTimer.Start();
                }
            }
            else
            {
                NextSong();
            }
        }

        /// <summary>
        /// 随机或顺序
        /// </summary>
        private void RandomOrSequenceSong()
        {
            IsRandom = !IsRandom;
            if(IsRandom)
            {
                Random random = new Random();
                num = random.Next(2,10);
                RandomOrSequencePicture = "../Resources/Pictures/Random.png";
            }
            else
            {
                num = 1;
                RandomOrSequencePicture = "../Resources/Pictures/Sequence.png";
            }
        }

        private void InitializeGifPlayer(string gifPath)
        {
            try
            {
                // 使用BitmapDecoder创建一个解码器，用于读取GIF文件
                var decoder = BitmapDecoder.Create(
                    new Uri(gifPath, UriKind.RelativeOrAbsolute),
                    BitmapCreateOptions.PreservePixelFormat,
                    BitmapCacheOption.Default);

                // 获取GIF的所有帧并存储在数组中
                _frames = decoder.Frames.ToArray();

                // 设置初始帧
                GifSource = _frames[0];

                // 初始化帧索引
                _currentFrameIndex = 0;

                // 创建一个定时器，用于控制GIF的播放速度
                _timer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromMilliseconds(100) // 调整此值以控制播放速度
                };
                // 定时器触发时，更新显示的帧
                _timer.Tick += Timer_Tick;
                //// 启动定时器
                //_timer.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading GIF: {ex.Message}");
            }
        }

        // 定时器触发的事件处理方法，用于更新显示的帧
        private void Timer_Tick(object sender, EventArgs e)
        {
            // 更新帧索引，实现循环播放
            _currentFrameIndex = (_currentFrameIndex + 1) % _frames.Length;
            // 设置Image控件的源为当前帧
            GifSource = _frames[_currentFrameIndex];
        }

        #endregion
    }
}
