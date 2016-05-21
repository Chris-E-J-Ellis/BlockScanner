namespace BlockScanner
{
    using Detectors;
    using Rendering;
    using System.Drawing;
    using System.Threading;

    public interface IScanner
    {
        Rectangle PlayfieldArea { get; }
        IDetector Detector { get; }
        IRenderer Renderer { get; }

        Bitmap DumpScanArea(string path);
        void Initialise(Rectangle playfield);
        void Scan(CancellationToken token);
    }

    public interface IScanner<T> : IScanner
    {
        T[][] AnalyseFrame(Bitmap frame);
    }
}
