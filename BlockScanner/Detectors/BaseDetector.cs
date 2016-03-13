namespace BlockScanner.Detectors
{
    using System;

    public abstract class BaseDetector<T> : IDetector<T>
    {
        protected Func<int, int, int> CoordinatesToIndex { get; private set; }

        public Type DetectedPointOutputType
        {
            get
            {
                return typeof(T);
            }
        }

        public virtual void SetCoordinatesToIndex(Func<int, int, int> coordinatesToIndexFunc)
        {
            this.CoordinatesToIndex = coordinatesToIndexFunc;
        }

        public virtual T Detect(byte[] bitmapData, int x, int y)
        {
            return default(T);
        }

        public virtual void HighlightSamplePoints(byte[] rgbData, int x, int y)
        {
            return;
        }
    }
}
