using UnityEngine;

public class GroundingField : MonoBehaviour
{
    // Wywo�ywane przy wej�ciu w trigger uziemienia
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Core"))
        {
            // Pr�bujemy znale�� odpowiedni komponent
            var core = other.GetComponent<CoreController>();
            if (core != null)
            {
                // Wy��czenie si�y magnetycznej
                core.TurnOffMagnetize();
                // Je�li masz dodatkow� metod� do efektu roz�adowania, wywo�aj j� tutaj (np. animacja)
            }
        }
        if (other.CompareTag("Volt"))
        { 
            // Przyk�ad dla VoltController
            var volt = other.GetComponent<VoltController>();
            if (volt != null)
            {
                volt.TurnOffCharge();
                // Tutaj te� mo�esz doda� animacje/effect
            }
        }
    }
}
