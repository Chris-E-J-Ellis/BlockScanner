namespace BlockScanner.Rendering
{
    using System;

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
}
