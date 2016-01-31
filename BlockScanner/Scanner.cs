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

        private int gridWidth = 10;
        private int gridHeight = 20;
        private float sampleWidth;
        private float sampleHeight;
        private int sampleXOffset = 0;
        private int sampleYOffset = 0;
        private Func<int, int, int> coordinatesToIndexFunc;

        public Scanner()
        {
        }

        public bool Scanning { get { return this.scanning; } }

        public int PlayfieldXCoord { get; set; }

        public int PlayfieldYCoord { get; set; }

        public int PlayfieldWidthPixels { get; set; }

        public int PlayfieldHeightPixels { get; set; }

        // Start/Launch a scan job, temporary until I sort out more of the structure.
        public void Start<T>(IDetector<T> detector, IRenderer<T> renderer)
        {
            if (scanning)
                return;

            scanning = true;
            Scan(detector, renderer);
        }

        public void Stop()
        { scanning = false; }

        /// <summary>
        /// Launches a grid scan using the current settings. Will currently block the active thread while the scan is running. 
        /// Temporary whilst I decide upon more structure details.
        /// </summary>
        public void Scan<T>(IDetector<T> detector, IRenderer<T> renderer)
        {
            Stopwatch timer = new Stopwatch();

            var initialFrame = CaptureImage(PlayfieldXCoord, PlayfieldYCoord, PlayfieldWidthPixels, PlayfieldHeightPixels);
            ConfigureScanner(initialFrame);

            var detectorFunc = detector.GetDetector(coordinatesToIndexFunc);

            while (scanning)
            {
                timer.Reset();
                timer.Start();

                var cap = CaptureImage(PlayfieldXCoord, PlayfieldYCoord, PlayfieldWidthPixels, PlayfieldHeightPixels);

                var frameData = AnalyseFrame(cap, detectorFunc);

                renderer.Render(frameData);

                timer.Stop();

                Console.WriteLine(timer.ElapsedMilliseconds);
            }
        }

        public void ConfigureScanner(Bitmap frame)
        {
            rectangle = new Rectangle(0, 0, frame.Width, frame.Height);

            BitmapData data = frame.LockBits(rectangle, ImageLockMode.ReadWrite, frame.PixelFormat);

            // Approximate block locations.
            sampleWidth = (float)data.Width / gridWidth;
            sampleHeight = (float)data.Height / gridHeight;

            sampleXOffset = (int)(sampleWidth / 2);
            sampleYOffset = (int)(sampleHeight / 2);

            coordinatesToIndexFunc = (x, y) =>
            {
                return x * pixelSize + y * data.Stride;
            };
        }

        public T[][] AnalyseFrame<T>(Bitmap frame, Func<byte[], int, int, T> detectBlock)
        {
            BitmapData data = frame.LockBits(rectangle, ImageLockMode.ReadWrite, frame.PixelFormat);

            // Get the address of the first line.
            IntPtr ptr = data.Scan0;

            // Declare an array to hold the bytes of the bitmap.
            int bytes = Math.Abs(data.Stride) * data.Height;
            byte[] rgbValues = new byte[bytes];

            // Copy the RGB values into the array.
            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);

            // Unlock the bits.
            frame.UnlockBits(data);

            Stopwatch timer = new Stopwatch();
            timer.Start();

            // Should only index, not skip sample widths.
            Func<int, int, int> GetIndex = (x, y) =>
            {
                return x * pixelSize + y * data.Stride;
            };

            var grid = new T[gridHeight][];

            for (var y = 0; y < gridHeight; y++)
            {
                grid[y] = new T[gridWidth];

                for (var x = 0; x < gridWidth; x++)
                {
                    grid[y][x] = detectBlock(rgbValues, (int)(sampleWidth * x) + sampleXOffset, (int)(sampleHeight * y) + sampleYOffset);
                }
            }

            timer.Stop();

            Console.WriteLine(timer.ElapsedTicks);

            return grid;
        }

        public void DumpScanArea(string path)
        {
            var scanZone = CaptureImage(PlayfieldXCoord, PlayfieldYCoord, PlayfieldWidthPixels, PlayfieldHeightPixels);

            DumpFrameWithSamplePoints(scanZone, path);

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

        private void DumpFrameWithSamplePoints(Bitmap frame, string filename)
        {
            BitmapData data = frame.LockBits(rectangle, ImageLockMode.ReadWrite, frame.PixelFormat);

            // Get the address of the first line.
            IntPtr ptr = data.Scan0;

            // Declare an array to hold the bytes of the bitmap.
            int bytes = Math.Abs(data.Stride) * data.Height;
            byte[] rgbValues = new byte[bytes];

            var detector = new BasicDetector();
            detector.GetDetector(this.coordinatesToIndexFunc);

            // Copy the RGB values into the array.
            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);

            for (var y = 0; y < gridHeight; y ++)
            {
                for (var x = 0; x < gridWidth; x ++)
                {
                    detector.HighlightSamplePoints(rgbValues, (int)(sampleWidth * x) + sampleXOffset, (int)(sampleHeight * y) + sampleYOffset);
                }
            }

            // Copy the RGB values back to the bitmap
            System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);

            // Unlock the bits.
            frame.UnlockBits(data);
        }
    }
}
