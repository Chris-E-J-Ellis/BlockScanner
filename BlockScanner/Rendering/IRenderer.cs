namespace BlockScanner.Rendering
{
    public interface IRenderer
    { }

    public interface IRenderer<T> : IRenderer
    {
        void Render(T[][] data);
    }
}
