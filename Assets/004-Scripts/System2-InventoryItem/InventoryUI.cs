using System.Collections.Generic;
using System.Data;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Tooltip("Drag all the UI Slot GameObjects here in order (0 to 23).")]
    public List<InventoryUISlot> UISlots;
    public List<InventoryUISlot> UIHotBarSlots_Inventory;
    public List<InventoryUISlot> UIHotBarSlots_InGame;

    [Header("Item Details Panel")]
    [SerializeField] private TextMeshProUGUI _textItemName;
    [SerializeField] private TextMeshProUGUI _textItemDescription;
    [SerializeField] private Image _imageItemDetailsIcon;

    [Header("Action Buttons")]
    [SerializeField] private GameObject _btnPrimaryActionObj;
    [SerializeField] private TextMeshProUGUI _textPrimaryAction;
    [SerializeField] private GameObject _btnHotbarToggleObj;
    [SerializeField] private TextMeshProUGUI _textHotbarAction;
    [SerializeField] private GameObject _btnDropItemObj;

    private int _currentlyEquippedSlotIndex = -1;
    private int _currentlySelectedDataIndex = -1;

    private void OnEnable()
    {
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnInventoryUpdated += RefreshAllUISlot;
        }
    }

    private void OnDisable()
    {
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnInventoryUpdated -= RefreshAllUISlot;
        }
    }

    private void Start()
    {
        ClearDetailsPanel();
        RefreshAllUISlot();
    }

    private void Update()
    {
        HandleHotbarInput();
    }

    public void RefreshAllUISlot()
    {
        int hotbarOffset = InventoryManager.Instance.HotbarSlots;

        // Hotbars read from data indices 0 to 7 (Offset = 0)
        for (int i = 0; i < UIHotBarSlots_Inventory.Count; i++)
        {
            RefreshUISlot(UIHotBarSlots_Inventory, i, 0);
        }

        for (int i = 0; i < UIHotBarSlots_InGame.Count; i++)
        {
            RefreshUISlot(UIHotBarSlots_InGame, i, 0);
        }

        // Main Inventory UI reads from data indices 8 to 39 (Offset = HotbarSlots)
        for (int i = 0; i < UISlots.Count; i++)
        {
            RefreshUISlot(UISlots, i, hotbarOffset);
        }

        if (_currentlySelectedDataIndex >= 0)
        {
            SelectSlot(_currentlySelectedDataIndex);
        }
    }

    private void RefreshUISlot(List<InventoryUISlot> uiSlotList, int listIndex, int dataOffset)
    {
        if (listIndex < 0 || listIndex >= uiSlotList.Count) return;

        int dataIndex = listIndex + dataOffset;
        InventorySlot dataSlot = InventoryManager.Instance.GetSlot(dataIndex);

        uiSlotList[listIndex].SetDataIndex(dataIndex, this);
        uiSlotList[listIndex].UpdateSlotVisuals(dataSlot);
    }

    public void SelectSlot(int dataIndex)
    {
        _currentlySelectedDataIndex = dataIndex;
        InventorySlot dataSlot = InventoryManager.Instance.GetSlot(dataIndex);

        if (dataSlot == null || dataSlot.IsEmpty)
        {
            ClearDetailsPanel();
            return;
        }

        // 1. Show Name and Description
        _textItemName.text = dataSlot.Item.ItemName;
        _textItemDescription.text = dataSlot.Item.Description;
        _imageItemDetailsIcon.sprite = dataSlot.Item.Icon;
        _imageItemDetailsIcon.color = Color.white;
        _imageItemDetailsIcon.SetNativeSize();

        // 2. Evaluate Item Usage for Button Display
        _btnDropItemObj.SetActive(true);

        if (dataSlot.Item.Usage == ItemUsage.None)
        {
            // Hide Primary and Hotbar buttons if it's just a raw resource
            _btnPrimaryActionObj.SetActive(false);
            _btnHotbarToggleObj.SetActive(false);
        }
        else
        {
            _btnPrimaryActionObj.SetActive(true);
            _btnHotbarToggleObj.SetActive(true);

            // Change the first button text to match the capability
            switch (dataSlot.Item.Usage)
            {
                case ItemUsage.Equippable: _textPrimaryAction.text = "EQUIP"; break;
                case ItemUsage.Consumable: _textPrimaryAction.text = "USE"; break;
                case ItemUsage.Placable: _textPrimaryAction.text = "PLACE"; break;
            }

            if (dataIndex < InventoryManager.Instance.HotbarSlots)
            {
                _textHotbarAction.text = "REMOVE FROM HOTBAR";
            }
            else
            {
                _textHotbarAction.text = "ADD TO HOTBAR";
            }
        }
    }

    private void ClearDetailsPanel()
    {
        _currentlySelectedDataIndex = -1;
        _textItemName.text = "ITEM NAME";
        _textItemDescription.text = "";
        _imageItemDetailsIcon.sprite = null;
        _imageItemDetailsIcon.color = Color.clear;

        _btnPrimaryActionObj.SetActive(false);
        _btnHotbarToggleObj.SetActive(false);
        _btnDropItemObj.SetActive(false);
    }

    private void HandleHotbarInput()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.digit1Key.wasPressedThisFrame) EquipFromHotbar(0);
        if (Keyboard.current.digit2Key.wasPressedThisFrame) EquipFromHotbar(1);
        if (Keyboard.current.digit3Key.wasPressedThisFrame) EquipFromHotbar(2);
        if (Keyboard.current.digit4Key.wasPressedThisFrame) EquipFromHotbar(3);
        if (Keyboard.current.digit5Key.wasPressedThisFrame) EquipFromHotbar(4);
        if (Keyboard.current.digit6Key.wasPressedThisFrame) EquipFromHotbar(5);
        if (Keyboard.current.digit7Key.wasPressedThisFrame) EquipFromHotbar(6);
        if (Keyboard.current.digit8Key.wasPressedThisFrame) EquipFromHotbar(7);

        if (Keyboard.current.gKey.wasPressedThisFrame) DropSelectedItem();
        if (Keyboard.current.eKey.wasPressedThisFrame) ExecutePrimaryAction();
    }

    public void DropSelectedItem()
    {
        if (_currentlySelectedDataIndex < 0) return;

        InventorySlot dataSlot = InventoryManager.Instance.GetSlot(_currentlySelectedDataIndex);
        if (dataSlot != null && !dataSlot.IsEmpty)
        {
            ItemData droppedItemData = dataSlot.Item;
            int droppedAmount = dataSlot.Amount;

            // Instantiate world prefab if assigned, at the player's position
            if (droppedItemData.WorldPrefab != null)
            {
                GameObject player = PlayerController.Instance.gameObject;
                Vector3 spawnPosition = player != null ? player.transform.position : transform.position;

                GameObject droppedObj = Instantiate(droppedItemData.WorldPrefab, spawnPosition, Quaternion.identity);

                // Pass the exact stack count to the world item component if it exists
                Item worldItemComp = droppedObj.GetComponent<Item>();
                if (worldItemComp != null)
                {
                    worldItemComp.ItemData = droppedItemData;
                    worldItemComp.ApplyItemDataInfo();
                    worldItemComp.IsPickUpInfinitely = false;
                    worldItemComp.SetItemCount(droppedAmount);
                }
            }

            GameStatusMessage.Instance.CreateMessage($"Dropped item: [{droppedAmount}] [{dataSlot.Item.ItemName}]", Color.softRed);

            // Remove entirely from inventory and refresh
            InventoryManager.Instance.RemoveItem(_currentlySelectedDataIndex, droppedAmount);
            ClearDetailsPanel();
            RefreshAllUISlot();
        }
    }

    // Connect this to your Primary Action Button's UnityEvent in the Inspector
    public void ExecutePrimaryAction()
    {
        if (_currentlySelectedDataIndex < 0) return;
        InventorySlot dataSlot = InventoryManager.Instance.GetSlot(_currentlySelectedDataIndex);
        if (dataSlot == null || dataSlot.IsEmpty) return;

        switch (dataSlot.Item.Usage)
        {
            case ItemUsage.Equippable:
                GameStatusMessage.Instance.CreateMessage($"Equipping item: [{dataSlot.Item.ItemName}]", Color.lightBlue);
                // Add your equip logic here
                break;

            case ItemUsage.Consumable:
                GameStatusMessage.Instance.CreateMessage($"Consuming item: [{dataSlot.Item.ItemName}]", Color.lightBlue);
                // Reduce stack count by 1 upon consumption
                InventoryManager.Instance.RemoveItem(_currentlySelectedDataIndex, 1);
                break;

            case ItemUsage.Placable:
                GameStatusMessage.Instance.CreateMessage($"Placing item: [{dataSlot.Item.ItemName}]", Color.lightBlue);
                // Add your placement logic here
                break;
        }

        RefreshAllUISlot();
    }

    public void ToggleHotbarAssignment()
    {
        if (_currentlySelectedDataIndex < 0) return;

        int hotbarLimit = InventoryManager.Instance.HotbarSlots;

        if (_currentlySelectedDataIndex < hotbarLimit)
        {
            // Case A: Currently in Hotbar -> Move to first available Main Inventory slot
            for (int i = hotbarLimit; i < 40; i++) // assuming 40 total slots
            {
                InventorySlot targetSlot = InventoryManager.Instance.GetSlot(i);
                if (targetSlot != null && targetSlot.IsEmpty)
                {
                    InventoryManager.Instance.SwapSlots(_currentlySelectedDataIndex, i);
                    SelectSlot(i); // Update selection to new index
                    RefreshAllUISlot();
                    return;
                }
            }
        }
        else
        {
            // Case B: Currently in Main Inventory -> Move to first available Hotbar slot
            for (int i = 0; i < hotbarLimit; i++)
            {
                InventorySlot targetSlot = InventoryManager.Instance.GetSlot(i);
                if (targetSlot != null && targetSlot.IsEmpty)
                {
                    InventoryManager.Instance.SwapSlots(_currentlySelectedDataIndex, i);
                    SelectSlot(i); // Update selection to new index
                    RefreshAllUISlot();
                    return;
                }
            }
        }
    }

    private void EquipFromHotbar(int hotbarIndex)
    {
        InventorySlot slotToEquip = InventoryManager.Instance.GetSlot(hotbarIndex);

        if (slotToEquip == null || slotToEquip.IsEmpty || slotToEquip.Item.Usage == ItemUsage.None)
        {
            return; // Can't equip nothing, or un-equipable items (like raw resources)
        }

        // Turn off previous highlight
        if (_currentlyEquippedSlotIndex >= 0 && _currentlyEquippedSlotIndex < UIHotBarSlots_InGame.Count)
        {
            UIHotBarSlots_InGame[_currentlyEquippedSlotIndex].SetHighlight(false);
        }

        // Highlight new selection
        _currentlyEquippedSlotIndex = hotbarIndex;
        UIHotBarSlots_InGame[_currentlyEquippedSlotIndex].SetHighlight(true);

        GameStatusMessage.Instance.CreateMessage($"Selected item: {slotToEquip.Item.ItemName}", Color.white);

        SelectSlot(hotbarIndex);
        RefreshAllUISlot();
    }
}