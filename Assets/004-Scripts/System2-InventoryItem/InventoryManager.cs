using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class InventorySlot
{
    public ItemData Item { get; private set; }
    public int Amount { get; private set; }
    public int RemainingSpace => Item != null ? Item.MaxStackSize - Amount : 0;
    public bool IsEmpty => Item == null || Amount <= 0;

    public void AddToStack(ItemData item, int amount)
    {
        Item = item;
        Amount += amount;
    }

    public void RemoveFromStack(int amount)
    {
        Amount -= amount;
        if (Amount <= 0)
        {
            ClearSlot();
        }
    }

    public void ClearSlot()
    {
        Item = null;
        Amount = 0;
    }
}

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private int _totalSlots = 40;
    [SerializeField] private int _hotbarSlots = 8;

    [SerializeField] private InventoryUI _inventoryUI;
    [SerializeField] private GameObject _inventoryUIPanel;

    public int HotbarSlots => _hotbarSlots;

    private InventorySlot[] _slots;

    public event Action OnInventoryUpdated;

    public static InventoryManager Instance;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        _slots = new InventorySlot[_totalSlots];

        for (int i = 0; i < _totalSlots; i++)
        {
            _slots[i] = new InventorySlot();
        }
    }

    public int AddItem(ItemData itemToAdd, int amountToAdd)
    {
        // 1. Fill existing partial stacks first (Searches ALL slots to respect manual sorting)
        for (int i = 0; i < _totalSlots; i++)
        {
            if (!_slots[i].IsEmpty && _slots[i].Item == itemToAdd && _slots[i].RemainingSpace > 0)
            {
                int amountToInsert = Mathf.Min(amountToAdd, _slots[i].RemainingSpace);
                _slots[i].AddToStack(itemToAdd, amountToInsert);
                amountToAdd -= amountToInsert;

                if (amountToAdd <= 0)
                {
                    OnInventoryUpdated?.Invoke();
                    return 0;
                }
            }
        }

        // 2. Spill over into new, empty slots
        // If Usage is None, start looking at index 8. Otherwise, start at index 0.
        int emptySlotStartIndex = (itemToAdd.Usage == ItemUsage.None) ? _hotbarSlots : 0;

        for (int i = emptySlotStartIndex; i < _totalSlots; i++)
        {
            if (_slots[i].IsEmpty)
            {
                int amountToInsert = Mathf.Min(amountToAdd, itemToAdd.MaxStackSize);
                _slots[i].AddToStack(itemToAdd, amountToInsert);
                amountToAdd -= amountToInsert;

                if (amountToAdd <= 0)
                {
                    OnInventoryUpdated?.Invoke();
                    return 0;
                }
            }
        }

        OnInventoryUpdated?.Invoke();
        return amountToAdd; // Return items that didn't fit (inventory full)
    }

    public void RemoveItem(int slotIndex, int amountToRemove)
    {
        if (slotIndex < 0 || slotIndex >= _totalSlots || _slots[slotIndex].IsEmpty) return;

        _slots[slotIndex].RemoveFromStack(amountToRemove);
        OnInventoryUpdated?.Invoke();
    }

    public void SwapSlots(int indexA, int indexB)
    {
        if (indexA < 0 || indexA >= _totalSlots || indexB < 0 || indexB >= _totalSlots) return;

        // If they are the same item, try to merge stacks instead of swapping
        if (!_slots[indexA].IsEmpty && !_slots[indexB].IsEmpty && _slots[indexA].Item == _slots[indexB].Item)
        {
            int spaceInB = _slots[indexB].RemainingSpace;
            if (spaceInB > 0)
            {
                int amountToMove = Mathf.Min(_slots[indexA].Amount, spaceInB);
                _slots[indexB].AddToStack(_slots[indexA].Item, amountToMove);
                _slots[indexA].RemoveFromStack(amountToMove);
                OnInventoryUpdated?.Invoke();
                return;
            }
        }

        // Standard Swap
        InventorySlot temp = _slots[indexA];
        _slots[indexA] = _slots[indexB];
        _slots[indexB] = temp;

        OnInventoryUpdated?.Invoke();
    }

    public void SortInventory()
    {
        // Extract all items, sort them by Type then by Name using LINQ
        var populatedSlots = _slots.Where(s => !s.IsEmpty)
                                   .OrderBy(s => s.Item.Type)
                                   .ThenBy(s => s.Item.ItemName)
                                   .ToList();

        // Re-populate the array
        for (int i = 0; i < _totalSlots; i++)
        {
            if (i < populatedSlots.Count)
            {
                _slots[i] = populatedSlots[i];
            }
            else
            {
                _slots[i] = new InventorySlot(); // Fill the rest with empty slots
            }
        }

        OnInventoryUpdated?.Invoke();
    }

    public List<InventorySlot> GetFilteredItems(ItemType targetType)
    {
        // Returns a list of slots that match the specific type for UI filtering
        return _slots.Where(s => !s.IsEmpty && s.Item.Type == targetType).ToList();
    }

    public InventorySlot GetSlot(int index)
    {
        if (index < 0 || index >= _totalSlots) return null;
        return _slots[index];
    }

    public void OpenInventoryUIPanel()
    {
        _inventoryUI.RefreshAllUISlot();
        _inventoryUIPanel.SetActive(!_inventoryUIPanel.activeInHierarchy);
    }
}
