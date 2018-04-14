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
        public double SelectionX { get; private set; }
        public double SelectionY { get; private set; }
        public double SelectionWidth { get; private set; }
        public double SelectionHeight { get; private set; }

        public Rect SelectionArea { get; private set; }
        public CaptureRegionViewModel CaptureRegion { get; private set; } = new CaptureRegionViewModel();

        public System.Drawing.Rectangle SelectionAreaRectangle
            => new System.Drawing.Rectangle((int)SelectionArea.X, (int)SelectionArea.Y, (int)SelectionArea.Width, (int)SelectionArea.Height);

        public PersistentCaptureWindow()
        {
            InitializeComponent();

            // Allow window to maximise on any monitor.
            // http://stackoverflow.com/questions/3121900/how-can-i-make-a-wpf-window-maximized-on-the-screen-with-the-mouse-cursor

            this.SourceInitialized += (s, a) => this.WindowState = WindowState.Maximized;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var screenCoordinates = this.PointToScreen(new Point(Canvas.GetLeft(CaptureTest.Region), Canvas.GetTop(CaptureTest.Region)));
            this.SelectionArea = new Rect(screenCoordinates, new Size(CaptureTest.Region.Width, CaptureTest.Region.Height));

            this.Close();
        }
    }
}
