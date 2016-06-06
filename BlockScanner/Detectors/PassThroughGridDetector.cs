namespace BlockScanner.Detectors
{
    using System.Drawing;

    public class PassThroughGridDetector : BaseGridDetector<Color>
    {
        public override Color AnalyseSample(byte[] bitmapData, int x, int y)
        {
            var index = this.CoordinatesToIndex(x, y);

            Color pixelColor = Color.FromArgb(
                //pixelSize == 3 ? 255 : rgbValues[index + 3], // A component if present
                255, // Assume our alpha is always 255 (we aren't capturing alpha in our bitmaps).
                bitmapData[index + 2], // R component
                bitmapData[index + 1], // G component
                bitmapData[index]      // B component
                );

            return pixelColor;
        }
    }
}
