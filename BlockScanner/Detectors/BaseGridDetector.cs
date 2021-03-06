﻿namespace BlockScanner.Detectors
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using Helpers;

    public class BaseGridDetector<T> : BaseDetector<T[][]>
    {
        private Rectangle frameRectangle;

        private float sampleWidth;
        private float sampleHeight;

        private int sampleXOffset;
        private int sampleYOffset;

        private int gridWidth = 10;
        private int gridHeight = 20;

        private int samplePointCentreHeightRatio = 3;
        private int samplePointCentreWidthRatio = 2;

        // Again, image specific, probably shouldn't live here.
        private int pixelSize = 3;

        public BaseGridDetector()
        {
        }

        public override T[][] Detect(Bitmap frame)
        {
            byte[] rgbValues = BitmapHelper.ExtractBitmapBytes(frame, frameRectangle);

            var grid = new T[gridHeight][];

            for (var y = 0; y < gridHeight; y++)
            {
                grid[y] = new T[gridWidth];

                for (var x = 0; x < gridWidth; x++)
                {
                    grid[y][x] = AnalyseSample(rgbValues, (int)(sampleWidth * x) + sampleXOffset, (int)(sampleHeight * y) + sampleYOffset);
                }
            }

            return grid;
        }

        public virtual T AnalyseSample(byte[] bitmapData, int x, int y) => default (T);

        public override void Initialise()
        {
            base.Initialise();
        }

        public override void InitialiseFromFrame(Bitmap sampleFrame)
        {
            frameRectangle = new Rectangle(0, 0, sampleFrame.Width, sampleFrame.Height);

            BitmapData data = BitmapHelper.ExtractBitmapData(sampleFrame, frameRectangle); 

            // Approximate block locations.
            sampleWidth = (float)data.Width / gridWidth;
            sampleHeight = (float)data.Height / gridHeight;

            sampleXOffset = (int)(sampleWidth / samplePointCentreWidthRatio);
            sampleYOffset = (int)(sampleHeight / samplePointCentreHeightRatio);

            SetCoordinatesToIndex((x, y) => x * pixelSize + y * data.Stride);
        }

        public override void HighlightSamplePoints(Bitmap frame)
        {
            // Todo: this image manipulation logic + duplicate iteration code probably doesn't need to be here.
            BitmapData data = frame.LockBits(frameRectangle, ImageLockMode.ReadWrite, frame.PixelFormat);

            // Get the address of the first line.
            IntPtr ptr = data.Scan0;

            // Declare an array to hold the bytes of the bitmap.
            int bytes = Math.Abs(data.Stride) * data.Height;
            byte[] rgbValues = new byte[bytes];

            // Copy the RGB values into the array.
            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);

            for (var y = 0; y < gridHeight; y++)
            {
                for (var x = 0; x < gridWidth; x++)
                {
                    HighlightSamplePoint(rgbValues, (int)(sampleWidth * x) + sampleXOffset, (int)(sampleHeight * y) + sampleYOffset);
                }
            }

            // Copy the RGB values back to the bitmap
            System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);

            // Unlock the bits.
            frame.UnlockBits(data);
        }

        public void SetGridSize(int gridWidth, int gridHeight)
        {
            this.gridWidth = gridWidth;
            this.gridHeight = gridHeight;
        }

        public void SetSamplePointCentre(int samplePointCentreWidthRatio, int samplePointCentreHeightRatio)
        {
            this.samplePointCentreWidthRatio = samplePointCentreWidthRatio;
            this.samplePointCentreHeightRatio = samplePointCentreHeightRatio;
        }

        private void HighlightSamplePoint(byte[] rgbData, int x, int y)
        {
            var index = CoordinatesToIndex(x, y);

            rgbData[index + 2] = 255;
            rgbData[index + 1] = 255;
            rgbData[index] = 255;
        }
    }
}
