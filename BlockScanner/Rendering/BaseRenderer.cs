namespace BlockScanner.Rendering
{
    using System;

    public abstract class BaseRenderer<T> : IRenderer<T>
    {
        public Type RendererInputType => typeof(T);

        public virtual void Initialise() { }

        public virtual void Render(T data) { }

        public override string ToString()
        {
            return GetType().Name;
        }
    }
}
