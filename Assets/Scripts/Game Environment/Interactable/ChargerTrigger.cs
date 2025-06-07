using UnityEngine;

public class ChargerTrigger: MonoBehaviour
{
    [SerializeField] private float chargedTime = 5f;
    [SerializeField] private bool isActive = true; // Flaga: czy ³adowarka dzia³a

    [SerializeField] private GameObject activeIcon;
    [SerializeField] private GameObject inactiveIcon;

    private VoltController volt; // Referencja do postaci Core


    public void Update()
    {
        if (activeIcon == null || inactiveIcon == null)
        {
            return;
        }
       
        if (isActive)
        {
            activeIcon.SetActive(true);
            inactiveIcon.SetActive(false);
        }
        else
        {
            inactiveIcon.SetActive(true);
            activeIcon.SetActive(false);
        }
    }

    // Funkcja wywo³ywana kiedy ktoœ jest W colliderze (wywo³ywana co frame)
    private void OnTriggerStay2D(Collider2D other)
    {
        if (!isActive) return; // <--- sprawdzamy aktywnoœæ

        if (other.CompareTag("Volt"))
        {
            if (volt == null)
                volt = other.GetComponent<VoltController>();

            if (volt != null)
                volt.GetCharge(chargedTime); // Wywo³uj co klatkê – np. odnawia efekt lub resetuje timer
        }
    }

    // Funkcja wywo³ywana kiedy ktoœ wychodzi z collidera
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Volt"))
        {
            if (volt != null)
            {
                volt = null;
            }
        }
    }

    public void Activate() { isActive = true; }
    public void Deactivate() { isActive = false; }
}