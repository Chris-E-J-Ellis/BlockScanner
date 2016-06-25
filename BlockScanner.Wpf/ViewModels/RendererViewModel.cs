namespace BlockScanner.Wpf.ViewModels
{
    using System;
    using Caliburn.Micro;
    using Rendering;

    public class RendererViewModel : PropertyChangedBase, IRendererViewModel, IDisposable
    {
        public RendererViewModel(ISingleSourceRenderer renderer)
        {
            this.Renderer = renderer;
        }

        public ScannerViewModel Scanner { get; private set; }

        public ISingleSourceRenderer Renderer { get; private set; }

        public void AttachScanner(IScanner scanner)
        {
            Renderer.AttachScanner(scanner);

            this.Scanner = new ScannerViewModel(scanner);

            NotifyOfPropertyChange(() => Scanner);
        }

        public void Dispose()
        {
        }

        public override string ToString()
        {
            return Renderer.GetType().Name;
        }
    }
}
