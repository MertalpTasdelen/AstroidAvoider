using UnityEngine;
using TMPro;

public class ScoreSystem : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private float scoreMultiplier;
    private float score;
    private bool isCrashed;

    private int highScore;

    public static ScoreSystem Instance { get; private set; }

    private void Awake()
    {
        LoadHighScore();

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
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
        }
    }

    public void AddScore(int amount)
    {
        if (!isCrashed)
        {
            score += amount;
            scoreText.text = Mathf.FloorToInt(score).ToString();

            if (score > highScore)
            {
                highScore = Mathf.FloorToInt(score);
                SaveHighScore();
            }
        }
    }

    public void AddAvoidBonus(int amount = 10)
    {
        if (!isCrashed)
        {
            score += amount;
            scoreText.text = Mathf.FloorToInt(score).ToString();
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
        scoreText.text = "0";
    }
}
