using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class VoltController : PlayerControllerBase
{
    [SerializeField] private bool hasDoubleJump = true;
    [SerializeField] public bool IsCharged { get; private set; } = false;

    [Header("Ikona")]
    [SerializeField] private GameObject boltIcon;

    [Header("DŸwiek pr¹du")]
    [SerializeField] private AudioSource boltSound;

    private Coroutine chargeCoroutine;
    private ControlPanel activePanel = null;
    private bool hasUsedDoubleJump = false;

    protected override void Jump(InputAction.CallbackContext obj)
    {
        if (!canMove) return;
        if (isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpingPower);
            hasUsedDoubleJump = false;
        }
        else if (hasDoubleJump && !hasUsedDoubleJump)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpingPower * 0.75f);
            hasUsedDoubleJump = true;
        }
    }

    public void EnterPanelControl(ControlPanel panel)
    {
        activePanel = panel;
        canJump = false;
        canMove = false; // wy³¹cz sterowanie postaci¹
                         // (opcjonalnie: animacja, dŸwiêk, zmiana kamery)
    }

    public void ExitPanelControl()
    {
        activePanel = null;
        canJump = true;
        canMove = true; // wróæ do normalnego ruchu
                        // (opcjonalnie: reset UI/kamery)
    }

    protected override void Interact(InputAction.CallbackContext context)
    {
        if (!IsCharged)
        {
            base.Interact(context);
            return;
        }

        Vector2 direction = isFacingRight ? Vector2.right : Vector2.left;
        Vector2 origin = transform.position;
        RaycastHit2D hit = Physics2D.Raycast(origin, direction, interactDistance, LayerMask.GetMask("Interactables"));
        if (hit.collider != null)
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                interactable.Interact(this);
            }
        }
        Debug.DrawRay(origin, direction * interactDistance, Color.green, 0.5f);
    }

    protected override void Update()
    {
        if (boltIcon != null)
        {
            if (IsCharged)
            {
                boltIcon.SetActive(true);
            }
            else
            {
                boltIcon.SetActive(false);
            }
        }

        if (activePanel != null)
        {
            // Przekazuj input do panelu
            Vector2 input = move.action.ReadValue<Vector2>();
            activePanel.SetPlatformInput(input);
        }
        else
        {
            base.Update();
        }
    }

    public void GetCharge(float time)
    {
        if (chargeCoroutine != null)
            StopCoroutine(chargeCoroutine);

        chargeCoroutine = StartCoroutine(DelayCharge(time));
    }

    IEnumerator DelayCharge(float delay)
    {
        IsCharged = true;

        yield return new WaitForSeconds(delay);

        IsCharged = false;
        ExitPanelControl();
    }
}
