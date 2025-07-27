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
        // Geminin baktığı yön (transform.up → world space yönü)
        Vector3 direction = firePoint.up;
        Vector3 spawnPos = firePoint.position + direction * 0.5f;
        spawnPos.z = 0f;

        // Prefab'ı direction yönüne döndürerek instantiate et
        Quaternion rotation = Quaternion.LookRotation(Vector3.forward, direction); // 2D uyumlu
        GameObject laser = Instantiate(laserPrefab, spawnPos, rotation);

        // Rigidbody'ye doğru yönü ver
        Rigidbody rb = laser.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = direction * laserSpeed;
        }

        Debug.Log($"[LASER] Lazer ateşlendi, pozisyon: {spawnPos}, yön: {direction}");
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
