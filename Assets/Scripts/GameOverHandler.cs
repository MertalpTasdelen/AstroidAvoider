using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class GameOverHandler : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private GameObject player;
    [SerializeField] private Button continueButton;
    [SerializeField] private TMP_Text gameOverText;
    [SerializeField] private ScoreSystem scoreSystem;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private AstreoidSpawner astreidSpwaner;

    [Header("Timing")]
    [SerializeField, Tooltip("Patlama animasyonunun görünmesi için bekleme")]
    private float gameOverDelay = 1.2f;

    [SerializeField] private bool useSlowMoOnCrash = true;
    [SerializeField] private float slowMoScale = 0.2f;
    [SerializeField] private float slowMoDuration = 0.15f; // realtime

    public void EndGame()
    {
        // UI'ı hemen açmak yerine coroutine'e devret
        StartCoroutine(EndGameRoutine());
    }

    private IEnumerator EndGameRoutine()
    {
        // Spawner ve skor sayacı durdurulsun (UI daha sonra açılacak)
        if (astreidSpwaner != null) astreidSpwaner.enabled = false;

        HapticsManager.Vibrate();

        var scoreRef = scoreSystem != null ? scoreSystem : ScoreSystem.Instance;
        if (scoreRef != null) scoreRef.PauseScore();

        // İsteğe bağlı kısa slow-mo (patlama hissini güçlendirir)
        if (useSlowMoOnCrash)
        {
            Time.timeScale = slowMoScale;
            yield return new WaitForSecondsRealtime(slowMoDuration);
            Time.timeScale = 1f;
        }

        // Patlamanın görünmesi için bekle
        yield return new WaitForSecondsRealtime(gameOverDelay);

        // Skorları yaz ve server'a gönder
        int finalScore = scoreRef != null ? Mathf.FloorToInt(scoreRef.GetScore()) : 0;
        int highScore  = scoreRef != null ? scoreRef.GetHighScore()               : 0;

        gameOverText.text = $"Game Over\nScore: {finalScore}\nHigh Score: {highScore}";

        string playerName = PlayerPrefs.GetString("PlayerName", "Player");
        if (LeaderboardApiClient.Instance != null && scoreRef != null)
            StartCoroutine(LeaderboardApiClient.Instance.SubmitScore(playerName, highScore));

        AchievementApiClient.Instance?.SubmitProgress("session_5", 1);
        AchievementApiClient.Instance?.SubmitProgress("session_20", 1);

        // UI'yı şimdi aç
        if (gameOverUI != null) gameOverUI.SetActive(true);
    }

    public void RestartGame()
    {
        ScoreSystem.Instance.ResetScore();
        ScoreSystem.Instance.StartTimer();
        DifficultyManager.Instance?.ResetDifficulty();
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }

    public void CotinueButton()
    {
        AdManager.Instance.ShowAd(this);
        continueButton.interactable = false;
    }

    public void ReturnToMainMenu()
    {
        if (ScoreSystem.Instance != null)
        {
            Destroy(ScoreSystem.Instance.gameObject);
            DifficultyManager.Instance?.ResetDifficulty();
        }
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    public void ContinueGame()
    {
        player.transform.position = Vector3.zero;
        player.SetActive(true);
        player.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;

        if (astreidSpwaner != null) astreidSpwaner.enabled = true;
        if (gameOverUI != null) gameOverUI.SetActive(false);

        if (scoreSystem != null) scoreSystem.gameObject.SetActive(true);
        ScoreSystem.Instance.StartTimer();
    }
}
