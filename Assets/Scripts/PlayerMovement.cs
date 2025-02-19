using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Parameters")]
    [SerializeField] private float _playerSpeed;
    [SerializeField] private float _jumpStrength; [Space]

    private Vector2 _dirX;

    private bool canMove   = true;
    private bool canJump   = true;
    private bool isHoldingDownDirection = false;

    [Header("Rigidbody")]
    [SerializeField] private Rigidbody2D _rb; [Space]
    [SerializeField] private float _friction;
    [SerializeField] private float _gravity;
    [SerializeField] private float _jumpGravityModifier = 3; [Space]

    [Header("Groundcheck")]
    [SerializeField] private Transform _groundCheckPos;
    [SerializeField] private Vector2 _groundCheckSize = new Vector2(0.5f, 0.5f); [Space]
    [SerializeField] private LayerMask groundLayer; [Space]

    [Header("Wallcheck")]
    [SerializeField] private Transform _wallCheckLeftPos;
    [SerializeField] private Transform _wallCheckRightPos;
    [SerializeField] private Vector2 _wallCheckLeftSize = new Vector2(0.5f, 0.5f);
    [SerializeField] private Vector2 _wallCheckRightSize = new Vector2(0.5f, 0.5f);


    public bool GetCanMove() {  return canMove; }
    public void SetCanMove(bool value) { canMove = value; }

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.linearDamping = _friction;
        _rb.gravityScale  = _gravity;
    }

    private void Update()
    {
        if (!canMove) return;

        _rb.linearVelocity += _dirX * _playerSpeed * Time.deltaTime;

        HandleWallClinging();
    }

    public void OnLeftRightMovement(InputAction.CallbackContext context)
    {
        if (!canMove) return;

        if (context.performed) isHoldingDownDirection = true;

        _dirX = context.ReadValue<Vector2>();

        if (context.canceled) isHoldingDownDirection  = false;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        Debug.Log("OnJump()");
        if (!canMove) return;

        if (!canJump) return;

        if (!isGrounded() & !(IsWalledLeft() || IsWalledRight())) return;

        if (context.performed)
        {
            Debug.Log("context.performed start");

            if (isGrounded())
            {
                _rb.linearVelocity = new Vector2(_rb.linearVelocityX, _jumpStrength);
            }

            else if (IsWalledLeft()) _rb.linearVelocity  = new Vector2(_jumpStrength * 0.5f, _jumpStrength * 0.5f);

            else if (IsWalledRight()) _rb.linearVelocity = new Vector2(_jumpStrength * -0.5f, _jumpStrength * 0.5f);

            Debug.Log("context.performed end");
        } 

        if (context.canceled)
        {
            Debug.Log("Context canceled");
            _rb.linearVelocity = new Vector2(_rb.linearVelocityX, -_jumpStrength * 0.5f);
        }

        Debug.Log("End of function");
    }

    public void VelocityReset()
    {
        _rb.linearVelocity = Vector2.zero;
    }

    private bool isGrounded()
    {
        if (Physics2D.OverlapBox(_groundCheckPos.position, _groundCheckSize, 0, groundLayer)) return true;

        else return false;
    }

    private bool IsWalledLeft()
    {
        if (Physics2D.OverlapBox(_wallCheckLeftPos.position, _wallCheckLeftSize, 0, groundLayer)) return true;

        else return false;
    }

    private bool IsWalledRight()
    {
        if (Physics2D.OverlapBox(_wallCheckRightPos.position, _wallCheckRightSize, 0, groundLayer)) return true;

        else return false;
    }

    private void HandleWallClinging()
    {
        if (isHoldingDownDirection && (IsWalledLeft() || IsWalledRight()))
        {
            _rb.gravityScale = 0f;
        }

        else if (_rb.linearVelocity.y < -0.1f)
        {
            _rb.gravityScale = _gravity * _jumpGravityModifier;
        }

        else
        {
            _rb.gravityScale = _gravity;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(_groundCheckPos.position, _groundCheckSize);
        Gizmos.DrawWireCube(_wallCheckLeftPos.position, _wallCheckLeftSize);
        Gizmos.DrawWireCube(_wallCheckRightPos.position, _wallCheckRightSize);
    }
}