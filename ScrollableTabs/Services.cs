using Dalamud.Plugin;
using Dalamud.Plugin.Services;

namespace ScrollableTabs;

public static class Services
{
    public static IDalamudPluginInterface PluginInterface { get; private set; } = null!;
    public static IPluginLog PluginLog { get; private set; } = null!;
    public static IFramework Framework { get; private set; } = null!;
    public static IGameConfig GameConfig { get; private set; } = null!;
    public static ICommandManager CommandManager { get; private set; } = null!;
    public static PluginConfig Config { get; private set; } = null!;

    // 台服移植：上游在這裡用 pluginInterface.GetService<T>()，那條路徑在 Dalamud 內部是 sync-over-async；
    // API13 只有同步建構子，搬過來就等於在載入執行緒上阻塞等 async。改成由 Plugin 類別上的 [PluginService] 靜態屬性接收——
    // Dalamud 是「先做屬性注入、再叫建構子」，所以建構子執行時這些值必定已經填好。
    internal static void Initialize(
        IDalamudPluginInterface pluginInterface,
        IPluginLog pluginLog,
        IFramework framework,
        IGameConfig gameConfig,
        ICommandManager commandManager)
    {
        PluginInterface = pluginInterface;
        PluginLog = pluginLog;
        Framework = framework;
        GameConfig = gameConfig;
        CommandManager = commandManager;
        Config = PluginConfig.Load();
    }
}
