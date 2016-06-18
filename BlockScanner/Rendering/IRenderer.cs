namespace BlockScanner.Rendering
{
    using System;
    using System.Collections.Generic;
    public interface IRenderer
    {
        Type RendererInputType { get; }

        void Initialise();

        void AttachScanner(IScanner scanner);
        void DetachScanner(IScanner scanner);
    }

    public interface IRenderer<T> : IRenderer
    {
        void Render(T data);
    }

    public interface IMultiSourceRenderer : IRenderer
    {
        IEnumerable<IScannerSlot> ScannerSlots { get; }

        IEnumerable<IScannerSlot> EmptyScannerSlots { get; }
    }
}
