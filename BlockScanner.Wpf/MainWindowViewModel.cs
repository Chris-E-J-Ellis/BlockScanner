namespace BlockScanner.Wpf
{
    using System;
    using System.Collections.Generic;
    using Detectors;
    using System.Linq;
    using Rendering;

    public class MainWindowViewModel
    {
        private ScannerViewModel scannerVM;

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
            LoadDetectors();
            LoadRenderers();
        }

        private void LoadDetectors()
        {
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => typeof(IDetector).IsAssignableFrom(p)
                && !p.IsInterface
                && !p.IsAbstract);

            foreach (var type in types)
            {
                var activatedType = Activator.CreateInstance(type);

                Detectors.Add((IDetector)activatedType);
            }
        }

        private void LoadRenderers()
        {
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => typeof(IRenderer).IsAssignableFrom(p)
                && !p.IsInterface
                && !p.IsAbstract);

            foreach (var type in types)
            {
                var activatedType = Activator.CreateInstance(type);

                Renderers.Add((IRenderer)activatedType);
            }
        }

        public ScannerViewModel ScannerVM { get { return this.scannerVM; } }
    }
}
