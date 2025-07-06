using PluginContracts;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using System.Windows;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using Application = System.Windows.Application;
using MainApp.Helpers;

namespace MainApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static Mutex? _mutex;
        public IServiceProvider ServiceProvider { get; }
        public static new App Current => (App)Application.Current;
        public App()
        {
            //string currentProcessName = Process.GetCurrentProcess().ProcessName;
            //var runningProcesses = Process.GetProcessesByName(currentProcessName).Where(p => p.Id != Environment.ProcessId);
            //foreach (var process in runningProcesses)
            //{
            //    process.Kill();
            //    process.WaitForExit();
            //}

            const string MutexName = "MyAppUniqueMutexName";

            bool createdNew;
            _mutex = new Mutex(true, MutexName, out createdNew);

            if (!createdNew)
            {
                System.Windows.MessageBox.Show("程序已经运行");
                Shutdown();
                return;
            }

            ServiceProvider = ConfigureServices();
        }
        private static ServiceProvider ConfigureServices()
        {
            List<IPlugin> _plugins = new();
            var pluginDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MyApp", "Plugins");
            if (!Directory.Exists(pluginDir)) Directory.CreateDirectory(pluginDir);
            _plugins = PluginLoader.LoadPlugins(pluginDir);
            IconHelper iconHelper = new IconHelper();
            new DesktopShortcutHelper().CreateDesktopShortcut("MainApp");
            var services = new ServiceCollection();
            var app = services.BuildServiceProvider();
            _plugins.ForEach(x => x.Init(app));
            return app;
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
        }
    }

}
