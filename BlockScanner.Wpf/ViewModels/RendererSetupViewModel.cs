namespace BlockScanner.Wpf.ViewModels
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Drawing;
    using Caliburn.Micro;
    using Detectors;
    using Helpers;
    using Rendering;
    using Factories;
    using Views;

    public class RendererSetupViewModel : PropertyChangedBase, IShell
    {
        private readonly IEnumerable<IDetector> detectors = new List<IDetector>();

        private IDetector selectedDetector;
        private IRendererViewModel renderer;

        private int consoleVisible = InteropHelper.SW_SHOW;

        public RendererSetupViewModel(ISingleSourceRenderer renderer, IEnumerable<IDetector> detectors)
        {
            this.detectors = detectors;
            this.renderer = new RendererViewModel(renderer);

            Initialise();
        }

        public ScannerViewModel Scanner { get { return renderer.Scanner; } }

        public IRendererViewModel Renderer => this.renderer;
        public IEnumerable<IDetector> Detectors => detectors;
        public IEnumerable<IDetector> ValidDetectors => Detectors.Where(d => d.DetectedPointOutputType == Renderer.Renderer.RendererInputType);

        public IDetector SelectedDetector
        {
            get { return this.selectedDetector; }
            set
            {
                this.selectedDetector = value;

                NotifyOfPropertyChange(() => CanCreateScanner);
            }
        }

        public bool CanCreateScanner => SelectedDetector != null
            && SelectedDetector.DetectedPointOutputType == Renderer.Renderer.RendererInputType;

        public bool CanDumpScanArea => Scanner?.ScanArea.Width >= 0 && Scanner?.ScanArea.Height > 0;

        public void Initialise()
        {
            InteropHelper.AllocConsole();
            consoleVisible = InteropHelper.SW_SHOW;

            // Quick test.
            Renderer.AttachScanner(ScannerFactory.CreateBasic());

            this.SelectedDetector = detectors.First();
        }

        public void CreateScanner()
        {
            Scanner.Dispose();

            var scanner = ScannerFactory.Create(SelectedDetector);

            Renderer.Renderer.Initialise();

            renderer.AttachScanner(scanner);

            // TODO: Remove view dependency.
            var captureZone = new CaptureWindow();
            captureZone.ShowDialog();

            Scanner.SetScanArea(captureZone.SelectionAreaRectangle);

            Scanner.DumpScanArea();

            Scanner.Scan();

            NotifyOfPropertyChange(() => CanDumpScanArea);
        }

        public void ToggleConsole()
        {
            var handle = InteropHelper.GetConsoleWindow();

            consoleVisible = consoleVisible == InteropHelper.SW_HIDE
                ? InteropHelper.SW_SHOW
                : InteropHelper.SW_HIDE;

            InteropHelper.ShowWindow(handle, consoleVisible);
        }

        public override string ToString()
        {
            return this.Renderer.Renderer.GetType().Name;
        }

        //Temporary whilst restructuring.
        public void KillScan()
        {
            this.Scanner?.Stop();
        }
    }
}
