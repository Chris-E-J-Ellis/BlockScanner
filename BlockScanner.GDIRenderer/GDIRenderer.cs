namespace BlockScanner.Rendering
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    public class GDIRenderer : BaseRenderer<Color[][]>, IDisposable
    {
        private RenderSurfaceForm renderSurface = new RenderSurfaceForm();

        public GDIRenderer()
        {

        }

        public override void Initialise()
        {
            if (renderSurface == null || renderSurface.IsDisposed)
            {
                renderSurface = new RenderSurfaceForm();
            }

            // Create the window handle.
            renderSurface.Show();
        }

        public override void Render(Color[][] data)
        {
            // Not entirely sure it's sensible to do this every frame, let's see what happens.
            // Waste of cycles, but hey, we're testing stuff =D
            using (Graphics graphics = renderSurface.CreateGraphics())
            {
                var width = renderSurface.ClientRectangle.Width;
                var height = renderSurface.ClientRectangle.Height;

                var dataHeight = data.GetLength(0);
                var dataWidth = data[0].GetLength(0);

                var renderWidth = width / dataWidth;
                var renderHeight = height / dataHeight;

                for (int y = 0; y < dataHeight; y++)
                {
                    for (int x = 0; x < dataWidth; x++)
                    {
                        graphics.FillRectangle(new SolidBrush(data[y][x]), new Rectangle(x * renderWidth, y * renderHeight, renderWidth, renderHeight));
                    }
                }
            }
        }

        public void Dispose()
        {
            this.renderSurface.Dispose();
        }
    }
}
