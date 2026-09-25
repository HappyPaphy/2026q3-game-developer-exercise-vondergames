using UnityEngine;
using UnityEngine.InputSystem;

public class MagicWand : Weapon
{
    [Header("Combat Settings")]
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private Transform _firePoint;
    [SerializeField] private float _fireCooldown = 0.4f;

    [Header("Resource Cost (Arcane Power / Stamina)")]
    [SerializeField] private float _healthCost = 5f;

    private float _lastFireTime = -999f;

    void Update()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            TryShoot();
        }
    }

    private void TryShoot()
    {
        if (Time.time < _lastFireTime + _fireCooldown) return;

        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.CharacterHealthComponent.TakeDamage(_healthCost);
        }

        Shoot();
        _lastFireTime = Time.time;
    }

    private void Shoot()
    {
        if (_projectilePrefab == null || _firePoint == null)
        {
            Debug.LogWarning("Wand is missing Projectile Prefab or FirePoint reference!");
            return;
        }

        Vector2 _currentTarget = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 shootDirection = _currentTarget - GetComponent<Rigidbody2D>().position;

        GameObject projObj = Instantiate(_projectilePrefab, _firePoint.position, _firePoint.rotation);
        Projectile projectileComponent = projObj.GetComponent<Projectile>();

        if (projectileComponent != null)
        {
            projectileComponent.Initialize(shootDirection);
        }
    }
}