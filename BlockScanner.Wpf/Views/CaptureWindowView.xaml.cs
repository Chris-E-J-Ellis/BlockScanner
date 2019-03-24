namespace BlockScanner.Wpf.Views
{
    using System.Windows;

    /// <summary>
    /// Interaction logic for CaptureWindow.xaml
    /// </summary>
    public partial class CaptureWindowView : Window
    {
        public CaptureWindowView()
        {
            InitializeComponent();

            // Allow window to maximise on any monitor.
            // http://stackoverflow.com/questions/3121900/how-can-i-make-a-wpf-window-maximized-on-the-screen-with-the-mouse-cursor

            this.SourceInitialized += (s, a) => this.WindowState = WindowState.Maximized;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //var screenCoordinates = this.PointToScreen(new Point(Canvas.GetLeft(CaptureTest.Region), Canvas.GetTop(CaptureTest.Region)));
            //this.SelectionArea = new Rect(screenCoordinates, new Size(CaptureTest.Region.Width, CaptureTest.Region.Height));

            this.Close();
        }
    }
}
