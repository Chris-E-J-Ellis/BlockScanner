namespace BlockScanner.Wpf
{
    using System.Collections.Generic;
    using Detectors;
    using Rendering;
    using Factories;

    public class MainWindowViewModel
    {
        public MainWindowViewModel()
        {
            Detectors = new List<IDetector>();
            Renderers = new List<IRenderer>();
        }

        public List<IDetector> Detectors { get; private set; }
        public List<IRenderer> Renderers { get; private set; }

        public IDetector SelectedDetector { get; set; }
        public IRenderer SelectedRenderer { get; set; }

        public void Initialise()
        {
            ScannerVM = new ScannerViewModel();

            DetectorFactory.Instance.LoadTypes();
            RendererFactory.Instance.LoadTypes();

            LoadDetectors();
            LoadRenderers();
        }

        private void LoadDetectors()
        {
            var detectors = DetectorFactory.Instance.LoadConcreteObjects();

            Detectors.AddRange(detectors);
        }

        private void LoadRenderers()
        {
            var renderers = RendererFactory.Instance.LoadConcreteObjects();

            Renderers.AddRange(renderers);
        }

        public ScannerViewModel ScannerVM { get; private set; } 
    }
}
