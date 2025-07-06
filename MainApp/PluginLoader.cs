using PluginContracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace MainApp
{
    public class PluginLoader
    {
        public class PluginManifest
        {
            public string Main { get; set; } = null!;
            public string Entry { get; set; } = null!;
            public string Name { get; set; } = null!;
        }

        static JsonSerializerOptions jsonSerializerOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };
        public static List<IPlugin> LoadPlugins(string pluginsRootPath)
        {
            List<IPlugin> _plugins = new();

            if (!Directory.Exists(pluginsRootPath)) return _plugins;

            foreach (var dir in Directory.GetDirectories(pluginsRootPath))
            {
                try
                {
                    var manifestPath = Path.Combine(dir, "manifest.json");
                    if (!File.Exists(manifestPath)) continue;

                    var manifestJson = File.ReadAllText(manifestPath);

                    var manifest = JsonSerializer.Deserialize<PluginManifest>(manifestJson, jsonSerializerOptions);
                    if (manifest == null) continue;

                    var dllPath = Path.Combine(dir, manifest.Main);
                    if (!File.Exists(dllPath)) continue;

                    var loadContext = new PluginLoadContext(dllPath);
                    var pluginAsm = loadContext.LoadFromAssemblyPath(dllPath);
                    var pluginType = pluginAsm.GetType(manifest.Entry);
                    var type = typeof(IPlugin).IsAssignableFrom(pluginType);
                    if (pluginType == null || !typeof(IPlugin).IsAssignableFrom(pluginType)) continue;

                    var plugin = (IPlugin?)Activator.CreateInstance(pluginType);
                    plugin?.Init(null);
                    _plugins.Add(plugin!);
                    plugin?.Run();
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show($"加载插件时发生错误: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                
            }

            return _plugins;
        }
    }
}
