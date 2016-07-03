namespace BlockScanner.Helpers
{
    using System.Drawing;

    public class SimpleBitmapProvider : IBitmapProvider
    {
        public Bitmap CaptureScreenRegion(Rectangle rect)
        {
            return BitmapHelper.CaptureImage(rect.X, rect.Y, rect.Width, rect.Height);
        }

        public void RegisterRegionOfInterest(Rectangle rect)
        {
        }

        public void UnregisterRegionOfInterest(Rectangle rect)
        {
        }
    }
}
