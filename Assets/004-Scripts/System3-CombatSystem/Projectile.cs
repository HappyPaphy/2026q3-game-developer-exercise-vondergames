using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float _speed = 12f;
    [SerializeField] private float _lifetime = 3f;

    private int _damage;
    [SerializeField] private Rigidbody2D _rb;
    private Vector2 _direction;

    private void Start()
    {
        _damage = Random.Range(3, 6);
    }

    public void Initialize(Vector2 direction)
    {
        _direction = direction.normalized;
    }

    private void FixedUpdate()
    {
        _rb.linearVelocity = _direction * _speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<EnemyEntity>(out var enemyEntity))
        {
            enemyEntity.CharacterHealthComponent.TakeDamage(_damage);
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Obstacle"))
        {
            Destroy(gameObject);
        }
    }
}