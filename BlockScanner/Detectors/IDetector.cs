namespace BlockScanner.Detectors
{
    using System;
    using System.Drawing;

    public interface IDetector
    {
        Type DetectedPointOutputType { get; }

        void Initialise();

        void InitialiseFromFrame(Bitmap sampleFrame);

        void SetCoordinatesToIndex(Func<int, int, int> coordinatesToIndex);

        void HighlightSamplePoints(Bitmap frame);
    }

    public interface IDetector<T> : IDetector
    {
        T Detect(Bitmap frame);
    }
}
