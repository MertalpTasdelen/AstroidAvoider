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
    [SerializeField] private float fadeDuration = 0.6f;
    [SerializeField] private float messageDisplayTime = 1.5f;

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        Instance = this;
        canvasGroup = stageText.GetComponent<CanvasGroup>();
    }

    // ✅ Gelişmiş versiyon: callback destekli
    public void PlayStageTransition(int stageNumber, Action onTransitionComplete)
    {
        StartCoroutine(TransitionRoutine(stageNumber, onTransitionComplete));
    }

    // ✅ Basit versiyon: callback yok
    public void PlayStageTransition(int stageNumber)
    {
        PlayStageTransition(stageNumber, null);
    }

    private IEnumerator TransitionRoutine(int stage, Action onComplete)
    {
        Time.timeScale = 0.3f;

        // İlk geçişse sadece INCOMING göster
        if (stage > 0)
        {
            yield return StartCoroutine(ShowText($"STAGE {stage} COMPLETED!", stageCompleteClip));
            yield return new WaitForSecondsRealtime(0.5f);
        }

        yield return StartCoroutine(ShowText($"STAGE {stage + 1} INCOMING!", stageIncomingClip));

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
