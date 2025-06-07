using UnityEngine;
using UnityEngine.InputSystem;

public abstract class PlayerControllerBase : MonoBehaviour
{
    [Header("Animacja i fizyka")]
    [SerializeField] protected Animator animator;
    [SerializeField] protected float jumpingPower = 23f;
    [SerializeField] protected float moveSpeed = 5f;
    [SerializeField] protected Transform groundCheckPoint;
    [SerializeField] protected LayerMask groundLayer;
    [SerializeField] protected LayerMask playerLayer;

    [Header("Wejœcia gracza")]
    //protected PlayerInput playerInput;
    [SerializeField] protected InputActionReference move;
    [SerializeField] protected InputActionReference jump;
    [SerializeField] protected InputActionReference interact;

    [Header("Specjalne zdolnoœci")]
    [SerializeField] protected bool canJump = true;
    [SerializeField] protected bool canInteract = true;

    protected Rigidbody2D rb;
    protected Vector2 _moveDirection;
    protected Vector2 groundCheckSize = new Vector2(0.5f, 0.5f);
    protected bool isFacingRight = true;
    protected bool isGrounded;
    protected bool canMove = true;
    protected float interactDistance = 1.5f;

    [Header("Efekty dŸwiêkowe poruszania")]
    [SerializeField] private AudioSource controllSource;
    [SerializeField] private AudioClip walkClip;
    [SerializeField] private AudioClip jumpClip;
    [SerializeField] private AudioClip interactClip;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    protected virtual void OnEnable()
    {
        if (jump != null) jump.action.started += Jump;
        if (interact != null) interact.action.started += Interact;
    }

    protected virtual void OnDisable()
    {
        if (jump != null) jump.action.started -= Jump;
        if (interact != null) interact.action.started -= Interact;
    }

    protected virtual void Update()
    {
        if (!canMove) return;
        _moveDirection = move.action.ReadValue<Vector2>();
        isGrounded = IsGrounded();
        Flip();
        if (animator != null)
        {
            animator.SetFloat("Speed", Mathf.Abs(_moveDirection.x));
            animator.SetBool("IsJumping", !isGrounded);
        }
    }

    protected virtual void FixedUpdate()
    {
        if (!canMove) return;
        rb.linearVelocity = new Vector2(_moveDirection.x * moveSpeed, rb.linearVelocity.y);
    }

    protected virtual void Jump(InputAction.CallbackContext obj)
    {
        if (!canMove) return;
        if (isGrounded && canJump)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpingPower);
        }
    }

    protected virtual void Interact(InputAction.CallbackContext context)
    {
        Vector2 direction = isFacingRight ? Vector2.right : Vector2.left;
        Vector2 origin = transform.position;
        RaycastHit2D hit = Physics2D.Raycast(origin, direction, interactDistance, LayerMask.GetMask("Interactables"));
        if (hit.collider != null)
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                interactable.Interact();
            }
        }
        Debug.DrawRay(origin, direction * interactDistance, Color.green, 0.5f);
    }

    protected virtual bool IsGrounded()
    {
        RaycastHit2D groundHit = Physics2D.BoxCast(
            groundCheckPoint.position,
            groundCheckSize,
            0f,
            Vector2.down,
            0.01f,
            groundLayer
        );
        if (groundHit.collider != null)
            return true;
        var playerHits = Physics2D.OverlapBoxAll(groundCheckPoint.position, groundCheckSize, 0f, playerLayer);
        foreach (var hit in playerHits)
        {
            if (hit.gameObject != this.gameObject)
                return true;
        }
        return false;
    }

    protected virtual void Flip()
    {
        if (isFacingRight && _moveDirection.x < 0f || !isFacingRight && _moveDirection.x > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    protected virtual void OnDrawGizmosSelected()
    {
        if (groundCheckPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(groundCheckPoint.position, groundCheckSize);
    }
}