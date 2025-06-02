using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider2D))]
public class PressurePlate : MonoBehaviour
{
    [Header("Warstwy aktywatorów")]
    [SerializeField] private LayerMask activatorLayers;

    [Header("Animacja")]
    [SerializeField] private Animator animator;  
    private static readonly int IsPressed = Animator.StringToHash("isPressed");

    [Header("Zdarzenia przy naciœniêciu")]
    public UnityEvent onActivate;

    [Header("Zdarzenia przy zwolnieniu")]
    public UnityEvent onDeactivate;

    private int _activatorCount = 0;

    private void Reset()
    {
        // upewnij siê, ¿e collider jest triggerem
        var bc = GetComponent<BoxCollider2D>();
        bc.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // sprawdzamy, czy warstwa obiektu nale¿y do activatorLayers
        if (((1 << other.gameObject.layer) & activatorLayers) != 0)
        {
            _activatorCount++;
            if (_activatorCount == 1)
            {
                if (animator != null) animator.SetBool(IsPressed, true);
                onActivate.Invoke();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & activatorLayers) != 0)
        {
            _activatorCount = Mathf.Max(0, _activatorCount - 1);
            if (_activatorCount == 0)
            {
                if (animator != null) animator.SetBool(IsPressed, false);
                onDeactivate.Invoke();
            }
                
        }
    }
}
