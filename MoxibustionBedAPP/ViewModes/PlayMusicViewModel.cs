﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MoxibustionBedAPP.Models;

namespace MoxibustionBedAPP.ViewModes
{
    public class PlayMusicViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        #region 变量定义
        private ObservableCollection<MusicModel> _fileNames;
        //public PlayerViewModel player;
        public MediaPlayer _mediaPlayer = new MediaPlayer();

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
        /// listbox点击事件
        /// </summary>
        public RelayCommand ItemSelectedCommand { get; set; }

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

        //public RelayCommand SliderValueChangedCommand { get; set; }

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
        private System.Timers.Timer UpdateProgressTimer = new System.Timers.Timer(1000);

        private int minutes;
        private int seconds;
        #endregion

        public PlayMusicViewModel()
        {
            //FileNames=MainWindowViewModel.FileNames;
            ReadFileNamesFromFolder(@".\Resources\Music");
            //ButtonClickCommand = new RelayCommand(Click);
            ItemSelectedCommand =new RelayCommand(OnItemSelected);
            //player=new PlayerViewModel();
            Name = "Song Name";
            SelectIndex = -1;
            Duration = "00:00";
            Previous = new RelayCommand(LastSong);
            Next = new RelayCommand(NextSong);
            PlayOrPause = new RelayCommand(PlayOrPauseSong);
            num = 1;
            IsRandom = false;
            RandomOrSequence = new RelayCommand(RandomOrSequenceSong);
            //SliderValueChangedCommand = new RelayCommand(OnSliderValueChanged);

            //音乐播放进度条+时间倒计时
            UpdateProgressTimer.Elapsed += (sender, e) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (_mediaPlayer.Source != null)
                    {
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
                //Directory.GetFiles(folderPath).Select(Path.GetFileName)
                FileNames = new ObservableCollection<MusicModel>();
                string name = "";
                foreach (string file in Directory.GetFiles(folderPath))
                {
                    if (file.EndsWith(".mp3") || file.EndsWith(".wav"))
                    {
                        name = Path.GetFileNameWithoutExtension(file);
                        name = name.Remove(name.IndexOf('_'), name.LastIndexOf("_") - name.IndexOf('_') + 1);
                        MusicModel music = new MusicModel
                        {
                            FilePath = Path.GetFullPath(file),
                            MusicName = name.Insert(name.LastIndexOfAny(new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' }) + 1, " - "),
                        };
                        FileNames.Add(music);//保存文件中的音乐信息
                        FileNames.Add(music);//保存文件中的音乐信息
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// listbox单击事件
        /// </summary>
        private void OnItemSelected()
        {
            MusicModel ss = (MusicModel)FileNames[selectIndex];
            //更换歌曲名称
            Name = ss.MusicName;
            
            //播放音乐
            PlayMusic();
        }

        /// <summary>
        /// 播放音乐
        /// </summary>
        public void PlayMusic()
        {
            _mediaPlayer.Open(new Uri(FileNames[selectIndex].FilePath));
            Thread.Sleep(150);
            _mediaPlayer.Play();
            IsPlaying = true;
            //加载音乐时长
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
        }

        /// <summary>
        /// 音乐停止
        /// </summary>
        public void StopMusic()
        {
            _mediaPlayer.Stop();
            // 停止定时器
            UpdateProgressTimer.Stop();
            IsPlaying = false;
        }
        
        /// <summary>
        /// 上一曲事件
        /// </summary>
        private void LastSong()
        {
            int i;
            i = SelectIndex - num;
            if (i < 0)
            {
                i = FileNames.Count()+i;
            }
            SelectIndex = i;
            PlayMusic();
        }
        
        /// <summary>
        /// 下一曲事件
        /// </summary>
        private void NextSong()
        {
            SelectIndex = (SelectIndex + num) % FileNames.Count;
            PlayMusic();
        }

        /// <summary>
        /// 播放或暂停
        /// </summary>
        private void PlayOrPauseSong()
        {
            if(IsPlaying)
            {
                PauseMusic();
            }
            else
            {
                _mediaPlayer.Play();
                IsPlaying = true;
                UpdateProgressTimer.Start();
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
            }
            else
            {
                num = 1;
            }
        }

        //private void OnSliderValueChanged(object parameter)
        //{
        //    if (parameter is double value && _mediaPlayer != null)
        //    {
        //        _mediaPlayer.Position = TimeSpan.FromSeconds(value);
        //    }
        //}
        #endregion
    }
}