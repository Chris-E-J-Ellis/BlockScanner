using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;

namespace BlockScanner
{
    public class Scanner
    {
        private bool scanning = false;

        public Scanner() { }

        public bool Scanning { get { return this.scanning; } }

        public int PlayfieldXCoord { get; set; }

        public int PlayfieldYCoord { get; set; }

        public int PlayfieldWidthPixels { get; set; }

        public int PlayfieldHeightPixels { get; set; }

        public void Start()
        {
            if (scanning)
                return;

            scanning = true;
            Scan();
        }

        public void Stop()
        { scanning = false; }

        public void Scan()
        {
            Stopwatch timer = new Stopwatch();
            while (scanning)
            {
                timer.Reset();
                timer.Start();

                var cap = CaptureImage(PlayfieldXCoord, PlayfieldYCoord, PlayfieldWidthPixels, PlayfieldHeightPixels);

                AnalyseFrame(cap);

                timer.Stop();

                Console.WriteLine(timer.ElapsedMilliseconds);
            }
        }

        public void DumpScanArea(string path)
        {
            var scanZone = CaptureImage(PlayfieldXCoord, PlayfieldYCoord, PlayfieldWidthPixels, PlayfieldHeightPixels);

            scanZone.Save(path);
        }

        private static Bitmap CaptureImage(int x, int y, int width, int height)
        {
            Bitmap b = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            using (Graphics g = Graphics.FromImage(b))
            {
                g.CopyFromScreen(x, y, 0, 0, new Size(width, height), CopyPixelOperation.SourceCopy);
            }
            return b;
        }

        private static void AnalyseFrame(Bitmap frame)
        {
            var playField = frame;

            Rectangle rect = new Rectangle(0, 0, playField.Width, playField.Height);

            BitmapData data = playField.LockBits(rect, ImageLockMode.ReadWrite, playField.PixelFormat);

            // Get the address of the first line.
            IntPtr ptr = data.Scan0;

            // Declare an array to hold the bytes of the bitmap.
            int bytes = Math.Abs(data.Stride) * data.Height;
            byte[] rgbValues = new byte[bytes];

            // Copy the RGB values into the array.
            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);

            // Approximate block locations. 
            var pixelSize = 3;
            var padding = data.Stride - (data.Width * pixelSize);

            var gridWidth = 10;
            var gridHeight = 20;
            var sampleWidth = data.Width / gridWidth;
            var sampleHeight = data.Height / gridHeight;
            var additionalPlayfieldW = data.Width % sampleWidth;
            var additionalPlayfieldH = data.Height % sampleHeight;

            Stopwatch timer = new Stopwatch();
            timer.Start();

            // Start from midpoint.
            var index = (sampleWidth / 2) * pixelSize
                + (sampleHeight / 2 * data.Stride);

            var builder = new StringBuilder();

            var maxSampleHeight = data.Height - additionalPlayfieldH;
            var maxSampleWidth = data.Width - additionalPlayfieldW;

            for (var y = 1; y < maxSampleHeight; y += sampleHeight)
            {
                for (var x = 1; x < maxSampleWidth; x += sampleWidth)
                {
                    Color pixelColor = Color.FromArgb(
                        pixelSize == 3 ? 255 : rgbValues[index + 3], // A component if present
                        rgbValues[index + 2], // R component
                        rgbValues[index + 1], // G component
                        rgbValues[index]      // B component
                        );

                    if (pixelColor.R > 110
                        || pixelColor.B > 100
                        || pixelColor.G > 100
                        || (pixelColor.R + pixelColor.G > 100) && pixelColor.B < 20
                        )
                    {
                        builder.Append("#");
                    }
                    else
                        builder.Append("_");

                    rgbValues[index] = 255;
                    rgbValues[index + 2] = 255;
                    rgbValues[index + 1] = 255;

                    index += pixelSize * sampleWidth;
                }

                builder.AppendLine();

                index += (additionalPlayfieldW * pixelSize) + padding // Skip padding.
                    + (sampleHeight - 1) * data.Stride; // Next Sample row.
            }

            timer.Stop();

            Console.WriteLine(timer.ElapsedTicks);
            Console.WriteLine(builder);

            // Copy the RGB values back to the bitmap
            System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);

            // Unlock the bits.
            playField.UnlockBits(data);
        }
    }
}
