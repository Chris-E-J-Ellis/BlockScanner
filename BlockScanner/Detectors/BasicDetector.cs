using System;
using System.Drawing;

namespace BlockScanner.Detectors
{
    public class BasicDetector : IDetector<bool>
    {
        public Func<int, int, int> coordinatesToIndexFunc;

        public Func<byte[], int, int, bool> GetDetector(Func<int, int, int> coordinatesToIndex)
        {
            coordinatesToIndexFunc = coordinatesToIndex;

            Func<byte[], int, int, bool> basicDetectorFunc =
                (rgbValues, x, y) =>
            {
                var index = coordinatesToIndexFunc(x, y);

                Color pixelColor = Color.FromArgb(
                    //pixelSize == 3 ? 255 : rgbValues[index + 3], // A component if present
                    255, // Assume our alpha is always 255 (we aren't capturing alpha in our bitmaps).
                    rgbValues[index + 2], // R component
                    rgbValues[index + 1], // G component
                    rgbValues[index]      // B component
                    );

                // Simple stab at colour detection.
                if (pixelColor.R > 110
                    || pixelColor.B > 100
                    || pixelColor.G > 100
                    || (pixelColor.R + pixelColor.G > 100) && pixelColor.B < 20 // Take a punt at the yellows/oranges.
                    )
                    return true;

                return false;
            };

            return basicDetectorFunc;
        }

        public void HighlightSamplePoints(byte[] rgbValues, int x, int y)
        {
            var index = coordinatesToIndexFunc(x, y);

            rgbValues[index + 2] = 255;
            rgbValues[index + 1] = 255;
            rgbValues[index] = 255;
        }
    }
}
