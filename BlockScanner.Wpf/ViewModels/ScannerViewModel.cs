namespace BlockScanner.Wpf.ViewModels
{
    using System;
    using System.Drawing;
    using System.Threading;
    using Caliburn.Micro;

    public class ScannerViewModel : PropertyChangedBase, IDisposable
    {
        public IScanner Scanner { get; }

        public Rectangle ScanArea => Scanner.PlayfieldArea;

        public ScannerViewModel(IScanner scanner)
        {
            Scanner = scanner;
        }

        public void SetScanArea(Rectangle sourceRectangle)
        {
            Scanner.Initialise(sourceRectangle);

            NotifyOfPropertyChange(() => ScanArea);
        }

        internal void Scan(CancellationToken token)
        {
            Scanner.Scan(token);
        }

        public void Dispose()
        {
            //Stop Scanning task.
        }
    }
}
