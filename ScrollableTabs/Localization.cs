using System.Collections.Frozen;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace ScrollableTabs;

public static class Localization
{
    // 🔴 台服繁中：TryGetTranslation 直接拿 Services.PluginInterface.UiLanguage 當字典鍵，
    //    而台服的該值恆為 "tw"（撞 ISO 639-1 的 Twi）。因為這裡是純字典而不是 resx，
    //    只要補 "tw" 鍵就會命中，完全繞開 satellite assembly 的 culture 陷阱
    //    ——不需要攔 LanguageChanged，也不需要 Language.zh-Hant.resx。
    //    視窗名稱一律照台服遊戲內用語（來源＝台服 EXD dump 的 Addon 表，括號內是列號）。
    private static readonly FrozenDictionary<string, Dictionary<string, string>> Localizations = new Dictionary<string, Dictionary<string, string>>()
    {
        ["ConfigWindow.WindowName"] = new() {
            { "en", "Scrollable Tabs Configuration" },
            { "de", "Scrollable Tabs Konfiguration" },
            { "tw", "滾輪切換分頁 設定" }
        },
        ["ConfigWindow.GitHubLink.Tooltip"] = new() {
            { "en", "Visit the Scrollable Tabs GitHub Repository" },
            { "de", "Zum Scrollable Tabs GitHub Repository" },
            { "tw", "前往台服移植版的 GitHub 儲存庫" }
        },
        ["ConfigWindow.UpstreamLink.Tooltip"] = new() {
            { "en", "Visit the original (upstream) repository by Haselnussbomber" },
            { "tw", "前往上游原作者 Haselnussbomber 的儲存庫" }
        },
        ["ConfigWindow.SponsorLink.Tooltip"] = new() {
            { "en", "Support me on GitHub Sponsors" },
            { "de", "Unterstütze mich auf GitHub Sponsors" },
            { "tw", "在 GitHub Sponsors 上贊助原作者" }
        },
        ["Config.Invert.Label"] = new() {
            { "en", "Invert scroll behaviour" },
            { "de", "Invertiertes Scrollverhalten" },
            { "zh", "反转滚轮行为" },
            { "ja", "スクロール方向を反転" },
            { "tw", "反轉滾輪方向" }
        },
        ["Config.AllowUnverifiedMJIFavoritesSwitch.Label"] = new() {
            { "en", "Allow switching to/from Favorites in the Island Minion Guide (unverified on TW)" },
            { "tw", "允許在無人島寵物一覽切換「我的最愛」（台服未驗證）" }
        },
        ["Config.AllowUnverifiedMJIFavoritesSwitch.Description"] = new() {
            { "en", "This path sends hard-coded addon command codes (0x407 / 0x40B) that have not been verified against the Taiwanese client. Normal tab scrolling in that window works regardless of this setting. Off by default." },
            { "tw", "這條路徑會送出寫死的 addon 命令碼（0x407／0x40B），尚未對台服客戶端驗證過。該視窗內一般的分頁捲動不受此設定影響。預設關閉。" }
        },
        ["Config.HandleAetherCurrent.Label"] = new() {
            { "en", "Enable in Aether Currents" },
            { "de", "Aktiviere für Windätherquellen" },
            { "zh", "在风脉泉窗口启用" },
            { "ja", "エーテル風脈で有効化" },
            { "tw", "在「風脈泉」啟用" }
        },
        ["Config.HandleArmouryBoard.Label"] = new() {
            { "en", "Enable in Armoury Chest" },
            { "de", "Aktiviere für Arsenal" },
            { "zh", "在兵装库窗口启用" },
            { "ja", "兵装庫で有効化" },
            { "tw", "在「兵裝庫」啟用" }
        },
        ["Config.HandleAOZNotebook.Label"] = new() {
            { "en", "Enable in Blue Magic Spellbook" },
            { "de", "Aktiviere für Zauberbuch der Blaumagie" },
            { "zh", "在青魔法书窗口启用" },
            { "ja", "青魔法手帳で有効化" },
            { "tw", "在「青魔法書」啟用" }
        },
        ["Config.HandleCharacter.Label"] = new() {
            { "en", "Enable in Character" },
            { "de", "Aktiviere für Charakter" },
            { "zh", "在角色窗口启用" },
            { "ja", "キャラクターで有効化" },
            { "tw", "在「人物」啟用" }
        },
        ["Config.HandleCharacterClass.Label"] = new() {
            { "en", "Enable in Character -> Classes/Jobs" },
            { "de", "Aktiviere für Charakter -> Klassen/Jobs" },
            { "zh", "在角色->职业&特职窗口启用" },
            { "ja", "キャラクター → クラス/ジョブで有効化" },
            { "tw", "在「人物 → 職業＆特職」啟用" }
        },
        ["Config.HandleCharacterRepute.Label"] = new() {
            { "en", "Enable in Character -> Reputation" },
            { "de", "Aktiviere für Charakter -> Ansehen" },
            { "zh", "在 角色->评价窗口启用" },
            { "ja", "キャラクター → 名声で有効化" },
            { "tw", "在「人物 → 評價」啟用" }
        },
        ["Config.HandleInventoryBuddy.Label"] = new() {
            { "en", "Enable in Chocobo Saddlebag" },
            { "de", "Aktiviere für Chocobo-Satteltasche" },
            { "zh", "在陆行鸟鞍囊窗口启用" },
            { "ja", "チョコボかばんで有効化" },
            { "tw", "在「陸行鳥鞍囊」啟用" }
        },
        ["Config.HandleInventoryBuddy.Description"] = new() {
            { "en", "The second tab requires a subscription to the Companion Premium Service" },
            { "de", "Der zweite Tab benötigt ein Abonnement des Premium-Nutzungsplans in der Companion-App." },
            { "zh", "第二页标签页需要开通陆行鸟鞍囊2服务" },
            { "ja", "2つ目のタブはコンパニオンアプリのプレミアムサービスへの加入が必要です。" },
            { "tw", "第二個分頁需要開通伙伴應用程式的付費服務。" }
        },
        ["Config.HandleBuddy.Label"] = new() {
            { "en", "Enable in Companion" },
            { "de", "Aktiviere für Mitstreiter" },
            { "zh", "在搭档窗口启用" },
            { "ja", "バディで有効化" },
            { "tw", "在「搭檔」啟用" }
        },
        ["Config.HandleCurrency.Label"] = new() {
            { "en", "Enable in Currency" },
            { "de", "Aktiviere für Vermögen" },
            { "zh", "在货币一览窗口启用" },
            { "ja", "所持金・通貨で有効化" },
            { "tw", "在「貨幣一覽」啟用" }
        },
        ["Config.HandleOrnamentNoteBook.Label"] = new() {
            { "en", "Enable in Fashion Accessories" },
            { "de", "Aktiviere für Modeaccessoires" },
            { "zh", "在时尚配饰窗口启用" },
            { "ja", "ファッションアクセサリーで有効化" },
            { "tw", "在「時尚配件」啟用" }
        },
        ["Config.HandleFieldRecord.Label"] = new() {
            { "en", "Enable in Field Records" },
            { "de", "Aktiviere für Frontbericht" },
            { "zh", "在战果记录窗口启用" },
            { "tw", "在「戰果記錄」啟用" }
        },
        ["Config.HandleFishGuide.Label"] = new() {
            { "en", "Enable in Fish Guide" },
            { "de", "Aktiviere für Fischverzeichnis" },
            { "zh", "在鱼类图鉴窗口启用" },
            { "ja", "魚類図鑑で有効化" },
            { "tw", "在「魚類圖鑑」啟用" }
        },
        ["Config.HandleFishGuide.Description"] = new() {
            { "en", "Note: unlike upstream, this also scrolls on the search tab (the CS field used to detect it is missing in our API13 pin)." },
            { "tw", "注意：與上游不同，搜尋分頁上也會捲動——我方釘住的 FFXIVClientStructs 沒有用來辨識搜尋分頁的欄位，刻意不從新版抄偏移。" }
        },
        ["Config.HandleMiragePrismPrismBox.Label"] = new() {
            { "en", "Enable in Glamour Dresser" },
            { "de", "Aktiviere für Projektionskommode" },
            { "zh", "在投影台窗口启用" },
            { "tw", "在「投影台」啟用" }
        },
        ["Config.HandleMiragePrismPrismBox.Description"] = new() {
            { "en", "Scrolls pages, not tabs." },
            { "de", "Blättert durch Seiten, nicht durch Tabs." },
            { "zh", "滚动页面，而非标签页。" },
            { "ja", "タブではなくページをスクロールします。" },
            { "tw", "捲動的是頁面，不是分頁。" }
        },
        ["Config.HandleGoldSaucerCardList.Label"] = new() {
            { "en", "Enable in Gold Saucer -> Card List" },
            { "de", "Aktiviere für Gold Saucer -> Karten" },
            { "zh", "在金碟游乐场->幻卡列表窗口启用" },
            { "ja", "ゴールドソーサー → カード一覧で有効化" },
            { "tw", "在「金碟遊樂園 → 幻卡列表」啟用" }
        },
        ["Config.HandleGoldSaucerCardDeckEdit.Label"] = new() {
            { "en", "Enable in Gold Saucer -> Decks -> Edit Deck" },
            { "de", "Aktiviere für Gold Saucer -> Decks -> Deck ändern" },
            { "zh", "在金碟游乐场->卡组->编辑卡组窗口启用" },
            { "ja", "ゴールドソーサー → デッキ → デッキ編集で有効化" },
            { "tw", "在「金碟遊樂園 → 卡組一覽 → 編輯卡組」啟用" }
        },
        ["Config.HandleLovmPaletteEdit.Label"] = new() {
            { "en", "Enable in Gold Saucer -> Lord of Verminion -> Minion Hotbar" },
            { "de", "Aktiviere für Gold Saucer -> Trabanten -> Kommandomenü bearbeiten" },
            { "zh", "在金碟游乐场->萌宠之王->宠物热键栏窗口启用" },
            { "ja", "ゴールドソーサー → ミニオンレース → ミニオンホットバーで有効化" },
            { "tw", "在「金碟遊樂園 → 萌寵之王 → 寵物快速鍵」啟用" }
        },
        ["Config.HandleInventory.Label"] = new() {
            { "en", "Enable in Inventory" },
            { "de", "Aktiviere für Inventar" },
            { "zh", "在物品栏窗口启用" },
            { "tw", "在「物品欄」啟用" }
        },
        ["Config.HandleInventory.Description"] = new() {
            { "en", "Note: unlike upstream, scrolling past the last tab does not jump to the Key Items window (it relies on a hard-coded addon command code that is unverified on the Taiwanese client)." },
            { "tw", "注意：與上游不同，捲到最後一個分頁後不會跳到「關鍵物品」視窗——那條路徑依賴未在台服驗證過的寫死命令碼，已整條移除。" }
        },
        ["Config.HandleMJIMinionNoteBook.Label"] = new() {
            { "en", "Enable in Island Minion Guide" },
            { "de", "Aktiviere für Insel-Begleiterliste" },
            { "zh", "在岛内宠物列表窗口启用" },
            { "ja", "島のミニオン図鑑で有効化" },
            { "tw", "在「無人島寵物一覽」啟用" }
        },
        ["Config.HandleMinionNoteBook.Label"] = new() {
            { "en", "Enable in Minions" },
            { "de", "Aktiviere für Begleiter-Verzeichnis" },
            { "zh", "在宠物窗口启用" },
            { "ja", "ミニオン図鑑で有効化" },
            { "tw", "在「寵物一覽」啟用" }
        },
        ["Config.HandleMountNoteBook.Label"] = new() {
            { "en", "Enable in Mounts" },
            { "de", "Aktiviere für Reittier-Verzeichnis" },
            { "zh", "在坐骑窗口启用" },
            { "ja", "マウント図鑑で有効化" },
            { "tw", "在「坐騎一覽」啟用" }
        },
        ["Config.HandleRetainer.Label"] = new() {
            { "en", "Enable in Retainer Inventory" },
            { "de", "Aktiviere für Gehilfeninventar" },
            { "zh", "在雇员物品栏窗口启用" },
            { "ja", "リテイナーインベントリで有効化" },
            { "tw", "在「僱員物品欄」啟用" }
        },
        ["Config.HandleFateProgress.Label"] = new() {
            { "en", "Enable in Shared FATE" },
            { "de", "Aktiviere für FATE-Fortschritt" },
            { "zh", "在危命任务完成度窗口启用" },
            { "tw", "在「F.A.T.E.完成度」啟用" }
        },
        ["Config.HandleAdventureNoteBook.Label"] = new() {
            { "en", "Enable in Sightseeing Log" },
            { "de", "Aktiviere für Eorzea Incognita" },
            { "zh", "在探索笔记窗口启用" },
            { "tw", "在「探索筆記」啟用" }
        }
    }.ToFrozenDictionary();

    public static string t(string key)
        => TryGetTranslation(key, out var text) ? text : key;

    public static string Translate(string key)
        => TryGetTranslation(key, out var text) ? text : key;

    public static bool TryGetTranslation(string key, [MaybeNullWhen(returnValue: false)] out string text)
    {
        text = string.Empty;
        return Localizations.TryGetValue(key, out var languages)
            && (languages.TryGetValue(Services.PluginInterface.UiLanguage, out text)
            || languages.TryGetValue("en", out text));
    }
}
