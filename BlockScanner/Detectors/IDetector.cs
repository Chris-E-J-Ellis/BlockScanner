using System;

namespace BlockScanner.Detectors
{
    public interface IDetector
    {
        Func<byte[], int, bool> GetDetector();
    }
}
