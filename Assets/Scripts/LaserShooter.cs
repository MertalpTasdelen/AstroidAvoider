using UnityEngine;

public class LaserShooter : MonoBehaviour
{
    [Header("Lazer Ayarları")]
    [SerializeField] private GameObject laserPrefab;
    [SerializeField] private float fireRate = 0.3f;
    [SerializeField] private float laserSpeed = 10f;

    private float fireCooldown;
    private bool isShooting = false;

    // Oyuncu gemisinin rigidbodysini saklarız ki hızını lazerlere ekleyebilelim.
    private Rigidbody playerRb;

    private void Start()
    {
        // Sahnedeki PlayerMovements bileşenini bul ve rigidbodysini al. Eğer bulunamazsa
        // playerRb null kalır ve lazerler yalnızca kendi hızlarıyla ateşlenir.
        var player = FindFirstObjectByType<PlayerMovements>();
        if (player != null)
        {
            playerRb = player.GetComponent<Rigidbody>();
        }
    }

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
        // Lazerin hareket yönünü belirle. Oyuncu gemisinin momentumu varsa onu kullan,
        Vector3 direction;
        if (playerRb != null && playerRb.linearVelocity.sqrMagnitude > 0.001f)
        {
            direction = playerRb.linearVelocity.normalized;
        }
        else
        {
            direction = transform.forward;
        }

        // Lazerin çıkış pozisyonu, gemi burnundan hafifçe ileriye kaydırılır.
        Vector3 spawnPos = transform.position + direction * 0.5f;
        spawnPos.z = 0f;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // 2D için z ekseninde 90 derece rotasyon
        Quaternion rotation = Quaternion.Euler(0f, 0f, angle);


        GameObject laser = Instantiate(laserPrefab, spawnPos, rotation);
        Debug.Log($"[LASER] Lazer rotationi: {rotation}");

        // Lazerin rigidbodysini al ve hızını ayarla. Mevcut gemi hızını ekle.
        Rigidbody rb = laser.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 shipVelocity = playerRb != null ? playerRb.linearVelocity : Vector3.zero;
            rb.linearVelocity = shipVelocity + direction * laserSpeed;
        }

        Debug.Log($"[LASER] Lazer ateşlendi, pozisyon: {spawnPos}, yön: {direction}");
    }

    public void EnableShooting()
    {
        isShooting = true;
        fireCooldown = 0f;
    }

    public void DisableShooting()
    {
        isShooting = false;
    }
}