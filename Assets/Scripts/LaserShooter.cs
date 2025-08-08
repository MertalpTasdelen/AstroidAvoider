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
        // Geminin baktığı yön (aşağıya doğru). Lazerlerin yönünü bu vektör belirler.
        Vector3 direction = transform.forward;
        Vector3 spawnPos = transform.position + direction * 0.5f;
        spawnPos.z = 0f;

        // 2D için z ekseninde 90 derece rotasyon
        Quaternion rotation = Quaternion.Euler(90, 0, 0);

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

    /// <summary>
    /// Lazer ateşleme işlemini başlatır. İlk atışı gecikmesiz yapması için fireCooldown sıfırlanır.
    /// </summary>
    public void EnableShooting()
    {
        isShooting = true;
        fireCooldown = 0f;
    }

    /// <summary>
    /// Lazer ateşleme işlemini durdurur.
    /// </summary>
    public void DisableShooting()
    {
        isShooting = false;
    }
}
