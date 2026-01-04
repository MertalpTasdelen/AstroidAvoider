using UnityEngine;

public class Astreoid : MonoBehaviour
{
    [HideInInspector] public GameObject prefabReference;
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

    private float targetSpeed;

    public bool canSplit = true;
    public float splitDelay = 2f;
    public GameObject splitAsteroidPrefab;

    private bool hasSplit = false;

    public float nearMissThreshold = 1.5f;
    private bool nearMissTriggered = false;
    private bool inNearZone = false;
    private float nearZoneEnterTime = 0f;
    private float nearMissDurationThreshold = 0.3f;

    // kaçınma istatistiği
    private bool avoidanceRegistered = false;
    private bool hasPassedPlayer = false;
    private Vector3 initialPlayerPosition;

    [SerializeField] private GameObject explosionPrefab;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // 2D kilitler
        rb.constraints = RigidbodyConstraints.FreezePositionZ
                       | RigidbodyConstraints.FreezeRotationX
                       | RigidbodyConstraints.FreezeRotationY;

        // Hızlı asteroidlerde çarpışma kaçırmayı azalt
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    void Start()
    {
        spawnTime = Time.time;

        // ZigZag behavior has been removed (was not fun / too random).
        useZigZag = false;

        if (rb != null)
        {
            direction = rb.linearVelocity.sqrMagnitude > 0.0001f
                ? rb.linearVelocity.normalized
                : transform.right; // emniyet

            targetSpeed = rb.linearVelocity.magnitude;
        }

        player = GameObject.FindWithTag("Player")?.transform;

        if (player != null)
            initialPlayerPosition = player.position;

        // Homing: fire-and-forget. Only aim once, at spawn time, to the player's current position.
        if (useHoming)
        {
            if (player == null)
                player = GameObject.FindWithTag("Player")?.transform;

            if (rb != null && player != null)
            {
                Vector3 toPlayer = (player.position - transform.position);
                toPlayer.z = 0f;
                if (toPlayer.sqrMagnitude > 0.0001f)
                {
                    direction = toPlayer.normalized;
                    targetSpeed = homingSpeed > 0.01f ? homingSpeed : Mathf.Max(0.1f, targetSpeed);
                    rb.linearVelocity = direction * targetSpeed;
                }
            }
        }
    }

    void Update()
    {
        if (canSplit && !hasSplit && Time.time - spawnTime > splitDelay)
        {
            hasSplit = true;
            SpawnSplitAsteroids();
            AstreoidPool.Instance?.Recycle(prefabReference, gameObject);
            return;
        }

        if (rb != null)
        {
            UpdateMovement();
        }

        CheckNearMiss();
        CheckAsteroidAvoidance();
    }

    private void UpdateMovement()
    {
        // Keep an updated baseline speed.
        float currentSpeed = rb.linearVelocity.magnitude;
        if (currentSpeed > 0.01f)
            targetSpeed = currentSpeed;

        // ZigZag removed.
    }

    // --- OYUNCU ÇARPIŞMASINI HEM TRIGGER HEM COLLISION'DA YAKALA ---

    private void OnTriggerEnter(Collider other)
    {
        var playerHealth = other.GetComponentInParent<PlayerHealth>();
        if (playerHealth != null)
        {
            HandlePlayerHit(playerHealth);
            return;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Önce oyuncu mu diye bak
        var playerHealth = collision.collider.GetComponentInParent<PlayerHealth>();
        if (playerHealth != null)
        {
            HandlePlayerHit(playerHealth);
            return;
        }

        // değilse asteroid-asteroid sekmesi uygula
        if (!collision.gameObject.CompareTag("Asteroid")) return;

        Rigidbody otherRb = collision.rigidbody;
        if (otherRb == null || rb == null) return;

        Vector3 thisPos = transform.position;
        Vector3 otherPos = otherRb.transform.position;
        Vector3 collisionVector = thisPos - otherPos;
        float distanceSq = collisionVector.sqrMagnitude;
        if (distanceSq < 0.0001f) return;

        Vector3 v1 = rb.linearVelocity;
        Vector3 v2 = otherRb.linearVelocity;

        float dot1 = Vector3.Dot(v1 - v2, collisionVector);
        Vector3 newV1 = v1 - (dot1 / distanceSq) * collisionVector;
        float dot2 = Vector3.Dot(v2 - v1, -collisionVector);
        Vector3 newV2 = v2 - (dot2 / distanceSq) * (-collisionVector);

        rb.linearVelocity = newV1;
        otherRb.linearVelocity = newV2;
    }

    private void HandlePlayerHit(PlayerHealth playerHealth)
    {
        // kaçınma sayımını iptal et
        avoidanceRegistered = true;

        var tracker = Object.FindFirstObjectByType<PlayerPerformanceTracker>();
        if (tracker != null) tracker.RegisterHit();

        // split süresi geldiyse önce böl
        if (canSplit && !hasSplit && Time.time - spawnTime > splitDelay)
        {
            hasSplit = true;
            SpawnSplitAsteroids();
            AstreoidPool.Instance?.Recycle(prefabReference, gameObject);
        }
        else
        {
            AstreoidPool.Instance?.Recycle(prefabReference, gameObject);
        }

        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }

        AstreoidPool.Instance?.Recycle(prefabReference, gameObject);
        playerHealth.Crash();
    }

    private void OnBecameInvisible()
    {
        if (canSplit && !hasSplit && Time.time - spawnTime > splitDelay)
        {
            hasSplit = true;
            SpawnSplitAsteroids();
            AstreoidPool.Instance?.Recycle(prefabReference, gameObject);
            return;
        }

        if (!avoidanceRegistered)
            RegisterAvoidance();

        AstreoidPool.Instance?.Recycle(prefabReference, gameObject);
    }

    private void SpawnSplitAsteroids()
    {
        if (splitAsteroidPrefab == null) return;

        for (int i = 0; i < 2; i++)
        {
            GameObject newAsteroid = AstreoidPool.Instance != null
                ? AstreoidPool.Instance.Get(splitAsteroidPrefab, transform.position, Quaternion.identity)
                : Instantiate(splitAsteroidPrefab, transform.position, Quaternion.identity);

            Rigidbody rb2 = newAsteroid.GetComponent<Rigidbody>();
            Vector3 randomDirection = Random.insideUnitCircle.normalized;
            float splitForce = Random.Range(2f, 4f);
            rb2.linearVelocity = randomDirection * splitForce;

            Astreoid splitScript = newAsteroid.GetComponent<Astreoid>();
            if (splitScript != null)
            {
                splitScript.canSplit = false;
                splitScript.splitAsteroidPrefab = null;
                splitScript.useHoming = false;
                splitScript.useZigZag = false;
                splitScript.prefabReference = splitAsteroidPrefab;

                // güvenli çarpışma için bunları da ayarlıyoruz
                var rbSplit = newAsteroid.GetComponent<Rigidbody>();
                if (rbSplit != null)
                {
                    rbSplit.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                    rbSplit.interpolation = RigidbodyInterpolation.Interpolate;
                }
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

                var tracker = Object.FindFirstObjectByType<PlayerPerformanceTracker>();
                if (tracker != null) tracker.RegisterNearMiss();
            }

            inNearZone = false;
        }
    }

    void CheckAsteroidAvoidance()
    {
        if (player == null || avoidanceRegistered) return;

        Vector3 currentPos = transform.position;
        Vector3 playerPos = player.position;

        float distanceFromStart = Vector3.Distance(currentPos, initialPlayerPosition);
        float minDistance = Vector3.Distance(transform.position, playerPos);

        if (distanceFromStart > 3f && minDistance > 2f && !hasPassedPlayer)
        {
            hasPassedPlayer = true;
            RegisterAvoidance();
        }

        Vector3 toPlayer = (playerPos - currentPos).normalized;
        Vector3 velocity = rb.linearVelocity.normalized;
        float dot = Vector3.Dot(velocity, toPlayer);

        if (dot < -0.5f && minDistance > 1.5f && Time.time - spawnTime > 1f)
        {
            RegisterAvoidance();
        }
    }

    void RegisterAvoidance()
    {
        if (avoidanceRegistered) return;
        avoidanceRegistered = true;

        var tracker = Object.FindFirstObjectByType<PlayerPerformanceTracker>();
        if (tracker != null) tracker.RegisterAsteroidAvoided();
    }

    private void LateUpdate()
    {
        if (Camera.main == null) return;
        if (!IsVisibleFrom(Camera.main))
        {
            RecycleOrDestroy();
        }
    }

    private bool IsVisibleFrom(Camera camera)
    {
        var col = GetComponent<Collider>();
        if (col == null) return true;
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
        return GeometryUtility.TestPlanesAABB(planes, col.bounds);
    }

    private void RecycleOrDestroy()
    {
        if (prefabReference != null && AstreoidPool.Instance != null)
            AstreoidPool.Instance.Recycle(prefabReference, gameObject);
        else
            Destroy(gameObject);
    }
}
