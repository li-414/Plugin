namespace PluginContracts
{
    public interface IPlugin
    {
        string Name { get; }
        string Version { get; }
        void Init(IServiceProvider services);
        void Run();
    }
}
