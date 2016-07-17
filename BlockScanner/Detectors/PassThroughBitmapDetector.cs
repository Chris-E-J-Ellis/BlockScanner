namespace BlockScanner.Detectors
{
    using System.Drawing;

    public class PassThroughBitmapDetector : BaseDetector<Bitmap>
    {
        public override Bitmap Detect(Bitmap bitmap)
        {
            return bitmap;
        }
    }
}
