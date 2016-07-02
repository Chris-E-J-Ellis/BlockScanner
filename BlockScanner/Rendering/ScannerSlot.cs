namespace BlockScanner.Rendering
{
    using System;

    public sealed class ScannerSlot<T> : IScannerSlot
    {
        private EventHandler<T> scanAction;
        private IScanner<T> scanner;

        public ScannerSlot(string name, EventHandler<T> scanHandler)
        {
            Name = name;
            scanAction = scanHandler;
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
                this.scanner.FrameScanned += scanAction;
            }
        }

        public void Clear()
        {
            if (Scanner == null)
                return;

            this.scanner.FrameScanned -= scanAction;
        }

        public override string ToString()
        {
            return this.Name;
        }

        public void Dispose()
        {
            this.scanner.FrameScanned -= scanAction;
        }
    }
}
