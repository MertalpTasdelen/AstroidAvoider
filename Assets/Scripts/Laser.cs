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
            // Bonus sahnesi kontrolü opsiyonel; kaldırırsanız her sahnede patlama olur
            bool bonusAktif = BonusStageManager.Instance != null &&
                              BonusStageManager.Instance.IsBonusActive();

            if (explosionEffect != null && bonusAktif)
            {
                Vector3 pos = transform.position;
                pos.z = 0f;
                Instantiate(explosionEffect, pos, Quaternion.identity);
            }

            Destroy(other.gameObject);
            Destroy(gameObject);
            ScoreSystem.Instance?.AddScore(15);
        }
    }
}
