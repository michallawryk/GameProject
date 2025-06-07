using UnityEngine;
using UnityEngine.Audio;

public class MovingPlatform : MonoBehaviour
{
    public enum PlatformMode
    {
        LoopBetweenPoints,        // Tryb 1: Ciągły ruch A<->B po aktywacji
        OneWayAndReturn,          // Tryb 2: Aktywacja -> do B, dezaktywacja -> do A
        DualPlayerActivate        // Tryb 3: Ruch jak w trybie 1, jeśli na platformie stoi 2 graczy
    }

    [Header("Punkty ruchu")]
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;

    [Header("Parametry ruchu")]
    [SerializeField] private float speed = 2f;
    [SerializeField] private float waitTime = 0.5f; // czas pauzy na końcach (tylko tryb 1 i 3)

    [Header("Tryb pracy platformy")]
    [SerializeField] private PlatformMode mode = PlatformMode.LoopBetweenPoints;
    [SerializeField] private bool isActivated = false;

    [Header("Efenty dźwiękowe")]
    [SerializeField] private AudioClip moveSoundClip;
    private AudioSource audioSource;

    private Vector3 target;
    private bool movingToB = true;
    private float waitTimer = 0f;

    private int playersOnPlatform = 0;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = moveSoundClip;
    }
    void Start()
    {  
        transform.position = pointA.position;
        target = pointB.position;
    }

    void Update()
    { 
        switch (mode)
        {
            case PlatformMode.LoopBetweenPoints:
                if (isActivated)
                    MoveLoop();
                break;

            case PlatformMode.OneWayAndReturn:
                MoveOneWay();
                break;

            case PlatformMode.DualPlayerActivate:
                // NOWA LOGIKA: 2 graczy → do B, mniej niż 2 → wraca do A
                target = (playersOnPlatform >= 2) ? pointB.position : pointA.position;
                transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
                break;
        }
        // Logika audio - dźwięk tylko podczas ruchu
        bool shouldPlaySound = Vector3.Distance(transform.position, target) > 0.01f;
        if (shouldPlaySound)
        {
            if (!audioSource.isPlaying)
                audioSource.Play();
        }
        else
        {
            if (audioSource.isPlaying)
                audioSource.Pause();
        }
    }

    void MoveLoop()
    {
        if (waitTimer > 0)
        {
            waitTimer -= Time.deltaTime;
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target) < 0.01f)
        {
            // Zamiana kierunku po dotarciu do punktu
            movingToB = !movingToB;
            target = movingToB ? pointB.position : pointA.position;
            waitTimer = waitTime;
        }
    }

    void MoveOneWay()
    {
        if (isActivated)
            target = pointB.position;
        else
            target = pointA.position;

        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
    }

    // Parenting gracza do platformy, żeby przewoziła go poprawnie
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Volt") || collision.gameObject.CompareTag("Core"))
        {
            collision.transform.SetParent(transform);
            playersOnPlatform++;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Volt") || collision.gameObject.CompareTag("Core"))
        {
            collision.transform.SetParent(null);
            playersOnPlatform = Mathf.Max(0, playersOnPlatform - 1);
        }
    }

    // --- PODPINASZ TE METODY DO UNITYEVENTÓW (przyciski, panele, płytki) ---
    public void Activate() { isActivated = true; }
    public void Deactivate() { isActivated = false; }

    // Jeśli chcesz mieć podgląd na platformę w edytorze
    private void OnDrawGizmos()
    {
        if (pointA && pointB)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(pointA.position, pointB.position);
            Gizmos.DrawSphere(pointA.position, 0.1f);
            Gizmos.DrawSphere(pointB.position, 0.1f);
        }
    }

    //[SerializeField] private Transform pointA;
    //[SerializeField] private Transform pointB;
    //[SerializeField] private float speed = 2f;

    //[SerializeField] private bool isActivated = false;
    //private Vector3 target;
    //private bool movingToB = true;

    //void Start()
    //{
    //    target = pointB.position;
    //    transform.position = pointA.position;
    //}

    //void Update()
    //{
    //    if (isActivated)
    //        MovePlatform();
    //}

    //void MovePlatform()
    //{
    //    transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

    //    if (Vector3.Distance(transform.position, target) < 0.01f)
    //    {
    //        if (movingToB)
    //            target = pointA.position;
    //        else
    //            target = pointB.position;
    //        movingToB = !movingToB;
    //    }
    //}

    //// --- PODPINASZ TE METODY DO UNITYEVENTÓW ---
    //public void Activate() { isActivated = true; }
    //public void Deactivate() { isActivated = false; }
}
