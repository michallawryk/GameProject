using UnityEngine;
using UnityEngine.InputSystem;

public class Player1Movement : MonoBehaviour
{
    [SerializeField] private float jumpingPower;
    [SerializeField] private float moveSpeed;    
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private InputActionReference move;
    [SerializeField] private InputActionReference jump;
    [SerializeField] private InputActionReference interact;

    private Vector2 _moveDirection;
    private bool isFacingRight = true;

    // Update is called once per frame
    void Update()
    {
        _moveDirection = move.action.ReadValue<Vector2>();

        Flip();
    }

    private void FixedUpdate()
    {
        //rb.linearVelocity = new Vector2(horizontal * speed, rb.linearVelocity.y);
        rb.linearVelocity = new Vector2(_moveDirection.x * moveSpeed, rb.linearVelocity.y);
    }

    private void OnEnable()
    {
        jump.action.started += Jump;
    }

    private void OnDisable()
    {
        jump.action.started -= Jump;
    }

    private void Jump(InputAction.CallbackContext obj)
    {
        //if (IsGrounded())
        //{  
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpingPower);
        //}
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
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
}
