using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum SlotType
{
    Ordinary,
    HotBar
}

public class InventoryUISlot : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private Image _iconImage;
    [SerializeField] private TextMeshProUGUI _amountText;
    [SerializeField] private GameObject _highlightOutline;
    [SerializeField] private GameObject _CountPanel;
    [SerializeField] private SlotType _slotType;

    private int _assignedDataIndex = -1;
    private InventoryUI _inventoryUI;

    public void UpdateSlotVisuals(InventorySlot slotData)
    {
        Navigation _nav = new Navigation();

        if (slotData == null || slotData.IsEmpty)
        {
            _iconImage.sprite = null;
            _iconImage.color = Color.clear;
            _amountText.text = string.Empty;
            _CountPanel.SetActive(false);

            if(_button != null)
            {
                _button.interactable = false;
                _nav.mode = Navigation.Mode.None;
                _button.navigation = _nav;
            }
            return;
        }

        _iconImage.sprite = slotData.Item.Icon;
        _iconImage.SetNativeSize();
        _iconImage.color = Color.white;

        if (_button != null)
        {
            _button.interactable = true;
            _nav.mode = Navigation.Mode.Automatic;
            _button.navigation = _nav;
        }

        _amountText.text = slotData.Amount > 0 ? slotData.Amount.ToString() : string.Empty;

        _CountPanel.SetActive(true);
    }

    public void SetHighlight(bool isActive)
    {
        if (_highlightOutline != null)
        {
            _highlightOutline.SetActive(isActive);
        }
    }

    public void SetDataIndex(int dataIndex, InventoryUI uiReference)
    {
        _assignedDataIndex = dataIndex;
        _inventoryUI = uiReference;
    }

    // Connect this method to the onClick UnityEvent on your UIFeedback component
    public void OnSlotClicked()
    {
        if (_inventoryUI != null && _assignedDataIndex >= 0)
        {
            _inventoryUI.SelectSlot(_assignedDataIndex);
        }
    }
}