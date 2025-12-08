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
    public float checkInterval = 1f;
    [SerializeField] private float stageIntervalSeconds = 60f;

    private float timeSinceLastCheck = 0f;
    private int difficultyLevel = 1;
    private bool isStageTransitionRunning = false;
    private float stageTimer = 0f;
    private int currentStage = 1;

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
        timeSinceLastCheck += Time.unscaledDeltaTime;

        if (timeSinceLastCheck >= checkInterval)
        {
            AdjustDifficulty();
            timeSinceLastCheck = 0f;
        }

        TickStageTimer();
    }

    private void AdjustDifficulty()
    {
        // Score'a dayalı difficulty hesaplama - her 100 puan için 1 seviye artış
        float currentScore = ScoreSystem.Instance != null ? ScoreSystem.Instance.GetScore() : 0f;
        int newLevel = Mathf.Clamp(1 + Mathf.FloorToInt(currentScore / 100f), 1, 10);
        
        if (newLevel > difficultyLevel)
        {
            difficultyLevel = newLevel;
            ApplyDifficultyFeedback();
        }
    }

    private void TickStageTimer()
    {
        if (isStageTransitionRunning)
            return;

        bool bonusActive = BonusStageManager.Instance != null && BonusStageManager.Instance.IsBonusActive();
        if (bonusActive)
            return;

        stageTimer += Time.unscaledDeltaTime;

        if (stageTimer >= stageIntervalSeconds)
        {
            TriggerStageTransition();
        }
    }

    private void TriggerStageTransition()
    {
        if (StageTransitionManager.Instance == null)
        {
            stageTimer = 0f;
            currentStage++;
            return;
        }

        isStageTransitionRunning = true;
        float overflow = stageTimer - stageIntervalSeconds;
        stageTimer = Mathf.Max(0f, overflow); // taşıyan süreyi koru
        int completedStage = currentStage;

        StageTransitionManager.Instance.PlayStageTransition(
            completedStage,
            () =>
            {
                currentStage++;
                stageTimer = 0f;
                isStageTransitionRunning = false;
                timeSinceLastCheck = 0f;
            });
    }

    public void ForceCheck()
    {
        AdjustDifficulty();
        timeSinceLastCheck = 0f;
    }

    private void ApplyDifficultyFeedback()
    {
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
        Debug.Log($"[STAGE] Before: {difficultyLevel}");

        difficultyLevel = 1;
        timeSinceLastCheck = 0f;
        performanceTracker?.ResetPerformance();
        isStageTransitionRunning = false;
        stageTimer = 0f;
        currentStage = 1;


        Debug.Log($"[STAGE] After: {difficultyLevel}");
    }
}
