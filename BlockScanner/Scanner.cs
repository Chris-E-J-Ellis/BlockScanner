using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using BlockScanner.Detectors;
using BlockScanner.Rendering;

namespace BlockScanner
{
    public class Scanner
    {
        private bool scanning = false;

        // Fields related to scanning area.
        private Rectangle rectangle;
        private int pixelSize = 3;
        private int padding;

        private int gridWidth = 10;
        private int gridHeight = 20;
        private int sampleWidth;
        private int sampleHeight;
        private int startingScanIndex;
        private int maxSampleHeight;
        private int maxSampleWidth;

        // Attempt to account for additional Height/Widths by boosting pixel positions.
        // May change loop/samples to use floats instead.
        private float missingPixelsPerColumn;
        private float missingPixelsPerRow;

        public Scanner()
        {
            Detector = new BasicDetector();
        }

        public bool Scanning { get { return this.scanning; } }

        public int PlayfieldXCoord { get; set; }

        public int PlayfieldYCoord { get; set; }

        public int PlayfieldWidthPixels { get; set; }

        public int PlayfieldHeightPixels { get; set; }

        public IDetector Detector { get; set; }

        // Start/Launch a scan job, temporary until I sort out more of the structure.
        public void Start()
        {
            if (scanning)
                return;

            scanning = true;
            Scan();
        }

        public void Stop()
        { scanning = false; }

        /// <summary>
        /// Launches a grid scan using the current settings. Will currently block the active thread while the scan is running. 
        /// Temporary whilst I decide upon more structure details.
        /// </summary>
        public void Scan()
        {
            Stopwatch timer = new Stopwatch();

            var initialFrame = CaptureImage(PlayfieldXCoord, PlayfieldYCoord, PlayfieldWidthPixels, PlayfieldHeightPixels);
            ConfigureScanner(initialFrame);

            // Simple Render.
            var simpleRenderer = new BasicRenderer();

            while (scanning)
            {
                timer.Reset();
                timer.Start();

                var cap = CaptureImage(PlayfieldXCoord, PlayfieldYCoord, PlayfieldWidthPixels, PlayfieldHeightPixels);

                var frameData = AnalyseFrame(cap, Detector.GetDetector());

                simpleRenderer.Render(frameData);

                timer.Stop();

                Console.WriteLine(timer.ElapsedMilliseconds);
            }
        }

        public void ConfigureScanner(Bitmap frame)
        {
            rectangle = new Rectangle(0, 0, frame.Width, frame.Height);

            BitmapData data = frame.LockBits(rectangle, ImageLockMode.ReadWrite, frame.PixelFormat);

            // Approximate block locations.
            padding = data.Stride - (data.Width * pixelSize);
            sampleWidth = data.Width / gridWidth;
            sampleHeight = data.Height / gridHeight;

            // Attempt to account for additional Height/Widths by boosting pixel positions.
            // May change loop/samples to use floats instead.
            missingPixelsPerColumn = (float)(data.Width % sampleWidth) / (float)gridWidth;
            missingPixelsPerRow = (float)(data.Height % sampleHeight) / (float)gridHeight;

            maxSampleHeight = sampleHeight * gridHeight;
            maxSampleWidth = sampleWidth * gridWidth;

            startingScanIndex = (sampleWidth / 2) * pixelSize
                + (sampleHeight / 2 * data.Stride);
        }

        public bool[][] AnalyseFrame(Bitmap frame, Func<byte[], int, bool> detectBlock)
        {
            BitmapData data = frame.LockBits(rectangle, ImageLockMode.ReadWrite, frame.PixelFormat);

            // Get the address of the first line.
            IntPtr ptr = data.Scan0;

            // Declare an array to hold the bytes of the bitmap.
            int bytes = Math.Abs(data.Stride) * data.Height;
            byte[] rgbValues = new byte[bytes];

            // Copy the RGB values into the array.
            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);

            float widthBoost = 0;
            float heightBoost = 0;

            Stopwatch timer = new Stopwatch();
            timer.Start();

            // Start from midpoint.
            var index = startingScanIndex;

            var grid = new bool[gridHeight][];

            for (var y = 0; y < gridHeight; y++)
            {
                widthBoost = 0;
                grid[y] = new bool[gridWidth];

                for (var x = 0; x < gridWidth; x++)
                {
                    grid[y][x] = detectBlock(rgbValues, index);

                    index += pixelSize * sampleWidth;

                    widthBoost += missingPixelsPerColumn;

                    if (widthBoost > 1)
                    {
                        index += pixelSize;
                        widthBoost -= 1;
                    }
                }

                // Skip any remaining fractional pixels.
                if (Math.Round(widthBoost) == 1)
                    index += pixelSize;

                index += padding // Skip padding.
                    + (sampleHeight - 1) * data.Stride; // Next Sample row.

                if (heightBoost > 1)
                {
                    index += data.Stride;

                    heightBoost -= 1;
                }
            }

            timer.Stop();

            Console.WriteLine(timer.ElapsedTicks);

            // Unlock the bits.
            frame.UnlockBits(data);

            return grid;
        }

        public void DumpScanArea(string path)
        {
            var scanZone = CaptureImage(PlayfieldXCoord, PlayfieldYCoord, PlayfieldWidthPixels, PlayfieldHeightPixels);

            DumpFrameWithSamplePoints(scanZone, path);
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

        private static void DumpFrameWithSamplePoints(Bitmap frame, string filename)
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

            // Attempt to account for additional Height/Widths by boosting pixel positions.
            // May change loop/samples to use floats instead.
            float missingPixelsPerColumn = (float)(data.Width % sampleWidth) / (float)gridWidth;
            float missingPixelsPerRow = (float)(data.Height % sampleHeight) / (float)gridHeight;
            float widthBoost = 0;
            float heightBoost = 0;

            // Start from midpoint.
            var index = (sampleWidth / 2) * pixelSize
                + (sampleHeight / 2 * data.Stride);

            var maxSampleHeight = sampleHeight * gridHeight;
            var maxSampleWidth = sampleWidth * gridWidth; 

            for (var y = 1; y < maxSampleHeight; y += sampleHeight)
            {
                widthBoost = 0;

                for (var x = 1; x < maxSampleWidth; x += sampleWidth)
                {
                    rgbValues[index] = 255;
                    rgbValues[index + 2] = 255;
                    rgbValues[index + 1] = 255;

                    index += pixelSize * sampleWidth;

                    widthBoost += missingPixelsPerColumn;

                    if (widthBoost > 1)
                    {
                        index += pixelSize;
                        widthBoost -= 1;
                    }
                }

                // Skip any remaining fractional pixels.
                if (Math.Round(widthBoost) == 1)
                    index += pixelSize;

                index += padding // Skip padding.
                    + (sampleHeight - 1) * data.Stride; // Next Sample row.

                // Boost any fractional pixels.
                heightBoost += missingPixelsPerRow;

                if (heightBoost > 1)
                {
                    index += data.Stride;

                    heightBoost -= 1;
                }
            }

            // Copy the RGB values back to the bitmap
            System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);

            // Unlock the bits.
            playField.UnlockBits(data);

            playField.Save(filename);
        }
    }
}
