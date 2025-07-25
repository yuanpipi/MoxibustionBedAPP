﻿using System;
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
    /// OperationGuideView.xaml 的交互逻辑
    /// </summary>
    public partial class OperationGuideView : UserControl
    {
        private OperationGuideViewModel viewmodel = new OperationGuideViewModel();
        public OperationGuideView()
        {
            InitializeComponent();
            this.DataContext = viewmodel;
        }

        private void RichTextBox_ManipulationBoundaryFeedback(object sender, ManipulationBoundaryFeedbackEventArgs e)
        {
            e.Handled = true; // 禁用边界弹跳效果
        }
    }
}
