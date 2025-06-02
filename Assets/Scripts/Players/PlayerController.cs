using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Ruch i skok")]
    [SerializeField] private float jumpingPower = 10f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private LayerMask groundLayer;

    [Header("Wej�cia gracza")]
    [SerializeField] private InputActionReference move;
    [SerializeField] private InputActionReference jump;
    [SerializeField] private InputActionReference interact;

    [Header("Specjalne zdolno�ci")]
    [SerializeField] private bool hasDoubleJump = false;
    [SerializeField] private bool canInteract = false;

    private Rigidbody2D rb;
    private Vector2 _moveDirection;
    private float groundCheckRadius = 0.2f;
    private bool isFacingRight = true;
    private bool isGrounded;
    private bool hasUsedDoubleJump;

    [SerializeField] private bool canMove = true;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        jump.action.started += Jump;
        interact.action.started += Interact;
    }

    private void OnDisable()
    {
        jump.action.started -= Jump;
        interact.action.started -= Interact;
    }

    private void Update()
    {
        if (!canMove) return;

        _moveDirection = move.action.ReadValue<Vector2>();
        isGrounded = IsGrounded();

        Flip();
    }

    private void FixedUpdate()
    {
        if (!canMove) return;

        rb.linearVelocity = new Vector2(_moveDirection.x * moveSpeed, rb.linearVelocity.y);
    }

    private void Jump(InputAction.CallbackContext obj)
    {
        if (!canMove) return;

        if (isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpingPower);
            hasUsedDoubleJump = false;
        }
        else if (hasDoubleJump && !hasUsedDoubleJump)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpingPower);
            hasUsedDoubleJump = true;
        }
    }

    private void Interact(InputAction.CallbackContext context)
    {
        if (!canInteract) return;

        Vector2 direction = isFacingRight ? Vector2.right : Vector2.left;
        Vector2 origin = transform.position;
        float interactDistance = 1.5f;

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

    private bool IsGrounded()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheckPoint.position, groundCheckRadius, groundLayer);

        foreach (var col in colliders)
        {
            if (col.gameObject == gameObject) continue;

            // Obiekt musi być niżej niż punkt gracza
            if ((col.CompareTag("Ground") || (col.CompareTag("Volt") || col.CompareTag("Core")) || col.CompareTag("Box")) &&
                col.transform.position.y < transform.position.y - 0.1f)
            {
                return true;
            }
        }

        return false;
    }

    private void Flip()
    {
        if (isFacingRight && _moveDirection.x < 0f || !isFacingRight && _moveDirection.x > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheckPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheckPoint.position, groundCheckRadius);
    }
}
