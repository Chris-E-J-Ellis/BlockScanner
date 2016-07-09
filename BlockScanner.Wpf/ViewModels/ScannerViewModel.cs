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

        public Rectangle ScanArea => Scanner == null ? new Rectangle() : Scanner.PlayfieldArea;

        public BitmapImage CapturePreview { get; private set; }

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
        }

        public void SingleScan()
        {
            // Cancel any existing scanning task.
            cancellationTokenSource.Cancel();

            Scanner.ScanOnce();
        }

        public void DumpScanArea()
        {
            var previewLocation = Path.Combine(Environment.CurrentDirectory, "Images", "cap.bmp");

            // TODO: Move path to config.
            var preview = Scanner.DumpScanArea(previewLocation);

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
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
