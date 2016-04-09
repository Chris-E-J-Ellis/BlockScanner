namespace BlockScanner.Detectors
{
    using System;

    class ConfigurableBasicDetectorConfig : BasicDetectorConfig
    {
        public override Type ConfigType => typeof(ConfigurableBasicDetectorConfig);

        public override string Name { get; set; } = "BasicDetectorDefault";

        public int RedMaxThreshold { get; set; } = 110;

        public int GreenMaxThreshold { get; set; } = 110;

        public int BlueMaxThreshold { get; set; } = 100;

        public int YellowOrangeRGMinThreshold { get; set; } = 100;

        public int YellowOrangeBMaxThreshold { get; set; } = 20;
    }
}
