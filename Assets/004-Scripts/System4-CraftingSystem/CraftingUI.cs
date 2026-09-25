using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class CraftingUI : MonoBehaviour
{
    public GameObject CraftingUIPanel;

    [Header("Recipe List View")]
    [SerializeField] private Transform _recipeContentParent;
    [SerializeField] private GameObject _recipeRowPrefab;
    [SerializeField] private List<CraftingRecipe> _availableRecipes;
    [SerializeField] private bool _isCraftingStation = false; // True if opened at a station, false for Instant

    [Header("Right Detail Panel")]
    [SerializeField] private Image _detailItemIcon;
    [SerializeField] private TextMeshProUGUI _detailRecipeName;
    [SerializeField] private TextMeshProUGUI _detailRecipeDescription;
    [SerializeField] private GameObject _btnCraftObject;

    [SerializeField] private bool isThisInstantCraftingUI = false;

    [SerializeField] private GameObject _interactHint;
    [SerializeField] private float _detectPlayerRadius;
    [SerializeField] private LayerMask _playerLayer;

    private bool _isPlayerInRange = false;
    private CraftingRecipe _selectedRecipe;

    private void OnEnable()
    {
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnInventoryUpdated += PopulateRecipeList;
        }
    }

    private void OnDisable()
    {
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnInventoryUpdated -= PopulateRecipeList;
        }
    }

    private void Start()
    {
        PopulateRecipeList();
        ClearDetailPanel();
    }

    private void Update()
    {
        if(Keyboard.current.iKey.wasPressedThisFrame)
        {
            SetActiveCraftingUIPanel(false);
        }

        if (_isCraftingStation)
        {
            CheckPlayerRange();
            HandlePlayerInRange();

            if (Keyboard.current.cKey.wasPressedThisFrame)
            {
                SetActiveCraftingUIPanel(false);
            }
        }
    }

    public void SetActiveCraftingUIPanel(bool isTrue)
    {
        CraftingManager.Instance.IsCraftingUIActive = isTrue;

        if (isTrue)
        {
            PopulateRecipeList();
        }

        if (!isThisInstantCraftingUI)
        {
            CraftingUIPanel.SetActive(isTrue);
            return;
        }

        CraftingUIPanel.SetActive(isTrue);
    }

    private void PopulateRecipeList()
    {
        // Clear old rows if any
        foreach (Transform child in _recipeContentParent)
        {
            Destroy(child.gameObject);
        }

        // Instantiate rows based on whether we are at a station or doing instant crafting
        foreach (var recipe in _availableRecipes)
        {
            // If it's instant crafting, skip recipes that require a station
            if (!_isCraftingStation && recipe.Type == CraftingType.StationOnly) continue;

            GameObject rowObj = Instantiate(_recipeRowPrefab, _recipeContentParent);
            CraftingRecipeRowUI rowUI = rowObj.GetComponent<CraftingRecipeRowUI>();

            if (rowUI != null)
            {
                rowUI.SetupRow(recipe, this);
            }
        }

        if (_selectedRecipe != null)
        {
            SelectRecipe(_selectedRecipe);
        }
    }

    public void SelectRecipe(CraftingRecipe recipe)
    {
        _selectedRecipe = recipe;

        if (_selectedRecipe == null)
        {
            ClearDetailPanel();
            return;
        }

        // Populate the right-side detail panel (similar to InventoryUI)
        if (_detailItemIcon != null && recipe.OutputItem != null)
        {
            _detailItemIcon.sprite = recipe.OutputItem.Icon;
            _detailItemIcon.color = Color.white;
            _detailItemIcon.SetNativeSize();
        }

        if (_detailRecipeName != null)
        {
            _detailRecipeName.text = recipe.RecipeName;
        }

        if (_detailRecipeDescription != null)
        {
            _detailRecipeDescription.text = recipe.Description;
        }

        if (_btnCraftObject != null)
        {
            _btnCraftObject.SetActive(true);
        }
    }

    private void ClearDetailPanel()
    {
        _selectedRecipe = null;

        if (_detailItemIcon != null)
        {
            _detailItemIcon.sprite = null;
            _detailItemIcon.color = Color.clear;
        }

        if (_detailRecipeName != null)
        {
            _detailRecipeName.text = "SELECT RECIPE";
        }

        if (_detailRecipeDescription != null)
        {
            _detailRecipeDescription.text = string.Empty;
        }

        if (_btnCraftObject != null)
        {
            _btnCraftObject.SetActive(false);
        }
    }

    // Connect this method to the "CRAFT" button's click event on the right panel
    public void OnClickCraftButton()
    {
        if (_selectedRecipe == null) return;

        bool success = CraftingManager.Instance.TryCraft(_selectedRecipe, _isCraftingStation);

        if (success)
        {
            Debug.Log($"Crafted successfully: {_selectedRecipe.RecipeName}");
            // Refresh the list UI so ingredient quantities update properly
            PopulateRecipeList();
            SelectRecipe(_selectedRecipe);
        }
        else
        {
            Debug.Log("Crafting failed: Not enough materials.");
        }
    }

    private void HandlePlayerInRange()
    {
        if (_isPlayerInRange)
        {
            if (!_interactHint.activeInHierarchy)
            {
                _interactHint.SetActive(true);
            }

            if (Keyboard.current.fKey.wasPressedThisFrame)
            {
                SetActiveCraftingUIPanel(true);
            }
        }
        else
        {
            if (_interactHint.activeInHierarchy)
            {
                _interactHint.SetActive(false);
                SetActiveCraftingUIPanel(false);
            }
        }
    }

    private void CheckPlayerRange()
    {
        _isPlayerInRange = Physics2D.OverlapCircle(transform.position, _detectPlayerRadius, _playerLayer);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _detectPlayerRadius);
    }
}