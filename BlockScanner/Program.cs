using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Timers;

namespace BlockScanner
{
    class Program
    {
        private readonly Stopwatch sw = new Stopwatch();

        static void Main(string[] args)
        {
            Stopwatch timer = new Stopwatch();
            while (true)
            {
                timer.Reset();
                timer.Start();

                var cap = CaptureImage(80, 180, 206, 406);

                timer.Stop();

                Console.WriteLine(timer.ElapsedMilliseconds);
                Output(cap);

                if (DateTime.Now.Second % 10 == 0)
                    cap.Save("Images/Cap.bmp");
            }

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

        static void Capture()
        {
            var playField = Scanner.LoadBMP("Images/Tetris.Real.bmp");

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
            var additionalPlayfieldW = data.Width % 20;
            var additionalPlayfieldH = data.Height % 10;

            Stopwatch timer = new Stopwatch();
            timer.Start();

            // Start from midpoint.
            var index = (sampleWidth / 2) * pixelSize
                + (sampleHeight / 2 * data.Stride);

            var builder = new StringBuilder();

            var maxSampleHeight = data.Height - additionalPlayfieldH;
            var maxSampleWidth = data.Width - additionalPlayfieldW;
            var thing = pixelSize * sampleWidth;

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

                    if (pixelColor.R > 230
                        || pixelColor.B > 210
                        || pixelColor.G > 210
                        || pixelColor.R + pixelColor.G > 300 && pixelColor.B < 50)
                    {
                        builder.Append("1");
                    }
                    else
                        builder.Append("0");

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

            playField.Save("Images/Test.bmp");
        }

        static void Output(Bitmap screenCapture)
        {
            var playField = screenCapture;

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

            //playField.Save("Images/Test.bmp");
        }
    }
}
