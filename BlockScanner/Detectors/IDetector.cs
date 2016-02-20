namespace BlockScanner.Detectors
{
    using System;

    public interface IDetector
    {
        void SetCoordinatesToIndex(Func<int, int, int> coordinatesToIndex);

        void HighlightSamplePoints(byte[] rgbData, int x, int y);
    }

    public interface IDetector<T> : IDetector
    {
        Func<byte[], int, int, T> GetDetector();

        Type DetectedPointOutputType { get; } 
    }
}
