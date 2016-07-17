namespace BlockScanner.Rendering.SingleSource
{
    using System;

    public class BaseSingleSourceRenderer<T> : BaseRenderer<T>, ISingleSourceRenderer<T>
    {
        private IScanner<T> scanner;

        private EventHandler<T> scanAction;

        public Type RendererInputType => typeof(T);

        public IScanner Scanner => scanner;

        public void AttachScanner(IScanner scanner)
        {
            var downcastScanner = (scanner as IScanner<T>);
            if (downcastScanner != null)
            {
                // Remove existing subscription. A tad messy.
                if (this.scanner != null)
                {
                    this.scanner.FrameScanned -= scanAction;
                }

                this.scanner = downcastScanner;

                this.scanAction = (o, e) => Render(e);

                this.scanner.FrameScanned += scanAction;
            }
        }

        public void DetachScanner(IScanner scanner)
        {
            var downcastScanner = (scanner as IScanner<T>);
            if (downcastScanner != null)
            {
                this.scanner = downcastScanner;

                this.scanner.FrameScanned -= scanAction;
            }
        }

        public virtual void Dispose()
        {
            if (scanner == null)
                return;

            scanner.FrameScanned -= scanAction;
        }

        public virtual void Render(T data) { }
    }
}
