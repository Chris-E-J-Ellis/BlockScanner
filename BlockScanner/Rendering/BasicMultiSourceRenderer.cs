namespace BlockScanner.Rendering
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class BasicMultiSourceRenderer : BaseRenderer<bool[][]>, IMultiSourceRenderer
    {
        private readonly IList<IScannerSlot> slots = new List<IScannerSlot>();
        private int lineCount;
        private int score;

        public BasicMultiSourceRenderer()
        {
            // Initialise Slots
            slots.Add(new ScannerSlot<bool[][]>(nameof(PlayfieldScanner), UpdatePlayfield));
            slots.Add(new ScannerSlot<int>(nameof(UpdateLineCount), UpdateLineCount));
            slots.Add(new ScannerSlot<int>(nameof(UpdateScore), UpdateScore));
        }

        public IEnumerable<IScannerSlot> ScannerSlots => slots;

        public IEnumerable<IScannerSlot> EmptyScannerSlots => slots.Where(s => s.IsEmpty);

        public IScanner<bool[][]> PlayfieldScanner { get; set; }

        public IScanner<int> LineCountScanner { get; set; }

        public override void Initialise()
        {
        }

        public override void Render(bool[][] data)
        {
            var output = string.Join(Environment.NewLine,
              data.Select(row => row.Select(block => block ? "#" : "_"))
              .Select(block => string.Join(string.Empty, block)));

            Console.WriteLine(output);
        }

        public override void Dispose()
        {
            foreach (var slot in ScannerSlots)
            {
                slot.Dispose();
            }

            base.Dispose();
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