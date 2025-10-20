using UnityEngine;

public class MagneticBox : MonoBehaviour
{
    private Rigidbody2D rb;

    [SerializeField] private float moveSmoothness = 0.7f; // 0-1, sugerowane 0.7

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void StartGrabbing(CoreController core)
    {
        rb.bodyType = RigidbodyType2D.Kinematic; // kinematic – kontrolujesz ruchem w kodzie
    }

    public void StopGrabbing()
    {
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.linearVelocity = Vector2.zero;
    }
    public void MoveTo(Transform coreTransform, bool isFacingRight)
    {
        Vector2 boxSize = GetComponent<Collider2D>().bounds.size;
        Vector2 coreSize = coreTransform.GetComponent<Collider2D>().bounds.size;

        // Wyznacz docelow¹ pozycjê X - przy boku Core'a, a Y na podstawie obecnej pozycji skrzyni
        float sideSign = isFacingRight ? 1f : -1f;
        float xOffset = (coreSize.x / 2) + (boxSize.x / 2) + 0.05f; // minimalny odstêp

        Vector2 targetPos = new Vector2(
            coreTransform.position.x + sideSign * xOffset,
            transform.position.y // ZACHOWAJ aktualn¹ wysokoœæ
        );

        rb.MovePosition(Vector2.Lerp(rb.position, targetPos, moveSmoothness));
    }
}
