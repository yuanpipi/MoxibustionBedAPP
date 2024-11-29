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
using System.Windows.Shapes;
using MoxibustionBedAPP.ViewModes;

namespace MoxibustionBedAPP.Views
{
    /// <summary>
    /// MainWindowCopyView.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindowCopyView : Window
    {
        public MainWindowCopyView(PlayMusicViewModel sharedPlayMusicModel)
        {
            InitializeComponent();
            DataContext=new MainWindowCopyViewModel { SharedVM = sharedPlayMusicModel };
        }
    }
}
