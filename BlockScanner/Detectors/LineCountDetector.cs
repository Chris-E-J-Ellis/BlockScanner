namespace BlockScanner.Detectors
{
    using System;
    using System.Drawing;

    public class LineCountDetector : BaseDetector<int> 
    {
        public override int Detect(Bitmap bitmap)
        {
            // Logic goes here, just a placeholder at the moment.
            return DateTime.Now.Second;
        }
    }
}
