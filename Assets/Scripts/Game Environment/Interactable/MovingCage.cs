using UnityEngine;

public class MovingCage : MonoBehaviour
{
    [Header("Punkty ruchu")]
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;

    [Header("Gniazda wymagane do aktywacji")]
    [SerializeField] private ElectricSocket[] requiredSockets;

    [Header("Prêdkoœæ przesuwania")]
    [SerializeField] private float moveSpeed = 2f;

    private bool allSocketsActive = false;
    private bool isMoving = false;

    private void Update()
    {
        // Sprawdzaj w ka¿dej klatce czy wszystkie gniazda s¹ aktywowane
        allSocketsActive = true;
        foreach (var socket in requiredSockets)
        {
            if (socket == null || !socket.IsPowered)
            {
                allSocketsActive = false;
                break;
            }
        }

        // Jeœli wszystkie aktywne i jeszcze nie rozpoczêto ruchu – zacznij przesuwaæ
        if (allSocketsActive && !isMoving)
        {
            isMoving = true;
        }

        // Przesuwaj klatkê z A do B, gdy aktywowane
        if (isMoving)
        {
            // Lerp – przesuwa obiekt p³ynnie z obecnej pozycji do punktu B
            transform.position = Vector3.MoveTowards(transform.position, pointB.position, moveSpeed * Time.deltaTime);

            // Jeœli dotarliœmy do celu – mo¿esz dodaæ dodatkowe zdarzenie
            if (Vector3.Distance(transform.position, pointB.position) < 0.01f)
            {
                transform.position = pointB.position;
                isMoving = false;
            }
        }
    }

    // (opcjonalnie) metoda, ¿eby ustawiæ pozycjê startow¹ na A w edytorze
    public void ResetToStart()
    {
        transform.position = pointA.position;
        isMoving = false;
    }
}
