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
    }

    public void RegisterHit()
    {
        timesHit++;
    }

    public void ResetStats()
    {
        elapsedTime = 0f;
        asteroidsAvoided = 0;
        timesHit = 0;
    }
}
