// NearMissFeedbackSystem.cs
using UnityEngine;

public class NearMissFeedbackSystem : MonoBehaviour
{
    public static NearMissFeedbackSystem Instance;

    [Header("Floating Bonus Icon")]
    [SerializeField] private GameObject bonusIconPrefab;
    [SerializeField] private Transform bonusSpawnPoint;

    [Header("Effects")]
    [SerializeField] private GameObject flameEffectObject;
    [SerializeField] private float flameScaleBoost = 1.2f;
    [SerializeField] private float flameBoostDuration = 0.3f;
    [SerializeField] private CameraShake cameraShake;

    [Header("Audio")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip nearMissClip;

    private Vector3 originalFlameScale;

    private void Awake()
    {
        Instance = this;
        if (flameEffectObject != null)
            originalFlameScale = flameEffectObject.transform.localScale;
    }

    public void TriggerNearMissFeedback()
    {
        ShowFloatingBonus();
        BoostFlameEffect();
        PlaySoundEffect();
        cameraShake?.Shake(0.1f, 0.05f);

        HapticsManager.Vibrate();

        AchievementApiClient.Instance?.SubmitProgress("near_miss_3", 1);
        AchievementApiClient.Instance?.SubmitProgress("near_miss_10", 1);
    }

    public void ShowFloatingBonus()
    {
        if (bonusIconPrefab == null || bonusSpawnPoint == null) return;

        GameObject icon = Instantiate(
            bonusIconPrefab,
            bonusSpawnPoint.position,
            Quaternion.identity,
            bonusSpawnPoint);

        RectTransform rt = icon.GetComponent<RectTransform>();
        rt.anchoredPosition = Vector2.zero;
    }

    private void BoostFlameEffect()
    {
        if (flameEffectObject == null) return;
        StopAllCoroutines();
        StartCoroutine(ScaleFlameEffect());
    }

    private System.Collections.IEnumerator ScaleFlameEffect()
    {
        flameEffectObject.transform.localScale = originalFlameScale * flameScaleBoost;
        yield return new WaitForSeconds(flameBoostDuration);
        flameEffectObject.transform.localScale = originalFlameScale;
    }

    private void PlaySoundEffect()
    {
        if (sfxSource != null && nearMissClip != null)
        {
            sfxSource.PlayOneShot(nearMissClip);
        }
    }
}