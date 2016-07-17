namespace BlockScanner.Rendering.MultiSource
{
    using System;
    using System.Linq;

    public class BasicMultiSourceRenderer : BaseMultiSourceRenderer<bool[][]>, IMultiSourceRenderer, IDisposable
    {
        private int lineCount;
        private int score;

        public BasicMultiSourceRenderer()
        {
            // Initialise Slots
            AddSlot(new ScannerSlot<bool[][]>(nameof(PlayfieldScanner), UpdatePlayfield));
            AddSlot(new ScannerSlot<int>(nameof(UpdateLineCount), UpdateLineCount));
            AddSlot(new ScannerSlot<int>(nameof(UpdateScore), UpdateScore));
        }

        public IScanner<bool[][]> PlayfieldScanner { get; set; }

        public IScanner<int> LineCountScanner { get; set; }

        public override void Initialise()
        {
        }

        private void UpdatePlayfield(object sender, bool[][] playfieldInfo)
        {
            var output = string.Join(Environment.NewLine,
              playfieldInfo.Select(row => row.Select(block => block ? "#" : "_"))
              .Select(block => string.Join(string.Empty, block)));

            // Not great, just testing.
            Console.WriteLine($"Lines: {lineCount} Score {score}");
            Console.WriteLine(output);
        }

        private void UpdateLineCount(object sender, int count)
        {
            this.lineCount = count;
        }

        private void UpdateScore(object sender, int score)
        {
            this.score = score;
        }
    }
}