namespace BlockScanner.Detectors
{
    using System.Drawing;
    using System.Drawing.Imaging;
    using Config;
    using Helpers;

    public class BaseGridDetector<T> : BaseDetector<T[][]>
    {
        private Rectangle frameRectangle;

        private float sampleWidth;
        private float sampleHeight;

        private int sampleXOffset;
        private int sampleYOffset;

        private int gridWidth;
        private int gridHeight;

        private int samplePointCentreHeightRatio = 3;
        private int samplePointCentreWidthRatio = 2;

        // Again, image specific, probably shouldn't live here.
        private int pixelSize = 3;

        public override T[][] Detect(Bitmap frame)
        {
            byte[] rgbValues = BitmapHelper.ExtractBitmapBytes(frame, frameRectangle);

            var grid = new T[gridHeight][];

            for (var y = 0; y < gridHeight; y++)
            {
                grid[y] = new T[gridWidth];

                for (var x = 0; x < gridWidth; x++)
                {
                    grid[y][x] = AnalyseSample(rgbValues, (int)(sampleWidth * x) + sampleXOffset, (int)(sampleHeight * y) + sampleYOffset);
                }
            }

            return grid;
        }

        public virtual T AnalyseSample(byte[] bitmapData, int x, int y)
        {
            return default (T);
        }

        public override void Initialise(IConfigManager configurationManager)
        {
            base.Initialise(configurationManager);
        }

        public override void Initialise(Bitmap sampleFrame)
        {
            frameRectangle = new Rectangle(0, 0, sampleFrame.Width, sampleFrame.Height);

            gridWidth = 10;
            gridHeight = 20;

            BitmapData data = BitmapHelper.ExtractBitmapData(sampleFrame, frameRectangle); 

            // Approximate block locations.
            sampleWidth = (float)data.Width / gridWidth;
            sampleHeight = (float)data.Height / gridHeight;

            sampleXOffset = (int)(sampleWidth / samplePointCentreWidthRatio);
            sampleYOffset = (int)(sampleHeight / samplePointCentreHeightRatio);

            SetCoordinatesToIndex((x, y) => x * pixelSize + y * data.Stride);
        }
    }
}
