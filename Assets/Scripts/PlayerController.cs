using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float runSpeed = 10f;
    [SerializeField] private float dashSpeed = 2000f;
    [SerializeField] private float dashDuration = 0.5f;
    [SerializeField] private float dashCooldown = 1f;

    private float _lastDashTime = -100f;
    private Rigidbody2D _rb2D;
    private Animator _animator;
    private bool _isWalking;
    private bool _isDashing;
    private bool _isRunning;
    private Vector2 _movementInput;
    private Vector2 _dashDirection;
    private CurrentDirection _currentDirection = CurrentDirection.Down;

    private static readonly int Walking = Animator.StringToHash("IsWalking");
    private static readonly int Direction = Animator.StringToHash("CurrentDirection");

    private enum CurrentDirection
    {
        Down,
        Up,
        Right,
        Left
    };

    private bool IsWalking
    {
        get => _isWalking;
        set
        {
            _isWalking = value; 
            _animator.SetBool(Walking, value);
        }
    }

    private void Start()
    {
        _rb2D = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        GetInput();
        UpdateMovement();
    }

    private void GetInput()
    {
        _movementInput.x = Input.GetAxisRaw("Horizontal");
        _movementInput.y = Input.GetAxisRaw("Vertical");

        IsWalking = _movementInput != Vector2.zero;

        if (Input.GetKeyDown(KeyCode.D))
        {
            _currentDirection = CurrentDirection.Right;
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            _currentDirection = CurrentDirection.Left;
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            _currentDirection = CurrentDirection.Down;
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            _currentDirection = CurrentDirection.Up;
        }
        _animator.SetInteger(Direction, (int)_currentDirection);
        _isDashing = Input.GetKeyDown(KeyCode.Space);
        _isRunning = Input.GetKey(KeyCode.LeftShift);
    }

    private void UpdateMovement()
    {
        if (_isDashing && Time.time > _lastDashTime + dashCooldown)
        {
            _lastDashTime = Time.time;
            _dashDirection = _movementInput.normalized;
            StartCoroutine(Dash());
        }
        else
        {
            var movement = _movementInput * moveSpeed;
            if (_isRunning)
            {
                movement *= runSpeed / moveSpeed;
            }
            _rb2D.velocity = movement;
        }
    }

    private IEnumerator Dash()
    {
        var timeElapsed = 0f;
        while (timeElapsed < dashDuration)
        {
            _rb2D.velocity = _dashDirection * dashSpeed;
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        _rb2D.velocity = Vector2.zero;
    }
}