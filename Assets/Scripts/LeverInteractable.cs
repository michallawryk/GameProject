using UnityEngine;
using UnityEngine.Events;

public class LeverInteractable : MonoBehaviour, IInteractable
{
    [Header("Pocz�tkowy stan d�wigni")]
    [SerializeField] protected bool isOn = false;

    [Header("Czy d�wigni� mo�na przestawi� tylko raz?")]
    [SerializeField] private bool oneUseOnly = false;

    [Header("Zdarzenia (akcje)")]
    public UnityEvent onLeverOn;     // Akcje po w��czeniu (stan ON)
    public UnityEvent onLeverOff;    // Akcje po wy��czeniu (stan OFF)

    [Header("Animacja")]
    [SerializeField] protected Animator animator;

    private bool _used = false;

    public virtual void Interact()
    {
        if (oneUseOnly && _used)
            return; // D�wigni� mo�na przestawi� tylko raz

        // Zmieniamy stan
        isOn = !isOn;

        if (oneUseOnly) _used = true;

        Animate();
    }
    protected void Animate()
    {
        // Wywo�ujemy odpowiednie zdarzenie
        if (isOn)
        {
            if (animator != null) animator.SetBool("IsOn", true);
            onLeverOn?.Invoke();
        }
        else
        {
            if (animator != null) animator.SetBool("IsOn", false);
            onLeverOff?.Invoke();
        }
    }
}
