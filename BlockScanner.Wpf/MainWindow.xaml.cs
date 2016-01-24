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
        private Scanner scanner = new Scanner();

        public MainWindow()
        {
            InitializeComponent();

            // Spawn Console for temp output monitoring.
            AllocConsole();
        }

        private void BeginSelection_Click(object sender, RoutedEventArgs e)
        {
            scanner.Stop();

            var captureZone = new CaptureWindow();
            captureZone.ShowDialog();
            XCoord.Text = captureZone.SelectionX.ToString();
            YCoord.Text = captureZone.SelectionY.ToString();
            ScanWidth.Text = captureZone.SelectionWidth.ToString();
            ScanHeight.Text = captureZone.SelectionHeight.ToString();

            scanner.PlayfieldWidthPixels = (int)captureZone.SelectionWidth;
            scanner.PlayfieldHeightPixels = (int)captureZone.SelectionHeight;
            scanner.PlayfieldXCoord = (int)captureZone.SelectionX;
            scanner.PlayfieldYCoord = (int)captureZone.SelectionY;

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
            scanner.PlayfieldWidthPixels = 400;
            scanner.PlayfieldHeightPixels = 400; 
            scanner.PlayfieldXCoord = 0;
            scanner.PlayfieldYCoord = 0; 

            Task scanTask = new Task(() => { scanner.Start(); });

            if (!scanner.Scanning)
                scanTask.Start();
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();
    }
}
