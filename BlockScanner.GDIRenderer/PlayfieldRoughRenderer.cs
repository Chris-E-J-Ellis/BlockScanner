namespace BlockScanner.Rendering
{
    using MultiSource;
    using System.Drawing;

    public class PlayfieldRough : BaseMultiSourceRenderer<Bitmap>
    {
        private Bitmap rank = new Bitmap(10, 10);
        private Bitmap score = new Bitmap(10, 10);
        private Bitmap lineCount = new Bitmap(10, 10);
        private Bitmap playField = new Bitmap(10, 10);

        private RenderSurfaceForm renderSurface = new RenderSurfaceForm();

        public PlayfieldRough()
        {
            // Initialise Slots
            AddSlot(new ScannerSlot<Bitmap>("Rank", UpdateRank));
            AddSlot(new ScannerSlot<Bitmap>("Score", UpdateScore));
            AddSlot(new ScannerSlot<Bitmap>("Line", UpdateLineCount));
            AddSlot(new ScannerSlot<Bitmap>("Playfield", UpdatePlayfield));
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

        private void UpdatePlayfield(object sender, Bitmap playfield)
        {
            // Testing, just do the render in here for the moment.
            // Not entirely sure it's sensible to do this every fram, let's see what happens.
            // Waste of cycles, but hey, we're testing stuff =D
            using (Graphics graphics = renderSurface.CreateGraphics())
            {
                var width = renderSurface.ClientRectangle.Width;
                var height = renderSurface.ClientRectangle.Height;

                var widthSplit = width / 2;
                var heightSplit = height / 2;

                graphics.DrawImage(playfield, new Rectangle(widthSplit, heightSplit, width - widthSplit, height - heightSplit), new Rectangle(0, 0, playfield.Width, playfield.Height),GraphicsUnit.Pixel);
                graphics.DrawImage(score, new Rectangle(0, 0, width - widthSplit, height - heightSplit), new Rectangle(0, 0, score.Width, score.Height),GraphicsUnit.Pixel);
                graphics.DrawImage(lineCount, new Rectangle(0, heightSplit, width - widthSplit, height - heightSplit), new Rectangle(0, 0, lineCount.Width, lineCount.Height),GraphicsUnit.Pixel);
                graphics.DrawImage(rank, new Rectangle(widthSplit, 0, width - widthSplit, height - heightSplit), new Rectangle(0, 0, rank.Width, rank.Height),GraphicsUnit.Pixel);
            }
        }

        private void UpdateScore(object sender, Bitmap score)
        {
            this.score = score;
        }

        private void UpdateLineCount(object sender, Bitmap lineCount)
        {
            this.lineCount = lineCount;
        }

        private void UpdateRank(object sender, Bitmap rank)
        {
            this.rank = rank;
        }
    }
}
