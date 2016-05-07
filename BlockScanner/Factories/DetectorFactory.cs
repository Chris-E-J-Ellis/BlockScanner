namespace BlockScanner.Factories
{
    using System;
    using Detectors;

    public sealed class DetectorFactory : PluginFactoryBase<IDetector>
    {
        private static readonly Lazy<DetectorFactory> lazy =
                new Lazy<DetectorFactory>(() => new DetectorFactory());

        public static DetectorFactory Instance { get { return lazy.Value; } }

        private DetectorFactory()
            : base(PluginHelpers.DefaultPluginDirectoryName, PluginHelpers.DefaultDetectorSearchPattern)
        { }
    }
}
