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

    [SerializeField] private float bonusStageDuration = 15f; // Bonus stage süresi
    private float remainingTime;
    private bool isBonusStageActive = false;
    private DifficultyManager difficultyManager; // DifficultyManager referansı

    private void Start()
    {
        difficultyManager = FindObjectOfType<DifficultyManager>();
    }

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
        remainingTime = bonusStageDuration;
        
        // Difficulty yönetimini duraklat
        if (difficultyManager != null)
        {
            difficultyManager.PauseDifficultyProgression();
        }
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
