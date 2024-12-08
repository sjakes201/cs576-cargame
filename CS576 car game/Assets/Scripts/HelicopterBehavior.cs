using UnityEngine;

public class HelicopterFollow : MonoBehaviour
{
    public Transform player;         
    public float followHeight = 20f;  
    public float followSpeed = 5f;   
    public Vector3 offset;            

    void Start()
    {
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Car");
            if (playerObject != null)
            {
                player = playerObject.transform;
            }
            else
            {
                Debug.LogError("Player with tag 'Car' not found!");
            }
        }
    }

    void Update()
    {
        if (player != null)
        {
            Vector3 targetPosition = new Vector3(player.position.x, followHeight, player.position.z) + offset;
            transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(90f, 0f, 0f); 
        }
    }
}
