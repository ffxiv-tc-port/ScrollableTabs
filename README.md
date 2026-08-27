<img align="left" src="ScrollableTabs/Assets/Icon.png" width="60px" height="60px" alt="Scrollable Tabs"/>

**滾輪切換分頁（Scrollable Tabs）** 是一個輕量外掛，讓你用滑鼠滾輪切換視窗分頁。<br/>
<br/>
<hr>

> 這是 [Haselnussbomber/ScrollableTabs](https://github.com/Haselnussbomber/ScrollableTabs) 的**台服（TC）移植版**，
> 釘在 Dalamud API13 / net9。上游追的是 API15 / net10。

支援下列視窗，可各自單獨開關（名稱採台服遊戲內用語）：

- 風脈泉
- 兵裝庫
- 青魔法書
- 人物
- 人物 → 職業＆特職
- 人物 → 評價
- 陸行鳥鞍囊
- 搭檔
- 貨幣一覽
- 時尚配件
- 戰果記錄
- 魚類圖鑑
- 投影台（捲動的是頁面，不是分頁）
- 金碟遊樂園 → 幻卡列表
- 金碟遊樂園 → 卡組一覽 → 編輯卡組
- 金碟遊樂園 → 萌寵之王 → 寵物快速鍵
- 物品欄
- 無人島寵物一覽
- 寵物一覽
- 坐騎一覽
- 僱員物品欄
- F.A.T.E.完成度
- 探索筆記

外掛設定可從外掛安裝器開啟。

## 與上游的差異

| 項目 | 差異 | 原因 |
|---|---|---|
| 指令面板音效抑制 | **整段移除** | 上游是對遊戲程式碼做記憶體修補（用 `EB 13` 覆蓋原位元組）。該特徵碼在台服執行檔 0 命中，等於掛著一個什麼都不做的勾選框；一併移除寫入路徑，不在本移植版留下記憶體修補能力。 |
| 臉部配件（GlassSelect） | **不支援** | 我方釘住的 FFXIVClientStructs 沒有 `AddonGlassSelect`。刻意不從上游 HEAD（較新版客戶端）抄偏移——該處理器會寫 `IsSelected`，偏移錯是靜默寫壞鄰居欄位。 |
| 物品欄 ↔ 關鍵物品 互切 | **移除** | 依賴寫死的 addon 回呼命令碼 22（台服未驗證）＋ 我方 CS 沒有的 `OpenerAddonId` 欄位。失效形式良性：捲到底就停住，不會跳窗。 |
| 無人島寵物「我的最愛」互切 | **預設關閉**，可在設定中自行開啟 | 依賴寫死的命令碼 `0x407`／`0x40B`，台服未驗證，猜錯的失敗形式是「送出別的指令」而不是報錯。該視窗內一般的分頁捲動不受影響。啟用時每次會寫一行 `Information` 診斷。 |
| 魚類圖鑑搜尋分頁 | 搜尋分頁上**也會**捲動 | 上游用 `AgentFishGuide.IsSearchTab` 排除，我方 CS 沒有這個欄位；拿掉 guard 比抄偏移安全。 |
| 拖曳判定 | 改用 `CursorInputs.MouseButtonHeldFlags` | 上游讀 `UIInputData.CurrentMouseDragButtons`，我方 CS 沒有該欄位。語意等價，且不需要新增任何偏移。 |
| 空指標防護 | `AtkStage` / `RaptureAtkModule` / `RaptureAtkUnitManager` / 各 `Agent` 的 `Instance()` 全部補判空 | 這些在載入初期或特定情境會合法回 null，而解參考 null 是 AccessViolationException——.NET Core 的 corrupted-state exception，`try/catch` 攔不到。 |
| 服務取得方式 | 改用 `[PluginService]` 屬性注入 | 上游在 `IAsyncDalamudPlugin.LoadAsync` 裡呼叫 `GetService<T>()`（Dalamud 內部是 sync-over-async）。API13 只有同步建構子，照搬會在載入執行緒上阻塞等 async。 |

## 建置

```
dotnet build -c Release ScrollableTabs/ScrollableTabs.csproj -p:DalamudLibPath=<Dalamud 路徑，尾端斜線不能省>/
```

⚠️ 本 repo 只有 `ScrollableTabs.slnx`、沒有 `.sln`——`dotnet build ScrollableTabs.sln` 會報 MSB1009，看起來像 repo 壞了。直接建 `.csproj`。

## 授權

上游採用 AGPL-3.0（見 `LICENSE`），本移植版沿用。
