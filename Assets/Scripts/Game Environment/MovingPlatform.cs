using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MovingPlatform : MonoBehaviour
{
    public enum PlatformMode
    {
        LoopBetweenPoints,        // Tryb 1: Ciągły ruch A<->B po aktywacji
        OneWayAndReturn,          // Tryb 2: Aktywacja -> do B, dezaktywacja -> do A
        DualPlayerActivate        // Tryb 3: Ruch jak w trybie 2, jeśli na platformie stoi 2 graczy
    }

    [Header("Punkty ruchu")]
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;

    [Header("Parametry ruchu")]
    [SerializeField, Min(0.01f)] private float speed = 2.5f;
    [SerializeField, Min(0.001f)] private float arriveEpsilon = 0.02f;

    [Header("Tryb pracy")]
    [SerializeField] private PlatformMode mode = PlatformMode.LoopBetweenPoints;
    [SerializeField] private bool isActivated = false;

    [Header("Opóźnienia")]
    [SerializeField, Min(0f)] private float loopEndpointDelay = 0.5f;
    [SerializeField, Min(0f)] private float dualDeactivateDelay = 0.75f;

    private float loopWaitTimer = 0f;
    private bool waitingAtEnd = false;

    private float dualWaitTimer = 0f;
    private bool dualDebouncedActivate = false;

    private Rigidbody2D rb;
    private Vector2 currentTarget;
    private bool movingToB = true;
    private int playersOnPlatform = 0;

    private Vector2 lastPos;
    public Vector2 Velocity { get; private set; }

    private void Reset()
    {
        //Automatycznie ustaw Kinetic i wyłącz grawitację
        rb = GetComponent<Rigidbody2D>();
        ConfigureRigidbody();
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        ConfigureRigidbody();

        if (!ValidatePoints()) return;
        InitializeLoopTarget();
        lastPos = rb.position;
    }

    private bool ValidatePoints()
    {
        if (pointA == null || pointB == null)
        {
            Debug.LogError($"{name}: Ustaw pointA i pointB.");
            enabled = false;
            return false;
        }
        if ((pointA.position - pointB.position).sqrMagnitude < 1e-6f)
            Debug.LogWarning($"{name}: pointA i pointB są bardzo blisko — ruch może być niewidoczny.");
        return true;
    }

    private void OnValidate()
    {
        if (Application.isPlaying) return;

        rb = GetComponent<Rigidbody2D>();
        if (rb) ConfigureRigidbody();
        if (pointA && pointB) currentTarget = pointB.position;
    }

    private void ConfigureRigidbody()
    {
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
    }

    private void InitializeLoopTarget()
    {
        movingToB = true;
        currentTarget = pointB.position;
    }

    private void FixedUpdate()
    {
        if (!enabled) return;

        switch (mode)
        {
            case PlatformMode.LoopBetweenPoints:
            { 
                if (!isActivated) break;
                LoopBetweenAandB();
                break;
            }

            case PlatformMode.OneWayAndReturn:
            {                
                MoveTowards(isActivated ? pointB.position : pointA.position);
                break;
            }

            case PlatformMode.DualPlayerActivate:
            {
                bool rawActive = playersOnPlatform >= 2;

                if (rawActive)
                {
                    dualDebouncedActivate = true;
                    dualWaitTimer = 0f;
                }
                else
                {
                    if (dualDebouncedActivate)
                    {
                        if (dualWaitTimer <= 0f)
                            dualWaitTimer = dualDeactivateDelay;

                        dualWaitTimer -= Time.fixedTime;

                        if (dualWaitTimer <= 0f)
                            dualDebouncedActivate = false;
                    }
                    else
                    {
                        dualWaitTimer = 0f;
                    }
                }
                MoveTowards(playersOnPlatform >= 2 ? pointB.position : pointA.position);
                break;
            }
        }

        var pos = rb.position;
        Velocity = (pos - lastPos) / Time.fixedDeltaTime;
        lastPos = pos;
    }

    private void LoopBetweenAandB()
    {
        if (waitingAtEnd)
        {
            loopWaitTimer -= Time.fixedDeltaTime;
            if (loopWaitTimer <= 0f)
            {
                waitingAtEnd = false;

                movingToB = !movingToB;
                currentTarget = movingToB ? pointB.position : pointA.position;
            }
            return;
        }

        MoveTowards(currentTarget);

        if (HasArrived(currentTarget))
        {
            if (loopEndpointDelay > 0f)
            {
                waitingAtEnd = true;
                loopWaitTimer = loopEndpointDelay;
            }
            else
            {
                movingToB = !movingToB;
                currentTarget = movingToB ? pointB.position : pointA.position;
            }
        }
    }

    private void MoveTowards(Vector2 target)
    {
        if (speed <= 0f) return;
        var newPos = Vector2.MoveTowards(rb.position, target, speed * Time.fixedDeltaTime);
        rb.MovePosition(newPos);   
    }

    private bool HasArrived(Vector2 target) =>
        Vector2.SqrMagnitude(rb.position - target) <= (arriveEpsilon * arriveEpsilon);

    public void Activate()      => isActivated = true; 
    public void Deactivate()    => isActivated = false;
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Volt") || collision.gameObject.CompareTag("Core"))
        {
            playersOnPlatform++;            
            collision.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Volt") || collision.gameObject.CompareTag("Core"))
        {
            playersOnPlatform = Mathf.Max(0, playersOnPlatform - 1);
            if (collision.transform.parent == transform)
                collision.transform.SetParent(null);
 
        }
    }

    private void OnDrawGizmos()
    {
        if (pointA && pointB)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(pointA.position, pointB.position);
            Gizmos.DrawWireSphere(pointA.position, 0.06f);
            Gizmos.DrawWireSphere(pointB.position, 0.06f);
        }
    }
}
