namespace BlockScanner.Configuration
{
    public interface IConfigurationManager
    {
        T Load<T>(string configName);

        void Save<T>(T config);
    }
}
