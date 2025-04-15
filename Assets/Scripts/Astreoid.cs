using UnityEngine;

public class Astreoid : MonoBehaviour
{

    public bool useZigZag = false;
    public float zigzagFrequency = 5f;
    public float zigzagMagnitude = 1f;

    public bool useHoming = false;
    public float homingStrength = 2f;

    private Transform player;
    private Rigidbody rb;


    private Vector3 direction;
    private float spawnTime;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = GameObject.FindWithTag("Player")?.transform;

        spawnTime = Time.time;

        if (rb != null)
        {
            direction = rb.linearVelocity.normalized;
        }

        // if (useHoming)
        // {
        //     PlayerMovement player = Object.FindFirstObjectByType<PlayerMovement>();
        //     if (player != null)
        //     {
        //         playerTransform = player.transform;
        //     }
        // }
    }

    void Update()
    {
        if (useZigZag)
        {
            float wave = Mathf.Sin((Time.time - spawnTime) * zigzagFrequency) * zigzagMagnitude;
            Vector3 perp = Vector3.Cross(direction, Vector3.forward); // dik açı

            transform.position += perp * wave * Time.deltaTime;
        }

        if (useHoming && player != null)
        {
            Vector3 toPlayer = (player.position - transform.position).normalized;
            Vector3 newVelocity = Vector3.Lerp(rb.linearVelocity.normalized, toPlayer, homingStrength * Time.deltaTime);
            rb.linearVelocity = newVelocity * rb.linearVelocity.magnitude; // mevcut hız korunur ama yön değişir
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth == null)
        {
            return;
        }

        // Yeni Unity API'si ile performans takibini bildir
        PlayerPerformanceTracker tracker = Object.FindFirstObjectByType<PlayerPerformanceTracker>();
        if (tracker != null)
        {
            tracker.RegisterHit();
        }

        playerHealth.Crash();
    }

    private void OnBecameInvisible()
    {
        PlayerPerformanceTracker tracker = Object.FindFirstObjectByType<PlayerPerformanceTracker>();
        if (tracker != null)
        {
            tracker.RegisterAsteroidAvoided();
        }

        Destroy(gameObject);
    }
}
