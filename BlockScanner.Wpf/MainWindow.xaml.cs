using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BlockScanner.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BeginSelection_Click(object sender, RoutedEventArgs e)
        {
            var captureZone = new CaptureWindow();
            captureZone.ShowDialog();
            X.Text = captureZone.SelectionX.ToString();
            Y.Text = captureZone.SelectionY.ToString();
            Width.Text = captureZone.SelectionWidth.ToString();
            Height.Text = captureZone.SelectionHeight.ToString();
        }
    }
}
