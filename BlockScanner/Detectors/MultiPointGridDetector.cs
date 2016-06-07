﻿namespace BlockScanner.Detectors
{
    using System.Collections.Generic;
    using System.Drawing;

    public class MultiPointGridDetector : BaseGridDetector<bool>
    {
        public override bool AnalyseSample(byte[] rgbValues, int x, int y)
        {
            var samplePoints = new HashSet<Point>();

            samplePoints.Add(new Point(x, y));
            samplePoints.Add(new Point(x-3, y-3));
            samplePoints.Add(new Point(x+3, y+3));

            bool output = false; 

            foreach (var point in samplePoints)
            {
                var index = CoordinatesToIndex(point.X, point.Y);

                Color pixelColor = Color.FromArgb(
                    255, // Assume our alpha is always 255 (we aren't capturing alpha in our bitmaps).
                    rgbValues[index + 2], // R component
                    rgbValues[index + 1], // G component
                    rgbValues[index]      // B component
                    );

                // Simple stab at colour detection.
                if (pixelColor.R > 90
                    || pixelColor.B > 95
                    || pixelColor.G > 80
                    || (pixelColor.R + pixelColor.G > 100) && pixelColor.B < 20 // Take a punt at the yellows/oranges.
                    )
                    output = true;
                else
                {
                    //Don't consider this block a hit.
                    output = false;
                    break;
                }
            }

            return output;
        }

        public override void HighlightSamplePoint(byte[] rgbValues, int x, int y)
        {
            var samplePoints = new HashSet<Point>();

            samplePoints.Add(new Point(x, y));
            samplePoints.Add(new Point(x - 3, y - 3));
            samplePoints.Add(new Point(x + 3, y + 3));

            foreach (var point in samplePoints)
            {
                var index = CoordinatesToIndex(point.X, point.Y);

                rgbValues[index + 2] = 255;
                rgbValues[index + 1] = 255;
                rgbValues[index] = 255;
            }
        }
    }
}