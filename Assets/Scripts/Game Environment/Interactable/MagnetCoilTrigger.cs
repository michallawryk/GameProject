using UnityEngine;

public class MagnetCoilTrigger : MonoBehaviour
{
    [SerializeField] private float magnetizeTime = 5f;
    [SerializeField] private bool isActive = true; // Flaga: czy �adowarka dzia�a

    [SerializeField] private GameObject activeIcon;
    [SerializeField] private GameObject inactiveIcon;

    private CoreController core; // Referencja do postaci Core


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
    // Funkcja wywo�ywana kiedy kto� jest W colliderze (wywo�ywana co frame)
    private void OnTriggerStay2D(Collider2D other)
    {
        if (!isActive) return; // <--- sprawdzamy aktywno��

        if (other.CompareTag("Core"))
        {
            if (core == null)
                core = other.GetComponent<CoreController>();

            if (core != null)
                core.GetMagnetized(magnetizeTime); // Wywo�uj co klatk� � np. odnawia efekt lub resetuje timer
        }
    }

    // Funkcja wywo�ywana kiedy kto� wychodzi z collidera
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Core"))
        {
            if (core != null)
            {
                core = null;
            }
        }
    }
    public void Activate() { isActive = true; }
    public void Deactivate() { isActive = false; }
}
