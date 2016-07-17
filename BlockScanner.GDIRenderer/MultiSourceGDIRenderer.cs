namespace BlockScanner.Rendering.MultiSource
{
    using System.Drawing;

    public class MultiSourceGDIRenderer : BaseMultiSourceRenderer<Color[][]>, IMultiSourceRenderer
    {
        private RenderSurfaceForm renderSurface = new RenderSurfaceForm();

        private int lineCount;

        public MultiSourceGDIRenderer()
        {
            // Initialise Slots
            AddSlot(new ScannerSlot<Color[][]>(nameof(PlayfieldScanner), UpdatePlayfield));
            AddSlot(new ScannerSlot<int>(nameof(UpdateLineCount), UpdateLineCount));
        }

        public IScanner<Color[][]> PlayfieldScanner { get; private set; }

        public IScanner<int> LineCountScanner { get; private set; }

        public override void Initialise()
        {
            if (renderSurface == null || renderSurface.IsDisposed)
            {
                renderSurface = new RenderSurfaceForm();
            }

            // Create the window handle.
            renderSurface.Show();
        }

        public override void Dispose()
        {
            this.renderSurface.Dispose();

            base.Dispose();
        }

        // Testing, using this as the primary render call.
        private void UpdatePlayfield(object sender, Color[][] playfieldInfo)
        {
            // Not entirely sure it's sensible to do this every frame, let's see what happens.
            // Waste of cycles, but hey, we're testing stuff =D
            using (Graphics graphics = renderSurface.CreateGraphics())
            {
                var width = renderSurface.ClientRectangle.Width;
                var height = renderSurface.ClientRectangle.Height;

                var dataHeight = playfieldInfo.GetLength(0);
                var dataWidth = playfieldInfo[0].GetLength(0);

                var renderWidth = width / dataWidth;
                var renderHeight = height / dataHeight;

                for (int y = 0; y < dataHeight; y++)
                {
                    for (int x = 0; x < dataWidth; x++)
                    {
                        graphics.FillRectangle(new SolidBrush(playfieldInfo[y][x]), new Rectangle(x * renderWidth, y * renderHeight, renderWidth, renderHeight));
                    }
                }

                graphics.DrawString(lineCount.ToString(), new Font("Arial", 10), new SolidBrush(Color.White), width / 2, height / 2);
            }
        }

        private void UpdateLineCount(object sender, int count)
        {
            lineCount = count;
        }
    }
}
