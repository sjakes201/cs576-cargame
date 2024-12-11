using UnityEngine;

public class HelicopterSpawner : MonoBehaviour
{
    public GameObject helicopterPrefab;

    public void SpawnHelicopter(Vector3 position)
    {
        GameObject helicopter = Instantiate(helicopterPrefab, position, Quaternion.identity);
        if (helicopter.GetComponent<HelicopterFollow>() == null)
            helicopter.AddComponent<HelicopterFollow>();
    }
}
