namespace BlockScanner.Wpf.Views
{
    using BlockScanner.Wpf.ViewModels;
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Shapes;

    /// <summary>
    /// Interaction logic for CaptureWindow.xaml
    /// </summary>
    // Capture technique based on this article: http://www.abhisheksur.com/2010/04/screen-capture-using-wpf-winforms.html
    public partial class PersistentCaptureWindow : Window
    {
        private double x;
        private double y;
        private double width;
        private double height;
        private bool isMouseDown = false;
        private Point startPoint;

        public double SelectionX { get; private set; }
        public double SelectionY { get; private set; }
        public double SelectionWidth { get; private set; }
        public double SelectionHeight { get; private set; }

        public Rect SelectionArea { get; private set; }
        public Rectangle CaptureRect { get; private set; }
        public CaptureRegionViewModel CaptureRegion { get; private set; } = new CaptureRegionViewModel();

        public System.Drawing.Rectangle SelectionAreaRectangle
            => new System.Drawing.Rectangle((int)SelectionArea.X, (int)SelectionArea.Y, (int)SelectionArea.Width, (int)SelectionArea.Height);

        public PersistentCaptureWindow()
        {
            InitializeComponent();

            // Allow window to maximise on any monitor.
            // http://stackoverflow.com/questions/3121900/how-can-i-make-a-wpf-window-maximized-on-the-screen-with-the-mouse-cursor

            //this.SourceInitialized += (s, a) => this.WindowState = WindowState.Maximized;
        }

        private void Window_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            return;
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

                    this.SelectionArea = new Rect(topLeft, new Size(r.Width, r.Height));

                    this.Close();
                }
            }
        }

        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            //Move the Thumb to the mouse position during the drag operation
            var myThumb = sender as Thumb;
            var thumbX = Canvas.GetLeft(myThumb);
            var thumbY = Canvas.GetTop(myThumb);

            if (e.HorizontalChange != 0 || e.VerticalChange != 0)
            {
                var newThumbX = thumbX + e.HorizontalChange;
                var newThumbY = thumbY + e.VerticalChange;
                Canvas.SetLeft(myThumb, newThumbX);
                Canvas.SetTop(myThumb, newThumbY);

                var newRectX = Canvas.GetLeft(MyRectangle);
                var newRectY = Canvas.GetTop(MyRectangle);
                var newRectWidth = MyRectangle.Width;
                var newRectHeight = MyRectangle.Height;

                // Resize rectangle.
                switch (myThumb.Name)
                {
                    case "myThumbTL":
                        newRectX = newThumbX;
                        newRectY = newThumbY;
                        newRectWidth = newRectWidth - e.HorizontalChange;
                        newRectHeight = newRectHeight - e.VerticalChange;
                        break;
                    case "myThumbTR":
                        newRectY = newThumbY;
                        newRectWidth = newRectWidth + e.HorizontalChange;
                        newRectHeight = newRectHeight - e.VerticalChange;
                        break;
                    case "myThumbBL":
                        newRectX = newThumbX;
                        newRectWidth = newRectWidth - e.HorizontalChange;
                        newRectHeight = newRectHeight + e.VerticalChange;
                        break;
                    case "myThumbBR":
                        newRectWidth = newRectWidth + e.HorizontalChange;
                        newRectHeight = newRectHeight + e.VerticalChange;
                        break;
                    case "MyRectangle":
                        newRectX = newThumbX;
                        newRectY = newThumbY;
                        break;
                    default:
                        break;
                }

                // Set Size
                MyRectangle.Width = newRectWidth >= 0 ? newRectWidth : MyRectangle.Width;
                MyRectangle.Height = newRectHeight >= 0 ? newRectHeight : MyRectangle.Height;

                // Set Set Position 
                Canvas.SetLeft(MyRectangle, newRectX);
                Canvas.SetTop(MyRectangle, newRectY);

                // Reposition handles
                UpdateHandlePositions();
            }
        }

        public void UpdateHandlePositions()
        {
            var rectX = Canvas.GetLeft(MyRectangle);
            var rectY = Canvas.GetTop(MyRectangle);
            var rectWidth = MyRectangle.Width;
            var rectHeight = MyRectangle.Height;
            var translationX = 10;
            var translationY = 10;

            Canvas.SetLeft(MyThumbTL, rectX);
            Canvas.SetLeft(MyThumbBL, rectX);
            Canvas.SetLeft(MyThumbTR, rectX + rectWidth - translationX);
            Canvas.SetLeft(MyThumbBR, rectX + rectWidth - translationX);
            Canvas.SetTop(MyThumbTL, rectY);
            Canvas.SetTop(MyThumbTR, rectY);
            Canvas.SetTop(MyThumbBL, rectY + rectHeight - translationY);
            Canvas.SetTop(MyThumbBR, rectY + rectHeight - translationY);
        }
    }
}
