using UnityEngine;

public class FanRotor : MonoBehaviour
{
    [Header("Ustawienia obrotu")]
    [SerializeField] private float rotationSpeed = 180f; // stopnie na sekundê
    [SerializeField] private bool isRotating = false;    // Czy aktualnie siê krêci
    [SerializeField] private bool rotateClockwise = false; // Kierunek: true = zgodnie z zegarem, false = przeciwnie

    // Funkcja do w³¹czenia obrotu
    public void ActivateRotation()
    {
        isRotating = true;
    }

    // Funkcja do wy³¹czenia obrotu
    public void DeactivateRotation()
    {
        isRotating = false;
    }

    private void Update()
    {
        if (isRotating)
        {
            float direction = rotateClockwise ? 1f : -1f;
            transform.Rotate(Vector3.forward, direction * rotationSpeed * Time.deltaTime);
        }
    }
}
