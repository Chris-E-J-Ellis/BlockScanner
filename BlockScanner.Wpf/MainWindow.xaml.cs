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
            var captureZone = new CaptureWindow();
            captureZone.ShowDialog();
            X.Text = captureZone.SelectionX.ToString();
            Y.Text = captureZone.SelectionY.ToString();
            Widthv.Text = captureZone.SelectionWidth.ToString();
            Heightv.Text = captureZone.SelectionHeight.ToString();

            scanner.PlayfieldWidthPixels = (int)captureZone.SelectionWidth;
            scanner.PlayfieldHeightPixels = (int)captureZone.SelectionHeight;
            scanner.PlayfieldXCoord = (int)captureZone.SelectionX;
            scanner.PlayfieldYCoord = (int)captureZone.SelectionY;

            Task scanTask = new Task(() => { scanner.Start(); });

            if (!scanner.Scanning)
                scanTask.Start();
        }

        private void DumpScanArea_Click(object sender, RoutedEventArgs e)
        {
            scanner.DumpScanArea("Images/cap.bmp");
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();
    }
}
