﻿namespace BlockScanner.Wpf.ViewModels
{
    using System;
    using Caliburn.Micro;
    using Rendering;

    public class RendererViewModel : PropertyChangedBase, IRendererViewModel, IDisposable
    {
        public RendererViewModel(IRenderer renderer)
        {
            this.Renderer = renderer;
        }

        public IRenderer Renderer { get; private set; }

        public void Dispose()
        {
        }

        public override string ToString()
        {
            return Renderer.GetType().Name;
        }
    }
}
