namespace BlockScanner.Detectors
{
    using Configuration;
    using System;

    public interface IDetector
    {
        Type DetectedPointOutputType { get; } 

        IDetectorConfig Configuration { get; }

        void Initialise(IConfigurationManager configurationManager = null);

        void SetCoordinatesToIndex(Func<int, int, int> coordinatesToIndex);

        void HighlightSamplePoints(byte[] rgbData, int x, int y);
    }

    public interface IDetector<T> : IDetector
    {
        T Detect(byte[] bitmapData, int x, int y);
    }
}
