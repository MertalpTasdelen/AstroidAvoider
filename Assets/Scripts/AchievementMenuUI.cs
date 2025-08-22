using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// - Artık TitleText'te "achievement_id" yerine veritabanından gelen **title** gösterilir.
/// - DescriptionText'te daha anlaşılır bir metin ve ilerleme (current / target) gösterilir.
/// - Backend değişikliği yok; global tanımlar client'ta cache'lenir.
/// </summary>
public class AchievementMenuUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject achievementItemPrefab;

    private Transform contentContainer;
    private GameObject panelRoot;
    private GameObject mainMenuRoot;
    private Button closeButton;

    private void Awake()
    {
        FindStaticReferences();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
    }

    private void Start()
    {
        RefreshReferences();

        if (panelRoot != null)
            panelRoot.SetActive(false);

        BindCloseButton();

        // İsteğe bağlı: oyun açılır açılmaz arkaplanda definisyonları cache'le
        StartCoroutine(AchievementApiClient.Instance.EnsureDefinitionsLoaded());
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RefreshReferences();
        BindCloseButton();
    }

    private void FindStaticReferences()
    {
        GameObject achievementsMenu = GameObject.Find("AchivementsMenu");
        if (achievementsMenu != null)
        {
            Transform canvas = achievementsMenu.transform.Find("Canvas");
            if (canvas != null)
            {
                panelRoot = canvas.gameObject;

                // İçerik konteynerini bul
                foreach (var t in panelRoot.GetComponentsInChildren<Transform>(true))
                {
                    if (t.name == "Content")
                    {
                        contentContainer = t;
                        break;
                    }
                }

                if (contentContainer == null)
                    Debug.LogError("[AchievementMenuUI] Content not found under Canvas.");
            }
        }

        mainMenuRoot = GameObject.Find("MainMenu");
    }

    private void BindCloseButton()
    {
        if (panelRoot == null) return;

        closeButton = panelRoot.GetComponentInChildren<Button>(true);
        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(TogglePanel);
        }
        else
        {
            Debug.LogError("[AchievementMenuUI] CloseButton not found inside Canvas.");
        }
    }

    public void RefreshReferences()
    {
        if (panelRoot == null || panelRoot.Equals(null))
            FindStaticReferences();

        if (closeButton == null || closeButton.Equals(null))
            BindCloseButton();
    }

    public void TogglePanel()
    {
        if (panelRoot == null)
        {
            Debug.LogError("[AchievementMenuUI] panelRoot is not assigned!");
            return;
        }

        bool opening = !panelRoot.activeSelf;

        if (opening)
        {
            panelRoot.SetActive(true);
            if (mainMenuRoot != null) mainMenuRoot.SetActive(false);
            StartCoroutine(LoadAndPopulate());
        }
        else
        {
            panelRoot.SetActive(false);
            if (mainMenuRoot != null) mainMenuRoot.SetActive(true);
            ClearAchievements();
        }
    }

    private IEnumerator LoadAndPopulate()
    {
        // 1) Global definisyonlar hazır mı?
        yield return AchievementApiClient.Instance.EnsureDefinitionsLoaded();

        // 2) Oyuncu başarımları
        string playerName = PlayerPrefs.GetString("PlayerName", "Player");
        bool done = false;
        List<AchievementApiClient.PlayerAchievement> items = null;

        yield return StartCoroutine(AchievementApiClient.Instance.FetchPlayerAchievements(
            playerName,
            result => { items = result; done = true; }
        ));

        while (!done) yield return null;

        PopulateAchievements(items ?? new List<AchievementApiClient.PlayerAchievement>());
    }

    private void PopulateAchievements(List<AchievementApiClient.PlayerAchievement> playerAchievements)
    {
        ClearAchievements();

        foreach (var data in playerAchievements)
        {
            GameObject item = Instantiate(achievementItemPrefab, contentContainer);

            var titleText = item.transform.Find("TitleText")?.GetComponent<TextMeshProUGUI>();
            var descriptionText = item.transform.Find("DescriptionText")?.GetComponent<TextMeshProUGUI>();
            var starIcon = item.transform.Find("Icon")?.GetComponent<Image>();
            var canvasGroup = item.GetComponent<CanvasGroup>() ?? item.AddComponent<CanvasGroup>();

            // ---- Başlık: id yerine veritabanından fetched title ----
            string title = AchievementApiClient.Instance.GetTitleFor(data.achievement_id);
            if (titleText != null) titleText.text = title;

            // ---- Açıklama + Progress ----
            // Daha anlaşılır/natural bir ifade verelim.
            int current = data.current_amount;
            int? target = AchievementApiClient.Instance.GetTargetFor(data.achievement_id);
            string friendlyProgress = target.HasValue ? $"{current}/{target.Value}" : current.ToString();

            // (İsteğe bağlı) açıklama metnini da alabiliriz:
            string desc = AchievementApiClient.Instance.GetDescriptionFor(data.achievement_id);
            if (string.IsNullOrWhiteSpace(desc))
            {
                // Description yoksa sade ve anlaşılır bir fallback:
                desc = $"Ilerleme: {friendlyProgress}";
            }
            else
            {
                // Varsa, sonuna ilerleme bilgisini ekleyip kullanıcıya netlik verelim.
                desc = $"{desc}\n\nIlerleme: {friendlyProgress}";
            }

            if (descriptionText != null) descriptionText.text = desc;

            // ---- Tamamlanma durumu görselleştirme ----
            if (data.is_completed)
            {
                canvasGroup.alpha = 1f;
                if (starIcon) starIcon.color = Color.yellow;
            }
            else
            {
                canvasGroup.alpha = 0.5f;
                if (starIcon) starIcon.color = Color.white;
            }
        }
    }

    private void ClearAchievements()
    {
        if (contentContainer == null) return;
        for (int i = contentContainer.childCount - 1; i >= 0; i--)
            Destroy(contentContainer.GetChild(i).gameObject);
    }
}
