using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Item : MonoBehaviour
{
    [SerializeField] private GameObject _interactHint;
    [SerializeField] private TextMeshProUGUI _textItemName;
    [SerializeField] private SpriteRenderer _sprRndrItemIcon;
    [SerializeField] private int _itemCount;

    [SerializeField] private float _detectPlayerRadius;
    [SerializeField] private LayerMask _playerLayer;

    public ItemData ItemData;
    public bool IsPickUpInfinitely = false;
    private bool _isPlayerInRange = false;

    void Start()
    {
        _interactHint.SetActive(false);
    }

    void Update()
    {
        CheckPlayerRange();
        HandlePlayerInRange();
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
                PickUpItem();
            }
        }
        else
        {
            if (_interactHint.activeInHierarchy)
            {
                _interactHint.SetActive(false);
            }
        }
    }

    private void PickUpItem()
    {
        int itemAddedCount = InventoryManager.Instance.AddItem(ItemData, _itemCount);

        if(itemAddedCount == 0)
        {
            if(!IsPickUpInfinitely)
            {
                Destroy(gameObject);
            }
            else
            {
                _itemCount = 1;
            }
        }
        else
        {
            _itemCount = itemAddedCount; // Remain what couldn't be picked up
        }

        GameStatusMessage.Instance.CreateMessage($"You pick up [{_itemCount}] [{ItemData.ItemName}]");
    }

    public void ApplyItemDataInfo()
    {
        _textItemName.text = ItemData.ItemName;
        _sprRndrItemIcon.sprite = ItemData.Icon;
    }

    public void SetItemCount(int count)
    {
        _itemCount = count;
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
