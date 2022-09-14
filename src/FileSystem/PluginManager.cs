using System.Reflection;
using EncryptionAlgorithms;
using HashAlgorithms;

namespace FileSystem
{
    internal class PluginManager
    {
        // Protected fields
        protected string _pluginsFolderPath = String.Empty;

        // Methods
        public PluginManager()
        {
            string? location = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location);
            if (location == null)
                throw new Exception("Executable location not received");

            _pluginsFolderPath = Path.Combine(
                location,
                "Plugins"
            );

            if (!Directory.Exists(_pluginsFolderPath))
                Directory.CreateDirectory(_pluginsFolderPath);
        }

        public string[]? GetAllPluginsTitles()
        {
            // Get all data files in plugins folder
            string? location = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location);
            if (location == null)
                return null;

            IEnumerable<string> dataFiles = Directory.GetFiles(_pluginsFolderPath, "*")
                .Where(path => Path.GetExtension(path) == ".dll")
                .Select(path => Path.GetFileNameWithoutExtension(path));

            return dataFiles.ToArray();
        }

        /// <summary>
        /// Returns the algorithm classes found in the library file
        /// </summary>
        protected List<T>? GetAlgorithms<T>(string pluginTitle) where T : class
        {
            Assembly? assembly = LoadAssembly(pluginTitle + ".dll");
            if (assembly == null)
                return null;

            List<T> result = new();
            try
            {
                foreach (Type type in assembly.GetExportedTypes())
                {
                    if (type.IsClass && typeof(T).IsAssignableFrom(type))
                    {
                        T? algorithm =
                            Activator.CreateInstance(type) as T;

                        if (algorithm != null)
                            result.Add(algorithm);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.GetType());
            }

            return result;
        }

        public List<IEncryptionAlgorithm>? GetEncryptionAlgorithms(string pluginTitle)
        {
            return GetAlgorithms<IEncryptionAlgorithm>(pluginTitle);
        }

        public List<IHashAlgorithm>? GetHashAlgorithms(string pluginTitle)
        {
            return GetAlgorithms<IHashAlgorithm>(pluginTitle);
        }

        protected Assembly? LoadAssembly(string pluginTitle)
        {
            String pluginPath = Path.Combine(
                _pluginsFolderPath,
                Path.GetFileNameWithoutExtension(pluginTitle) + ".dll"
            );
            if (!File.Exists(pluginPath))
                return null;

            try
            {
                Assembly assembly = Assembly.LoadFrom(pluginPath);
                return assembly;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.GetType());
                return null;
            }
        }
    }
}
