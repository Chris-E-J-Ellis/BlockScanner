using System;

namespace BlockScanner.Rendering
{
    public interface IScannerSlot : IDisposable
    {
        string Name { get; }
        bool IsEmpty { get; }
        Type ScanType { get; }
    }
}
