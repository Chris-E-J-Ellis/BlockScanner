namespace BlockScanner.Config
{
    public interface IConfigManager
    {
        T Load<T>(string configName) 
            where T : IConfig, new();

        void Save<T>(T config)
            where T : IConfig, new();
    }
}
