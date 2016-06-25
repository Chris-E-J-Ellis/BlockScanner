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

        IRenderer IRendererViewModel.Renderer => this.Renderer;

        public void Dispose()
        {
        }

        public override string ToString()
        {
            return Renderer.GetType().Name;
        }
    }
}
