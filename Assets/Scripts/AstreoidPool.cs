using System.Collections.Generic;
using UnityEngine;

public class AstreoidPool : MonoBehaviour
{
    public static AstreoidPool Instance { get; private set; }

    [SerializeField] private GameObject[] prefabsToPreload;
    [SerializeField] private int preloadCount = 5;

    private readonly Dictionary<GameObject, Queue<GameObject>> pools = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializePools();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializePools()
    {
        if (prefabsToPreload == null) return;

        foreach (var prefab in prefabsToPreload)
        {
            if (prefab == null) continue;

            var q = new Queue<GameObject>();
            for (int i = 0; i < preloadCount; i++)
            {
                GameObject obj = Instantiate(prefab);
                obj.SetActive(false);
                q.Enqueue(obj);
            }
            pools[prefab] = q;
        }
    }

    public GameObject Get(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (!pools.TryGetValue(prefab, out Queue<GameObject> q) || q.Count == 0)
        {
            return Instantiate(prefab, position, rotation);
        }

        GameObject obj = q.Dequeue();
        obj.transform.position = position;
        obj.transform.rotation = rotation;
        obj.SetActive(true);

        Astreoid script = obj.GetComponent<Astreoid>();
        if (script != null)
            script.prefabReference = prefab;

        return obj;
    }

    public void Recycle(GameObject prefab, GameObject obj)
    {
        obj.SetActive(false);
        if (!pools.TryGetValue(prefab, out Queue<GameObject> q))
        {
            q = new Queue<GameObject>();
            pools[prefab] = q;
        }
        q.Enqueue(obj);
    }
}
