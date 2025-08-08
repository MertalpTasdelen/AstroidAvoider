using UnityEngine;

public class BonusStageManager : MonoBehaviour
{
    public static BonusStageManager Instance;

    [Header("Bonus Ayarları")]
    [SerializeField] private float bonusStageDuration = 15f; // Bonus stage süresi

    private bool isBonusActive = false;
    private float remainingTime;
    private bool isBonusStageActive = false;
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
        if (isBonusStageActive)
        {
            remainingTime -= Time.deltaTime;

            if (remainingTime <= 0)
            {
                EndBonusStage();
            }
        }
    }

    public void StartBonusStage()
    {
        isBonusStageActive = true;
        isBonusActive = true;
        remainingTime = bonusStageDuration;

        if (DifficultyManager.Instance != null)
            DifficultyManager.Instance.enabled = false;

        if (playerLaser == null)
            playerLaser = FindFirstObjectByType<LaserShooter>();

        playerLaser?.EnableShooting();
    }

    public void EndBonusStage()
    {
        Debug.Log("[BONUS STAGE] Bitti!");

        isBonusStageActive = false;
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