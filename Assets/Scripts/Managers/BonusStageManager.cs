using UnityEngine;

public class BonusStageManager : MonoBehaviour
{
    public static BonusStageManager Instance;

    [Header("Bonus Ayarları")]
    public float bonusDuration = 15f;

    private bool isBonusActive = false;
    private float bonusTimer = 0f;
    // Bonus aşamasında oyuncu gemisinin önündeki lazeri kontrol edecek script
    private LaserShooter playerLaser;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        // Oyuncu gemisindeki LaserShooter bileşenini bul
        if (playerLaser == null)
            playerLaser = FindFirstObjectByType<LaserShooter>();
    }

    private void Update()
    {
        if (!isBonusActive) return;

        bonusTimer -= Time.deltaTime;

        if (bonusTimer <= 0f)
        {
            EndBonusStage();
        }
    }

    public void StartBonusStage()
    {
        if (isBonusActive) return;

        isBonusActive = true;
        bonusTimer = bonusDuration;

        StageTransitionManager.Instance.PlayBonusTransition(() =>
        {
            Debug.Log("[BONUS STAGE] Başladı!");

            // Lazer ateşlemeyi başlat
            if (playerLaser == null)
                playerLaser = FindFirstObjectByType<LaserShooter>();

            playerLaser?.EnableShooting();

            // Buraya sonraki aşamalarda ekleyeceğiz:
            // - Lazer aç
            // - Asteroid zayıflat
            // - Özel müzik veya efekt

        });
    }


    public void EndBonusStage()
    {
        Debug.Log("[BONUS STAGE] Bitti!");

        isBonusActive = false;

        // Burada tüm geçici modlar eski haline dönecek (Aşama 3-4-5)
    }

    public bool IsBonusActive()
    {
        return isBonusActive;
    }
}
