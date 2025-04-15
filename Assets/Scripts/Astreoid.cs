using UnityEngine;

public class Astreoid : MonoBehaviour
{

    public bool useZigZag = false;
    public float zigzagFrequency = 5f;
    public float zigzagMagnitude = 1f;


    private Vector3 direction;
    private float spawnTime;

    void Start()
    {
        spawnTime = Time.time;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            direction = rb.linearVelocity.normalized;
        }
    }

    void Update()
    {
        if (useZigZag)
        {
            float wave = Mathf.Sin((Time.time - spawnTime) * zigzagFrequency) * zigzagMagnitude;
            Vector3 perp = Vector3.Cross(direction, Vector3.forward); // dik açı

            transform.position += perp * wave * Time.deltaTime;
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
