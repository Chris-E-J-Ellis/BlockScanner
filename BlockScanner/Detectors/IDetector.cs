using System;

namespace BlockScanner.Detectors
{
    public interface IDetector<T>
    {
        Func<byte[], int, int, T> GetDetector(Func<int, int, int> coordinatesToIndex);

        void HighlightSamplePoints(byte[] rgbData, int x, int y);
    }
}
