using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CraftingRecipeRowUI : MonoBehaviour
{
    [Header("Row Data")]
    [SerializeField] private CraftingRecipe _assignedRecipe; // Assignable directly in the Inspector or via code

    [Header("Output Display")]
    [SerializeField] private Image _outputIconImage;
    [SerializeField] private TextMeshProUGUI _outputAmountText;

    [Header("Dynamic Ingredient Prefab Settings")]
    [SerializeField] private Transform _ingredientsContainer;      // The layout group parent container inside the row
    [SerializeField] private GameObject _ingredientSlotPrefab;     // Prefab for a single ingredient slot

    private CraftingUI _craftingUI;

    public CraftingRecipe AssignedRecipe => _assignedRecipe;

    private void Start()
    {
        // Auto-initialize if placed directly in the scene with a preset recipe data
        if (_assignedRecipe != null && _craftingUI == null)
        {
            _craftingUI = FindFirstObjectByType<CraftingUI>();
            SetupRow(_assignedRecipe, _craftingUI);
        }
    }

    public void SetupRow(CraftingRecipe recipe, CraftingUI craftingUI)
    {
        _assignedRecipe = recipe;
        _craftingUI = craftingUI;

        // 1. Setup Output Icon
        if (recipe.OutputItem != null)
        {
            _outputIconImage.sprite = recipe.OutputItem.Icon;
            _outputIconImage.color = Color.white;
            _outputIconImage.SetNativeSize();

            if (_outputAmountText != null)
            {
                _outputAmountText.text = recipe.OutputAmount > 0 ? recipe.OutputAmount.ToString() : string.Empty;
            }
        }

        // 2. Clear existing dynamic ingredient slots from previous renders
        foreach (Transform child in _ingredientsContainer)
        {
            Destroy(child.gameObject);
        }

        // 3. Dynamically instantiate ingredient prefabs (hiding unused slots completely by not spawning them)
        foreach (var ingredient in recipe.Ingredients)
        {
            if (ingredient.ItemData != null)
            {
                GameObject slotObj = Instantiate(_ingredientSlotPrefab, _ingredientsContainer);
                CraftingIngredientSlotUI slotUI = slotObj.GetComponent<CraftingIngredientSlotUI>();

                if (slotUI != null)
                {
                    slotUI.Setup(ingredient.ItemData, ingredient.RequiredAmount);
                }
            }
        }
    }

    // Connect this method to the onClick event of your Row's UIFeedback/Button component
    public void OnRowClicked()
    {
        if (_craftingUI != null && _assignedRecipe != null)
        {
            _craftingUI.SelectRecipe(_assignedRecipe);
        }
    }
}