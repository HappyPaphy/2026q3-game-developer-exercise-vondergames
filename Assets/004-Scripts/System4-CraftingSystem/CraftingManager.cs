using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CraftingManager : MonoBehaviour
{
    public static CraftingManager Instance;

    [SerializeField] private List<CraftingRecipe> _allRecipes;
    [SerializeField] private CraftingUI _instantCraftingUI;

    [HideInInspector] public bool IsCraftingUIActive = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        HandleToggleCraftingUIPanel();
    }

    private void HandleToggleCraftingUIPanel()
    {
        if (Keyboard.current.cKey.wasPressedThisFrame)
        {
            if (!_instantCraftingUI.CraftingUIPanel.activeInHierarchy)
            {
                _instantCraftingUI.SetActiveCraftingUIPanel(true);
            }
            else
            {
                _instantCraftingUI.SetActiveCraftingUIPanel(false);
            }
        }
    }

    public bool CanCraft(CraftingRecipe recipe, bool isAtStation)
    {
        // 1. Check station restriction rule
        if (recipe.Type == CraftingType.StationOnly && !isAtStation)
        {
            return false;
        }

        // 2. Check if player has enough of each ingredient across all inventory slots
        foreach (var ingredient in recipe.Ingredients)
        {
            int totalFound = 0;

            InventoryUI _inventoryUI = InventoryManager.Instance.InventoryUI;

            for (int i = 0; i < _inventoryUI.UISlots.Count; i++)
            {
                var slot = InventoryManager.Instance.GetSlot(i);
                if (slot != null && !slot.IsEmpty && slot.Item == ingredient.ItemData)
                {
                    totalFound += slot.Amount;
                }
            }

            for (int i = 0; i < _inventoryUI.UIHotBarSlots_Inventory.Count; i++)
            {
                var slot = InventoryManager.Instance.GetSlot(i);
                if (slot != null && !slot.IsEmpty && slot.Item == ingredient.ItemData)
                {
                    totalFound += slot.Amount;
                }
            }

            if (totalFound < ingredient.RequiredAmount)
            {
                return false;
            }
        }

        return true;
    }

    public bool TryCraft(CraftingRecipe recipe, bool isAtStation)
    {
        if (!CanCraft(recipe, isAtStation))
        {
            Debug.Log($"Cannot craft {recipe.RecipeName}. Missing requirements or not at a crafting station.");
            return false;
        }

        // 1. Deduct ingredient amounts across all inventory/hotbar slots
        foreach (var ingredient in recipe.Ingredients)
        {
            int amountNeeded = ingredient.RequiredAmount;

            for (int i = 0; i < 40; i++)
            {
                var slot = InventoryManager.Instance.GetSlot(i);
                if (slot != null && !slot.IsEmpty && slot.Item == ingredient.ItemData)
                {
                    int takeAmount = Mathf.Min(amountNeeded, slot.Amount);

                    // Deduct using existing inventory manager logic
                    InventoryManager.Instance.RemoveItem(i, takeAmount);
                    amountNeeded -= takeAmount;

                    if (amountNeeded <= 0) break;
                }
            }
        }

        // 2. Add the crafted output item into inventory[cite: 6]
        int overflow = InventoryManager.Instance.AddItem(recipe.OutputItem, recipe.OutputAmount);

        if (overflow > 0 && recipe.OutputItem.WorldPrefab != null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            Vector3 spawnPosition = player != null ? player.transform.position : transform.position;

            GameObject droppedObj = Instantiate(recipe.OutputItem.WorldPrefab, spawnPosition, Quaternion.identity);
            Item worldItemComp = droppedObj.GetComponent<Item>();
            if (worldItemComp != null)
            {
                worldItemComp.SetItemCount(overflow);
            }

            Debug.LogWarning($"Inventory full! Dropped {overflow} {recipe.OutputItem.ItemName} into the world.");
        }

        Debug.Log($"Successfully crafted: {recipe.RecipeName}");
        return true;
    }
}