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

    void Update()
    {
        elapsedTime += Time.deltaTime;
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

    public void RegisterHit()
    {
        timesHit++;
        DifficultyManager.Instance?.ForceCheck();

    }

    public void ResetStats()
    {
        elapsedTime = 0f;
        asteroidsAvoided = 0;
        timesHit = 0;
    }

    public void ResetPerformance()
    {
        asteroidsAvoided = 0;
        timesHit = 0;
    }
}
