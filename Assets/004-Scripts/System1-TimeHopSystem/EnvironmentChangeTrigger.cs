using UnityEngine;
using UnityEngine.InputSystem;

public class EnvironmentChangeTrigger : MonoBehaviour
{
    [SerializeField] private GameObject _interactHint;

    [SerializeField] private float _detectPlayerRadius;
    [SerializeField] private LayerMask _playerLayer;

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
        if(_isPlayerInRange)
        {
            if(!_interactHint.activeInHierarchy)
            {
                _interactHint.SetActive(true);
            }

            if(Keyboard.current.fKey.wasPressedThisFrame)
            {
                EnvironmentVisualManager.Instance.TriggerNextPeriod();
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
