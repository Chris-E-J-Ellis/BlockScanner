namespace BlockScanner.Helpers
{
    using System.Drawing;

    public interface IBitmapProvider
    {
        Bitmap CaptureScreenRegion(Rectangle rect);

        void RegisterRegionOfInterest(Rectangle rect);
        void UnregisterRegionOfInterest(Rectangle rect);
    }
}
