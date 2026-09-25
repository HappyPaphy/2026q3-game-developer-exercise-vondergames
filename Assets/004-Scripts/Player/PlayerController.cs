using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerController : PlayerEntity 
{
    [Header("Movement Settings")]
    [SerializeField] private float _moveSpeed = 8f;
    [SerializeField] private float _jumpForce = 12f;

    [Header("Ground Environment")]
    [SerializeField] private Transform _groundCheckPoint;
    [SerializeField] private float _groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask _groundLayer;

    [SerializeField] private SpriteRenderer _sprRndr;

    public List<Weapon> RangeWeapons;
    public Transform WeaponPos;
    private float _weaponPosOffSet = 0.2f;

    private Vector2 _currentTarget = Vector2.zero;
    private Rigidbody2D _rb;
    private PlayerEntity _playerEntity;

    private float _horizontalInput;
    private bool _isGrounded;

    public static PlayerController Instance;

    protected override void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        _rb = GetComponent<Rigidbody2D>();
        _playerEntity = GetComponent<PlayerEntity>();

        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
    }
    protected override void Update()
    {
        GatherInput();
        CheckGroundedStatus();
        HandleJump();

        WeaponSetPos();
        PlayerAim();

        base.Update();
    }

    private void FixedUpdate()
    {
        ApplyMovement();
    }

    private void GatherInput()
    {
        if(Keyboard.current.aKey.wasPressedThisFrame)
        {
            _horizontalInput = -1f;
        }
        else if(Keyboard.current.aKey.wasReleasedThisFrame)
        {
            _horizontalInput = 0f;
        }

        if (Keyboard.current.dKey.wasPressedThisFrame)
        {
            _horizontalInput = 1;
        }
        else if (Keyboard.current.dKey.wasReleasedThisFrame)
        {
            _horizontalInput = 0f;
        }

        if (Keyboard.current.iKey.wasPressedThisFrame)
        {
            InventoryManager.Instance.ToggleInventoryUIPanel();
        }
    }

    private void CheckGroundedStatus()
    {
        if (_groundCheckPoint != null)
        {
            _isGrounded = Physics2D.OverlapCircle(_groundCheckPoint.position, _groundCheckRadius, _groundLayer);
        }
    }

    private void HandleJump()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame && _isGrounded)
        {
            ExecuteJump();
        }
    }

    private void ExecuteJump()
    {
        _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, 0f);
        _rb.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
    }

    private void ApplyMovement()
    {
        _rb.linearVelocity = new Vector2(_horizontalInput * _moveSpeed, _rb.linearVelocity.y);

        if (_horizontalInput > 0)
        {
            _sprRndr.flipX = false;
        }
        else
        {
            _sprRndr.flipX = true;
        }
    }

    private void WeaponSetPos()
    {
        if(RangeWeapons.Count <= 0) { return; }

        foreach (Weapon weapon in RangeWeapons)
        {
            if (weapon != null)
            {
                if ((weapon.transform.localEulerAngles.z > 90) && (weapon.transform.localEulerAngles.z < 270))
                {
                    weapon.transform.position = WeaponPos.position + new Vector3(0f, _weaponPosOffSet, 0f);
                    _sprRndr.flipX = true;
                    weapon.GetComponent<Weapon>().WeaponSprRndr.flipY = true;
                }
                else
                {
                    weapon.transform.position = WeaponPos.position - new Vector3(0f, 0f, 0f);
                    _sprRndr.flipX = false;
                    weapon.GetComponent<Weapon>().WeaponSprRndr.flipY = false;
                }
            }
        }
    }

    private void PlayerAim()
    {
        if (RangeWeapons.Count <= 0) { return; }

        _currentTarget = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        foreach (Weapon weapon in RangeWeapons)
        {
            if (weapon != null)
            {
                Vector2 lookDirection = _currentTarget - weapon.GetComponent<Rigidbody2D>().position;
                float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
                weapon.GetComponent<Rigidbody2D>().rotation = angle;
            }
        }
    }

    public override void TakeDamage(float damageValue)
    {
        base.TakeDamage(damageValue);
    }

    private void OnDrawGizmosSelected()
    {
        // Helpful visualization for reviewers checking the ground detection radius
        if (_groundCheckPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_groundCheckPoint.position, _groundCheckRadius);
        }
    }
}
