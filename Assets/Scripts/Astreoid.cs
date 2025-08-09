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

    public bool canSplit = true;
    public float splitDelay = 2f;
    public GameObject splitAsteroidPrefab;

    private bool hasSplit = false;

    public float nearMissThreshold = 1.5f;
    private bool nearMissTriggered = false;
    private bool inNearZone = false;
    private float nearZoneEnterTime = 0f;
    private float nearMissDurationThreshold = 0.3f;

    // Asteroid kaçınma algılama değişkenleri
    private bool avoidanceRegistered = false;
    private bool hasPassedPlayer = false;
    private Vector3 initialPlayerPosition;

//Its make them mutiple times collide but velocity is the problem
    // private void Awake()
    // {
    //     rb = GetComponent<Rigidbody>();
    //     rb.useGravity = false;
    //     rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    //     rb.interpolation = RigidbodyInterpolation.Interpolate;

    //     rb.constraints = RigidbodyConstraints.FreezePositionZ 
    //                    | RigidbodyConstraints.FreezeRotationX 
    //                    | RigidbodyConstraints.FreezeRotationY;
    // }

    void Start()
    {
        spawnTime = Time.time;
        rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            direction = rb.linearVelocity.normalized;
        }

        player = GameObject.FindWithTag("Player")?.transform;

        // Başlangıç oyuncu pozisyonunu kaydet
        if (player != null)
        {
            initialPlayerPosition = player.position;
        }

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
            AstreoidPool.Instance?.Recycle(prefabReference, gameObject);
            return;
        }

        if (useZigZag)
        {
            float wave = Mathf.Sin((Time.time - spawnTime) * zigzagFrequency) * zigzagMagnitude;
            Vector3 perp = Vector3.Cross(direction, Vector3.forward);
            transform.position += perp * wave * Time.deltaTime;
        }
        CheckNearMiss();
        CheckAsteroidAvoidance();
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            // Çarpışma oldu, kaçınma kaydını engelle
            avoidanceRegistered = true;

            PlayerPerformanceTracker tracker = Object.FindFirstObjectByType<PlayerPerformanceTracker>();
            if (tracker != null)
            {
                tracker.RegisterHit();
            }

            if (canSplit && !hasSplit && Time.time - spawnTime > splitDelay)
            {
                hasSplit = true;
                SpawnSplitAsteroids();
                AstreoidPool.Instance?.Recycle(prefabReference, gameObject);
                return;
            }

            playerHealth.Crash();
            AstreoidPool.Instance?.Recycle(prefabReference, gameObject);
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        // Sadece diğer asteroidlerle ilgilen
        if (!collision.gameObject.CompareTag("Asteroid")) return;

        Rigidbody otherRb = collision.gameObject.GetComponent<Rigidbody>();
        if (otherRb == null || rb == null) return;

        // İki asteroidin konumu ve aralarındaki vektör
        Vector3 thisPos = transform.position;
        Vector3 otherPos = otherRb.transform.position;
        Vector3 collisionVector = thisPos - otherPos;
        float distanceSq = collisionVector.sqrMagnitude;
        if (distanceSq < 0.0001f) return;

        // Çarpışma öncesi hızlar
        Vector3 v1 = rb.linearVelocity;
        Vector3 v2 = otherRb.linearVelocity;

        // Eşit kütleler için elastik çarpışma formülü
        float dot1 = Vector3.Dot(v1 - v2, collisionVector);
        Vector3 newV1 = v1 - (dot1 / distanceSq) * collisionVector;
        float dot2 = Vector3.Dot(v2 - v1, -collisionVector);
        Vector3 newV2 = v2 - (dot2 / distanceSq) * (-collisionVector);

        // Yeni hızları ata
        rb.linearVelocity = newV1;
        otherRb.linearVelocity = newV2;
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

        // Eğer asteroid kaçınma olarak kaydedilmemişse, ekrandan çıktığında kaydet
        if (!avoidanceRegistered)
        {
            RegisterAvoidance();
        }

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
                splitScript.prefabReference = splitAsteroidPrefab;
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

    void CheckAsteroidAvoidance()
    {
        if (player == null || avoidanceRegistered)
            return;

        // Asteroid oyuncudan geçti mi kontrol et
        Vector3 currentPos = transform.position;
        Vector3 playerPos = player.position;

        // Asteroid oyuncuyu geçerse kaçınma olarak say
        float distanceFromStart = Vector3.Distance(currentPos, initialPlayerPosition);
        float minDistance = Vector3.Distance(transform.position, playerPos);

        // Eğer asteroid oyuncudan 3 unit uzaklaştıysa ve çarpışma olmadıysa
        if (distanceFromStart > 3f && minDistance > 2f && !hasPassedPlayer)
        {
            hasPassedPlayer = true;
            RegisterAvoidance();
        }

        // Alternatif: Asteroid'in hareket yönü oyuncudan uzaklaşıyorsa
        Vector3 toPlayer = (playerPos - currentPos).normalized;
        Vector3 velocity = rb.linearVelocity.normalized;
        float dot = Vector3.Dot(velocity, toPlayer);

        // Eğer asteroid oyuncudan uzaklaşıyor ve minimum mesafeden geçtiyse
        if (dot < -0.5f && minDistance > 1.5f && Time.time - spawnTime > 1f)
        {
            RegisterAvoidance();
        }
    }

    void RegisterAvoidance()
    {
        if (avoidanceRegistered) return;

        avoidanceRegistered = true;
        PlayerPerformanceTracker tracker = Object.FindFirstObjectByType<PlayerPerformanceTracker>();
        if (tracker != null)
        {
            tracker.RegisterAsteroidAvoided();
        }
    }

    private System.Collections.IEnumerator TemporarySlowMotion()
    {
        Time.timeScale = 0.3f;
        yield return new WaitForSecondsRealtime(0.15f);
        Time.timeScale = 1f;
    }

    private void LateUpdate()
    {
        if (!IsVisibleFrom(Camera.main))
        {
            RecycleOrDestroy();
        }
    }

    private bool IsVisibleFrom(Camera camera)
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
        return GeometryUtility.TestPlanesAABB(planes, GetComponent<Collider>().bounds);
    }

    private void RecycleOrDestroy()
    {
        if (prefabReference != null && AstreoidPool.Instance != null)
        {
            AstreoidPool.Instance.Recycle(prefabReference, gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

}
