namespace BlockScanner.Rendering
{
    using System;

    public interface IRenderer
    {
        Type RendererInputType { get; }
    }

    public interface IRenderer<T> : IRenderer
    {
        void Render(T[][] data);
    }
}
