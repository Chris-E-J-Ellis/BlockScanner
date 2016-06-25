namespace BlockScanner.Rendering
{
    using System;

    public abstract class BaseRenderer<T> : ISingleSourceRenderer<T> , IDisposable
    {
        private IScanner<T> scanner;

        public Type RendererInputType => typeof(T);

        public IScanner Scanner => scanner;

        public void AttachScanner(IScanner scanner)
        {
            var downcastScanner = (scanner as IScanner<T>);
            if (downcastScanner != null)
            {
                this.scanner = downcastScanner;
                this.scanner.FrameScanned += (o, e) => Render(e);
            }
        }

        public void DetachScanner(IScanner scanner)
        {
            var downcastScanner = (scanner as IScanner<T>);
            if (downcastScanner != null)
            {
                this.scanner = downcastScanner;
                this.scanner.FrameScanned -= (o, e) => Render(e);
            }
        }

        public virtual void Dispose()
        {
            if (scanner == null)
                return;

            scanner.FrameScanned -= (o, e) => Render(e);
        }

        public virtual void Initialise() { }

        public virtual void Render(T data) { }

        public override string ToString()
        {
            return GetType().Name;
        }
    }
}
