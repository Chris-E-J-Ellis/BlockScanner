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

        void HighlightSamplePoints(Bitmap frame);
    }

    public interface IDetector<T> : IDetector
    {
        T Detect(Bitmap frame);
    }
}
