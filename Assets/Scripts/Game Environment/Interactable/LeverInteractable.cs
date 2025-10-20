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
    [SerializeField] private Animator animator;

    [Header("Efenty dŸwiêkowe")]
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
            return; // DŸwigniê mo¿na przestawiæ tylko raz

        // Zmieniamy stan
        isOn = !isOn;

        if (oneUseOnly) used = true;

        Animate();
        audioSource.Play();
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
