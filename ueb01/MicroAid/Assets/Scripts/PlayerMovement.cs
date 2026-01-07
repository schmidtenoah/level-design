using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")] 
    [SerializeField] private float moveSpeed;
    [SerializeField] private Transform orientation;
    [SerializeField] private float groundDrag;
    public bool IsWalking { get; private set; }

    [SerializeField] private  float jumpForce;
    [SerializeField] private  float jumpCooldown;
    [SerializeField] private  float captureCooldown;
    [SerializeField] private  float airMultiplier;
    private bool _jumpReady;

    private Vector2 _moveInput;
    private float _horizontalInput;
    private float _verticalInput;

    private Vector3 _moveDir;
    private Rigidbody _rb;

    [Header("Ground Check")] 
    [SerializeField] private float playerHeight;
    [SerializeField] private float playerRadius;
    [SerializeField] private  LayerMask groundLayer;
    private bool _grounded;
    
    [Header("Bugnet")]
    [SerializeField] private Collider bugnetCollider;
    
    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.freezeRotation = true;
        _jumpReady = true;
        bugnetCollider.enabled = false;
    }

    private void Update()
    {
        Vector3 rayOrigin = transform.position + Vector3.up * 0.1f;
        _grounded = Physics.Raycast(rayOrigin, Vector3.down, 0.3f, groundLayer);
        
        SpeedControl();

        _rb.drag = (_grounded) ? groundDrag : 0;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void OnMove(InputValue val)
    {
        _moveInput = val.Get<Vector2>();
        _horizontalInput = _moveInput.x;
        _verticalInput = _moveInput.y;
    }
    
    private bool CanMove(Vector3 moveDirection)
    {
        return !Physics.CapsuleCast(
            transform.position,
            transform.position + Vector3.up * playerHeight,
            playerRadius,
            moveDirection.normalized,
            moveSpeed * Time.deltaTime * 1.1f, groundLayer
        );
    }

    private void MovePlayer()
    {
        _moveDir = orientation.forward * _verticalInput + orientation.right * _horizontalInput;

        if (!CanMove(_moveDir))
        {
            Vector3 moveDirX = orientation.right * _horizontalInput;
            Vector3 moveDirZ = orientation.forward * _verticalInput;

            if (moveDirX != Vector3.zero && CanMove(moveDirX))
            {
                _moveDir = moveDirX;
            }
            else if (moveDirZ != Vector3.zero && CanMove(moveDirZ))
            {
                _moveDir = moveDirZ;
            }
            else
            {
                _moveDir = Vector3.zero;
            }
        }

        IsWalking = _moveDir != Vector3.zero;
        if (IsWalking)
        {
            _rb.AddForce(_moveDir.normalized * (moveSpeed * (_grounded ? 1 : airMultiplier)), ForceMode.Force);
        }
    }
    
    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitVel = flatVel.normalized * moveSpeed;
            _rb.velocity = new Vector3(limitVel.x, _rb.velocity.y, limitVel.z); 
        }
    }

    private void Jump()
    {
        _rb.velocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);
        _rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        _jumpReady = true;
    }

    private void OnJump(InputValue val)
    {
        if (_jumpReady && _grounded)
        {
            _jumpReady = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void OnCapture(InputValue val)
    {
        bugnetCollider.enabled = true;
        BugnetAnimator.TriggerCapturingAnim();
        Invoke(nameof(ResetCaptureCollider), captureCooldown);
    }
    
    private void ResetCaptureCollider()
    {
        bugnetCollider.enabled = false;
    }

}
