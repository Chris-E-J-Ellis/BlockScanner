namespace BlockScanner.Wpf.ViewModels
{
    public interface IRendererViewModel
    {
        bool IsMultiSource { get; }

        void HaltScanners();
    }
}
