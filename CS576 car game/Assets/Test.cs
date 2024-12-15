using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public HelicopterSpawner Spawner;
    // Start is called before the first frame update
    void Start()
    {
        Spawner.SpawnHelicopter(new Vector3(10f, 10f, 10f));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
