// DifficultyManager.cs (Harmanlanmış Sürüm)
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class DifficultyManager : MonoBehaviour
{
    public static DifficultyManager Instance;

    [SerializeField] private PlayerPerformanceTracker performanceTracker;
    [SerializeField] private TMP_Text difficultyText;
    public float checkInterval = 5f;

    private float timeSinceLastCheck = 0f;
    private int difficultyLevel = 1;

    [Header("Camera Shake Settings")]
    public float shakeDuration = 0.6f;
    public float shakeMagnitude = 0.2f;

    [Header("Screen Flash Settings")]
    public Image screenOverlayImage;
    public Color flashColor = new Color(1f, 0f, 0f, 0.25f);
    public float flashDuration = 0.3f;

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

    void Update()
    {
        timeSinceLastCheck += Time.deltaTime;

        if (timeSinceLastCheck >= checkInterval)
        {
            AdjustDifficulty();
            timeSinceLastCheck = 0f;
        }
    }

    private void AdjustDifficulty()
    {
        float score = performanceTracker.asteroidsAvoided - performanceTracker.timesHit;

        int newLevel = Mathf.Clamp(1 + Mathf.FloorToInt(score / 5f), 1, 10);

        if (newLevel > difficultyLevel)
        {
            difficultyLevel = newLevel;
            Debug.Log($"[DIFFICULTY] New difficulty level set to: {difficultyLevel}");

            AstreoidSpawner.Instance?.SetDifficultyLevel(difficultyLevel);
            StartCoroutine(SinusoidalShake());
            StartCoroutine(FlashScreen());

            if (difficultyText != null)
                difficultyText.text = $"Difficulty: {difficultyLevel}";

            if (difficultyLevel >= 5)
                AchievementApiClient.Instance?.SubmitProgress("difficulty_5", 1);
            if (difficultyLevel >= 10)
                AchievementApiClient.Instance?.SubmitProgress("difficulty_10", 1);
        }
    }

    private IEnumerator SinusoidalShake()
    {
        Vector3 originalPos = Camera.main.transform.position;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float x = Mathf.Sin(Time.time * 40f) * shakeMagnitude;
            float y = Mathf.Cos(Time.time * 40f) * shakeMagnitude;

            Camera.main.transform.position = originalPos + new Vector3(x, y, 0);
            elapsed += Time.deltaTime;

            yield return null;
        }

        Camera.main.transform.position = originalPos;
    }

    private IEnumerator FlashScreen()
    {
        if (screenOverlayImage == null) yield break;

        screenOverlayImage.color = flashColor;

        yield return new WaitForSeconds(flashDuration);

        screenOverlayImage.color = Color.clear;
    }

    public int GetCurrentDifficulty()
    {
        return difficultyLevel;
    }

    public void ResetDifficulty()
    {
        Debug.Log($"[DIFFICULTY] Before: {difficultyLevel}");

        difficultyLevel = 1;
        timeSinceLastCheck = 0f;
        performanceTracker?.ResetPerformance();  // ← önemli ekleme


        Debug.Log($"[DIFFICULTY] After: {difficultyLevel}");

    }

}
