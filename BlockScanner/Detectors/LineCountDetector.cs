namespace BlockScanner.Detectors
{
    using System;
    using System.Drawing;

    public class LineCountDetector : BaseDetector<int> 
    {
        public override int Detect(Bitmap bitmap)
        {
            // Logic goes here.
            return DateTime.Now.Second;
        }
    }
}
