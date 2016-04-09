namespace BlockScanner.Rendering
{
    using System;

    public abstract class BaseRenderer<T> : IRenderer<T>
    {
        public Type RendererInputType => typeof(T);

        public virtual void Render(T[][] data)
        {
            return;
        }
    }
}
