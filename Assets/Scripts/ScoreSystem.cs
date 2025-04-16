using UnityEngine;
using TMPro;

public class ScoreSystem : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private float scoreMultiplier;
    private float score;
    private bool isCrashed;

    public static ScoreSystem Instance { get; private set; }

    private void Awake()
    {
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

    private void Update()
    {
        if (!isCrashed)
        {
            score += Time.deltaTime * scoreMultiplier;
            scoreText.text = Mathf.FloorToInt(score).ToString();
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
