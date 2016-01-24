using System;

namespace BlockScanner.Detectors
{
    public interface IDetector<T>
    {
        Func<byte[], int, T> GetDetector();
    }
}
