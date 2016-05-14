namespace BlockScanner.Config
{
    public interface IConfigurable<T>
        where T : IConfig
    {
        T Config { get; }

        void SetConfig(T config);
    }
}
