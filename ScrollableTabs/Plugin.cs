using System;
using Dalamud.Interface.Windowing;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.System.Input;
using FFXIVClientStructs.FFXIV.Client.UI;

namespace ScrollableTabs;

public unsafe class Plugin : IDalamudPlugin
{
    // 🔴 屬性注入發生在建構子之前（Dalamud/IoC/Internal/ServiceContainer.cs CreateAsync：
    //    先 InjectProperties 再 ctor.Invoke），所以建構子裡直接讀這些欄位是安全的。
    [PluginService] internal static IDalamudPluginInterface DalamudPluginInterface { get; private set; } = null!;
    [PluginService] internal static IPluginLog DalamudPluginLog { get; private set; } = null!;
    [PluginService] internal static IFramework DalamudFramework { get; private set; } = null!;
    [PluginService] internal static IGameConfig DalamudGameConfig { get; private set; } = null!;

    private PluginWindowSystem? _windowSystem;
    private ConfigWindow? _configWindow;

    public Plugin()
    {
        Services.Initialize(DalamudPluginInterface, DalamudPluginLog, DalamudFramework, DalamudGameConfig);

        _windowSystem = new PluginWindowSystem();
        _configWindow = new ConfigWindow();
        _windowSystem.AddWindow(_configWindow);

        Services.Framework.Update += OnFrameworkUpdate;
    }

    public void Dispose()
    {
        Services.Framework.Update -= OnFrameworkUpdate;

        if (_configWindow != null)
        {
            _windowSystem?.RemoveWindow(_configWindow);
            _configWindow.Dispose();
            _configWindow = null;
        }

        _windowSystem?.Dispose();
        _windowSystem = null;
    }

    private void OnFrameworkUpdate(IFramework framework)
    {
        var atkModule = RaptureAtkModule.Instance();
        if (atkModule == null || atkModule->UIScene != GameUIScene.GameMain)
            return;

        var hoveredUnitBase = atkModule->AtkCollisionManager.IntersectingAddon;
        if (hoveredUnitBase == null)
            return;

        var inputData = UIInputData.Instance();
        if (inputData == null)
            return;

        // 台服移植：上游讀 UIInputData.CurrentMouseDragButtons（[FieldOffset(0x9AC)]），
        // 我方 API13 釘住的 FFXIVClientStructs 沒有這個欄位。刻意「不」把上游 HEAD 的偏移抄過來
        // ——那份追的是比 7.20 更新的全球版客戶端，偏移錯了是靜默讀到鄰居欄位。
        // 改用同一顆結構裡已經存在、且語意等價的旗標：游標有任何鍵按住＝正在拖曳，不處理滾輪。
        if (inputData->CursorInputs.MouseButtonHeldFlags != MouseButtonFlags.None)
            return;

        var wheelState = inputData->CursorInputs.MouseWheel;
        if (wheelState == 0)
            return;

        wheelState = Math.Clamp(wheelState, -1, 1);

        if (!Services.Config.Invert)
            wheelState = -wheelState;

        ScrollHandlers.Handle(hoveredUnitBase, wheelState);
    }

    public class PluginWindowSystem : WindowSystem, IDisposable
    {
        public PluginWindowSystem() : base("ScrollableTabs")
        {
            Services.PluginInterface.UiBuilder.Draw += Draw;
        }

        public void Dispose()
        {
            Services.PluginInterface.UiBuilder.Draw -= Draw;
            RemoveAllWindows();
        }
    }
}
