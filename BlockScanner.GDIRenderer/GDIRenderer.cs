namespace BlockScanner.Rendering
{
    using System;
    using System.Drawing;

    public class GDIRenderer : BaseRenderer<Color>
    {
        private readonly RenderSurfaceForm renderSurface = new RenderSurfaceForm();

        public GDIRenderer()
        {
            // Create the window handle.
            renderSurface.Show();
            renderSurface.Hide();
        }

        public override void Initialise()
        {
            renderSurface.BeginInvoke(new Action(() => renderSurface.Show()));
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
    }
}
