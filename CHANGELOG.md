# Changelog

## 台服移植版

台服（TC）移植版的變更記在這一節；上游的版本歷史在下面。
逐項差異與原因見 README 的「與上游的差異」表。

### 未發版

- 移植到 Dalamud API13 / net9（上游為 API15 / net10）。
- 移除上游用於抑制指令面板音效的記憶體修補功能（`QuickPanelPlaySoundEffectPatch`），連同設定欄位與 UI 一併移除。
- 介面字串繁體中文化，視窗名稱採台服遊戲內用語。
- 補上一批原生指標的空值防護（`AtkStage` / `RaptureAtkModule` / `RaptureAtkUnitManager` / 各 `Agent`）。
- 補上 `AtkComponentButton.IsEnabled` 的 `OwnerNode` 空值防護（貨幣視窗分頁、幻境之扉上一頁／下一頁）——該屬性解的是 `OwnerNode` 而不是 `AtkResNode`，只判元件指標非 null 並不夠。
- 移除或關閉依賴未驗證 addon 命令碼的路徑。

## 上游

### [1.0.0]

First release. 🥳

[1.0.0]: https://github.com/Haselnussbomber/ScrollableTabs/commit/6fbf4da3c254586cb8e23329938edfc9823c4e8f
