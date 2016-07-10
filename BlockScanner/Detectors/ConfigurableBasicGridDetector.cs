namespace BlockScanner.Detectors
{
    using Config;
    using Config.Detectors;

    public class ConfigurableBasicGridDetector<T> : BaseGridDetector<T>, IConfigurable<BasicGridDetectorConfig>
    {
        private IConfigManager configurationManager;

        public ConfigurableBasicGridDetector()
            : this(ConfigManager.Instance)
        {
        }

        public ConfigurableBasicGridDetector(IConfigManager configurationManager)
        {
            this.configurationManager = configurationManager;
        }

        public BasicGridDetectorConfig Config { get; private set; } = new BasicGridDetectorConfig();

        public void SetConfig(BasicGridDetectorConfig config)
        {
            this.Config = config;

            SetGridSize(config.GridWidth, config.GridHeight);
            SetSamplePointCentre(config.SamplePointCentreWidthRatio, config.SamplePointCentreHeightRatio);
        }

        public override void Initialise()
        {
            var config = configurationManager.Load<BasicGridDetectorConfig>("Default")
                ?? new BasicGridDetectorConfig();

            SetConfig(config);
        }
    }
}
