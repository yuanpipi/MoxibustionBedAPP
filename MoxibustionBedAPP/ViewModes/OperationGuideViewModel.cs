using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using MoxibustionBedAPP.Models;
using Newtonsoft.Json;

namespace MoxibustionBedAPP.ViewModes
{
    public class OperationGuideViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string _myText;
        public string MyText
        {
            get
            {
                return _myText;
            }
            set
            {
                System.Diagnostics.Debug.WriteLine($"RichTextContent is being set to: {value}");
                _myText = value;
                OnPropertyChanged(nameof(MyText));
            }
        }
        private string _myFirstText;
        public string MyFirstText
        {
            get
            {
                return _myFirstText;
            }
            set
            {
                _myFirstText = value;
                OnPropertyChanged(nameof(MyFirstText));
            }
        }

        private bool isClick;
        public bool IsClick
        {
            get
            {
                return isClick;
            }
            set
            {
                isClick = value;
                OnPropertyChanged(nameof(IsClick));
            }
        }

        /// <summary>
        /// 操作指南点击事件
        /// </summary>
        public ICommand GuideClick { get; private set; }

        /// <summary>
        /// 禁忌症点击事件
        /// </summary>
        public ICommand ContraindicationsClick { get; private set; }

        public ICommand ReturnClick { get; private set; }

        public OperationGuideViewModel()
        {
            GuideClick = new RelayCommand(GuideClickCommand);
            ContraindicationsClick=new RelayCommand(ContraindicationsClickCommand);
            ReturnClick=new RelayCommand(ReturnClickCommand);
            IsClick = false;
            MyText = ReadFile("Contraindications.txt");
        }

        private void GuideClickCommand()
        {
            MyText = ReadFile("Contraindications.txt");
        }
        

        private void ContraindicationsClickCommand()
        {
            MyText = ReadFile("Guide.txt");
        }


        private void ReturnClickCommand()
        {
            App.PropertyModelInstance.IsClickQ=!App.PropertyModelInstance.IsClickQ;
        }

        private string ReadFile(string fileName)
        {
            if (File.Exists(fileName))//判断文件是否存在
            {
                try
                {
                    string data = File.ReadAllText(fileName);
                    return data;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            return null;
        }

    }
}
