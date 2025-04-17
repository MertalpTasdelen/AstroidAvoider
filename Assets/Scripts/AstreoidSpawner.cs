using UnityEngine;

public class AstreoidSpawner : MonoBehaviour
{
    public static AstreoidSpawner Instance { get; private set; }

    [SerializeField] private GameObject[] astreoidPrefabs;
    [SerializeField] private GameObject splitAsteroidPrefab;

    [SerializeField] private float secondsBetweenAstreoids;
    [SerializeField] private Vector2 forceRange;

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
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            SpawnAstreid();

            timer += secondsBetweenAstreoids;
        }
    }

    private void SpawnAstreid()
    {
        int side = Random.Range(0, 4);

        Vector2 spawnPosition = Vector2.zero;
        Vector2 force = Vector2.zero;

        switch (side)
        {
            case 0:
                spawnPosition.x = 0;
                spawnPosition.y = Random.value;
                force = new Vector2(1f, Random.Range(-1f, 1f));
                break;
            case 1:
                spawnPosition.x = 1;
                spawnPosition.y = Random.value;
                force = new Vector2(-1f, Random.Range(-1f, 1f));
                break;
            case 2:
                spawnPosition.y = 0;
                spawnPosition.x = Random.value;
                force = new Vector2(Random.Range(-1f, 1f), 1f);
                break;
            case 3:
                spawnPosition.y = 1;
                spawnPosition.x = Random.value;
                force = new Vector2(Random.Range(-1f, 1f), -1f);
                break;

        }

        Vector3 worldSpawnPoint = mainCamera.ViewportToWorldPoint(spawnPosition);
        worldSpawnPoint.z = 0;

        GameObject astreoidInstance = Instantiate(
            astreoidPrefabs[Random.Range(0, astreoidPrefabs.Length)],
            worldSpawnPoint,
            Quaternion.Euler(0f, 0f, Random.Range(0f, 360f)));

        Rigidbody rb = astreoidInstance.GetComponent<Rigidbody>();

        rb.linearVelocity = force.normalized * Random.Range(forceRange.x, forceRange.y);

        Astreoid asteroidScript = astreoidInstance.GetComponent<Astreoid>();

        if (asteroidScript != null)
        {
            if (currentDifficultyLevel >= 2)
            {
                asteroidScript.useZigZag = true;
                asteroidScript.zigzagFrequency = 5f + currentDifficultyLevel;
                asteroidScript.zigzagMagnitude = 0.5f + currentDifficultyLevel * 0.5f;
            }

            if (currentDifficultyLevel >= 4)
            {
                asteroidScript.useHoming = true;
                asteroidScript.homingSpeed = Random.Range(2.5f, 4.5f);
            }

            // 💣 Split özelliğini sadece bazı asteroidlere ver!
            if (currentDifficultyLevel >= 5 && Random.value > 0.5f) // örnek: sadece %50’si split
            {
                asteroidScript.canSplit = true;
                asteroidScript.splitDelay = 2f + Random.Range(0f, 0.5f);
                asteroidScript.splitAsteroidPrefab = splitAsteroidPrefab;
            }
            else
            {
                asteroidScript.canSplit = false; // split olmayanlar yok olmasın!
            }
        }
    }

    public void SetDifficultyLevel(int level)
    {
        currentDifficultyLevel = level;

        secondsBetweenAstreoids = Mathf.Max(0.5f, 2.5f - (level * 0.2f));

        forceRange = new Vector2(5f + level * 0.5f, 1f + level);
    }

}
