using UnityEngine;
using UnityEngine.Events;

public class LeverInteractable : MonoBehaviour, IInteractable
{
    [Header("Pocz¹tkowy stan dŸwigni")]
    [SerializeField] protected bool isOn = false;

    [Header("Czy dŸwigniê mo¿na przestawiæ tylko raz?")]
    [SerializeField] private bool oneUseOnly = false;

    [Header("Zdarzenia (akcje)")]
    public UnityEvent onLeverOn;     // Akcje po w³¹czeniu (stan ON)
    public UnityEvent onLeverOff;    // Akcje po wy³¹czeniu (stan OFF)

    [Header("Animacja")]
    [SerializeField] protected Animator animator;

    private bool _used = false;

    public virtual void Interact()
    {
        if (oneUseOnly && _used)
            return; // DŸwigniê mo¿na przestawiæ tylko raz

        // Zmieniamy stan
        isOn = !isOn;

        if (oneUseOnly) _used = true;

        Animate();
    }
    protected void Animate()
    {
        // Wywo³ujemy odpowiednie zdarzenie
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
