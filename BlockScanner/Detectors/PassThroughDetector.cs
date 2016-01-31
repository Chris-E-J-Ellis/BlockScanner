using System;
using System.Drawing;

namespace BlockScanner.Detectors
{
    public class PassThroughDetector : IDetector<Color>
    {
        public Func<byte[], int, int, Color> GetDetector(Func<int, int, int> coordinatesToIndex)
        {
            Func<byte[], int, int, Color> passThroughDetectorFunc =
                (rgbValues, x, y) =>
                {
                    var index = coordinatesToIndex(x, y);

                    Color pixelColor = Color.FromArgb(
                        //pixelSize == 3 ? 255 : rgbValues[index + 3], // A component if present
                        255, // Assume our alpha is always 255 (we aren't capturing alpha in our bitmaps).
                        rgbValues[index + 2], // R component
                        rgbValues[index + 1], // G component
                        rgbValues[index]      // B component
                        );

                    return pixelColor;
                };

            return passThroughDetectorFunc;
        }

        public void HighlightSamplePoints(byte[] rgbData, int x, int y)
        {
        }
    }
}
