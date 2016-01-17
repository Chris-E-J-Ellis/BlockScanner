using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using BlockScanner.Detectors;

namespace BlockScanner
{
    public class Scanner
    {
        private bool scanning = false;

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

            while (scanning)
            {
                timer.Reset();
                timer.Start();

                var cap = CaptureImage(PlayfieldXCoord, PlayfieldYCoord, PlayfieldWidthPixels, PlayfieldHeightPixels);

                AnalyseFrame(cap, Detector.GetDetector());

                timer.Stop();

                Console.WriteLine(timer.ElapsedMilliseconds);
            }
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

        private static void AnalyseFrame(Bitmap frame, Func<byte[], int, bool> DetectBlock)
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
                    if (DetectBlock(rgbValues, index))
                        builder.Append("#");
                    else
                        builder.Append("_");

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
            var additionalPlayfieldW = data.Width % sampleWidth;
            var additionalPlayfieldH = data.Height % sampleHeight;

            // Start from midpoint.
            var index = (sampleWidth / 2) * pixelSize
                + (sampleHeight / 2 * data.Stride);

            var maxSampleHeight = data.Height - additionalPlayfieldH;
            var maxSampleWidth = data.Width - additionalPlayfieldW;

            for (var y = 1; y < maxSampleHeight; y += sampleHeight)
            {
                for (var x = 1; x < maxSampleWidth; x += sampleWidth)
                {
                    rgbValues[index] = 255;
                    rgbValues[index + 2] = 255;
                    rgbValues[index + 1] = 255;

                    index += pixelSize * sampleWidth;
                }

                index += (additionalPlayfieldW * pixelSize) + padding // Skip padding.
                    + (sampleHeight - 1) * data.Stride; // Next Sample row.
            }

            // Copy the RGB values back to the bitmap
            System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);

            // Unlock the bits.
            playField.UnlockBits(data);

            playField.Save(filename);
        }
    }
}
