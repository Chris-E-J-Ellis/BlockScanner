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

        public abstract Func<byte[], int, int, T> GetDetector();

        public abstract void HighlightSamplePoints(byte[] rgbData, int x, int y);

        public virtual void SetCoordinatesToIndex(Func<int, int, int> coordinatesToIndexFunc)
        {
            this.CoordinatesToIndex = coordinatesToIndexFunc;
        }
    }
}
