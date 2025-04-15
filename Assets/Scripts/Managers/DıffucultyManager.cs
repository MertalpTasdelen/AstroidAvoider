using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // renk efekti için gerekli
using System.Collections;
using TMPro; // Eğer TMP Text kullanıyorsan


public class DifficultyManager : MonoBehaviour
{
    [SerializeField] private PlayerPerformanceTracker performanceTracker;
    [SerializeField] private TMP_Text difficultyText; // Eğer TMP Text kullanıyorsan
    public float checkInterval = 5f;

    private float timeSinceLastCheck = 0f;
    private int difficultyLevel = 1;

    [Header("Camera Shake Settings")]
    public float shakeDuration = 0.6f;
    public float shakeMagnitude = 0.2f;

    [Header("Screen Flash Settings")]
    public Image screenOverlayImage; // UI Canvas altındaki yarı saydam Image
    public Color flashColor = new Color(1f, 0f, 0f, 0.25f); // Hafif kırmızı
    public float flashDuration = 0.3f;

    void Update()
    {
        timeSinceLastCheck += Time.deltaTime;
        // Debug.Log("DifficultyManager is running: " + timeSinceLastCheck);


        if (timeSinceLastCheck >= checkInterval)
        {
            AdjustDifficulty();
            timeSinceLastCheck = 0f;
        }
    }

    private void AdjustDifficulty()
    {
        float score = performanceTracker.asteroidsAvoided - performanceTracker.timesHit;

        int newLevel = Mathf.Clamp(1 + Mathf.FloorToInt(score / 5f), 1, 10); // her 5 puanda 1 seviye

        if (newLevel > difficultyLevel)
        {
            difficultyLevel = newLevel;

            AstreoidSpawner.Instance.SetDifficultyLevel(difficultyLevel);
            StartCoroutine(SinusoidalShake());
            StartCoroutine(FlashScreen());

            if (difficultyText != null)
                difficultyText.text = $"Difficulty: {difficultyLevel}";
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
}
