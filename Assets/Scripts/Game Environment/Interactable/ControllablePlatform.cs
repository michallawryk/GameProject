using UnityEngine;

public class ControllablePlatform: MonoBehaviour
{
    [Header("Limity ruchu")]
    [SerializeField] private Transform limitPointA;
    [SerializeField] private Transform limitPointB;

    [Header("Parametry")]
    [SerializeField] private float moveSpeed = 4f;

    // Bie¿¹cy input (ustawiany przez panel)
    private Vector2 input;

    void Update()
    {
        // Przesuwaj tylko jeœli dostajesz input (czyli gracz steruje)
        if (input != Vector2.zero)
        {
            MovePlatform(input);
        }
    }

    // Wywo³ywana przez panel sterowania
    public void SetMoveInput(Vector2 moveInput)
    {
        input = moveInput;
    }

    // Mo¿esz wywo³aæ, by zatrzymaæ platformê (np. przy wyjœciu gracza z panelu)
    public void StopPlatform()
    {
        input = Vector2.zero;
    }

    private void MovePlatform(Vector2 moveInput)
    {
        Vector3 target = transform.position + (Vector3)(moveInput * moveSpeed * Time.deltaTime);

        // Wyznacz min/max X/Y na podstawie dwóch punktów
        float minX = Mathf.Min(limitPointA.position.x, limitPointB.position.x);
        float maxX = Mathf.Max(limitPointA.position.x, limitPointB.position.x);
        float minY = Mathf.Min(limitPointA.position.y, limitPointB.position.y);
        float maxY = Mathf.Max(limitPointA.position.y, limitPointB.position.y);

        // Clamp
        target.x = Mathf.Clamp(target.x, minX, maxX);
        target.y = Mathf.Clamp(target.y, minY, maxY);

        transform.position = target;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("MagneticBox") || collision.gameObject.CompareTag("ElectricBox") || collision.gameObject.CompareTag("Box") || collision.gameObject.CompareTag("Core"))
        {
            collision.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("MagneticBox") || collision.gameObject.CompareTag("ElectricBox") || collision.gameObject.CompareTag("Box") || collision.gameObject.CompareTag("Core"))
        {
            collision.transform.SetParent(null);
        }
    }

    // (Opcjonalnie) wizualizacja obszaru w edytorze
    private void OnDrawGizmosSelected()
    {
        if (limitPointA != null && limitPointB != null)
        {
            Gizmos.color = Color.cyan;
            Vector3 p1 = limitPointA.position;
            Vector3 p2 = limitPointB.position;
            Gizmos.DrawWireCube((p1 + p2) / 2, new Vector3(Mathf.Abs(p1.x - p2.x), Mathf.Abs(p1.y - p2.y), 0.1f));
        }
    }
}