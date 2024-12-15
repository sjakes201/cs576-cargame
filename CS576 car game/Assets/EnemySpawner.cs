using UnityEngine;

public class HelicopterSpawner : MonoBehaviour
{
    public GameObject helicopterPrefab;
    public GameObject interceptorPrefab;

    private float interceptorSpawnTimer = 0f;
    private float helicopterSpawnTimer = 0f;

    void Start()
    {
        SpawnInterceptor(new Vector3(0, 5, 0));
        SpawnHelicopter(new Vector3(0, 50, 0));
    }

    void Update()
    {
        interceptorSpawnTimer += Time.deltaTime;
        helicopterSpawnTimer += Time.deltaTime;

        if (interceptorSpawnTimer >= 60f)
        {
            SpawnInterceptor(new Vector3(0, 0, 0));
            interceptorSpawnTimer = 0f;
        }

        if (helicopterSpawnTimer >= 120f)
        {
            SpawnHelicopter(new Vector3(0, 50, 0));
            helicopterSpawnTimer = 0f;
        }
    }

    public void SpawnHelicopter(Vector3 position)
    {
        GameObject helicopter = Instantiate(helicopterPrefab, position, Quaternion.identity);
        if (helicopter.GetComponent<HelicopterFollow>() == null)
            helicopter.AddComponent<HelicopterFollow>();
    }

    public void SpawnInterceptor(Vector3 position)
    {
        GameObject interceptor = Instantiate(interceptorPrefab, position, Quaternion.identity);
        if (interceptor.GetComponent<PoliceCarChase>() == null)
            interceptor.AddComponent<PoliceCarChase>();
    }
}
