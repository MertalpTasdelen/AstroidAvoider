using UnityEngine;

public class LaserShooter : MonoBehaviour
{
    [Header("Lazer Ayarları")]
    [SerializeField] private GameObject laserPrefab;
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
        Vector3 direction = transform.forward; //Geminin baktığı yön (aşağıya doğru)
        Vector3 spawnPos = transform.position + direction * 0.5f;
        spawnPos.z = 0f;

        // Quaternion rotation = Quaternion.LookRotation(Vector3.forward, direction); // 2D için
        Quaternion rotation = Quaternion.Euler(90, 0, 0); // 2D için z ekseninde 90 derece rotasyon

        GameObject laser = Instantiate(laserPrefab, spawnPos, rotation);
        Debug.Log($"[LASER] Lazer rotationi: {rotation}");

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
