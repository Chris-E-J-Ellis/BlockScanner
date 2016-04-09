namespace BlockScanner.Detectors
{
    using Configuration;
    using System.Drawing;

    public class ConfigurableBasicDetector : BaseDetector<bool>
    {
        private ConfigurableBasicDetectorConfig configuration = new ConfigurableBasicDetectorConfig();

        public override IDetectorConfig Configuration => this.configuration;

        public override void Initialise(IConfigurationManager configurationManager)
        {
            this.configuration = configurationManager.Load<ConfigurableBasicDetectorConfig>("Default")
                ?? new ConfigurableBasicDetectorConfig();
        }

        public override bool Detect(byte[] bitmapData, int x, int y)
        {
            var index = CoordinatesToIndex(x, y);

            Color pixelColor = Color.FromArgb(
                255, // Assume our alpha is always 255 (we aren't capturing alpha in our bitmaps).
                bitmapData[index + 2], // R component
                bitmapData[index + 1], // G component
                bitmapData[index]      // B component
                );

            // Simple stab at colour detection.
            if (pixelColor.R > configuration.RedMaxThreshold
                || pixelColor.B > configuration.BlueMaxThreshold
                || pixelColor.G > configuration.GreenMaxThreshold
                || (pixelColor.R + pixelColor.G > configuration.YellowOrangeRGMinThreshold)
                    && pixelColor.B < configuration.YellowOrangeBMaxThreshold // Take a punt at the yellows/oranges.
                )
                return true;

            return false;
        }

        public override void HighlightSamplePoints(byte[] rgbValues, int x, int y)
        {
            var index = CoordinatesToIndex(x, y);

            rgbValues[index + 2] = 255;
            rgbValues[index + 1] = 255;
            rgbValues[index] = 255;
        }
    }
}
