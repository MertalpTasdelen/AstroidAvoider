using UnityEngine;
using TMPro;
using System.Collections;
using System;

public class StageTransitionManager : MonoBehaviour
{
    public static StageTransitionManager Instance;

    [SerializeField] private TMP_Text stageText;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip stageCompleteClip;
    [SerializeField] private AudioClip stageIncomingClip;
    [SerializeField] private float fadeDuration = 0.4f;
    [SerializeField] private float messageDisplayTime = 1.0f;

    private CanvasGroup canvasGroup;

    [SerializeField] private float transitionDelay = 2f;
    private BonusStageManager bonusStageManager;
    // private DifficultyManager difficultyManager;

    private void Start()
    {
        bonusStageManager = FindFirstObjectByType<BonusStageManager>();
        // difficultyManager = FindObjectOfType<DifficultyManager>();
    }

    private void Awake()
    {
        Instance = this;
        canvasGroup = stageText.GetComponent<CanvasGroup>();
    }

    public void PlayStageTransition(int stageNumber, Action onTransitionComplete)
    {
        StartCoroutine(TransitionRoutine(stageNumber, onTransitionComplete));
    }

    public void PlayStageTransition(int stageNumber)
    {
        PlayStageTransition(stageNumber, null);
    }

    private IEnumerator TransitionRoutine(int stage, Action onComplete)
    {
        Time.timeScale = 0.3f;

        if (stage > 0)
        {
            yield return StartCoroutine(ShowText($"STAGE {stage} COMPLETED!", stageCompleteClip));
            yield return new WaitForSecondsRealtime(0.5f);
        }

        // Bonus stage intro + subtle haptic (double pulse)
        HapticsManager.VibratePulses(2, 0.12f);
        yield return StartCoroutine(BonusTransitionRoutine(null));

        // Resume normal timescale during the bonus stage
        Time.timeScale = 1f;

        // Start the bonus stage if a manager is assigned. The BonusStageManager
        // internally handles laser activation and difficulty pausing.
        if (bonusStageManager != null)
        {
            bonusStageManager.StartBonusStage();
            float bonusDuration = bonusStageManager.GetBonusStageDuration();
            yield return new WaitForSecondsRealtime(bonusDuration);
            if (bonusStageManager.IsBonusActive())
            {
                bonusStageManager.EndBonusStage();
            }
        }

        // Slow down time for the next stage intro
        Time.timeScale = 0.3f;

        // Incoming stage message + subtle haptic (single pulse)
        HapticsManager.VibratePulses(1);
        yield return StartCoroutine(ShowText($"STAGE {stage + 1} INCOMING!", stageIncomingClip));

        // Restore normal timescale and notify completion
        Time.timeScale = 1f;
        onComplete?.Invoke();
    }

    public void PlayBonusTransition(System.Action onComplete = null)
    {
        StartCoroutine(BonusTransitionRoutine(onComplete));
    }

    private IEnumerator BonusTransitionRoutine(System.Action onComplete)
    {
        Time.timeScale = 0.3f;

        yield return StartCoroutine(ShowText("BONUS STAGE", stageIncomingClip));

        Time.timeScale = 1f;
        onComplete?.Invoke();
    }

    private IEnumerator ShowText(string text, AudioClip sfx)
    {
        stageText.text = text;
        if (canvasGroup != null) canvasGroup.alpha = 0f;
        stageText.gameObject.SetActive(true);

        if (sfxSource != null && sfx != null)
            sfxSource.PlayOneShot(sfx);

        float timer = 0f;

        // Fade in
        while (timer < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(0, 1, timer / fadeDuration);
            timer += Time.unscaledDeltaTime;
            yield return null;
        }
        canvasGroup.alpha = 1;

        yield return new WaitForSecondsRealtime(messageDisplayTime);

        // Fade out
        timer = 0f;
        while (timer < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(1, 0, timer / fadeDuration);
            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        stageText.gameObject.SetActive(false);
    }
}