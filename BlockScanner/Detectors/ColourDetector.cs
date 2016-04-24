namespace BlockScanner.Detectors
{
    using System.Drawing;

    public class ColourDetector : BaseDetector<Color>
    {
        public override Color Detect(byte[] bitmapData, int x, int y)
        {
            var index = CoordinatesToIndex(x, y);

            Color pixelColor = Color.FromArgb(
                //pixelSize == 3 ? 255 : rgbValues[index + 3], // A component if present
                255, // Assume our alpha is always 255 (we aren't capturing alpha in our bitmaps).
                bitmapData[index + 2], // R component
                bitmapData[index + 1], // G component
                bitmapData[index]      // B component
                );

            // Simple stab at colour detection.
            if (pixelColor.R > 110
                || pixelColor.B > 110
                || pixelColor.G > 100
                || (pixelColor.R + pixelColor.G > 100) && pixelColor.B < 20 // Take a punt at the yellows/oranges.
                )
                return pixelColor;

            return Color.Black;
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
