using UnityEngine;

public class BonusStageManager : MonoBehaviour
{
    public static BonusStageManager Instance;

    [Header("Bonus Ayarları")]
    public float bonusDuration = 7f;

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
        bonusTimer = bonusDuration; // 15 saniyelik süre

        //  Zorluk ayarlarını durdur
        if (DifficultyManager.Instance != null)
            DifficultyManager.Instance.enabled = false;

        StageTransitionManager.Instance.PlayBonusTransition(() =>
        {
            Debug.Log("[BONUS STAGE] Başladı!");
            // Lazer atışı vs. zaten burada açılıyor
            playerLaser = playerLaser ?? FindFirstObjectByType<LaserShooter>();
            playerLaser?.EnableShooting();
        });
    }
    public void EndBonusStage()
    {
        Debug.Log("[BONUS STAGE] Bitti!");

        isBonusActive = false;

        if (DifficultyManager.Instance != null)
            DifficultyManager.Instance.enabled = true;

        if (playerLaser == null)
            playerLaser = FindFirstObjectByType<LaserShooter>();

        playerLaser?.DisableShooting();
    }

    public bool IsBonusActive()
    {
        return isBonusActive;
    }
}
