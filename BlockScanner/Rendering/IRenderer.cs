namespace BlockScanner.Rendering
{
    public interface IRenderer<T>
    {
        void Render(T[][] data);
    }
}
