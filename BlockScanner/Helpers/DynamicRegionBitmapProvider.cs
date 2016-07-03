namespace BlockScanner.Helpers
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Linq;

    // In theory, captures the largest area required from the screen and creates required sub regions (first attempt).
    public sealed class DynamicRegionBitmapProvider : IBitmapProvider
    {
        private static readonly Lazy<DynamicRegionBitmapProvider> lazy =
        new Lazy<DynamicRegionBitmapProvider>(() => new DynamicRegionBitmapProvider());

        public static DynamicRegionBitmapProvider Instance { get { return lazy.Value; } }

        // Unsure if I want this as a singleton, or something initialised via DI.
        public DynamicRegionBitmapProvider() { }

        // This actually doesn't need to be a ConcurrentDictionary now the locks are in place, but I like the TryAdd/TryRemoves.
        private readonly ConcurrentDictionary<Rectangle, Bitmap> capturedRegions = new ConcurrentDictionary<Rectangle, Bitmap>();
        private readonly HashSet<Rectangle> captureRegions = new HashSet<Rectangle>();
        private readonly object captureLockObject = new object();

        public Rectangle CaptureArea { get; private set; }
        public Bitmap MainCapturedRegion { get; private set; }

        public IReadOnlyDictionary<Rectangle, Bitmap> CapturedRegions => new ReadOnlyDictionary<Rectangle, Bitmap>(capturedRegions);

        public Bitmap CaptureScreenRegion(Rectangle rect)
        {
            Bitmap region;

            lock (captureLockObject)
            {
                if (capturedRegions.TryRemove(rect, out region))
                {
                    return region;
                }
                return CaptureAndPopulateRegions(rect);
            }
        }

        public void RegisterRegionOfInterest(Rectangle rect)
        {
            lock (captureLockObject)
            {
                captureRegions.Add(rect);

                RecalculateCaptureArea();
            }
        }

        public void UnregisterRegionOfInterest(Rectangle rect)
        {
            lock (captureLockObject)
            {
                captureRegions.Remove(rect);
                RecalculateCaptureArea();

                // Remove any lingering regions.
                Bitmap bitmap;
                capturedRegions.TryRemove(rect, out bitmap);
            }
        }

        public Bitmap GenerateHighlightedMainCaptureRegion()
        {
            Bitmap highlightedImage = MainCapturedRegion.Clone(
                new Rectangle(0, 0, MainCapturedRegion.Width, MainCapturedRegion.Height)
                , PixelFormat.Format24bppRgb);

            using (Graphics g = Graphics.FromImage(highlightedImage))
            {
                foreach (var region in captureRegions)
                {
                    g.DrawRectangle(new Pen(new SolidBrush(Color.Red)) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dash }, region);
                }

                g.Save();
            }

            return highlightedImage;
        }

        private Bitmap CaptureAndPopulateRegions(Rectangle rect)
        {
            MainCapturedRegion = BitmapHelper.CaptureImage(CaptureArea.X, CaptureArea.Y, CaptureArea.Width, CaptureArea.Height);

            // Update all regions other than the one requested.
            foreach (var region in captureRegions.Where(r => r != rect))
            {
                // Potentially cache the transformed rectangles.
                var transformedRegion = new Rectangle(region.X - CaptureArea.X, region.Y - CaptureArea.Y, region.Width, region.Height);
                var subRegion = MainCapturedRegion.Clone(transformedRegion, PixelFormat.Format24bppRgb);
                capturedRegions.AddOrUpdate(region, subRegion, (x, y) => subRegion); // We can lose some information here (may be problematic).
            }

            return MainCapturedRegion.Clone(new Rectangle(0, 0, rect.Width, rect.Height), PixelFormat.Format24bppRgb);
        }

        private void RecalculateCaptureArea()
        {
            int left = int.MaxValue;
            int right = 0;
            int top = int.MaxValue;
            int bottom = 0;

            foreach (var rect in captureRegions)
            {
                left = (left > rect.Left) ? rect.Left : left;
                right = (right < rect.Right) ? rect.Right : right;
                top = (top > rect.Top) ? rect.Top : top;
                bottom = (bottom < rect.Bottom) ? rect.Bottom : bottom;
            }

            CaptureArea = new Rectangle(left, top, right - left, bottom - top);
        }
    }
}
