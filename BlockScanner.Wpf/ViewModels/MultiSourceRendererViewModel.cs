namespace BlockScanner.Wpf.ViewModels
{
    using System;
    using Caliburn.Micro;
    using Rendering;

    public class MultiSourceRendererViewModel : PropertyChangedBase, IRendererViewModel, IDisposable 
    {
        public MultiSourceRendererViewModel(IMultiSourceRenderer renderer)
        {
            this.Renderer = renderer;
        }

        public IMultiSourceRenderer Renderer
        {
            get; private set;
        }

        public ScannerViewModel Scanner
        {
            get
            {
                return null;
            }
        }

        ISingleSourceRenderer IRendererViewModel.Renderer
        {
            get
            {
                return null;
            }
        }

        // Attach to the first available slot?
        public void AttachScanner(IScanner scanner)
        {
            //throw new NotImplementedException();
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
