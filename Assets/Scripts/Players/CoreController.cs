using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public sealed class CoreController : PlayerControllerBase
{
    [SerializeField] private bool canMagnet;
    public bool CanMagnet => canMagnet;

    [Header("Ikona")]
    [SerializeField] private GameObject magnetIcon;

    private Coroutine magnetCoroutine;
    private MagneticBox grabbedBox = null;
    private bool isClimbing = false;

    protected override void Interact(InputAction.CallbackContext context)
    {
        if (grabbedBox != null)
        {
            ReleaseBox();
            return;
        }

        if (isClimbing)
        {
            StopClimbing();
            return;
        }

        if (TryGrabBox())
            return;

        if (TryClimbWall())
            return;

        base.Interact(context);
    }

    IEnumerator DelayedMagnetize(float delay)
    {
        canMagnet = true;

        yield return new WaitForSeconds(delay);

        TurnOffMagnetize();
    }

    public void GetMagnetized(float time)
    {
        if (magnetCoroutine != null)
            StopCoroutine(magnetCoroutine);

        magnetCoroutine = StartCoroutine(DelayedMagnetize(time));
    }
    public void TurnOffMagnetize()
    {
        canMagnet = false;
        StopClimbing();
        ReleaseBox();

        magnetCoroutine = null;
    }

    protected override void Update()
    {
        if (magnetIcon != null)
        {
            if (canMagnet)
            {
                magnetIcon.SetActive(true);
            }
            else
            {
                magnetIcon.SetActive(false);
            }
        }

        if (isClimbing)
        {
            Vector2 input = move.action.ReadValue<Vector2>();
            
            rb.linearVelocity = new Vector2(0, input.y * moveSpeed);
        }
        base.Update();
    }

    protected override void FixedUpdate()
    {
        if (isClimbing)
        {
            rb.linearVelocity = new Vector2(0, _moveDirection.y * 2f);
        }
        else
        {
            rb.linearVelocity = new Vector2(_moveDirection.x * moveSpeed, rb.linearVelocity.y);
        }
        base.FixedUpdate();

        if (grabbedBox != null)
        {
            grabbedBox.MoveTo(transform, isFacingRight);

            // Jeœli postaæ spada (velocity.y < -0.1) lub nie stoi na ziemi (nie jest isGrounded)
            if (rb.linearVelocity.y < -0.1f || !isGrounded)
            {
                ReleaseBox();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (isClimbing && other.CompareTag("MagneticWall"))
        {
            StopClimbing();
        }
        else if (grabbedBox != null && other.CompareTag("MagneticBox"))
        {
            ReleaseBox();
        }
    }

    private bool TryGrabBox()
    {
        Vector2 checkPos = (Vector2)transform.position + (isFacingRight ? Vector2.right : Vector2.left) * 0.8f;
        Collider2D boxCol = Physics2D.OverlapBox(checkPos, new Vector2(1.2f, 1f), 0f, LayerMask.GetMask("Box"));

        if (boxCol != null && boxCol.CompareTag("MagneticBox"))
        {
            grabbedBox = boxCol.GetComponent<MagneticBox>();
            if (grabbedBox != null)
            {
                Collider2D coreCol = GetComponent<Collider2D>();
                Collider2D boxCollider = grabbedBox.GetComponent<Collider2D>();
                if (coreCol != null && boxCollider != null)
                {
                    Physics2D.IgnoreCollision(coreCol, boxCollider, true);
                    grabbedBox.StartGrabbing(this);
                    canJump = false;
                    return true;
                }
            }
        }
        return false;
    }

    private void ReleaseBox()
    {
        Collider2D coreCol = GetComponent<Collider2D>();
        if (grabbedBox != null)
        {
            Collider2D boxCollider = grabbedBox.GetComponent<Collider2D>();
            if (coreCol != null && boxCollider != null)
            {
                Physics2D.IgnoreCollision(coreCol, boxCollider, false);
                grabbedBox.StopGrabbing();
            }
        }
        grabbedBox = null;
        canJump = true;
    }

    private bool TryClimbWall()
    {
        Vector2 checkPos = (Vector2)transform.position + (isFacingRight ? Vector2.right : Vector2.left) * 0.7f;
        Collider2D wallCol = Physics2D.OverlapBox(checkPos, new Vector2(0.8f, 2f), 0f, LayerMask.GetMask("Interactables"));
        if (wallCol != null && wallCol.CompareTag("MagneticWall"))
        {
            isClimbing = true;
            canJump = false;
            rb.gravityScale = 0f; // wy³¹cz grawitacjê
            rb.linearVelocity = Vector2.zero;
            return true;
        }
        return false;
    }

    private void StopClimbing()
    {
        isClimbing = false;
        canJump = true;
        rb.gravityScale = 10f; // domyœlna grawitacja
    }
}
