using UnityEngine;

public class LaserShooter : MonoBehaviour
{
    [Header("Lazer Ayarları")]
    [SerializeField] private GameObject laserPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 0.3f;
    [SerializeField] private float laserSpeed = 10f;

    private float fireCooldown;
    private bool isShooting = false;

    void Update()
    {
        if (!isShooting) return;

        fireCooldown -= Time.deltaTime;

        if (fireCooldown <= 0f)
        {
            FireLaser();
            fireCooldown = fireRate;
        }
    }

    private void FireLaser()
    {
        Debug.Log("[LASER] FireLaser çağrıldı.");

        Vector3 direction = firePoint.right;
        Vector3 spawnPos = firePoint.position + direction * 0.3f;

        GameObject laser = Instantiate(laserPrefab, spawnPos, firePoint.rotation);

        Rigidbody rb = laser.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = firePoint.up * laserSpeed;
            Debug.Log("Lazer ateşlendi, pozisyon: " + spawnPos);

        }

        // patlama efektini veya lazer sesini burada tetikleyebilirsin
    }

    public void EnableShooting()
    {
        isShooting = true;
        fireCooldown = 0f; // ilk atışı gecikmesiz yapsın
    }

    public void DisableShooting()
    {
        isShooting = false;
    }
}
