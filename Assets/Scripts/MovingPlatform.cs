using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float speed = 2f;

    private bool isActivated = false;
    private Vector3 target;
    private bool movingToB = true;

    void Start()
    {
        target = pointB.position;
        transform.position = pointA.position;
    }

    void Update()
    {
        if (isActivated)
            MovePlatform();
    }

    void MovePlatform()
    {
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target) < 0.01f)
        {
            if (movingToB)
                target = pointA.position;
            else
                target = pointB.position;
            movingToB = !movingToB;
        }
    }

    // --- PODPINASZ TE METODY DO UNITYEVENTÓW ---
    public void Activate() { isActivated = true; }
    public void Deactivate() { isActivated = false; }
}
