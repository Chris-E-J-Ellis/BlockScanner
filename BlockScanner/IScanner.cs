namespace BlockScanner
{
    using System.Drawing;
    using System.Threading;

    public interface IScanner
    {
        Rectangle PlayfieldArea { get; }

        void DumpScanArea(string path);
        void Initialise(Rectangle playfield);
        void Scan(CancellationToken token);
    }

    public interface IScanner<T> : IScanner
    {
        T[][] AnalyseFrame(Bitmap frame);
    }
}
