namespace BlockScanner.Factories
{
    using System;
    using Rendering;

    public sealed class RendererFactory : PluginFactoryBase<IRenderer>
    {
        private static readonly Lazy<RendererFactory> lazy =
                new Lazy<RendererFactory>(() => new RendererFactory());

        public static RendererFactory Instance { get { return lazy.Value; } }

        private RendererFactory()
            : base(PluginHelpers.DefaultPluginDirectoryName, PluginHelpers.DefaultRendererSearchPattern)
        { }
    }
}
