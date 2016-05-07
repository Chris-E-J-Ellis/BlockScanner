namespace BlockScanner.Factories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public abstract class PluginFactoryBase<T>
    {
        private IEnumerable<Type> types;
        private IEnumerable<T> concreteObjects;
        private readonly string pluginFolder;
        private readonly string searchPattern;

        public PluginFactoryBase(string pluginFolder, string searchPattern)
        {
            this.pluginFolder = pluginFolder;
            this.searchPattern = searchPattern;
        }

        public T Create(Type type)
        {
            if (type == null)
                throw new ArgumentException("type is null");

            if (!typeof(T).IsAssignableFrom(type))
                throw new ArgumentException($"type is not a valid {typeof(T)}");

            return (T)Activator.CreateInstance(type);
        }

        public virtual void LoadTypes()
        {
            var foundTypes = new List<Type>();

            foundTypes.AddRange(GetDomainRenderers());
            foundTypes.AddRange(GetPluginRenderers(this.pluginFolder, this.searchPattern));

            types = foundTypes;
        }

        public IEnumerable<T> LoadConcreteObjects()
        {
            concreteObjects = types.Select(Create);

            return concreteObjects;
        }

        private static IEnumerable<Type> GetDomainRenderers()
        {
            var assemblies = PluginHelpers.GetCurrentDomainAssemblies();

            var types = assemblies.GetTypes<T>();

            return types;
        }

        private static IEnumerable<Type> GetPluginRenderers(string pluginFolder, string searchPattern)
        {
            var assemblies = PluginHelpers.GetPluginFolderAssemblies(Environment.CurrentDirectory, searchPattern);

            var types = assemblies.GetTypes<T>();

            return types;
        }
    }
}
