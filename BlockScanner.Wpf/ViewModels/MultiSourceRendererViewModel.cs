namespace BlockScanner.Wpf.ViewModels
{
    using System;
    using Caliburn.Micro;
    using Rendering;

    public class MultiSourceRendererViewModel : PropertyChangedBase, IDisposable
    {
        public MultiSourceRendererViewModel(IMultiSourceRenderer renderer)
        {
            this.Renderer = renderer;
        }

        public IMultiSourceRenderer Renderer
        {
            get; private set;
        }

        public void Dispose()
        {
        }
    }
}
