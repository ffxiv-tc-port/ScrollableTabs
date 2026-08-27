using System;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using FFXIVClientStructs.Interop;

namespace ScrollableTabs;

public static unsafe class ScrollHandlers
{
    public const int NumArmouryBoardTabs = 12;
    public const int NumInventoryTabs = 5;
    public const int NumInventoryLargeTabs = 4;
    public const int NumInventoryExpansionTabs = 2;
    public const int NumInventoryRetainerTabs = 6;
    public const int NumInventoryRetainerLargeTabs = 3;
    public const int NumBuddyTabs = 3;

    // 台服移植：我方 API13 釘住的 FFXIVClientStructs 的 AtkEventType 有 33/34/35/36/38，
    // 獨缺 37（上游 HEAD 命名為 ListItemHighlight）。這是引擎級的事件編號常數，不是結構偏移，
    // 同一個引擎版本跨區穩定 ⇒ 直接用數值，比抄整份新版列舉安全。
    private const AtkEventType ListItemHighlight = (AtkEventType)37;

    // 台服移植：我方 API13 釘住的 FFXIVClientStructs 的 Interop.Pointer<T> 沒有 Cast<U>()
    // 這個泛型方法（上游 HEAD 才有）。這裡不自己補一個擴充方法，直接把參數改成裸指標
    // 並用 C 式轉型——語意完全相同，而且少一層看不見的包裝。
    public static void Handle(AtkUnitBase* unitBase, int wheelState)
    {
        switch (unitBase->NameString)
        {
            case "Buddy":
            case "BuddyAction":
            case "BuddySkill":
            case "BuddyAppearance":
                UpdateBuddy(wheelState);
                break;

            case "Character":
            case "CharacterStatus":
            case "CharacterProfile":
                UpdateCharacter(wheelState);
                break;

            case "InventoryCrystalGrid":
                if (Services.GameConfig.UiConfig.TryGet("ItemInventryWindowSizeType", out uint size) && size == 2)
                    UpdateInventoryExpansion(wheelState);
                else
                    UpdateInventoryLarge(wheelState);
                break;

            case "Inventory":
            case "InventoryGrid":
            case "InventoryGridCrystal":
                UpdateInventory(wheelState);
                break;

            case "InventoryLarge":
            case "InventoryEventGrid0":
            case "InventoryEventGrid1":
            case "InventoryEventGrid2":
            case "InventoryGrid0":
            case "InventoryGrid1":
                UpdateInventoryLarge(wheelState);
                break;

            case "InventoryExpansion":
            case "InventoryEventGrid0E":
            case "InventoryEventGrid1E":
            case "InventoryEventGrid2E":
            case "InventoryGrid0E":
            case "InventoryGrid1E":
            case "InventoryGrid2E":
            case "InventoryGrid3E":
                UpdateInventoryExpansion(wheelState);
                break;

            case "InventoryEvent":
            case "InventoryEventGrid":
                UpdateInventoryEvent(wheelState);
                break;

            case "InventoryBuddy":
            case "InventoryBuddy2":
                UpdateInventoryBuddy(wheelState);
                break;

            case "InventoryRetainer":
            case "RetainerGridCrystal":
            case "RetainerGrid":
                UpdateInventoryRetainer(wheelState);
                break;

            case "InventoryRetainerLarge":
            case "RetainerCrystalGrid":
            case "RetainerGrid0":
            case "RetainerGrid1":
            case "RetainerGrid2":
            case "RetainerGrid3":
            case "RetainerGrid4":
                UpdateInventoryRetainerLarge(wheelState);
                break;

            case "MinionNoteBook":
            case "MountNoteBook":
                UpdateMountMinion(((AddonMinionMountBase*)unitBase), wheelState);
                break;

            case "CharacterClass":
                UpdateCharacterClass(((AddonCharacterClass*)unitBase), wheelState);
                break;
            case "CharacterRepute":
                UpdateCharacterRepute(((AddonCharacterRepute*)unitBase), wheelState);
                break;
            case "AOZNotebook":
                UpdateAOZNotebook(((AddonAOZNotebook*)unitBase), wheelState);
                break;
            case "AetherCurrent":
                UpdateAetherCurrent(((AddonAetherCurrent*)unitBase), wheelState);
                break;
            case "ArmouryBoard":
                UpdateArmouryBoard(((AddonArmouryBoard*)unitBase), wheelState);
                break;
            case "Currency":
                UpdateCurrency(((AddonCurrency*)unitBase), wheelState);
                break;
            case "FateProgress":
                UpdateFateProgress(((AddonFateProgress*)unitBase), wheelState);
                break;
            // 台服移植：上游有 "GlassSelect"（臉部配件）的處理器，但我方 API13 釘住的
            // FFXIVClientStructs 沒有 AddonGlassSelect 這個結構。刻意「不」從上游 HEAD 抄偏移
            // ——那份追的是比 7.20 更新的全球版，而這個處理器會寫 IsSelected，
            // 偏移錯的話是靜默寫壞鄰居欄位。整個處理器連同設定項一併移除。
            case "MJIMinionNoteBook":
                UpdateMJIMinionNoteBook(((AddonMJIMinionNoteBook*)unitBase), wheelState);
                break;
            case "MYCWarResultNotebook":
                UpdateFieldNotes(((AddonMYCWarResultNotebook*)unitBase), wheelState);
                break;
            case "MiragePrismPrismBox":
                UpdateMiragePrismPrismBox(((AddonMiragePrismPrismBox*)unitBase), wheelState);
                break;

            case "AdventureNoteBook":
                UpdateTabController(unitBase, &((AddonAdventureNoteBook*)unitBase)->TabController, Services.Config.HandleAdventureNoteBook, wheelState);
                break;
            // 台服移植：上游這裡多一個 `&& !AgentFishGuide.Instance()->IsSearchTab` 的條件，
            // 但我方 API13 的 AgentFishGuide 沒有 IsSearchTab 欄位（上游 HEAD 才有，[FieldOffset(0x30)]）。
            // 拿掉這個 guard 而不是抄偏移：抄錯偏移是靜默讀錯記憶體，
            // 拿掉只是「在魚類圖鑑的搜尋分頁上也會捲」這個良性差異。
            case "FishGuide2":
                UpdateTabController(unitBase, &((AddonFishGuide2*)unitBase)->TabController, Services.Config.HandleFishGuide, wheelState);
                break;
            case "GSInfoCardList":
                UpdateTabController(unitBase, &((AddonGSInfoCardList*)unitBase)->TabController, Services.Config.HandleGoldSaucerCardList, wheelState);
                break;
            case "GSInfoEditDeck":
                UpdateTabController(unitBase, &((AddonGSInfoEditDeck*)unitBase)->TabController, Services.Config.HandleGoldSaucerCardDeckEdit, wheelState);
                break;
            case "LovmPaletteEdit":
                UpdateTabController(unitBase, &((AddonLovmPaletteEdit*)unitBase)->TabController, Services.Config.HandleLovmPaletteEdit, wheelState);
                break;
            case "OrnamentNoteBook":
                UpdateTabController(unitBase, &((AddonOrnamentNoteBook*)unitBase)->TabController, Services.Config.HandleOrnamentNoteBook, wheelState);
                break;
        }
    }

    public static void UpdateArmouryBoard(AddonArmouryBoard* addon, int wheelState)
    {
        if (!Services.Config.HandleArmouryBoard)
            return;

        var tabIndex = GetTabIndex(addon->TabIndex, NumArmouryBoardTabs, wheelState);

        if (addon->TabIndex < tabIndex)
            addon->NextTab(0);
        else if (addon->TabIndex > tabIndex)
            addon->PreviousTab(0);
    }

    public static void UpdateInventory(int wheelState)
    {
        if (!Services.Config.HandleInventory)
            return;

        if (!TryGetAddon<AddonInventory>("Inventory"u8, out var addon))
            return;

        // 台服移植：上游在最後一個分頁再往下捲時，會用 FireCallback 送出寫死的命令碼 22
        // 跳到「關鍵物品」視窗。這條路徑在我方環境有兩個各自獨立的問題：
        //   ① 命令碼 22 沒有對台服驗證過，猜錯的失敗形式是「送出別的指令」而不是報錯；
        //   ② 它要讀 AddonInventory.OpenerAddonId，而我方釘住的 FFXIVClientStructs 沒有這個欄位
        //      ——從上游 HEAD（更新版客戶端）抄偏移就是靜默讀到鄰居欄位，再把讀到的垃圾送進回呼。
        // ⇒ 整條「物品欄↔關鍵物品」互切移除。失效形式良性：捲到底就停住，不會跳窗。
        var tabIndex = GetTabIndex(addon->TabIndex, NumInventoryTabs, wheelState);

        if (addon->TabIndex == tabIndex)
            return;

        addon->SetTab(tabIndex);
    }

    public static void UpdateInventoryEvent(int wheelState)
    {
        if (!Services.Config.HandleInventory)
            return;

        if (!TryGetAddon<AddonInventoryEvent>("InventoryEvent"u8, out var addon))
            return;

        // 台服移植：與 UpdateInventory 同理，上游的「關鍵物品→物品欄」互切
        // 同樣依賴未驗證的命令碼 22 ＋ 我方沒有的 AddonInventoryEvent.OpenerAddonId 欄位，整條移除。
        var numEnabledButtons = 0;
        foreach (ref var button in addon->Buttons)
        {
            // 台服加固：按鈕陣列的元素可能是 null（分頁尚未建好），上游直接解參考。
            if (button.Value == null)
                continue;

            if ((button.Value->AtkComponentButton.Flags & 0x40000) != 0)
                numEnabledButtons++;
        }

        var tabIndex = GetTabIndex(addon->TabIndex, numEnabledButtons, wheelState);

        if (addon->TabIndex == tabIndex)
            return;

        addon->SetTab(tabIndex);
    }

    public static void UpdateInventoryLarge(int wheelState)
    {
        if (!Services.Config.HandleInventory)
            return;

        if (!TryGetAddon<AddonInventoryLarge>("InventoryLarge"u8, out var addon))
            return;

        var tabIndex = GetTabIndex(addon->TabIndex, NumInventoryLargeTabs, wheelState);

        if (addon->TabIndex == tabIndex)
            return;

        addon->SetTab(tabIndex);
    }

    public static void UpdateInventoryExpansion(int wheelState)
    {
        if (!Services.Config.HandleInventory)
            return;

        if (!TryGetAddon<AddonInventoryExpansion>("InventoryExpansion"u8, out var addon))
            return;

        var tabIndex = GetTabIndex(addon->TabIndex, NumInventoryExpansionTabs, wheelState);

        if (addon->TabIndex == tabIndex)
            return;

        addon->SetTab(tabIndex, false);
    }

    public static void UpdateInventoryRetainer(int wheelState)
    {
        if (!Services.Config.HandleRetainer)
            return;

        if (!TryGetAddon<AddonInventoryRetainer>("InventoryRetainer"u8, out var addon))
            return;

        var tabIndex = GetTabIndex(addon->TabIndex, NumInventoryRetainerTabs, wheelState);

        if (addon->TabIndex == tabIndex)
            return;

        addon->SetTab(tabIndex);
    }

    public static void UpdateInventoryRetainerLarge(int wheelState)
    {
        if (!Services.Config.HandleRetainer)
            return;

        if (!TryGetAddon<AddonInventoryRetainerLarge>("InventoryRetainerLarge"u8, out var addon))
            return;

        var tabIndex = GetTabIndex(addon->TabIndex, NumInventoryRetainerLargeTabs, wheelState);

        if (addon->TabIndex == tabIndex)
            return;

        addon->SetTab(tabIndex);
    }

    public static void UpdateAOZNotebook(AddonAOZNotebook* addon, int wheelState)
    {
        if (!Services.Config.HandleAOZNotebook)
            return;

        var tabIndex = GetTabIndex(addon->TabIndex, addon->TabCount, wheelState);

        if (addon->TabIndex == tabIndex)
            return;

        addon->SetTab(tabIndex, true);
    }

    public static void UpdateAetherCurrent(AddonAetherCurrent* addon, int wheelState)
    {
        if (!Services.Config.HandleAetherCurrent)
            return;

        var tabIndex = GetTabIndex(addon->TabIndex, addon->TabCount, wheelState);

        if (addon->TabIndex == tabIndex)
            return;

        addon->SetTab(tabIndex);

        for (var i = 0; i < addon->Tabs.Length; i++)
            addon->Tabs[i].Value->IsSelected = i == tabIndex;
    }

    public static void UpdateFateProgress(AddonFateProgress* addon, int wheelState)
    {
        if (!Services.Config.HandleFateProgress)
            return;

        var tabIndex = GetTabIndex(addon->TabIndex, addon->TabCount, wheelState);

        if (!addon->IsLoaded || addon->TabIndex == tabIndex)
            return;

        // fake event, so it can call SetEventIsHandled
        var atkEvent = new AtkEvent();
        addon->SetTab(tabIndex, &atkEvent);
    }

    public static void UpdateFieldNotes(AddonMYCWarResultNotebook* addon, int wheelState)
    {
        if (!Services.Config.HandleFieldRecord)
            return;

        // 台服加固：RaptureAtkModule.Instance() 走 UIModule.Instance()，會回 null。
        var atkModule = RaptureAtkModule.Instance();
        if (atkModule == null)
            return;

        if (atkModule->AtkCollisionManager.IntersectingCollisionNode == addon->DescriptionCollisionNode)
            return;

        var atkEvent = new AtkEvent();
        var eventParam = Math.Clamp(addon->CurrentNoteIndex % 10 + wheelState, -1, addon->MaxNoteIndex - 1);

        if (eventParam == -1)
        {
            if (addon->CurrentPageIndex > 0)
            {
                var page = addon->CurrentPageIndex - 1;
                addon->ReceiveEvent(AtkEventType.ButtonClick, page + 10, &atkEvent);
                addon->ReceiveEvent(AtkEventType.ButtonClick, 9, &atkEvent);
            }
        }
        else if (eventParam == 10)
        {
            if (addon->CurrentPageIndex < 4)
            {
                var page = addon->CurrentPageIndex + 1;
                addon->ReceiveEvent(AtkEventType.ButtonClick, page + 10, &atkEvent);
            }
        }
        else
        {
            addon->ReceiveEvent(AtkEventType.ButtonClick, eventParam, &atkEvent);
        }
    }

    public static void UpdateMountMinion(AddonMinionMountBase* addon, int wheelState)
    {
        var isEnabled = addon->NameString switch
        {
            "MinionNoteBook" => Services.Config.HandleMinionNoteBook,
            "MountNoteBook" => Services.Config.HandleMountNoteBook,
            _ => false,
        };

        if (!isEnabled)
            return;

        if (addon->CurrentView == AddonMinionMountBase.ViewType.Normal)
        {
            if (addon->TabController.TabIndex == 0 && wheelState < 0)
            {
                addon->SwitchToFavorites();
            }
            else
            {
                UpdateTabController((AtkUnitBase*)addon, &addon->TabController, true, wheelState);
            }
        }
        else if (addon->CurrentView == AddonMinionMountBase.ViewType.Favorites && wheelState > 0)
        {
            addon->TabController.CallbackFunction(0, (AtkUnitBase*)addon);
        }
    }

    public static void UpdateMJIMinionNoteBook(AddonMJIMinionNoteBook* addon, int wheelState)
    {
        if (!Services.Config.HandleMJIMinionNoteBook)
            return;

        // 台服加固：[Agent(...)] 產生的 Instance() 有兩層合法回 null。
        var agent = AgentMJIMinionNoteBook.Instance();
        if (agent == null)
            return;

        // 🔴 台服未驗證（R3）：0x407 / 0x40B 是寫死的 addon 命令碼。命令碼在台服對不對，
        //    離線證明不了；若台服的分派表不同，送出去的會是「別的指令」而不是報錯。
        //    ⇒ 只有「我的最愛↔一般」這條跨清單切換受此旗標控制，預設關；
        //    同一個視窗內的一般分頁切換（不需要命令碼）照常運作。
        var allowFavoritesSwitch = Services.Config.AllowUnverifiedMJIFavoritesSwitch;

        if (agent->CurrentView == AgentMJIMinionNoteBook.ViewType.Normal)
        {
            if (addon->TabController.TabIndex == 0 && wheelState < 0)
            {
                if (!allowFavoritesSwitch)
                    return;

                agent->CurrentView = AgentMJIMinionNoteBook.ViewType.Favorites;
                agent->SelectedFavoriteMinion.TabIndex = 0;
                agent->SelectedFavoriteMinion.SlotIndex = agent->SelectedNormalMinion.SlotIndex;
                agent->SelectedFavoriteMinion.MinionId = agent->GetSelectedMinionId();
                agent->SelectedMinion = &agent->SelectedFavoriteMinion;
                Services.PluginLog.Information("[ScrollableTabs] 送出未驗證的 addon 命令碼 0x407（無人島寵物：切到我的最愛）");
                agent->HandleCommand(0x407);
            }
            else
            {
                UpdateTabController((AtkUnitBase*)addon, &addon->TabController, true, wheelState);

                if (!allowFavoritesSwitch)
                    return;

                Services.PluginLog.Information("[ScrollableTabs] 送出未驗證的 addon 命令碼 0x40B（無人島寵物：更新清單）");
                agent->HandleCommand(0x40B);
            }
        }
        else if (agent->CurrentView == AgentMJIMinionNoteBook.ViewType.Favorites && wheelState > 0)
        {
            if (!allowFavoritesSwitch)
                return;

            agent->CurrentView = AgentMJIMinionNoteBook.ViewType.Normal;
            agent->SelectedNormalMinion.TabIndex = 0;
            agent->SelectedNormalMinion.SlotIndex = agent->SelectedFavoriteMinion.SlotIndex;
            agent->SelectedNormalMinion.MinionId = agent->GetSelectedMinionId();
            agent->SelectedMinion = &agent->SelectedNormalMinion;

            addon->TabController.TabIndex = 0;
            addon->TabController.CallbackFunction(0, (AtkUnitBase*)addon);
            Services.PluginLog.Information("[ScrollableTabs] 送出未驗證的 addon 命令碼 0x40B（無人島寵物：切回一般）");
            agent->HandleCommand(0x40B);
        }
    }

    public static void UpdateCurrency(AddonCurrency* addon, int wheelState)
    {
        if (!Services.Config.HandleCurrency)
            return;

        // 🔴 台服加固（部署閘門 R1）：AtkStage.Instance() 是
        //    [StaticAddress("...", 3, isPointer: true)]，也就是「讀一個指標」而不是 lea 一個位址，
        //    所以它**可以**回 null（我方 CS fork 的台服加固 commit 讓 isPointer:true 的 Instance()
        //    一律先判空）。上游這裡直接 atkStage->GetNumberArrayData(...) 沒有判空。
        //    解參考 null 的後果是 AccessViolationException，那是 .NET Core 的 corrupted-state
        //    exception，try/catch 與任何例外隔離都攔不到 ⇒ 直接把遊戲弄崩。
        //    改成假設不成立也只是「這個視窗不捲」。
        var atkStage = AtkStage.Instance();
        if (atkStage == null)
            return;

        var numberArray = atkStage->GetNumberArrayData(NumberArrayType.Currency);
        if (numberArray == null)
            return;

        var numberArrays = atkStage->GetNumberArrayData();
        var stringArrays = atkStage->GetStringArrayData();
        if (numberArrays == null || stringArrays == null)
            return;

        var currentTab = numberArray->IntArray[0];
        var newTab = currentTab;

        var enableStates = new bool[addon->Tabs.Length];
        for (var i = 0; i < addon->Tabs.Length; i++)
            enableStates[i] = addon->Tabs[i].Value != null && addon->Tabs[i].Value->IsEnabled;

        if (wheelState > 0 && currentTab < enableStates.Length)
        {
            for (var i = currentTab + 1; i < enableStates.Length; i++)
            {
                if (enableStates[i])
                {
                    newTab = i;
                    break;
                }
            }
        }
        else if (currentTab > 0)
        {
            for (var i = currentTab - 1; i >= 0; i--)
            {
                if (enableStates[i])
                {
                    newTab = i;
                    break;
                }
            }
        }

        if (currentTab == newTab)
            return;

        numberArray->SetValue(0, newTab);
        addon->OnRequestedUpdate(numberArrays, stringArrays);
    }

    public static void UpdateInventoryBuddy(int wheelState)
    {
        if (!Services.Config.HandleInventoryBuddy)
            return;

        if (!PlayerState.Instance()->HasPremiumSaddlebag)
            return;

        if (!TryGetAddon<AddonInventoryBuddy>("InventoryBuddy"u8, out var addon))
            return;

        var tabIndex = GetTabIndex(addon->TabIndex, 2, wheelState);

        if (addon->TabIndex == tabIndex)
            return;

        addon->SetTab((byte)tabIndex);
    }

    public static void UpdateBuddy(int wheelState)
    {
        if (!Services.Config.HandleBuddy)
            return;

        if (!TryGetAddon<AddonBuddy>("Buddy"u8, out var addon))
            return;

        var tabIndex = GetTabIndex(addon->TabIndex, NumBuddyTabs, wheelState);

        if (addon->TabIndex == tabIndex)
            return;

        addon->SetTab(tabIndex);

        for (var i = 0; i < NumBuddyTabs; i++)
        {
            var button = addon->RadioButtons.GetPointer(i);
            if (button->Value != null)
                button->Value->IsSelected = i == addon->TabIndex;
        }
    }

    public static void UpdateMiragePrismPrismBox(AddonMiragePrismPrismBox* addon, int wheelState)
    {
        if (!Services.Config.HandleMiragePrismPrismBox)
            return;

        if (addon->JobDropdown == null ||
            addon->JobDropdown->List == null ||
            addon->JobDropdown->List->OwnerNode == null ||
            addon->JobDropdown->List->OwnerNode->IsVisible())
        {
            return;
        }

        if (addon->OrderDropdown == null ||
            addon->OrderDropdown->List == null ||
            addon->OrderDropdown->List->OwnerNode == null ||
            addon->OrderDropdown->List->OwnerNode->IsVisible())
        {
            return;
        }

        var prevButton = Services.Config.Invert ? addon->PrevButton : addon->NextButton;
        var nextButton = Services.Config.Invert ? addon->NextButton : addon->PrevButton;

        var isPrev = wheelState == (Services.Config.Invert ? -1 : 1);
        if (prevButton == null || (isPrev && !prevButton->IsEnabled))
            return;

        var isNext = wheelState == (Services.Config.Invert ? 1 : -1);
        if (nextButton == null || (isNext && !nextButton->IsEnabled))
            return;

        if (TryGetAddon<AtkUnitBase>("MiragePrismPrismBoxFilter"u8, out var filterAddon) && filterAddon->IsVisible)
            return;

        // 台服加固：[Agent(...)] 產生的 Instance() 有兩層合法回 null。
        var agent = AgentMiragePrismPrismBox.Instance();
        if (agent == null)
            return;

        agent->PageIndex += (byte)wheelState;
        agent->UpdateItems(false, false);
    }

    public static void UpdateCharacter(int wheelState)
    {
        if (!Services.Config.HandleCharacter)
            return;

        if (!TryGetAddon<AddonCharacter>("Character"u8, out var addon))
            return;

        if (!addon->AddonControl.IsChildSetupComplete)
            return;

        // 台服加固：同上，RaptureAtkModule.Instance() 會回 null。
        var atkModule = RaptureAtkModule.Instance();
        if (atkModule == null)
            return;

        if (atkModule->AtkCollisionManager.IntersectingCollisionNode == addon->PreviewController.CollisionNode)
            return;

        var tabIndex = GetTabIndex(addon->TabIndex, addon->TabCount, wheelState);

        if (addon->TabIndex == tabIndex)
            return;

        addon->SetTab(tabIndex);

        for (var i = 0; i < addon->TabCount; i++)
        {
            var button = addon->Tabs.GetPointer(i);
            if (button->Value != null)
                button->Value->IsSelected = i == addon->TabIndex;
        }
    }

    public static void UpdateCharacterClass(AddonCharacterClass* addon, int wheelState)
    {
        // prev or next embedded addon
        if (!Services.Config.HandleCharacterClass || addon->TabIndex + wheelState < 0 || addon->TabIndex + wheelState > 1)
        {
            UpdateCharacter(wheelState);
            return;
        }

        var tabIndex = GetTabIndex(addon->TabIndex, 2, wheelState);

        if (addon->TabIndex == tabIndex)
            return;

        addon->SetTab(tabIndex);
    }

    public static void UpdateCharacterRepute(AddonCharacterRepute* addon, int wheelState)
    {
        if (addon->ExpansionsDropDownList == null || addon->ExpansionsDropDownList->List == null)
            return;

        if (addon->ExpansionsDropDownList->IsOpen)
            return;

        var currentIndex = addon->ExpansionsDropDownList->GetSelectedItemIndex();

        // prev embedded addon
        if (!Services.Config.HandleCharacterRepute || currentIndex + wheelState < 0)
        {
            UpdateCharacter(wheelState);
            return;
        }

        var itemCount = addon->ExpansionsDropDownList->List->GetItemCount();
        var tabIndex = GetTabIndex(currentIndex, itemCount, wheelState);
        if (currentIndex == tabIndex)
            return;

        var atkEvent = new AtkEvent();
        var data = new AtkEventData();
        data.ListItemData.SelectedIndex = tabIndex;
        addon->AtkUnitBase.ReceiveEvent(ListItemHighlight, 0, &atkEvent, &data);

        addon->ExpansionsDropDownList->SelectItem(tabIndex);
    }

    private static void UpdateTabController(AtkUnitBase* addon, TabController* tabController, bool isEnabled, int wheelState)
    {
        if (!isEnabled)
            return;

        var tabIndex = GetTabIndex(tabController->TabIndex, tabController->TabCount, wheelState);

        if (tabController->TabIndex == tabIndex)
            return;

        tabController->TabIndex = tabIndex;
        tabController->CallbackFunction(tabIndex, addon);
    }

    private static int GetTabIndex(int currentTabIndex, int numTabs, int wheelState)
    {
        return Math.Clamp(currentTabIndex + wheelState, 0, numTabs - 1);
    }

    // 🔴 台服加固：RaptureAtkUnitManager.Instance() 內部是
    //    UIModule.Instance() -> RaptureAtkModule -> &...RaptureAtkUnitManager，
    //    前兩層都會回 null（登入前／模組尚未就緒）。上游直接 ->GetAddonByName 解參考。
    //    這條路徑每次滾輪都會走到，判空的成本可以忽略，不判的代價是 AVE（攔不到）。
    private static bool TryGetAddon<T>(ReadOnlySpan<byte> name, out T* addon) where T : unmanaged
    {
        addon = null;

        var unitManager = RaptureAtkUnitManager.Instance();
        if (unitManager == null)
            return false;

        var unitbase = unitManager->GetAddonByName(name);
        addon = (T*)unitbase;
        return unitbase != null && unitbase->IsReady;
    }
}
