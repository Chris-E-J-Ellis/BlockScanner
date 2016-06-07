﻿namespace BlockScanner.Detectors
{
    using Config;
    using Config.Detectors;
    using System.Drawing;

    public class ConfigurableBasicGridDetector : BaseGridDetector<bool>, IConfigurable<ConfigurableBasicDetectorConfig>
    {
        public ConfigurableBasicDetectorConfig Config { get; private set; } = new ConfigurableBasicDetectorConfig();

        public void SetConfig(ConfigurableBasicDetectorConfig config)
        {
            this.Config = config;
        }

        public override void Initialise(IConfigManager configurationManager)
        {
            this.Config = configurationManager.Load<ConfigurableBasicDetectorConfig>("Default")
                ?? new ConfigurableBasicDetectorConfig();
        }

        public override bool AnalyseSample(byte[] bitmapData, int x, int y)
        {
            var index = CoordinatesToIndex(x, y);

            Color pixelColor = Color.FromArgb(
                255, // Assume our alpha is always 255 (we aren't capturing alpha in our bitmaps).
                bitmapData[index + 2], // R component
                bitmapData[index + 1], // G component
                bitmapData[index]      // B component
                );

            // Simple stab at colour detection.
            if (pixelColor.R > Config.RedMaxThreshold
                || pixelColor.B > Config.BlueMaxThreshold
                || pixelColor.G > Config.GreenMaxThreshold
                || (pixelColor.R + pixelColor.G > Config.YellowOrangeRGMinThreshold)
                    && pixelColor.B < Config.YellowOrangeBMaxThreshold // Take a punt at the yellows/oranges.
                )
                return true;

            return false;
        }
    }
}