using UnityEngine;

public class BonusStageManager : MonoBehaviour
{
    public static BonusStageManager Instance;

    [Header("Bonus Ayarları")]
    public float bonusDuration = 15f;

    private bool isBonusActive = false;
    private float bonusTimer = 0f;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Update()
    {
        if (!isBonusActive) return;

        bonusTimer -= Time.deltaTime;

        if (bonusTimer <= 0f)
        {
            EndBonusStage();
        }
    }

    public void StartBonusStage()
    {
        if (isBonusActive) return;

        isBonusActive = true;
        bonusTimer = bonusDuration;

        StageTransitionManager.Instance.PlayBonusTransition(() =>
        {
            Debug.Log("[BONUS STAGE] Başladı!");

            // Buraya sonraki aşamalarda ekleyeceğiz:
            // - Lazer aç
            // - Asteroid zayıflat
            // - Özel müzik veya efekt

        });
    }


    public void EndBonusStage()
    {
        Debug.Log("[BONUS STAGE] Bitti!");

        isBonusActive = false;

        // Burada tüm geçici modlar eski haline dönecek (Aşama 3-4-5)
    }

    public bool IsBonusActive()
    {
        return isBonusActive;
    }
}
