namespace BlockScanner.Config
{
    using System;
    using System.Drawing;

    [Serializable]
    public class ScannerConfig : BasicConfig<ScannerConfig>
    {
        public Rectangle ScanArea { get; set; } = new Rectangle(0, 0, 0, 0);

        public int SamplePointCentreHeightRatio { get; set; } = 3;

        public int SamplePointCentreWidthRatio { get; set; } = 2;

        public int GridWidth { get; set; } = 100;

        public int GridHeight { get; set; } = 100;
    }
}
