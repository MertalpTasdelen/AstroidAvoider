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

    public float nearMissThreshold = 1.5f;
    private bool nearMissTriggered = false;
    private bool inNearZone = false;
    private float nearZoneEnterTime = 0f;
    private float nearMissDurationThreshold = 0.3f;

    void Start()
    {
        spawnTime = Time.time;
        rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            direction = rb.linearVelocity.normalized;
        }

        player = GameObject.FindWithTag("Player")?.transform;

        if (useHoming && player != null)
        {
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            rb.linearVelocity = directionToPlayer * homingSpeed;
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
            Vector3 perp = Vector3.Cross(direction, Vector3.forward);
            transform.position += perp * wave * Time.deltaTime;
        }
        //With this astreoid, the homing is not working as expected. It is not following the player correctly.
        // if (useHoming && player != null)
        // {
        //     Vector3 toPlayer = (player.position - transform.position).normalized;
        //     Vector3 newVelocity = Vector3.Lerp(rb.linearVelocity.normalized, toPlayer, homingStrength * Time.deltaTime);
        //     rb.linearVelocity = newVelocity * rb.linearVelocity.magnitude;
        // }

        CheckNearMiss();
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
                Destroy(gameObject);
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
            Destroy(gameObject);
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

            Astreoid splitScript = newAsteroid.GetComponent<Astreoid>();
            if (splitScript != null)
            {
                splitScript.canSplit = false;
                splitScript.splitAsteroidPrefab = null;
                splitScript.useHoming = false;
                splitScript.useZigZag = false;
            }
        }
    }

    void CheckNearMiss()
    {
        if (player == null)
        {
            player = GameObject.FindWithTag("Player")?.transform;
            if (player == null) return;
        }

        if (nearMissTriggered || Time.time - spawnTime < 1f) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance < nearMissThreshold && !inNearZone)
        {
            inNearZone = true;
            nearZoneEnterTime = Time.time;
        }

        if (inNearZone && distance > nearMissThreshold)
        {
            float timeSpentClose = Time.time - nearZoneEnterTime;

            if (timeSpentClose <= nearMissDurationThreshold && !nearMissTriggered)
            {
                nearMissTriggered = true;
                ScoreSystem.Instance?.AddAvoidBonus(10);
                NearMissFeedbackSystem.Instance?.TriggerNearMissFeedback();
                // StartCoroutine(TemporarySlowMotion());
            }

            inNearZone = false;
        }
    }

    private System.Collections.IEnumerator TemporarySlowMotion()
    {
        Time.timeScale = 0.3f;
        yield return new WaitForSecondsRealtime(0.15f);
        Time.timeScale = 1f;
    }
}
