namespace BlockScanner
{
    using Detectors;
    using Rendering;
    using System;
    using System.Drawing;
    using System.Threading;

    public interface IScanner
    {
        Rectangle PlayfieldArea { get; }
        IDetector Detector { get; }

        void AttachRenderer(IRenderer renderer);
        void DetachRenderer(IRenderer renderer);
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
