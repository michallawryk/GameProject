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
    [SerializeField] private Animator animator;

    [Header("Efenty d�wi�kowe")]
    [SerializeField] private AudioSource audioSource;

    private bool used = false;


    protected virtual void Awake()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }
    public virtual void Interact(VoltController player)
    {
        if (oneUseOnly && used)
            return; // D�wigni� mo�na przestawi� tylko raz

        // Zmieniamy stan
        isOn = !isOn;

        if (oneUseOnly) used = true;

        Animate();
        audioSource.Play();
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
