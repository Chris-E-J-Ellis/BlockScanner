namespace BlockScanner.Config.Detectors
{
    // First pass, just some rough values.
    public class BasicGridDetectorConfig : BasicConfig<BasicGridDetectorConfig>
    {
        // Grid scan options.
        public int SamplePointCentreHeightRatio { get; set; } = 3;

        public int SamplePointCentreWidthRatio { get; set; } = 2;

        public int GridWidth { get; set; } = 20;

        public int GridHeight { get; set; } = 20;

        // Colour options.

        public int RedMaxThreshold { get; set; } = 110;

        public int GreenMaxThreshold { get; set; } = 110;

        public int BlueMaxThreshold { get; set; } = 100;

        public int YellowOrangeRGMinThreshold { get; set; } = 100;

        public int YellowOrangeBMaxThreshold { get; set; } = 20;
    }
}
