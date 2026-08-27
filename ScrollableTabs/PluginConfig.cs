using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Dalamud.Configuration;
using Dalamud.Utility;

namespace ScrollableTabs;

public partial class PluginConfig : IPluginConfiguration
{
    [JsonIgnore]
    public const int CURRENT_CONFIG_VERSION = 1;

    [JsonIgnore]
    public int LastSavedConfigHash { get; set; }

    [JsonIgnore]
    public static JsonSerializerOptions? SerializerOptions { get; private set; }

    public event Action<string>? ConfigOptionChanged;

    public static PluginConfig Load()
    {
        SerializerOptions = new JsonSerializerOptions()
        {
            IncludeFields = true,
            WriteIndented = true,
        };

        var fileInfo = Services.PluginInterface.ConfigFile;
        if (!fileInfo.Exists || fileInfo.Length < 2)
            return new();

        var json = File.ReadAllText(fileInfo.FullName);
        if (JsonNode.Parse(json) is not JsonObject config)
            return new();

        return config.Deserialize<PluginConfig>(SerializerOptions) ?? new();
    }

    public void Save()
    {
        try
        {
            var serialized = JsonSerializer.Serialize(this, SerializerOptions);
            var hash = StringComparer.Ordinal.GetHashCode(serialized);

            if (LastSavedConfigHash != hash)
            {
                FilesystemUtil.WriteAllTextSafe(Services.PluginInterface.ConfigFile.FullName, serialized);
                LastSavedConfigHash = hash;
                Services.PluginLog.Information("Configuration saved.");
            }
        }
        catch (Exception e)
        {
            Services.PluginLog.Error(e, "Error saving config");
        }
    }

    public void RaiseConfigOptionChanged(string fieldName)
    {
        ConfigOptionChanged?.Invoke(fieldName);
    }
}

public partial class PluginConfig
{
    public int Version { get; set; } = CURRENT_CONFIG_VERSION;

    public bool Invert = false;

    // 台服移植：上游的 SuppressQuickPanelSounds 需要對遊戲程式碼做記憶體修補，整段功能已移除。

    // 🔴 台服未驗證：這一項控制的是「寫死的 addon 命令碼」路徑（HandleCommand 0x407、0x40B）。
    //    命令碼在台服對不對離線證明不了，猜錯的失敗形式是「送出別的指令」而不是報錯 ⇒ 預設關。
    //    實機確認過之後可以自行打開，或改成預設開。
    //    （上游另一條用命令碼 22 的「物品欄↔關鍵物品」互切已整條移除，見 ScrollHandlers。）
    public bool AllowUnverifiedMJIFavoritesSwitch = false;

    public bool HandleAetherCurrent = true;
    public bool HandleArmouryBoard = true;
    public bool HandleAOZNotebook = true;
    public bool HandleCharacter = true;
    public bool HandleCharacterClass = true;
    public bool HandleCharacterRepute = true;
    public bool HandleInventoryBuddy = true;
    public bool HandleBuddy = true;
    public bool HandleCurrency = true;

    // 台服移植：HandleGlassSelect（臉部配件）已移除——我方 CS 沒有 AddonGlassSelect 結構。

    public bool HandleOrnamentNoteBook = true;
    public bool HandleFieldRecord = true;
    public bool HandleFishGuide = true;
    public bool HandleMiragePrismPrismBox = true;
    public bool HandleGoldSaucerCardList = true;
    public bool HandleGoldSaucerCardDeckEdit = true;
    public bool HandleLovmPaletteEdit = true;
    public bool HandleInventory = true;
    public bool HandleMJIMinionNoteBook = true;
    public bool HandleMinionNoteBook = true;
    public bool HandleMountNoteBook = true;
    public bool HandleRetainer = true;
    public bool HandleFateProgress = true;
    public bool HandleAdventureNoteBook = true;
}
