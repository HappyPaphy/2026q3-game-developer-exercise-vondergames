using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CraftingIngredientSlotUI : MonoBehaviour
{
    [SerializeField] private Image _iconImage;
    [SerializeField] private TextMeshProUGUI _amountText;

    public void Setup(ItemData itemData, int requiredAmount)
    {
        if (itemData != null)
        {
            _iconImage.sprite = itemData.Icon;
            _iconImage.color = Color.white;
            _iconImage.SetNativeSize();

            int playerHas = GetPlayerItemCount(itemData);
            _amountText.text = $"{playerHas}/{requiredAmount}";

            // Highlight red if the player doesn't have enough resources
            _amountText.color = playerHas >= requiredAmount ? Color.white : Color.red;
        }
    }

    private int GetPlayerItemCount(ItemData targetItem)
    {
        int total = 0;
        if (InventoryManager.Instance == null) return total;

        for (int i = 0; i < 40; i++) // Total inventory slots
        {
            var slot = InventoryManager.Instance.GetSlot(i);
            if (slot != null && !slot.IsEmpty && slot.Item == targetItem)
            {
                total += slot.Amount;
            }
        }
        return total;
    }
}