using System;

namespace BlockScanner.Detectors
{
    public interface IDetectorConfig
    {
        string Name { get; set; }

        Type ConfigType { get; }
    }
}
