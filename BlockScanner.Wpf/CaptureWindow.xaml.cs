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
using System.Windows.Shapes;

namespace BlockScanner.Wpf
{
    /// <summary>
    /// Interaction logic for CaptureWindow.xaml
    /// </summary>
    public partial class CaptureWindow : Window
    {
        private double x;
        private double y;
        private double width;
        private double height;
        private bool isMouseDown = false;

        public double SelectionX { get; private set; }
        public double SelectionY { get; private set; }
        public double SelectionWidth { get; private set;}
        public double SelectionHeight { get; private set;}

        public CaptureWindow()
        {
            InitializeComponent();
        }

        private void Window_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            isMouseDown = true;
            x = e.GetPosition(null).X;
            y = e.GetPosition(null).Y;
        }

        private void Window_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (this.isMouseDown)
            {
                double curx = e.GetPosition(null).X;
                double cury = e.GetPosition(null).Y;

                System.Windows.Shapes.Rectangle r = new System.Windows.Shapes.Rectangle();
                SolidColorBrush brush = new SolidColorBrush(Colors.White);
                r.Stroke = brush;
                r.Fill = brush;
                r.StrokeThickness = 1;

                var recWidth = (curx - x);
                var recHeight = (cury - y);

                r.Width = Math.Abs(recWidth);
                r.Height = Math.Abs(recHeight);

                canvas.Children.Clear();
                canvas.Children.Add(r);

                if (recWidth <= 0)
                    Canvas.SetLeft(r, curx);
                else
                    Canvas.SetLeft(r, x);

                if (recHeight <= 0)
                    Canvas.SetTop(r, cury);
                else
                    Canvas.SetTop(r, y);

                if (e.LeftButton == MouseButtonState.Released)
                {
                    canvas.Children.Clear();
                    width = e.GetPosition(null).X - x;
                    height = e.GetPosition(null).Y - y;
                    this.x = this.y = 0;
                    this.isMouseDown = false;


                    this.SelectionHeight = r.Height;
                    this.SelectionWidth = r.Width;
                    this.SelectionX = curx;
                    this.SelectionY = cury;

                    this.Close();
                }
            }
        }
    }
}
