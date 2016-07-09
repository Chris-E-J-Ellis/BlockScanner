namespace BlockScanner.Config.Detectors
{
    // First pass, just some rough values.
    public class BasicDetectorConfig : BasicConfig<BasicDetectorConfig>
    {
        public int RedMaxThreshold { get; set; } = 110;

        public int GreenMaxThreshold { get; set; } = 110;

        public int BlueMaxThreshold { get; set; } = 100;

        public int YellowOrangeRGMinThreshold { get; set; } = 100;

        public int YellowOrangeBMaxThreshold { get; set; } = 20;
    }
}
