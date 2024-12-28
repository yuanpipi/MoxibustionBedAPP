using System;
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
using System.Windows.Threading;
using MoxibustionBedAPP.Models;
using MoxibustionBedAPP.Properties;
using NAudio.Wave;

namespace MoxibustionBedAPP.ViewModes
{
    public class PlayMusicViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        #region 变量定义
        private int i = 0;
        private ObservableCollection<MusicModel> _fileNames;
        public MediaPlayer _mediaPlayer = new MediaPlayer();
        private WaveOut waveOut;
        private AudioFileReader audioFileReader;

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
        private System.Timers.Timer UpdateProgressTimer = new System.Timers.Timer(1000);

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
        #endregion

        public PlayMusicViewModel()
        {
            ReadFileNamesFromFolder(@"./Resources/Music");
            ItemSelectedCommand =new RelayCommand(OnItemSelected);
            SelectedIndexChangedCommand = new RelayCommand(SelectedIndexChandedMethod);
            Name = "Song Name";
            SelectIndex = -1;
            Duration = "00:00";
            RandomOrSequencePicture = "pack://application:,,,/Resources/Pictures/Sequence.png";
            PlayOrPausePicture = "pack://application:,,,/Resources/Pictures/PlayMusic.png";
            Previous = new RelayCommand(LastSong);
            Next = new RelayCommand(NextSong);
            PlayOrPause = new RelayCommand(PlayOrPauseSong);
            num = 1;
            IsRandom = false;
            RandomOrSequence = new RelayCommand(RandomOrSequenceSong);
            waveOut = new WaveOut();

            //音乐播放进度条+时间倒计时
            UpdateProgressTimer.Elapsed += (sender, e) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (_mediaPlayer.Source != null)
                    {
                        CurrentPosition = _mediaPlayer.Position.TotalSeconds;
                        TotalDuration = _mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;
                        audioFileReader.CurrentTime=TimeSpan.FromSeconds(CurrentPosition);
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
        /// listbox双击事件
        /// </summary>
        private void OnItemSelected()
        {
            //MusicModel ss = (MusicModel)FileNames[SelectIndex];
            ////更换歌曲名称
            //Name = ss.MusicName;

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
            audioFileReader = new AudioFileReader(FileNames[selectIndex].FilePath);
            waveOut.Init(audioFileReader);
            _mediaPlayer.Open(new Uri(FileNames[selectIndex].FilePath));            
            //加载音乐时长
            while (!_mediaPlayer.NaturalDuration.HasTimeSpan)
            {
                Thread.Sleep(10);
            }
            
            _mediaPlayer.Play();
            waveOut.Play();
            waveOut.Volume = 0;
            IsPlaying = true;
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
            UpdateAmplitudes();
        }

        /// <summary>
        /// 音乐暂停
        /// </summary>
        public void PauseMusic()
        {
            _mediaPlayer.Pause();
            waveOut.Pause();
            // 停止定时器
            UpdateProgressTimer.Stop();
            IsPlaying = false;
            PlayOrPausePicture = "pack://application:,,,/Resources/Pictures/PlayMusic.png";
        }

        /// <summary>
        /// 音乐停止
        /// </summary>
        public void StopMusic()
        {
            _mediaPlayer.Stop();
            waveOut.Stop();
            Lines.Clear();
            // 停止定时器
            UpdateProgressTimer.Stop();
            IsPlaying = false;
        }
        
        /// <summary>
        /// 上一曲事件
        /// </summary>
        public void LastSong()
        {
            IsDoubleClick = true;
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
        public void NextSong()
        {
            IsDoubleClick = true;
            SelectIndex = (SelectIndex + num) % FileNames.Count;
            PlayMusic();
        }

        /// <summary>
        /// 播放或暂停
        /// </summary>
        public void PlayOrPauseSong()
        {
            if(SelectIndex!=-1)
            {
                if (IsPlaying)
                {
                    PauseMusic();
                }
                else
                {
                    _mediaPlayer.Play();
                    waveOut.Play();
                    IsPlaying = true;
                    PlayOrPausePicture = "pack://application:,,,/Resources/Pictures/StopMusic.png";
                    UpdateProgressTimer.Start();
                    UpdateAmplitudes();
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

        private void AddLine(Point startPoint, Point endPoint)
        {
            Lines.Add(new LineModel { StartPoint = startPoint, EndPoint = endPoint });
        }

        private async void UpdateAmplitudes()
        {
            const int bufferSize = 64;
            var buffer = new float[bufferSize];
            while (IsPlaying)
            {
                int samplesRead = audioFileReader.Read(buffer, 0, bufferSize);
                if (samplesRead == 0)
                {
                    break;
                }

                //if (Lines != null&&Lines.Count()!=0)
                //{
                //    Lines.Clear();
                //}
                Point startPoint;
                Point endPoint;
                for (int i = 0; i < samplesRead; i++)
                {
                    double x = (i / (double)samplesRead) * 350;
                    double y = (buffer[i] + 1) * 0.55 * 180;
                    startPoint = new Point(x, (180 - y)*1.4);
                    endPoint = new Point(x, y*1.4);
                    AddLine(startPoint, endPoint);
                }
                await Task.Delay(50);
                Lines.Clear();
            }
        }

        #endregion
    }
}
