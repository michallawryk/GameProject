using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider2D))]
public class PressurePlate : MonoBehaviour
{
    [Header("Warstwy aktywatorów")]
    [SerializeField] private LayerMask activatorLayers;

    [Header("Animacja")]
    [SerializeField] private Animator animator;

    [Header("Efenty dŸwiêkowe")]
    [SerializeField] private AudioClip pressSoundClip;
    [SerializeField] private AudioClip releaseSoundClip;
    private AudioSource audioSource;

    [Header("Zdarzenia przy naciœniêciu")]
    public UnityEvent onActivate;

    [Header("Zdarzenia przy zwolnieniu")]
    public UnityEvent onDeactivate;

    [SerializeField] private bool singleUse = false;

    private bool wasActivatedOnce = false;
    private int _activatorCount = 0;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Reset()
    {
        // upewnij siê, ¿e collider jest triggerem
        var bc = GetComponent<BoxCollider2D>();
        bc.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & activatorLayers) != 0)
        {
            if (singleUse && wasActivatedOnce)
                return;

            _activatorCount++;
            if (_activatorCount == 1)
            {
                if (animator != null) animator.SetBool("isPressed", true);

                audioSource.clip = pressSoundClip;
                audioSource.Play();
                
                onActivate.Invoke();
                if (singleUse)
                {
                    wasActivatedOnce = true;
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & activatorLayers) != 0)
        {
            if (singleUse && wasActivatedOnce)
                return;

            _activatorCount = Mathf.Max(0, _activatorCount - 1);
            if (_activatorCount == 0)
            {
                if (animator != null) animator.SetBool("isPressed", false);
                audioSource.clip = releaseSoundClip;
                audioSource.Play();
                onDeactivate.Invoke();
            }
                
        }
    }
}
