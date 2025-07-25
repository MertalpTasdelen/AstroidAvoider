using UnityEngine;

public class Laser : MonoBehaviour
{
    public float lifetime = 2f;
    public GameObject explosionEffect; // opsiyonel patlama efekti

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Asteroid"))
        {
            // Patlama efekti oluştur
            if (explosionEffect != null)
            {
                Instantiate(explosionEffect, transform.position, Quaternion.identity);
            }

            Destroy(other.gameObject); // asteroid yok et
            Destroy(gameObject);       // lazeri yok et

            ScoreSystem.Instance?.AddScore(15); // bonus puan
        }
    }
}
