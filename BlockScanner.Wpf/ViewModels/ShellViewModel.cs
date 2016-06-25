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
    using Caliburn.Micro;

    public class ShellViewModel : PropertyChangedBase, IShell
    {
        private readonly List<IDetector> detectors = new List<IDetector>();
        private readonly List<IRendererViewModel> renderers = new List<IRendererViewModel>();

        private IDetector selectedDetector;
        private IRendererViewModel selectedRenderer;

        private int consoleVisible = InteropHelper.SW_SHOW;

        public ShellViewModel()
        {
            Initialise();
        }

        public ScannerViewModel Scanner { get; private set; }

        public IEnumerable<IDetector> Detectors => detectors;
        public IEnumerable<IDetector> ValidDetectors
        {
            get
            {
                var multiSource = (SelectedRenderer?.Renderer as IMultiSourceRenderer);
                if (multiSource != null)
                {
                    return Detectors.Where(d => multiSource.ScannerSlots.Any(s => s.ScanType == d.DetectedPointOutputType));
                }

                return Detectors.Where(d => d.DetectedPointOutputType == (SelectedRenderer?.Renderer as ISingleSourceRenderer)?.RendererInputType);
            }
        }

        public IEnumerable<IRendererViewModel> Renderers => renderers; 

        public IDetector SelectedDetector
        {
            get { return this.selectedDetector; }
            set
            {
                this.selectedDetector = value;

                NotifyOfPropertyChange(() => CanCreateScanner);
            }
        }

        public IRendererViewModel SelectedRenderer
        {
            get { return this.selectedRenderer; }
            set
            {
                this.selectedRenderer = value; 

                NotifyOfPropertyChange(() => ValidDetectors);
                NotifyOfPropertyChange(() => CanCreateScanner);
                NotifyOfPropertyChange(() => SelectedRenderer);
            }
        }

        public bool CanCreateScanner => SelectedDetector != null 
            && SelectedDetector.DetectedPointOutputType == (SelectedRenderer.Renderer as ISingleSourceRenderer)?.RendererInputType;

        public bool CanDumpScanArea => Scanner.ScanArea.Width >= 0 && Scanner.ScanArea.Height > 0;

        public void Initialise()
        {
            InteropHelper.AllocConsole();
            consoleVisible = InteropHelper.SW_SHOW;

            LoadDetectors();
            LoadRenderers();

            Scanner = new ScannerViewModel(ScannerFactory.CreateBasic());

            // Quick test.
            var renderer = Renderers.OfType<IRendererViewModel>().First(r => r.Renderer.GetType() == typeof(BasicRenderer));
            renderer.Renderer.Initialise();
            (renderer.Renderer as ISingleSourceRenderer).AttachScanner(Scanner.Scanner);

            SelectedRenderer = renderer;
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

            SelectedRenderer.Renderer.Initialise();

            (SelectedRenderer.Renderer as ISingleSourceRenderer)?.AttachScanner(scanner);

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
            var loadedDetectors = DetectorFactory.Instance.LoadConcreteObjects();

            detectors.AddRange(loadedDetectors);
        }

        private void LoadRenderers()
        {
            var loadedRenderers = RendererFactory.Instance.LoadConcreteObjects();

            // Separate to vm factory?
            var singleSlotRenderers = loadedRenderers.OfType<ISingleSourceRenderer>();

            var multiSlotRenderers = loadedRenderers.OfType<IMultiSourceRenderer>();

            renderers.AddRange(singleSlotRenderers.Select(r => new RendererViewModel(r)));
            renderers.AddRange(multiSlotRenderers.Select(r => new MultiSourceRendererViewModel(r)));
        }
    }
}