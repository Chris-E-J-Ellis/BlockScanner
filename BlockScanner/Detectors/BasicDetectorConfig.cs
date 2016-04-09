namespace BlockScanner.Detectors
{
    using System;

    public class BasicDetectorConfig : IDetectorConfig
    {
        public virtual Type ConfigType => typeof(BasicDetectorConfig);

        public virtual string Name { get; set; } = "BasicDetectorDefault";
    }
}
