namespace BlockScanner.Wpf.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Caliburn.Micro;
    using Detectors;
    using Rendering;
    using Factories;
    using Views;

    public class SingleSourceRendererViewModel : PropertyChangedBase, IRendererViewModel, IDisposable
    {
        private readonly IEnumerable<IDetector> detectors = new List<IDetector>();
        private readonly ISingleSourceRenderer renderer;
        private ScannerViewModel scanner;

        public SingleSourceRendererViewModel(ISingleSourceRenderer renderer, IEnumerable<IDetector> detectors)
        {
            this.detectors = detectors;
            this.renderer = renderer;

            Initialise();
        }

        public ScannerViewModel Scanner
        {
            get { return this.scanner; }
            set
            {
                this.scanner = value;

                NotifyOfPropertyChange(() => Scanner);
            }
        }

        public ISingleSourceRenderer Renderer => this.renderer;

        public IEnumerable<IDetector> Detectors => detectors;

        public IEnumerable<IDetector> ValidDetectors => Detectors.Where(d => d.DetectedPointOutputType == Renderer.RendererInputType);

        public IDetector SelectedDetector { get; set; }

        public bool IsMultiSource => false;

        public void Initialise()
        {
            SelectedDetector = ValidDetectors.First();
        }

        public void AttachScanner()
        {
            Scanner?.Dispose();

            var scanner = ScannerFactory.Create(SelectedDetector);

            Scanner = new ScannerViewModel(scanner);

            Renderer.Initialise();

            Renderer.AttachScanner(scanner);

            // TODO: Remove view dependency.
            var windowManager = new WindowManager();

            var captureZone = new CaptureWindowViewModel(); 
            windowManager.ShowDialog(captureZone);

            var rect = captureZone.CaptureRegions.First().Region;

            Scanner.SetScanArea(rect);

            Scanner.DumpScanArea();

            Scanner.Scan();
        }

        public override string ToString()
        {
            return Renderer.GetType().Name;
        }

        //Temporary whilst restructuring.
        public void HaltScanners()
        {
            Scanner?.Stop();
        }

        public void Dispose()
        {
            Scanner?.Dispose();
        }
    }
}
