using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoxibustionBedAPP.ViewModes
{
    public class SharedViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyCharged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string _displayText;
        public string DisplayText
        {
            get { return _displayText; }
            set
            { 
                _displayText = value;
                OnPropertyCharged(nameof(DisplayText));
            }
        }
    }
}
