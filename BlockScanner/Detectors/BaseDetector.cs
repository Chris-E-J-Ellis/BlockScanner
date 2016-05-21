namespace BlockScanner.Detectors
{
    using Config;
    using System;

    public abstract class BaseDetector<T> : IDetector<T>
    {
        protected Func<int, int, int> CoordinatesToIndex { get; private set; }

        public Type DetectedPointOutputType => typeof(T);

        public IConfig Configuration { get; private set; }

        public virtual void Initialise(IConfigManager configurationManager)
        {
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

        public override string ToString()
        {
            return GetType().Name;
        }
    }
}
