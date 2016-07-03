namespace BlockScanner
{
    using Detectors;
    using System;
    using System.Drawing;
    using System.Threading;

    public interface IScanner
    {
        Rectangle PlayfieldArea { get; }
        IDetector Detector { get; }

        Bitmap DumpScanArea(string path);
        void Initialise(Rectangle playfield);
        void Scan(CancellationToken token);
        void ScanOnce();
    }

    public interface IScanner<T> : IScanner
    {
        T AnalyseFrame(Bitmap frame);

        event EventHandler<T> FrameScanned;
    }
}
