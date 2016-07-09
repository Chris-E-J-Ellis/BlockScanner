namespace BlockScanner.Config
{
    using System;
    using System.Drawing;

    [Serializable]
    public class ScannerConfig : BasicConfig<ScannerConfig>
    {
        public Rectangle ScanArea { get; set; } = new Rectangle(0, 0, 0, 0);
    }
}
