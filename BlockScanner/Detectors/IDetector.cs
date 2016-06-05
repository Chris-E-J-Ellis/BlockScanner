namespace BlockScanner.Detectors
{
    using System;
    using System.Drawing;
    using Config;

    public interface IDetector
    {
        Type DetectedPointOutputType { get; }

        void Initialise(IConfigManager configurationManager = null);

        void Initialise(Bitmap sampleFrame);

        void SetCoordinatesToIndex(Func<int, int, int> coordinatesToIndex);

        void HighlightSamplePoints(byte[] rgbData, int x, int y);
    }

    public interface IDetector<T> : IDetector
    {
        T Detect(byte[] bitmapData, int x, int y);

        T[][] Detect(Bitmap frame);
    }
}
