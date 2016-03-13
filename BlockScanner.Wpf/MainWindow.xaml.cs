using BlockScanner.Detectors;
using BlockScanner.Rendering;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;

namespace BlockScanner.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowViewModel mainViewModel = new MainWindowViewModel();
        private CaptureWindowViewModel captureViewModel = new CaptureWindowViewModel();

        private IScanner scanner;

        public MainWindow()
        {
            InitializeComponent();

            // Spawn Console for temp output monitoring.
            AllocConsole();

            this.DataContext = mainViewModel;

            mainViewModel.Initialise();

            scanner = ScannerFactory.CreateBasic(); 
        }

        private void BeginSelection_Click(object sender, RoutedEventArgs e)
        {
            scanner.Stop();

            var captureZone = new CaptureWindow(captureViewModel);
            captureZone.ShowDialog();

            XCoord.Text = captureZone.SelectionX.ToString();
            YCoord.Text = captureZone.SelectionY.ToString();
            ScanWidth.Text = captureZone.SelectionWidth.ToString();
            ScanHeight.Text = captureZone.SelectionHeight.ToString();

            scanner.Initialise(
                new Rectangle((int)captureZone.SelectionX, (int)captureZone.SelectionY, (int)captureZone.SelectionWidth, (int)captureZone.SelectionHeight));

            // Rough, whilst I sort out what this is going to look like.
            // Should use a cancellation token.
            Task scanTask = new Task(() => { scanner.Start(); });

            if (!scanner.Scanning)
                scanTask.Start();
        }

        private void DumpScanArea_Click(object sender, RoutedEventArgs e)
        {
            scanner.DumpScanArea("Images/cap.bmp");
        }

        private void RunTestArea_Click(object sender, RoutedEventArgs e)
        {
            scanner.Initialise(new Rectangle(0, 0, 400, 400));

            Task scanTask = new Task(() => { scanner.Start(); });

            if (!scanner.Scanning)
                scanTask.Start();
        }

        private void CreateScanner_Click(object sender, RoutedEventArgs e)
        {
            var detector = mainViewModel.SelectedDetector;
            var renderer = mainViewModel.SelectedRenderer;

            var scanner = ScannerFactory.Create(detector, renderer);
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();
    }
}
