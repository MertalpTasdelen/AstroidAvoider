using UnityEngine;

public class Astreoid : MonoBehaviour
{

    public bool useZigZag = false;
    public float zigzagFrequency = 5f;
    public float zigzagMagnitude = 1f;

    public bool useHoming = false;
    public float homingStrength = 2f;
    public float homingSpeed = 4f;

    private Transform player;
    private Rigidbody rb;


    private Vector3 direction;
    private float spawnTime;

    public bool canSplit = true;
    public float splitDelay = 2f;
    public GameObject splitAsteroidPrefab;

    private bool hasSplit = false;

    void Start()
    {
        spawnTime = Time.time;


        rb = GetComponent<Rigidbody>();

        spawnTime = Time.time;

        if (rb != null)
        {
            direction = rb.linearVelocity.normalized;
        }

        if (useHoming)
        {
            Transform player = GameObject.FindWithTag("Player")?.transform;
            if (player != null)
            {
                Vector3 directionToPlayer = (player.position - transform.position).normalized;
                rb.linearVelocity = directionToPlayer * homingSpeed;
            }
        }
    }

    void Update()
    {
        if (canSplit && !hasSplit && Time.time - spawnTime > splitDelay)
        {
            hasSplit = true;
            SpawnSplitAsteroids();
            Destroy(gameObject);
        }

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
        if (playerHealth != null)
        {
            PlayerPerformanceTracker tracker = Object.FindFirstObjectByType<PlayerPerformanceTracker>();
            if (tracker != null)
            {
                tracker.RegisterHit();
            }

            if (canSplit && !hasSplit && Time.time - spawnTime > splitDelay)
            {
                hasSplit = true;
                SpawnSplitAsteroids();
                Destroy(gameObject); // sadece Update’te çağrıyorsan
                Debug.Log("Asteroid split!");
            }

            playerHealth.Crash();
            Destroy(gameObject);
        }
    }

    private void OnBecameInvisible()
    {
        if (canSplit && !hasSplit && Time.time - spawnTime > splitDelay)
        {
            hasSplit = true;
            SpawnSplitAsteroids();
            Destroy(gameObject); // sadece Update’te çağrıyorsan
        }

        PlayerPerformanceTracker tracker = Object.FindFirstObjectByType<PlayerPerformanceTracker>();
        if (tracker != null)
        {
            tracker.RegisterAsteroidAvoided();
        }

        Destroy(gameObject);
    }

    private void SpawnSplitAsteroids()
    {
        if (splitAsteroidPrefab == null) return;

        for (int i = 0; i < 2; i++)
        {
            GameObject newAsteroid = Instantiate(
                splitAsteroidPrefab,
                transform.position,
                Quaternion.identity);

            Rigidbody rb = newAsteroid.GetComponent<Rigidbody>();
            Vector3 randomDirection = Random.insideUnitCircle.normalized;
            float splitForce = Random.Range(2f, 4f);
            rb.linearVelocity = randomDirection * splitForce;

            // 🛡️ Bu asteroidler tekrar bölünmemeli!
            Astreoid splitScript = newAsteroid.GetComponent<Astreoid>();
            if (splitScript != null)
            {
                splitScript.canSplit = false; // ❗️Bir daha bölünmesin
                splitScript.splitAsteroidPrefab = null;
                splitScript.useHoming = false;
                splitScript.useZigZag = false;
            }
        }
    }
}
