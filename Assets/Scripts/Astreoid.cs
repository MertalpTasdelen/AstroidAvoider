using UnityEngine;

public class Astreoid : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
        // If it isn't the player, return
        if (playerHealth == null)
        {
            return;
        }
        
        playerHealth.Crash();
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
