namespace BlockScanner.Detectors
{
    using System;
    using System.Drawing;

    public abstract class BaseDetector<T> : IDetector<T>
    {
        protected Func<int, int, int> CoordinatesToIndex { get; private set; }

        public Type DetectedPointOutputType => typeof(T);

        public virtual void Initialise()
        {
        }

        public virtual void InitialiseFromFrame(Bitmap sampleFrame)
        {
        }

        public virtual void SetCoordinatesToIndex(Func<int, int, int> coordinatesToIndexFunc)
        {
            this.CoordinatesToIndex = coordinatesToIndexFunc;
        }

        public virtual T Detect(Bitmap bitmap) => default(T);        

        public virtual void HighlightSamplePoints(Bitmap frame)
        {
        }

        public override string ToString() => GetType().Name;
    }
}
