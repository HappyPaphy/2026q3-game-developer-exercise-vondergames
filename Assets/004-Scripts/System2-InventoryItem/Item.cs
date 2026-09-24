using UnityEngine;
using UnityEngine.InputSystem;

public class Item : MonoBehaviour
{
    [SerializeField] private GameObject _interactHint;
    [SerializeField] private ItemData _itemData;
    [SerializeField] private int _itemCount;

    [SerializeField] private float _detectPlayerRadius;
    [SerializeField] private LayerMask _playerLayer;
    [SerializeField] private bool isPickUpInfinitely = false;

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
        int itemAddedCount = InventoryManager.Instance.AddItem(_itemData, _itemCount);

        if(itemAddedCount == 0)
        {
            if(!isPickUpInfinitely)
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
