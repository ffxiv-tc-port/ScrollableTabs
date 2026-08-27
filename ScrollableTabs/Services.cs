using Dalamud.Plugin;
using Dalamud.Plugin.Services;

namespace ScrollableTabs;

public static class Services
{
    public static IDalamudPluginInterface PluginInterface { get; private set; } = null!;
    public static IPluginLog PluginLog { get; private set; } = null!;
    public static IFramework Framework { get; private set; } = null!;
    public static IGameConfig GameConfig { get; private set; } = null!;
    public static PluginConfig Config { get; private set; } = null!;

    // 台服移植：上游原本在這裡用 pluginInterface.GetService<T>()，那條路徑在 Dalamud 內部是
    // sync-over-async（ServiceScope.GetService 會 .GetAwaiter().GetResult()）。上游把它放在
    // IAsyncDalamudPlugin.LoadAsync 裡所以無妨，但 API13 只有同步建構子，搬過來就等於在載入
    // 執行緒上阻塞等 async。改成由 Plugin 類別上的 [PluginService] 靜態屬性接收——
    // Dalamud 的 ServiceContainer.CreateAsync 是「先做屬性注入、再叫建構子」，
    // 所以建構子執行時這些值必定已經填好，而且完全不經過 sync-over-async。
    internal static void Initialize(
        IDalamudPluginInterface pluginInterface,
        IPluginLog pluginLog,
        IFramework framework,
        IGameConfig gameConfig)
    {
        PluginInterface = pluginInterface;
        PluginLog = pluginLog;
        Framework = framework;
        GameConfig = gameConfig;
        Config = PluginConfig.Load();
    }
}
