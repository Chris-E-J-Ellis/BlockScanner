namespace BlockScanner.Rendering
{
    using System;

    public sealed class ScannerSlot<T> : IScannerSlot
    {
        private Action<T> scanAction;

        public ScannerSlot(string name, Action<T> renderAction)
        {
            Name = name;
            scanAction = renderAction;
        }

        public string Name { get; private set; }

        public IScanner<T> Scanner { get; private set; }

        public bool IsEmpty => Scanner == null;

        public Type ScanType => typeof(T); 

        public void Assign(IScanner<T> scanner)
        {
            Scanner = scanner;

            Scanner.FrameScanned += (o, e) => scanAction(e);
        }

        public void Dispose()
        {
            Scanner.FrameScanned -= (o, e) => scanAction(e);
        }
    }
}
