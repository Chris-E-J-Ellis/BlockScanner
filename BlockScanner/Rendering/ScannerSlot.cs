namespace BlockScanner.Rendering
{
    using System;

    public sealed class ScannerSlot<T> : IScannerSlot
    {
        private Action<T> scanAction;
        private IScanner<T> scanner;

        public ScannerSlot(string name, Action<T> renderAction)
        {
            Name = name;
            scanAction = renderAction;
        }

        public string Name { get; private set; }

        public IScanner Scanner => this.scanner; 

        public bool IsEmpty => Scanner == null;

        public Type ScanType => typeof(T); 

        public void Assign(IScanner scanner)
        {
            var downcastScanner = (scanner as IScanner<T>);
            if (downcastScanner != null)
            {
                Clear();

                this.scanner = downcastScanner;
                this.scanner.FrameScanned += (o, e) => scanAction(e);
            }
        }

        public void Clear()
        {
            if (Scanner == null)
                return;

            this.scanner.FrameScanned -= (o, e) => scanAction(e);
        }

        public override string ToString()
        {
            return this.Name;
        }

        public void Dispose()
        {
            this.scanner.FrameScanned -= (o, e) => scanAction(e);
        }
    }
}
