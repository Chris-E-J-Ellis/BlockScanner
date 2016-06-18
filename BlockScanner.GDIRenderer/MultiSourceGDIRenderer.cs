namespace BlockScanner.Rendering
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    public class MultiSourceGDIRenderer : BaseRenderer<Color[][]>, IMultiSourceRenderer, IDisposable
    {
        private RenderSurfaceForm renderSurface = new RenderSurfaceForm();

        private readonly IList<IScannerSlot> slots = new List<IScannerSlot>();

        public MultiSourceGDIRenderer()
        {
            // Initialise Slots
            slots.Add(new ScannerSlot<Color[][]>(nameof(PlayfieldScanner), UpdatePlayfield));
            slots.Add(new ScannerSlot<int>(nameof(UpdateLineCount), UpdateLineCount));
        }

        public IEnumerable<IScannerSlot> ScannerSlots => slots;

        public IEnumerable<IScannerSlot> EmptyScannerSlots => slots.Where(s => s.IsEmpty);

        public IScanner<Color[][]> PlayfieldScanner { get; set; }

        public IScanner<int> LineCountScanner { get; set; }

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
        }

        public override void Dispose()
        {
            this.renderSurface.Dispose();

            foreach (var slot in ScannerSlots)
            {
                slot.Dispose();
            }

            base.Dispose();
        }

        private void UpdatePlayfield(Color[][] playfieldInfo)
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
            }
        }

        private void UpdateLineCount(int count)
        {
        }
    }
}
