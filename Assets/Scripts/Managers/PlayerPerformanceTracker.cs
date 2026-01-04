using UnityEngine;

/// <summary>
/// Oyuncunun performansını takip eden sınıf.
/// Toplam geçen süre, kaç asteroidden kaçıldığı ve kaç kere hasar alındığı bilgilerini tutar.
/// </summary>
public class PlayerPerformanceTracker : MonoBehaviour
{
    public float elapsedTime { get; private set; }
    public int asteroidsAvoided { get; private set; }
    public int timesHit { get; private set; }
    public int nearMisses { get; private set; }

    public float secondsSinceLastHit { get; private set; }

    // Recent-performance signals (simple decay, used for adaptive difficulty).
    public float nearMissHeat { get; private set; }
    public float hitHeat { get; private set; }

    [Header("Adaptive Difficulty (optional)")]
    [Tooltip("How quickly near-miss 'heat' decays per second.")]
    [SerializeField] private float nearMissHeatDecayPerSecond = 0.25f;

    [Tooltip("How quickly hit 'heat' decays per second.")]
    [SerializeField] private float hitHeatDecayPerSecond = 0.35f;

    void Update()
    {
        elapsedTime += Time.deltaTime;
        secondsSinceLastHit += Time.deltaTime;

        if (nearMissHeat > 0f)
            nearMissHeat = Mathf.Max(0f, nearMissHeat - nearMissHeatDecayPerSecond * Time.deltaTime);
        if (hitHeat > 0f)
            hitHeat = Mathf.Max(0f, hitHeat - hitHeatDecayPerSecond * Time.deltaTime);
    }

    public void RegisterAsteroidAvoided()
    {
        asteroidsAvoided++;

        if (ScoreSystem.Instance != null)
        {
            ScoreSystem.Instance.AddAvoidBonus(10); // her kaçınma 10 puan
        }

        AchievementApiClient.Instance?.SubmitProgress("dodge_50", 1);
        AchievementApiClient.Instance?.SubmitProgress("dodge_200", 1);

        DifficultyManager.Instance?.ForceCheck();
    }

    public void RegisterNearMiss()
    {
        nearMisses++;
        nearMissHeat += 1f;

        DifficultyManager.Instance?.ForceCheck();
    }

    public void RegisterHit()
    {
        timesHit++;
        secondsSinceLastHit = 0f;
        hitHeat += 1f;
        DifficultyManager.Instance?.ForceCheck();

    }

    public void ResetStats()
    {
        elapsedTime = 0f;
        asteroidsAvoided = 0;
        timesHit = 0;
        nearMisses = 0;
        secondsSinceLastHit = 0f;
        nearMissHeat = 0f;
        hitHeat = 0f;
    }

    public void ResetPerformance()
    {
        asteroidsAvoided = 0;
        timesHit = 0;
        nearMisses = 0;
        secondsSinceLastHit = 0f;
        nearMissHeat = 0f;
        hitHeat = 0f;
    }
}
