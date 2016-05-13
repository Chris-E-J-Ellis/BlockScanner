namespace BlockScanner.Wpf.ViewModels
{
    using System;
    using System.Drawing;
    using System.Threading;
    using Caliburn.Micro;
    using System.Threading.Tasks;

    public class ScannerViewModel : PropertyChangedBase, IDisposable
    {
        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        public ScannerViewModel(IScanner scanner)
        {
            Scanner = scanner;
        }

        public IScanner Scanner { get; }

        public Rectangle ScanArea => Scanner.PlayfieldArea;

        public void SetScanArea(Rectangle sourceRectangle)
        {
            Scanner.Initialise(sourceRectangle);

            NotifyOfPropertyChange(() => ScanArea);
        }

        internal void Scan()
        {
            // Cancel any existing scanning task.
            cancellationTokenSource.Cancel();

            cancellationTokenSource = new CancellationTokenSource();
            
            Task scanTask = new Task(() => Scanner.Scan(cancellationTokenSource.Token), cancellationTokenSource.Token);

            scanTask.Start();
        }

        public void DumpScanArea()
        {
            // TODO: Move path to config.
            Scanner.DumpScanArea("Images/cap.bmp");
        }

        public void Dispose()
        {
            //Stop Scanning task.
            cancellationTokenSource.Cancel();
        }
    }
}
