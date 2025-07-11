using UnityEngine;
using TMPro;

public class ScoreSystem : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private float scoreMultiplier;

    private float score;
    private bool isCrashed;
    private int highScore;
    private float timeTracker;

    public static ScoreSystem Instance { get; private set; }

    private void Awake()
    {
        LoadHighScore();

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // ensure leaderboard has at least our current saved high score
            if (highScore > 0)
            {
                string playerName = PlayerPrefs.GetString("PlayerName", "Player");
                GlobalScoreBoardManager.Instance?.AddScore(playerName, highScore);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadHighScore()
    {
        highScore = PlayerPrefs.GetInt("HighScore", 0);
    }

    private void SaveHighScore()
    {
        PlayerPrefs.SetInt("HighScore", highScore);
        PlayerPrefs.Save();

        string playerName = PlayerPrefs.GetString("PlayerName", "Player");
        GlobalScoreBoardManager.Instance?.AddScore(playerName, highScore);
    }

    private void UpdateHighScoreIfNeeded()
    {
        if (score > highScore)
        {
            highScore = Mathf.FloorToInt(score);
            SaveHighScore();
        }
    }

    public int GetHighScore()
    {
        return highScore;
    }

    private void Update()
    {
        if (!isCrashed)
        {
            score += Time.deltaTime * scoreMultiplier;
            scoreText.text = Mathf.FloorToInt(score).ToString();

            timeTracker += Time.deltaTime;
            if (timeTracker >= 1f)
            {
                AchievementApiClient.Instance?.SubmitProgress("survive_60", 1);
                AchievementApiClient.Instance?.SubmitProgress("survive_180", 1);

                timeTracker = 0f;
            }

            UpdateHighScoreIfNeeded();
        }
    }

    public void AddScore(int amount)
    {
        if (!isCrashed)
        {
            score += amount;
            scoreText.text = Mathf.FloorToInt(score).ToString();
            UpdateHighScoreIfNeeded();
        }
    }

    public void AddAvoidBonus(int amount = 10)
    {
        if (!isCrashed)
        {
            score += amount;
            scoreText.text = Mathf.FloorToInt(score).ToString();
            UpdateHighScoreIfNeeded();
        }
    }

    public void PauseScore()
    {
        isCrashed = true;
    }

    public void StartTimer()
    {
        isCrashed = false;
    }

    public float GetScore()
    {
        return score;
    }

    public void ResetScore()
    {
        score = 0f;
        timeTracker = 0f;
        scoreText.text = "0";
    }
}
