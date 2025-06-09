using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class CoreController : PlayerControllerBase
{
    [SerializeField] public bool canMagnet { get; private set; } = false;
    [Header("Ikona")]
    [SerializeField] private GameObject magnetIcon;

    [Header("DŸwiek magnesu")]
    [SerializeField] private AudioSource magnetSound;

    private Coroutine magnetCoroutine;
    private MagneticBox grabbedBox = null;
    private bool isClimbing = false;

    protected override void Interact(InputAction.CallbackContext context)
    {
        if (grabbedBox != null)
        {
            // Przywróæ kolizjê z odpuszczan¹ skrzyni¹
            ReleaseBox();
            return;
        }

        if (isClimbing)
        {
            // jesteœ ju¿ na œcianie – puszczasz j¹
            StopClimbing();
            return;
        }

        if (TryGrabBox())
            return;

        if (TryClimbWall())
            return;

        base.Interact(context);
    }

    public void IgnoreCoreCollision(Collider2D coreCol, bool ignore)
    {
        Collider2D boxCol = GetComponent<Collider2D>();
        Physics2D.IgnoreCollision(coreCol, boxCol, ignore);
    }

    protected override void Update()
    {
        HandleMagnetizeAudio();

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

        // Tu mo¿esz dodaæ obs³ugê magnesu lub innych zdolnoœci Core
        if (isClimbing)
        {
            Vector2 input = move.action.ReadValue<Vector2>();

            // Tylko pionowy ruch na œcianie (mo¿esz ograniczyæ poziomy jeœli chcesz)
            rb.linearVelocity = new Vector2(0, input.y * moveSpeed);

            // (opcjonalnie: animacja wspinania)
        }
        else
        {
            // Normalne sterowanie
            // Ruch lewo/prawo: input.x
            // Skok na ground: input.y > 0 && isGrounded
            // ...
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
                // Zignoruj kolizjê z t¹ skrzyni¹
                Collider2D coreCol = GetComponent<Collider2D>();
                Collider2D boxCollider = grabbedBox.GetComponent<Collider2D>();
                if (coreCol != null && boxCollider != null)
                {
                    Physics2D.IgnoreCollision(coreCol, boxCollider, true);
                    grabbedBox.StartGrabbing(this);
                    canJump = false;
                    // (opcjonalnie) dŸwiêk, animacja itp.
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
            // (opcjonalnie: animacja, dŸwiêk)
            return true;
        }
        return false;
    }

    private void StopClimbing()
    {
        isClimbing = false;
        canJump = true;
        rb.gravityScale = 10f; // domyœlna grawitacja
                              // opcjonalnie: animacja/dŸwiêk
    }

    public void GetMagnetized(float time)
    {
        if (magnetCoroutine != null)
            StopCoroutine(magnetCoroutine);

        magnetCoroutine = StartCoroutine(DelayedMagnetize(time));
    }

    IEnumerator DelayedMagnetize(float delay)
    {
        canMagnet = true;

        yield return new WaitForSeconds(delay); // Czekaj okreœlony czas

        TurnOffMagnetize();
    }

    private void HandleMagnetizeAudio()
    {
        if (canMagnet)
        {
            if (!magnetSound.isPlaying)
            {
                magnetSound.Play();
            }
        }
        else
        {
            // Zatrzymaj tylko jeœli ten dŸwiêk gra
            if (magnetSound.isPlaying)
            {
                magnetSound.Stop();
            }
        }
    }

    public void TurnOffMagnetize()
    {
        canMagnet = false;
        StopClimbing();
        ReleaseBox();

        magnetCoroutine = null;
    }
}
