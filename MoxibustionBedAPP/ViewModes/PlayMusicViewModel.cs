using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
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

        private ObservableCollection<MusicModel> _fileNames;
        public PlayerViewModel player;

        public RelayCommand ItemSelectedCommand { get; set; }
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
        public PlayMusicViewModel()
        {
            //FileNames=MainWindowViewModel.FileNames;
            ReadFileNamesFromFolder(@".\Resources\Music");
            //ButtonClickCommand = new RelayCommand(Click);
            ItemSelectedCommand=new RelayCommand(OnItemSelected);
            player=new PlayerViewModel();
            Name = "Song Name";
            SelectIndex = -1;
        }

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
                            MusicName = name.Insert(name.LastIndexOfAny(new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' }) + 1, " - ")
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

        private MediaPlayer _mediaPlayer = new MediaPlayer();
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
        /// 播放音乐
        /// </summary>
        public void PlayMusic()
        {
            _mediaPlayer.Open(new Uri(FileNames[selectIndex].FilePath));
            _mediaPlayer.Play();
            IsPlaying = true;
        }

        /// <summary>
        /// 音乐暂停
        /// </summary>
        public void PauseMusic()
        {
            _mediaPlayer.Pause();
            IsPlaying = false;
        }

        /// <summary>
        /// 音乐停止
        /// </summary>
        public void StopMusic()
        {
            _mediaPlayer.Stop();
            IsPlaying = false;
        }
    }
}
