﻿namespace BlockScanner
{
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Threading;
    using Detectors;
    using Rendering;
    using Config;
    using Helpers;

    public class Scanner<T> : IScanner<T>, IConfigurable<ScannerConfig>
    {
        private IDetector<T> detector;
        private IRenderer<T> renderer;
        private IConfigManager configManager;

        public Scanner(IDetector<T> detector, IRenderer<T> renderer)
            : this(detector, renderer, ConfigManager.Instance) { }

        public Scanner(IDetector<T> detector, IRenderer<T> renderer, IConfigManager configManager)
        {
            this.detector = detector;
            this.renderer = renderer;
            this.configManager = configManager;
        }

        public IDetector Detector => detector;

        public IRenderer Renderer => renderer;

        public Rectangle PlayfieldArea => Config.ScanArea;

        public ScannerConfig Config { get; private set; } = new ScannerConfig();

        public void Initialise(Rectangle scanArea)
        {
            Config = configManager.Load<ScannerConfig>("default");
            Config.ScanArea = scanArea;

            var sampleFrame = CaptureImage(PlayfieldArea.X, PlayfieldArea.Y, PlayfieldArea.Width, PlayfieldArea.Height);

            detector.Initialise(ConfigManager.Instance);
            detector.Initialise(sampleFrame);
            renderer.Initialise();
        }

        /// <summary>
        /// Launches a grid scan using the current settings. Will currently block the active thread while the scan is running. 
        /// Temporary whilst I decide upon more structure details.
        /// </summary>
        public void Scan(CancellationToken token)
        {
            var timer = new Stopwatch();

            try
            {
                while (!token.IsCancellationRequested)
                {
                    timer.Reset();
                    timer.Start();

                    var cap = CaptureImage(PlayfieldArea.X, PlayfieldArea.Y, PlayfieldArea.Width, PlayfieldArea.Height);

                    var frameData = TimerHelper.Profile(() => AnalyseFrame(cap), "Frame Analysis");

                    renderer.Render(frameData);

                    timer.Stop();

                    // Not great, the console takes time to render this.
                    Console.WriteLine($"Capture->Render Cycle: {timer.Elapsed.TotalMilliseconds}ms");
                }
            }
            catch (Exception ex)
            {
                // Super basic, just fail and stop scanning.
                Console.WriteLine($"Encountered an exception, scan halted: '{ex};");
            }
        }

        // Probably temporary, collapse with above function.
        public void ScanOnce()
        {
            var timer = new Stopwatch();

            try
            {
                timer.Reset();
                timer.Start();

                var cap = CaptureImage(PlayfieldArea.X, PlayfieldArea.Y, PlayfieldArea.Width, PlayfieldArea.Height);

                var frameData = TimerHelper.Profile(() => AnalyseFrame(cap), "Frame Analysis");

                renderer.Render(frameData);

                timer.Stop();

                // Not great, the console takes time to render this.
                Console.WriteLine($"Capture->Render Cycle: {timer.Elapsed.TotalMilliseconds}ms");
            }
            catch (Exception ex)
            {
                // Super basic, just fail and stop scanning.
                Console.WriteLine($"Encountered an exception, scan halted: '{ex};");
            }
        }

        public void SetConfig(ScannerConfig config)
        {
            this.Config = config;
        }

        public T AnalyseFrame(Bitmap frame)
        {
            return detector.Detect(frame);
        }

        public Bitmap DumpScanArea(string path)
        {
            var scanZone = CaptureImage(PlayfieldArea.X, PlayfieldArea.Y, PlayfieldArea.Width, PlayfieldArea.Height);

            detector.HighlightSamplePoints(scanZone);

            scanZone.Save(path);

            return scanZone;
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
    }
}
