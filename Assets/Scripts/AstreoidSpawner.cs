using UnityEngine;

public class AstreoidSpawner : MonoBehaviour
{
    public static AstreoidSpawner Instance { get; private set; }

    [SerializeField] private GameObject[] astreoidPrefabs;
    [SerializeField] private GameObject splitAsteroidPrefab;

    [SerializeField] private float secondsBetweenAstreoids;
    [SerializeField] private Vector2 speedRange;

    [Header("Control")]
    [Tooltip("If false, this spawner will not auto-spawn in Update and expects an external controller (e.g., PatternSpawner).")]
    [SerializeField] private bool useInternalTimer = true;

    private int currentDifficultyLevel = 1;

    private Camera mainCamera;
    private float timer;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        mainCamera = Camera.main;

        SetDifficultyLevel(1); // Başlangıç zorluk seviyesi
    }

    // Update is called once per frame
    void Update()
    {
        if (!useInternalTimer)
            return;

        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            SpawnAstreid();

            timer += secondsBetweenAstreoids;
        }
    }

    public void SetExternalControl(bool externalControlEnabled)
    {
        useInternalTimer = !externalControlEnabled;
    }

    private void SpawnAstreid()
    {
        SpawnRequest request = SpawnRequest.CreateDefault();
        SpawnWithRequest(ref request);
    }

    public struct SpawnRequest
    {
        public int? sideOverride;
        public Vector2? directionOverride;
        public float speedMultiplier;

        public float zigzagChance;
        public float homingChance;
        public float splitChance;

        public static SpawnRequest CreateDefault()
        {
            return new SpawnRequest
            {
                sideOverride = null,
                directionOverride = null,
                speedMultiplier = 1f,
                zigzagChance = -1f,
                homingChance = -1f,
                splitChance = -1f,
            };
        }
    }

    public void SpawnWithRequest(ref SpawnRequest request)
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
        if (mainCamera == null)
            return;

        int side = request.sideOverride.HasValue ? Mathf.Clamp(request.sideOverride.Value, 0, 3) : Random.Range(0, 4);

        Vector2 spawnPosition = Vector2.zero;
        Vector2 direction = Vector2.zero;

        switch (side)
        {
            case 0:
                spawnPosition.x = 0;
                spawnPosition.y = Random.value;
                direction = new Vector2(1f, Random.Range(-1f, 1f));
                break;
            case 1:
                spawnPosition.x = 1;
                spawnPosition.y = Random.value;
                direction = new Vector2(-1f, Random.Range(-1f, 1f));
                break;
            case 2:
                spawnPosition.y = 0;
                spawnPosition.x = Random.value;
                direction = new Vector2(Random.Range(-1f, 1f), 1f);
                break;
            case 3:
                spawnPosition.y = 1;
                spawnPosition.x = Random.value;
                direction = new Vector2(Random.Range(-1f, 1f), -1f);
                break;
        }

        if (request.directionOverride.HasValue && request.directionOverride.Value.sqrMagnitude > 0.0001f)
        {
            direction = request.directionOverride.Value;
        }

        Vector3 worldSpawnPoint = mainCamera.ViewportToWorldPoint(spawnPosition);
        worldSpawnPoint.z = 0;

        GameObject prefab = astreoidPrefabs[Random.Range(0, astreoidPrefabs.Length)];
        GameObject astreoidInstance = AstreoidPool.Instance != null
            ? AstreoidPool.Instance.Get(prefab, worldSpawnPoint, Quaternion.Euler(0f, 0f, Random.Range(0f, 360f)))
            : Instantiate(prefab, worldSpawnPoint, Quaternion.Euler(0f, 0f, Random.Range(0f, 360f)));

        Rigidbody rb = astreoidInstance.GetComponent<Rigidbody>();
        if (rb != null)
        {
            float minSpeed = Mathf.Min(speedRange.x, speedRange.y);
            float maxSpeed = Mathf.Max(speedRange.x, speedRange.y);
            float speed = Random.Range(minSpeed, maxSpeed) * Mathf.Max(0.1f, request.speedMultiplier);
            rb.linearVelocity = direction.normalized * speed;
        }

        Astreoid asteroidScript = astreoidInstance.GetComponent<Astreoid>();
        if (asteroidScript != null)
            asteroidScript.prefabReference = prefab;

        ApplyBehaviors(asteroidScript, request);
    }

    private void ApplyBehaviors(Astreoid asteroidScript, SpawnRequest request)
    {
        if (asteroidScript == null)
            return;

        // ZigZag removed completely.
        float zigzagChance = 0f;
        float homingChance = request.homingChance >= 0f ? request.homingChance : (currentDifficultyLevel >= 4 ? 0.45f : 0f);
        float splitChance = request.splitChance >= 0f ? request.splitChance : (currentDifficultyLevel >= 5 ? 0.5f : 0f);

        asteroidScript.useHoming = Random.value < homingChance;
        if (asteroidScript.useHoming)
        {
            // "Homing" here is fire-and-forget: aim once at spawn, no continuous tracking.
            asteroidScript.homingSpeed = Random.Range(2.8f, 4.8f) + (currentDifficultyLevel * 0.18f);

            // Don't combine with zigzag by default; it would fight for control of velocity.
            asteroidScript.useZigZag = false;
        }
        else
        {
            asteroidScript.useZigZag = false;
        }

        // Split only for some asteroids.
        bool allowSplit = Random.value < splitChance;
        asteroidScript.canSplit = allowSplit;
        if (allowSplit)
        {
            asteroidScript.splitDelay = 2f + Random.Range(0f, 0.5f);
            asteroidScript.splitAsteroidPrefab = splitAsteroidPrefab;
        }
        else
        {
            asteroidScript.splitAsteroidPrefab = null;
        }
    }

    public void SetDifficultyLevel(int level)
    {
        currentDifficultyLevel = level;

        secondsBetweenAstreoids = Mathf.Max(0.5f, 2.5f - (level * 0.2f));

        // Speed range must be min..max (the old values were reversed, which could make speeds feel wrong).
        float minSpeed = 1.5f + level * 0.35f;
        float maxSpeed = 4.5f + level * 0.55f;
        speedRange = new Vector2(minSpeed, maxSpeed);
    }

}
