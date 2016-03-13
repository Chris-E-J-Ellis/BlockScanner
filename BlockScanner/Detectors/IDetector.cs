namespace BlockScanner.Detectors
{
    using System;

    public interface IDetector
    {
        void SetCoordinatesToIndex(Func<int, int, int> coordinatesToIndex);

        void HighlightSamplePoints(byte[] rgbData, int x, int y);

        Type DetectedPointOutputType { get; } 
    }

    public interface IDetector<T> : IDetector
    {
        T Detect(byte[] bitmapData, int x, int y);
    }
}
