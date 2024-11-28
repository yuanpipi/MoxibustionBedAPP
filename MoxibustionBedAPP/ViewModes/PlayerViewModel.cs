using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using MoxibustionBedAPP.Models;

namespace MoxibustionBedAPP.ViewModes
{
    public class PlayerViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private MediaPlayer _mediaPlayer;
        private ObservableCollection<MusicModel> _musicList;
        private MusicModel _currentMusic;
        private bool _isPlaying;

        public ObservableCollection<MusicModel> MusicList
        {
            get { return _musicList; }
            set
            {
                _musicList = value;
                OnPropertyChanged("MusicList");
            }
        }

        public MusicModel CurrentMusic
        {
            get { return _currentMusic; }
            set
            {
                _currentMusic = value;
                OnPropertyChanged("CurrentMusic");
            }
        }

        public bool IsPlaying
        {
            get { return _isPlaying; }
            set 
            {
                _isPlaying = value;
                OnPropertyChanged("IsPlaying");
            }
        }

        //public PlayerViewModel()
        //{
        //    string musicFolderPath = @".\Resources\Music";
        //    try
        //    {
        //        foreach (string file in Directory.GetFiles(musicFolderPath))
        //        {
        //            if (file.EndsWith(".mp3") || file.EndsWith(".wav"))
        //            {
        //                MusicModel music = new MusicModel
        //                {
        //                    FilePath = file,
        //                    MusicName = Path.GetFileNameWithoutExtension(file)
        //                };
        //                MusicList.Add(music);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //}

        /// <summary>
        /// 播放音乐
        /// </summary>
        public void PlayMusic(MusicModel music)
        {
            //""new Uri(music.FilePath)
            _mediaPlayer.Open(new Uri(music.FilePath));
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
