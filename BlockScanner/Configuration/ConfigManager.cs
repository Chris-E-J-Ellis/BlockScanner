namespace BlockScanner.Config
{
    using System;
    using System.IO;
    using System.Xml.Serialization;

    public sealed class ConfigManager : IConfigManager
    {
        private static readonly Lazy<ConfigManager> lazyConfigManager
            = new Lazy<ConfigManager>(() => new ConfigManager());

        public static ConfigManager Instance { get { return lazyConfigManager.Value; } }

        // Potentially make configurable.
        private readonly string configDirectory = "Config";
        private readonly string xmlExtension = ".xml";

        public T Load<T>(string configName) 
            where T : IConfig, new()
        {
            var fullPath = Path.Combine(configDirectory, configName + xmlExtension);

            if (!File.Exists(fullPath))
                return new T();

            using (var fileStream = new FileStream(fullPath, FileMode.Open))
            {
                var serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(fileStream);
            }
        }

        public void Save<T>(T config)
            where T : IConfig, new()
        {
            if (!Directory.Exists(configDirectory))
                Directory.CreateDirectory(configDirectory);

            using (var fileStream = new FileStream(Path.Combine(configDirectory, config.Name + xmlExtension), FileMode.OpenOrCreate))
            {
                var serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(fileStream, config);
            }
        }
    }
}
