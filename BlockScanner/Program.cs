﻿using System;
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
        static void Main(string[] args)
        {
            var playField = Scanner.LoadBMP("Images/Tetris.Real.bmp");

            Rectangle rect = new Rectangle(0, 0, playField.Width, playField.Height);

            BitmapData data = playField.LockBits(rect, ImageLockMode.ReadWrite, playField.PixelFormat);

            // Get the address of the first line.
            IntPtr ptr = data.Scan0;

            //data.Width = 240;
            //data.Height = 480;

            // Declare an array to hold the bytes of the bitmap.
            int bytes = Math.Abs(data.Stride) * data.Height;
            byte[] rgbValues = new byte[bytes];

            // Copy the RGB values into the array.
            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);

            // Approximate block locations. 
            var pixelSize = 3;
            var padding = data.Stride - (data.Width * pixelSize);
            var additionalPlayfieldW = data.Width % 20;
            var additionalPlayfieldH = data.Height % 10;

            var gridWidth = 10;
            var gridHeight = 20;
            var sampleWidth = data.Width / gridWidth;
            var sampleHeight = data.Height / gridHeight;

            Stopwatch timer = new Stopwatch();
            timer.Start();

            // Start from midpoint.
            var index = (sampleWidth / 2) * pixelSize
                + (sampleHeight / 2 * data.Stride);

            var builder = new StringBuilder();

            for (var y = 1; y < data.Height - additionalPlayfieldH; y += sampleHeight)
            {
                for (var x = 1; x < data.Width - additionalPlayfieldW; x += sampleWidth)
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
                    + (sampleHeight-1) * data.Stride; // Next Sample row.
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
    }
}
