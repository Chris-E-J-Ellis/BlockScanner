namespace BlockScanner.Config
{
    using System;

    public class BasicConfig<T> : IConfig
    {
        public Type ConfigType => typeof(T);

        public string Name { get; set; } = $"{typeof(T).Name}.Default";
    }
}
