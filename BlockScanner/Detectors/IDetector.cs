namespace BlockScanner.Detectors
{
    using Config;
    using System;

    public interface IDetector
    {
        Type DetectedPointOutputType { get; }

        void Initialise(IConfigManager configurationManager = null);

        void SetCoordinatesToIndex(Func<int, int, int> coordinatesToIndex);

        void HighlightSamplePoints(byte[] rgbData, int x, int y);
    }

    public interface IDetector<T> : IDetector
    {
        T Detect(byte[] bitmapData, int x, int y);
    }
}
