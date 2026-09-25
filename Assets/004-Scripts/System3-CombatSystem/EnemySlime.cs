using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemySlime : EnemyEntity
{
    [Header("Slime Configuration")]
    [SerializeField] private bool IsSmallSlime = false;
    [SerializeField] private GameObject _smallSlimePrefab;
    [SerializeField] private float _maxHealth = 20f;

    [Header("Movement & AI Settings")]
    [SerializeField] private float _moveSpeed = 3f;
    [SerializeField] private float _wanderDuration = 3f;
    [SerializeField] private float _sightDetectionRange = 6f;
    [SerializeField] private float _closeRangeAttackDistance = 1.2f;

    private Rigidbody2D _rb;
    private Transform _playerTransform;
    private float _wanderTimer;
    private float _currentMoveDirection = 1f;
    private bool _isAttacking = false;
    private float _currentAttackCooldown = 0f;
    [SerializeField] private float _attackDuration = 0f;
    [SerializeField] private float _attackCooldownDuration = 0f;

    private enum SlimeState { Wander, Chase, Attack }
    private SlimeState _currentState = SlimeState.Wander;

    protected override void Awake()
    {
        base.Awake();
        _rb = GetComponent<Rigidbody2D>();
    }

    protected override void Start()
    {
        base.Start();

        if (IsSmallSlime)
        {
            _maxHealth = 5f;
            transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
        }
        else
        {
            _maxHealth = 20f;
        }

        if (CharacterHealthComponent != null)
        {
            CharacterHealthComponent.SetMaxHP(_maxHealth);
            CharacterHealthComponent.SetHP(_maxHealth);
        }

        _wanderTimer = _wanderDuration;

        GameObject playerObj = PlayerController.Instance.gameObject;
        if (playerObj != null)
        {
            _playerTransform = playerObj.transform;
        }
    }

    protected override void Update()
    {
        base.Update();

        // Stop AI execution if dead
        if (CharacterHealthComponent != null && CharacterHealthComponent.CurrentHP <= 0)
        {
            return;
        }

        HandleAIStateMachine();
    }

    private void HandleAIStateMachine()
    {
        if (_playerTransform == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, _playerTransform.position);

        if (distanceToPlayer <= _closeRangeAttackDistance)
        {
            _currentState = SlimeState.Attack;
        }
        else if (distanceToPlayer <= _sightDetectionRange)
        {
            _currentState = SlimeState.Chase;
        }
        else
        {
            _currentState = SlimeState.Wander;
        }

        // 2. Execute Behavior per State
        switch (_currentState)
        {
            case SlimeState.Wander:
                ExecuteWander();
                break;
            case SlimeState.Chase:
                ExecuteChase();
                break;
            case SlimeState.Attack:
                {
                    if (_currentAttackCooldown > 0f && !_isAttacking)
                    {
                        _currentAttackCooldown -= Time.deltaTime;
                    }
                    else if(_currentAttackCooldown <= 0f && !_isAttacking)
                    {
                        _isAttacking = true;
                        _currentAttackCooldown = _attackCooldownDuration;
                        StartCoroutine(ExecuteAttack());
                    }
                }
                break;
        }
    }

    private void ExecuteWander()
    {
        _wanderTimer -= Time.deltaTime;
        if (_wanderTimer <= 0f)
        {
            _currentMoveDirection *= -1f;
            _wanderTimer = _wanderDuration;
        }

        _rb.linearVelocity = new Vector2(_currentMoveDirection * (_moveSpeed * 0.5f), _rb.linearVelocity.y);
        FlipSprite(_currentMoveDirection);
    }

    private void ExecuteChase()
    {
        float directionToPlayer = Mathf.Sign(_playerTransform.position.x - transform.position.x);
        _rb.linearVelocity = new Vector2(directionToPlayer * _moveSpeed, _rb.linearVelocity.y);
        FlipSprite(directionToPlayer);
    }

    private IEnumerator ExecuteAttack()
    {
        yield return new WaitForSeconds(_attackDuration);

        _rb.linearVelocity = new Vector2(0f, _rb.linearVelocity.y);

        PlayerController.Instance.TakeDamage(5f);

        _isAttacking = false;
    }

    private void FlipSprite(float direction)
    {
        if (direction != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(direction) * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    public override void TakeDamage(float damageValue)
    {
        base.TakeDamage(damageValue);

        // Instantly switch from wandering to chasing if attacked
        if (_currentState == SlimeState.Wander && _playerTransform != null)
        {
            _currentState = SlimeState.Chase;
        }
    }

    public override void Die()
    {
        // Requirement: Large slimes split into two smaller slimes (5 HP each) upon defeat
        if (!IsSmallSlime && _smallSlimePrefab != null)
        {
            SpawnSplitSlime(-0.5f); // Left split
            SpawnSplitSlime(0.5f);  // Right split
        }

        base.Die();
        Destroy(gameObject);
    }

    private void SpawnSplitSlime(float xOffset)
    {
        Vector3 spawnPos = transform.position + new Vector3(xOffset, 0.2f, 0f);
        GameObject splitSlimeObj = Instantiate(_smallSlimePrefab, spawnPos, Quaternion.identity);

        EnemySlime smallSlimeComp = splitSlimeObj.GetComponent<EnemySlime>();
        if (smallSlimeComp != null)
        {
            smallSlimeComp.IsSmallSlime = true;
        }
    }
}