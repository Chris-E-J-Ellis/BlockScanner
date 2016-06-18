namespace BlockScanner.Wpf.ViewModels
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using Detectors;
    using Factories;
    using Rendering;
    using Helpers;
    using Views;

    public class ShellViewModel : Caliburn.Micro.PropertyChangedBase, IShell
    {
        private readonly List<IDetector> detectors = new List<IDetector>();
        private readonly List<IRenderer> renderers = new List<IRenderer>();

        private IDetector selectedDetector;
        private IRenderer selectedRenderer;

        private int consoleVisible = InteropHelper.SW_SHOW;

        public ShellViewModel()
        {
            Initialise();
        }

        public ScannerViewModel Scanner { get; private set; }

        public IEnumerable<IDetector> Detectors => detectors;
        public IEnumerable<IRenderer> Renderers => renderers;
        public IEnumerable<IRenderer> ValidRenderers => renderers.Where(r => r.RendererInputType == SelectedDetector?.DetectedPointOutputType);

        public IDetector SelectedDetector
        {
            get { return this.selectedDetector; }
            set
            {
                this.selectedDetector = value;

                NotifyOfPropertyChange(() => SelectedDetector);
                NotifyOfPropertyChange(() => ValidRenderers);
                NotifyOfPropertyChange(() => CanCreateScanner);
            }
        }

        public IRenderer SelectedRenderer
        {
            get { return this.selectedRenderer; }
            set
            {
                this.selectedRenderer = value;

                NotifyOfPropertyChange(() => CanCreateScanner);
            }
        }

        public bool CanCreateScanner => SelectedDetector != null 
            && SelectedDetector.DetectedPointOutputType == SelectedRenderer?.RendererInputType;

        public bool CanDumpScanArea => Scanner.ScanArea.Width >= 0 && Scanner.ScanArea.Height > 0;

        public void Initialise()
        {
            InteropHelper.AllocConsole();
            consoleVisible = InteropHelper.SW_SHOW;

            LoadDetectors();
            LoadRenderers();

            Scanner = new ScannerViewModel(ScannerFactory.CreateBasic());

            var renderer = new BasicRenderer();
            renderer.Initialise();
            Scanner.AttachRenderer(renderer);
        }

        public void BeginSelection()
        {
            // TODO: Remove view dependency.
            var captureZone = new CaptureWindow();
            captureZone.ShowDialog();

            Scanner.SetScanArea(captureZone.SelectionAreaRectangle);

            Scanner.DumpScanArea();

            Scanner.Scan();

            NotifyOfPropertyChange(() => CanDumpScanArea);
        }

        public void SingleScan()
        {
            Scanner.SingleScan();
        }

        public void DumpScanArea()
        {
            Scanner.DumpScanArea();
        }

        public void RunTestArea()
        {
            Scanner.SetScanArea(new Rectangle(0, 0, 400, 400));

            Scanner.DumpScanArea();

            Scanner.Scan();
        }

        public void CreateScanner()
        {
            Scanner.Dispose();

            var scanner = ScannerFactory.Create(SelectedDetector);

            SelectedRenderer.Initialise();

            scanner.AttachRenderer(selectedRenderer);

            Scanner = new ScannerViewModel(scanner);

            NotifyOfPropertyChange(() => Scanner);
        }

        public void ToggleConsole()
        {
            var handle = InteropHelper.GetConsoleWindow();

            consoleVisible = consoleVisible == InteropHelper.SW_HIDE
                ? InteropHelper.SW_SHOW
                : InteropHelper.SW_HIDE;

            InteropHelper.ShowWindow(handle, consoleVisible);
        }

        private void LoadDetectors()
        {
            var loadedDetector = DetectorFactory.Instance.LoadConcreteObjects();

            detectors.AddRange(loadedDetector);
        }

        private void LoadRenderers()
        {
            var loadedRenderer = RendererFactory.Instance.LoadConcreteObjects();

            renderers.AddRange(loadedRenderer);
        }
    }
}