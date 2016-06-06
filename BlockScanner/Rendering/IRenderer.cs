namespace BlockScanner.Rendering
{
    using System;

    public interface IRenderer
    {
        Type RendererInputType { get; }

        void Initialise();
    }

    public interface IRenderer<T> : IRenderer
    {
        void Render(T data);
    }
}
