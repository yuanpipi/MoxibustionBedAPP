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
using System.Windows.Navigation;
using System.Windows.Shapes;
using MoxibustionBedAPP.ViewModes;

namespace MoxibustionBedAPP.Views
{
    /// <summary>
    /// ParameterSettingView.xaml 的交互逻辑
    /// </summary>
    public partial class ParameterSettingView : UserControl
    {
        public ParameterSettingView()
        {
            InitializeComponent();
            DataContext = new ParameterSettingViewModel();
        }

        private void ScrollViewer_ManipulationBoundaryFeedback(object sender, ManipulationBoundaryFeedbackEventArgs e)
        {
            e.Handled= true;
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (App.PropertyModelInstance.UpperAlarmCabinTemperature < App.PropertyModelInstance.Upper_CabinTemperature)
            {
                App.PropertyModelInstance.UpperAlarmCabinTemperature = App.PropertyModelInstance.Upper_CabinTemperature;
            }
        }

        private void Slider_ValueChanged_1(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            App.PropertyModelInstance.UpperAlarmCabinTemperature = App.PropertyModelInstance.Upper_CabinTemperature;
        }

        private void Slider_ValueChanged_2(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            App.PropertyModelInstance.BackAlarmTemperature = App.PropertyModelInstance.BackTemperature;
        }

        private void Slider_ValueChanged_3(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (App.PropertyModelInstance.BackAlarmTemperature < App.PropertyModelInstance.BackTemperature)
            {
                App.PropertyModelInstance.BackAlarmTemperature = App.PropertyModelInstance.BackTemperature;
            }
        }

        private void Slider_ValueChanged_4(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            App.PropertyModelInstance.LegAlarmTemperature=App.PropertyModelInstance.LegTemperature;
        }

        private void Slider_ValueChanged_5(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if(App.PropertyModelInstance.LegAlarmTemperature < App.PropertyModelInstance.LegTemperature)
            {
                App.PropertyModelInstance.LegAlarmTemperature = App.PropertyModelInstance.LegTemperature;
            }
        }
    }
}
