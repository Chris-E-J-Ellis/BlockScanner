﻿namespace BlockScanner
{
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.Threading;
    using Detectors;
    using Config;
    using Helpers;

    public class Scanner<T> : IScanner<T>, IConfigurable<ScannerConfig>, IDisposable
    {
        private readonly IDetector<T> detector;
        private readonly IConfigManager configManager;
        private readonly IBitmapProvider bitmapProvider;

        public Scanner(IDetector<T> detector)
            : this(detector, ConfigManager.Instance, DynamicRegionBitmapProvider.Instance) { }

        public Scanner(IDetector<T> detector, IConfigManager configManager, IBitmapProvider bitmapProvider)
        {
            this.detector = detector;
            this.configManager = configManager;
            this.bitmapProvider = bitmapProvider;
        }

        public event EventHandler<T> FrameScanned;

        public IDetector Detector => detector;

        public Rectangle PlayfieldArea => Config.ScanArea;

        public ScannerConfig Config { get; private set; } = new ScannerConfig();

        public void Initialise(Rectangle scanArea)
        {
            Config = configManager.Load<ScannerConfig>("default");
            Config.ScanArea = scanArea;

            bitmapProvider.RegisterRegionOfInterest(scanArea);
            var sampleFrame = bitmapProvider.CaptureScreenRegion(PlayfieldArea);

            detector.Initialise();
            detector.InitialiseFromFrame(sampleFrame);
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

                    Scan();

                    timer.Stop();

                    // Not great, the console takes time to render this.
                    Debug.WriteLine($"Capture->Render Cycle: {timer.Elapsed.TotalMilliseconds}ms");
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

                Scan();

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

        private void Scan()
        {
            var cap = bitmapProvider.CaptureScreenRegion(PlayfieldArea);

            var frameData = AnalyseFrame(cap);

            OnRender(frameData);
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
            var scanZone = bitmapProvider.CaptureScreenRegion(PlayfieldArea);

            detector.HighlightSamplePoints(scanZone);

            try
            {
                scanZone.Save(path);
            }
            catch (System.Runtime.InteropServices.ExternalException ex)
            {
                // Catch Unhelpful Generic GDI errors (likely write access/bitmap format related).
                Console.WriteLine($"An exception occured whilst saving image: {ex}");
            }

            return scanZone;
        }

        public void Dispose()
        {
            this.ShutDown();
        }

        public void ShutDown()
        {
            // Not entirely convinced this is legitimate, but signals that we're done with updating at least.
            FrameScanned = null;

            bitmapProvider.UnregisterRegionOfInterest(Config.ScanArea);
        }

        private void OnRender(T frameData)
        {
            // Potentially ASync this up.
            FrameScanned?.Invoke(this, frameData);
        }
    }
}
