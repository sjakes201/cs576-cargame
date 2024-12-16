// AISpawnManager.cs
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AISpawnManager : MonoBehaviour
{
    [System.Serializable]
    public class SpawnPoint
    {
        public Transform point;
        public SpawnType type;
        public float spawnInterval = 5f;
        public int maxSpawned = 5;
    }

    public enum SpawnType
    {
        Pedestrian,
        Vehicle
    }

    [Header("Spawn Settings")]
    [SerializeField] private SpawnPoint[] spawnPoints;
    [SerializeField] private GameObject pedestrianPrefab;
    [SerializeField] private GameObject[] vehiclePrefabs;
    [SerializeField] private int maxTotalPedestrians = 20;
    [SerializeField] private int maxTotalVehicles = 10;

    [Header("Pool Settings")]
    [SerializeField] private int initialPoolSize = 10;

    private Dictionary<SpawnType, Queue<GameObject>> objectPools;
    private Dictionary<SpawnType, List<GameObject>> activeObjects;
    private Dictionary<SpawnPoint, float> lastSpawnTimes;

    private void Start()
    {
        InitializePools();
        lastSpawnTimes = new Dictionary<SpawnPoint, float>();

        // Start the spawn routine
        StartCoroutine(SpawnRoutine());
    }

    private void InitializePools()
    {
        objectPools = new Dictionary<SpawnType, Queue<GameObject>>();
        activeObjects = new Dictionary<SpawnType, List<GameObject>>();

        // Initialize pedestrian object pool
        objectPools[SpawnType.Pedestrian] = new Queue<GameObject>();
        activeObjects[SpawnType.Pedestrian] = new List<GameObject>();
        for (int i = 0; i < initialPoolSize; i++)
        {
            CreatePoolObject(SpawnType.Pedestrian);
        }

        // Initialize vehicle object pool
        objectPools[SpawnType.Vehicle] = new Queue<GameObject>();
        activeObjects[SpawnType.Vehicle] = new List<GameObject>();
        for (int i = 0; i < initialPoolSize; i++)
        {
            CreatePoolObject(SpawnType.Vehicle);
        }
    }

    private void CreatePoolObject(SpawnType type)
    {
        GameObject obj = null;
        if (type == SpawnType.Pedestrian)
        {
            obj = Instantiate(pedestrianPrefab);
        }
        else
        {
            int randomIndex = Random.Range(0, vehiclePrefabs.Length);
            obj = Instantiate(vehiclePrefabs[randomIndex]);
        }

        obj.SetActive(false);
        objectPools[type].Enqueue(obj);
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            foreach (SpawnPoint sp in spawnPoints)
            {
                if (CanSpawn(sp))
                {
                    SpawnObject(sp);
                }
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    private bool CanSpawn(SpawnPoint sp)
    {
        // Check spawn interval
        if (lastSpawnTimes.ContainsKey(sp))
        {
            if (Time.time - lastSpawnTimes[sp] < sp.spawnInterval)
            {
                return false;
            }
        }

        // Check max spawned
        int currentCount = activeObjects[sp.type].Count;
        int maxAllowed = sp.type == SpawnType.Pedestrian ? maxTotalPedestrians : maxTotalVehicles;
        
        return currentCount < maxAllowed && currentCount < sp.maxSpawned;
    }

    private void SpawnObject(SpawnPoint sp)
    {
        // Check if there are objects in the pool
        if (objectPools[sp.type].Count == 0)
        {
            CreatePoolObject(sp.type);
        }

        // Get the object from the pool
        GameObject obj = objectPools[sp.type].Dequeue();
        obj.transform.position = sp.point.position;
        obj.transform.rotation = sp.point.rotation;
        obj.SetActive(true);

        // update the active objects list
        activeObjects[sp.type].Add(obj);
        lastSpawnTimes[sp] = Time.time;
    }

    public void ReturnToPool(GameObject obj, SpawnType type)
    {
        if (activeObjects[type].Contains(obj))
        {
            activeObjects[type].Remove(obj);
            obj.SetActive(false);
            objectPools[type].Enqueue(obj);
        }
    }

    public void ClearAllAI()
    {
        foreach (var type in System.Enum.GetValues(typeof(SpawnType)))
        {
            SpawnType spawnType = (SpawnType)type;
            foreach (var obj in activeObjects[spawnType])
            {
                ReturnToPool(obj, spawnType);
            }
        }
    }
}