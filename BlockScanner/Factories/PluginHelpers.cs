﻿namespace BlockScanner.Factories
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    public static class PluginHelpers
    {
        public const string DefaultPluginDirectoryName = "Plugins";
        public const string DefaultRendererSearchPattern = "*Renderer.dll";
        public const string DefaultDetectorSearchPattern = "*Detector.dll";

        internal static IEnumerable<Assembly> GetCurrentDomainAssemblies()
        {
            return AppDomain.CurrentDomain.GetAssemblies();
        }

        internal static IEnumerable<Assembly> GetPluginFolderAssemblies(string pluginFolder, string searchPattern = null)
        {
            if (!Directory.Exists(pluginFolder))
                return Enumerable.Empty<Assembly>();

            var files = Directory.GetFiles(pluginFolder, searchPattern).ToList();

            var assemblies = files
                .Select(LoadAssembly)
                .Where(a => a != null);

            return assemblies;
        }

        internal static IEnumerable<Type> GetTypes<T>(this IEnumerable<Assembly> assemblies)
        {
            if (assemblies == null)
                return Enumerable.Empty<Type>(); 

            var types = assemblies
                .SelectMany(t => t.GetTypes()
                .Where(p => typeof(T).IsAssignableFrom(p)
                && !p.IsInterface
                && !p.IsAbstract
                && !p.ContainsGenericParameters)); 
            return types;
        }

        private static Assembly LoadAssembly(string filePath)
        {
            Assembly assembly = null;

            try
            {
                assembly = Assembly.LoadFile(filePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An exception occurred whilst loading assembly '{filePath}': {ex}");
            }

            return assembly;
        }
    }
}
