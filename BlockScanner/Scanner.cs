namespace BlockScanner
{
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Threading;
    using Detectors;
    using Rendering;
    using Config;

    public class Scanner<T> : IScanner<T>, IConfigurable<ScannerConfig>
    {
        private IDetector<T> detector;
        private IRenderer<T> renderer;
        private IConfigManager configManager;

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

        public Scanner(IDetector<T> detector, IRenderer<T> renderer)
            : this(detector, renderer, ConfigManager.Instance) { }

        public Scanner(IDetector<T> detector, IRenderer<T> renderer, IConfigManager configManager)
        {
            this.detector = detector;
            this.renderer = renderer;
            this.configManager = configManager;
        }

        public Rectangle PlayfieldArea => Config.ScanArea;

        public ScannerConfig Config { get; private set; } = new ScannerConfig();

        public void Initialise(Rectangle scanArea)
        {
            Config = configManager.Load<ScannerConfig>("default");
            Config.ScanArea = scanArea;

            gridWidth = Config.GridWidth;
            gridHeight = Config.GridHeight;

            var sampleFrame = CaptureImage(PlayfieldArea.X, PlayfieldArea.Y, PlayfieldArea.Width, PlayfieldArea.Height);
            ConfigureScanner(sampleFrame);

            detector.Initialise(ConfigManager.Instance);
            renderer.Initialise();

            detector.SetCoordinatesToIndex(coordinatesToIndexFunc);
        }

        /// <summary>
        /// Launches a grid scan using the current settings. Will currently block the active thread while the scan is running. 
        /// Temporary whilst I decide upon more structure details.
        /// </summary>
        public void Scan(CancellationToken token)
        {
            var timer = new Stopwatch();

            while (!token.IsCancellationRequested)
            {
                timer.Reset();
                timer.Start();

                var cap = CaptureImage(PlayfieldArea.X, PlayfieldArea.Y, PlayfieldArea.Width, PlayfieldArea.Height);

                var frameData = AnalyseFrame(cap);

                renderer.Render(frameData);

                timer.Stop();

                Console.WriteLine($"Capture->Render Cycle: {timer.ElapsedMilliseconds}ms");
            }
        }

        public void SetConfig(ScannerConfig config)
        {
            this.Config = config;
        }

        private void ConfigureScanner(Bitmap frame)
        {
            rectangle = new Rectangle(0, 0, frame.Width, frame.Height);

            BitmapData data = frame.LockBits(rectangle, ImageLockMode.ReadWrite, frame.PixelFormat);

            // Approximate block locations.
            sampleWidth = (float)data.Width / gridWidth;
            sampleHeight = (float)data.Height / gridHeight;

            sampleXOffset = (int)(sampleWidth / Config.SamplePointCentreWidthRatio);
            sampleYOffset = (int)(sampleHeight / Config.SamplePointCentreHeightRatio);

            coordinatesToIndexFunc = (x, y) =>
            {
                return x * pixelSize + y * data.Stride;
            };
        }

        public T[][] AnalyseFrame(Bitmap frame)
        {
            byte[] rgbValues = GetFrameData(frame);

            Stopwatch timer = new Stopwatch();
            timer.Start();

            var grid = new T[gridHeight][];

            for (var y = 0; y < gridHeight; y++)
            {
                grid[y] = new T[gridWidth];

                for (var x = 0; x < gridWidth; x++)
                {
                    grid[y][x] = detector.Detect(rgbValues, (int)(sampleWidth * x) + sampleXOffset, (int)(sampleHeight * y) + sampleYOffset);
                }
            }

            timer.Stop();

            Console.WriteLine($"Frame Analysis: {timer.ElapsedTicks} ticks");

            return grid;
        }

        public Bitmap DumpScanArea(string path)
        {
            var scanZone = CaptureImage(PlayfieldArea.X, PlayfieldArea.Y, PlayfieldArea.Width, PlayfieldArea.Height);

            DumpFrameWithSamplePoints(scanZone);

            scanZone.Save(path);

            return scanZone;
        }
        
        private byte[] GetFrameData(Bitmap frame)
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

            return rgbValues;
        }

        // At some point, all the Bitmap capturing stuff can be unpicked/replaced with a generic data source.
        private static Bitmap CaptureImage(int x, int y, int width, int height)
        {
            var bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);

            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.CopyFromScreen(x, y, 0, 0, new Size(width, height), CopyPixelOperation.SourceCopy);
            }

            return bitmap;
        }

        private void DumpFrameWithSamplePoints(Bitmap frame)
        {
            BitmapData data = frame.LockBits(rectangle, ImageLockMode.ReadWrite, frame.PixelFormat);

            // Get the address of the first line.
            IntPtr ptr = data.Scan0;

            // Declare an array to hold the bytes of the bitmap.
            int bytes = Math.Abs(data.Stride) * data.Height;
            byte[] rgbValues = new byte[bytes];

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
