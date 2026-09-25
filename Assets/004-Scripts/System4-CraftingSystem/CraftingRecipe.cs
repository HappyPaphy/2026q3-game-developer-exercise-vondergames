using System;
using System.Collections.Generic;
using UnityEngine;

public enum CraftingType
{
    Instant,
    StationOnly
}

[Serializable]
public struct CraftingIngredient
{
    [SerializeField] private ItemData _itemData;
    [SerializeField] private int _requiredAmount;

    public ItemData ItemData => _itemData;
    public int RequiredAmount => _requiredAmount;
}

[CreateAssetMenu(fileName = "New Crafting Recipe", menuName = "Crafting/Recipe")]
public class CraftingRecipe : ScriptableObject
{
    [field: SerializeField] public string RecipeName { get; private set; }
    [field: SerializeField, TextArea(3, 6)] public string Description { get; private set; }
    [field: SerializeField] public CraftingType Type { get; private set; }

    [Header("Crafting Ingredients (Max 5)")]
    [SerializeField] private List<CraftingIngredient> _ingredients = new List<CraftingIngredient>();
    public IReadOnlyList<CraftingIngredient> Ingredients => _ingredients;

    [field: SerializeField] public ItemData OutputItem { get; private set; }
    [field: SerializeField] public int OutputAmount { get; private set; } = 1;
    [field: SerializeField] public Sprite RecipeIcon { get; private set; }

    private void OnValidate()
    {
        // Enforce the strict rule of max 5 ingredients in the inspector
        if (_ingredients.Count > 5)
        {
            _ingredients.RemoveRange(5, _ingredients.Count - 5);
        }
    }
}