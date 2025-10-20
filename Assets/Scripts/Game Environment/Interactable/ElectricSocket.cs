using UnityEngine;
using UnityEngine.Events;

public class ElectricSocket : MonoBehaviour
{
    [Header("Zdarzenia gniazda")]
    public UnityEvent OnPowered;
    public UnityEvent OnUnpowered;

    private bool isPowered = false;

    // Przechowuj referencj� do obiektu, kt�ry zasila gniazdo (Volt lub ElectricBox)
    private GameObject currentSource;

    private void Update()
    {
        // Sprawdzanie, czy Volt nadal zasila gniazdo
        if (isPowered && currentSource != null)
        {
            VoltController volt = currentSource.GetComponent<VoltController>();
            if (volt != null && !volt.IsCharged)
            {
                Disconnect();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Wej�cie Volta
        VoltController volt = other.GetComponent<VoltController>();
        if (volt != null && volt.IsCharged && !isPowered)
        {
            isPowered = true;
            currentSource = volt.gameObject;
            OnPowered?.Invoke();
            return;
        }

        // Wej�cie ElectricBoxa
        if (other.CompareTag("ElectricBox") && !isPowered)
        {
            isPowered = true;
            currentSource = other.gameObject;
            OnPowered?.Invoke();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Wyj�cie Volta
        VoltController volt = other.GetComponent<VoltController>();
        if (volt != null && currentSource == volt.gameObject && isPowered)
        {
            Disconnect();
            return;
        }

        // Wyj�cie ElectricBoxa
        if (other.CompareTag("ElectricBox") && currentSource == other.gameObject && isPowered)
        {
            Disconnect();
        }
    }

    private void Disconnect()
    {
        isPowered = false;
        currentSource = null;
        OnUnpowered?.Invoke();
    }

    public void Activate()
    {
        isPowered = true;
    }
    public void Deactivate()
    {
        isPowered = false;
    }

    public bool IsPowered => isPowered;
}
