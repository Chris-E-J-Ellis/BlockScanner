using System;
using System.Drawing;

namespace BlockScanner.Detectors
{
    public class PassThroughDetector : IDetector<Color>
    {
        public static Func<byte[], int, Color> BasicDetectorFunc =
            new Func<byte[], int, Color>((rgbValues, index) =>
            {
                Color pixelColor = Color.FromArgb(
                    //pixelSize == 3 ? 255 : rgbValues[index + 3], // A component if present
                    255, // Assume our alpha is always 255 (we aren't capturing alpha in our bitmaps).
                    rgbValues[index + 2], // R component
                    rgbValues[index + 1], // G component
                    rgbValues[index]      // B component
                    );

                return pixelColor;
            });

        public Func<byte[], int, Color> GetDetector()
        {
            return BasicDetectorFunc;
        }

        public void HighlightSamplePoints(byte[] rgbData, int index)
        {
        }
    }
}
