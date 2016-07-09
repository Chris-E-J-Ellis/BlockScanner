namespace BlockScanner.Detectors
{
    using Config;
    using System;
    using System.Drawing;

    public abstract class BaseDetector<T> : IDetector<T>
    {
        protected Func<int, int, int> CoordinatesToIndex { get; private set; }

        public Type DetectedPointOutputType => typeof(T);

        public IConfig Configuration { get; private set; }

        public virtual void Initialise(IConfigManager configurationManager)
        {
        }

        public virtual void Initialise(Bitmap sampleFrame)
        {
        }

        public virtual void SetCoordinatesToIndex(Func<int, int, int> coordinatesToIndexFunc)
        {
            this.CoordinatesToIndex = coordinatesToIndexFunc;
        }

        public virtual T Detect(Bitmap bitmap) =>            default(T);        

        public virtual void HighlightSamplePoints(Bitmap frame)
        {
        }

        public override string ToString() => GetType().Name;
    }
}
