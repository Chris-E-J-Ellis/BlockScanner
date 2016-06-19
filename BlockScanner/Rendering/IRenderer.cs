namespace BlockScanner.Rendering
{
    using System;
    using System.Collections.Generic;

    public interface IRenderer
    {
        void Initialise();
    }

    public interface ISingleSourceRenderer : IRenderer
    {
        Type RendererInputType { get; }

        void AttachScanner(IScanner scanner);

        void DetachScanner(IScanner scanner);
    }

    public interface ISingleSourceRenderer<T> : ISingleSourceRenderer
    {
        void Render(T data);
    }

    // Might be a case for making everything a Multisource.
    public interface IMultiSourceRenderer : IRenderer
    {
        IEnumerable<IScannerSlot> ScannerSlots { get; }

        IEnumerable<IScannerSlot> EmptyScannerSlots { get; }
    }
}
