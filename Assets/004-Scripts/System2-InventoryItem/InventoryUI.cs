using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class InventoryUI : MonoBehaviour
{

    [Tooltip("Drag all the UI Slot GameObjects here in order (0 to 23).")]
    [SerializeField] private List<InventoryUISlot> _uiSlots;
    [SerializeField] private List<InventoryUISlot> _uiHotBarSlots_Inventory;
    [SerializeField] private List<InventoryUISlot> _uiHotBarSlots_InGame;

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
        for (int i = 0; i < _uiHotBarSlots_Inventory.Count; i++)
        {
            RefreshUISlot(_uiHotBarSlots_Inventory, i, 0);
        }

        for (int i = 0; i < _uiHotBarSlots_InGame.Count; i++)
        {
            RefreshUISlot(_uiHotBarSlots_InGame, i, 0);
        }

        // Main Inventory UI reads from data indices 8 to 39 (Offset = HotbarSlots)
        for (int i = 0; i < _uiSlots.Count; i++)
        {
            RefreshUISlot(_uiSlots, i, hotbarOffset);
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
    }

    public void DropSelectedItem()
    {
        if (_currentlySelectedDataIndex < 0) return;

        InventorySlot dataSlot = InventoryManager.Instance.GetSlot(_currentlySelectedDataIndex);
        if (dataSlot != null && !dataSlot.IsEmpty)
        {
            // Remove the total amount held in this slot
            InventoryManager.Instance.RemoveItem(_currentlySelectedDataIndex, dataSlot.Amount);
            ClearDetailsPanel();
        }
    }

    // Connect this to your Primary Action Button's UnityEvent in the Inspector
    public void ExecutePrimaryAction()
    {
        if (_currentlySelectedDataIndex < 0) return;
        InventorySlot dataSlot = InventoryManager.Instance.GetSlot(_currentlySelectedDataIndex);

        // Handle logic based on dataSlot.Item.Usage here
    }

    private void EquipFromHotbar(int hotbarIndex)
    {
        InventorySlot slotToEquip = InventoryManager.Instance.GetSlot(hotbarIndex);

        if (slotToEquip == null || slotToEquip.IsEmpty || !slotToEquip.Item.IsEquipable)
        {
            return; // Can't equip nothing, or un-equipable items (like raw resources)
        }

        // Turn off previous highlight
        if (_currentlyEquippedSlotIndex >= 0 && _currentlyEquippedSlotIndex < _uiSlots.Count)
        {
            _uiSlots[_currentlyEquippedSlotIndex].SetHighlight(false);
        }

        // Highlight new selection
        _currentlyEquippedSlotIndex = hotbarIndex;
        _uiSlots[_currentlyEquippedSlotIndex].SetHighlight(true);

        // In a full game, you would fire another event here telling the PlayerController 
        // to instantiate the weapon/tool prefab in the character's hands.
        Debug.Log($"Equipped {slotToEquip.Item.ItemName}");
    }
}