namespace BlockScanner.Rendering
{
    public abstract class BaseRenderer<T> : IRenderer 
    {
        public virtual void Initialise() { }

        public override string ToString() => GetType().Name;
    }
}
