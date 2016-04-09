using System;

namespace BlockScanner.Configuration
{
    public sealed class ConfigurationManager : IConfigurationManager
    {
        private static readonly Lazy<ConfigurationManager> lazyConfigManager
            = new Lazy<ConfigurationManager>(() => new ConfigurationManager());

        public static ConfigurationManager Instance { get { return lazyConfigManager.Value; } }

        public T Load<T>(string configName)
        {
            return default(T);
        }

        public void Save<T>(T configName)
        {
        }
    }
}
