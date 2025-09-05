using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public sealed class VoltController : PlayerControllerBase
{
    [SerializeField] private bool hasDoubleJump = true;
    [SerializeField] private bool isCharged;
    public bool IsCharged => isCharged;

    [Header("Ikona")]
    [SerializeField] private GameObject boltIcon;

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
        canMove = false;
    }

    public void ExitPanelControl()
    {
        activePanel = null;
        canJump = true;
        canMove = true;
    }

    protected override void Interact(InputAction.CallbackContext context)
    {
        Vector2 direction = isFacingRight ? Vector2.right : Vector2.left;
        Vector2 origin = transform.position;
        RaycastHit2D hit = Physics2D.Raycast(origin, direction, interactDistance, LayerMask.GetMask("Interactables"));
        if (hit.collider != null)
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                interactable.Interact(this);
                return;
            }
        }
        base.Interact(context);
    }

    protected override void Update()
    {
        if (boltIcon != null)
        {
            if (isCharged)
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
        isCharged = true;

        yield return new WaitForSeconds(delay);

        TurnOffCharge();
    }

    public void TurnOffCharge()
    {
        isCharged = false;
        ExitPanelControl();

        chargeCoroutine = null;
    }
}
