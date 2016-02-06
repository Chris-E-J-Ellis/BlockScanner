using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace BlockScanner.Wpf
{
    /// <summary>
    /// Interaction logic for CaptureWindow.xaml
    /// </summary>
    // Capture technique based on this article: http://www.abhisheksur.com/2010/04/screen-capture-using-wpf-winforms.html
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

            // Allow window to maximise on any monitor.
            // http://stackoverflow.com/questions/3121900/how-can-i-make-a-wpf-window-maximized-on-the-screen-with-the-mouse-cursor

            this.SourceInitialized += (s, a) => this.WindowState = WindowState.Maximized;
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

                    // Convert from Wpf coordinates;
                    Point topLeft = this.PointToScreen(new Point(Canvas.GetLeft(r), Canvas.GetTop(r)));
                    
                    this.SelectionHeight = r.Height;
                    this.SelectionWidth = r.Width;
                    this.SelectionX = topLeft.X;
                    this.SelectionY = topLeft.Y;

                    this.Close();
                }
            }
        }
    }
}
