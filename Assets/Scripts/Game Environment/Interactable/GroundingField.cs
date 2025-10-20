using UnityEngine;

public class GroundingField : MonoBehaviour
{
    // Wywo³ywane przy wejœciu w trigger uziemienia
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Core"))
        {
            // Próbujemy znaleŸæ odpowiedni komponent
            var core = other.GetComponent<CoreController>();
            if (core != null)
            {
                // Wy³¹czenie si³y magnetycznej
                core.TurnOffMagnetize();
                // Jeœli masz dodatkow¹ metodê do efektu roz³adowania, wywo³aj j¹ tutaj (np. animacja)
            }
        }
        if (other.CompareTag("Volt"))
        { 
            // Przyk³ad dla VoltController
            var volt = other.GetComponent<VoltController>();
            if (volt != null)
            {
                volt.TurnOffCharge();
                // Tutaj te¿ mo¿esz dodaæ animacje/effect
            }
        }
    }
}
