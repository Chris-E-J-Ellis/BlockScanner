using BlockScanner.Detectors;
using BlockScanner.Rendering;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
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
        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

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
            var captureZone = new CaptureWindow(captureViewModel);
            captureZone.ShowDialog();

            XCoord.Text = captureZone.SelectionX.ToString();
            YCoord.Text = captureZone.SelectionY.ToString();
            ScanWidth.Text = captureZone.SelectionWidth.ToString();
            ScanHeight.Text = captureZone.SelectionHeight.ToString();

            // Should use a cancellation token.
            cancellationTokenSource.Cancel();

            cancellationTokenSource = new CancellationTokenSource();

            Task scanTask = new Task(() =>
            {
                scanner.Initialise(
                    new Rectangle((int)captureZone.SelectionX, (int)captureZone.SelectionY, (int)captureZone.SelectionWidth, (int)captureZone.SelectionHeight));

                scanner.Scan(cancellationTokenSource.Token);
            }, cancellationTokenSource.Token);

            scanTask.Start();
        }

        private void DumpScanArea_Click(object sender, RoutedEventArgs e)
        {
            scanner.DumpScanArea("Images/cap.bmp");
        }

        private void RunTestArea_Click(object sender, RoutedEventArgs e)
        {
            cancellationTokenSource.Cancel();

            cancellationTokenSource = new CancellationTokenSource();

            Task scanTask = new Task(() => 
            {
                scanner.Initialise(new Rectangle(0, 0, 400, 400));
                scanner.Scan(cancellationTokenSource.Token);
            }, cancellationTokenSource.Token);

            scanTask.Start();
        }

        private void CreateScanner_Click(object sender, RoutedEventArgs e)
        {
            var detector = mainViewModel.SelectedDetector;
            var renderer = mainViewModel.SelectedRenderer;

            this.scanner = ScannerFactory.Create(detector, renderer);
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();
    }
}
