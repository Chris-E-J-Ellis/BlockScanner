namespace BlockScanner.Wpf.ViewModels
{
    using Rendering;

    public interface IRendererViewModel
    {
        ISingleSourceRenderer Renderer { get; }
        ScannerViewModel Scanner { get; }

        void AttachScanner(IScanner scanner);
    }
}
