namespace BlockScanner.Wpf.ViewModels
{
    using System;
    using System.Drawing;
    using System.Threading;
    using System.Threading.Tasks;
    using System.IO;
    using System.Drawing.Imaging;
    using System.Windows.Media.Imaging;
    using Caliburn.Micro;

    public class ScannerViewModel : PropertyChangedBase, IDisposable
    {
        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        public ScannerViewModel(IScanner scanner)
        {
            Scanner = scanner;
        }

        public IScanner Scanner { get; }

        public string DetectorName => Scanner?.Detector?.ToString();

        public Rectangle ScanArea => Scanner == null ? new Rectangle() : Scanner.PlayfieldArea;

        public BitmapImage CapturePreview { get; private set; }

        public bool IsScanning => cancellationTokenSource != null && !cancellationTokenSource.IsCancellationRequested;

        public void SetScanArea(Rectangle sourceRectangle)
        {
            Scanner.Initialise(sourceRectangle);

            NotifyOfPropertyChange(() => ScanArea);
        }

        public void Scan()
        {
            // Cancel any existing scanning task.
            cancellationTokenSource.Cancel();

            cancellationTokenSource = new CancellationTokenSource();

            Task scanTask = new Task(() => Scanner.Scan(cancellationTokenSource.Token), cancellationTokenSource.Token);

            scanTask.Start();

            NotifyOfPropertyChange(() => IsScanning);
        }

        public void SingleScan()
        {
            // Cancel any existing scanning task.
            cancellationTokenSource.Cancel();

            Scanner.ScanOnce();

            NotifyOfPropertyChange(() => IsScanning);
        }

        public void DumpScanArea()
        {
            var previewLocation = "Images";
            var previewName = "cap.bmp";
            var previewPath = Path.Combine(Environment.CurrentDirectory, previewLocation);

            // Not an ideal side effect to have here, may warrent a move.
            try
            {
                Directory.CreateDirectory(previewPath);
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"An exception occurred whilst creating a directory to store the captured image: {ex}");
            }

            // TODO: Move path to config.
            var preview = Scanner.DumpScanArea(Path.Combine(previewPath, previewName));

            // Quick preview: http://stackoverflow.com/questions/94456/load-a-wpf-bitmapimage-from-a-system-drawing-bitmap
            using (MemoryStream memory = new MemoryStream())
            {
                preview.Save(memory, ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                CapturePreview = bitmapImage;
            }

            NotifyOfPropertyChange(() => CapturePreview);
        }

        public void RunTestArea()
        {
            SetScanArea(new Rectangle(0, 0, 400, 400));

            DumpScanArea();

            Scan();
        }

        public void Stop()
        {
            //Stop Scanning task.
            cancellationTokenSource.Cancel();

            Scanner?.ShutDown();

            NotifyOfPropertyChange(() => IsScanning);
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
