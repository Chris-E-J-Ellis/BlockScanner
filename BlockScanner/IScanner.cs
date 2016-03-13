using System.Drawing;

namespace BlockScanner
{
    public interface IScanner
    {
        Rectangle PlayfieldArea { get; }
        bool Scanning { get; }

        void DumpScanArea(string path);
        void Initialise(Rectangle playfield);
        void Start();
        void Stop();
        void Scan();
    }

    public interface IScanner<T> : IScanner
    {
        T[][] AnalyseFrame(Bitmap frame);
    }
}
