using UnityEngine;

public class AstreoidSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] astreoidPrefabs;
    [SerializeField] private float secondsBetweenAstreoids;
    [SerializeField] private Vector2 forceRange;

    private Camera mainCamera;
    private float timer;

    void Start()
    {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;

        if(timer <= 0)
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

        switch(side)
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
    }
}
