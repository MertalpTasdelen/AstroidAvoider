using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GlobalScoreboardMenuUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject scoreItemPrefab;  // ✅ Inspector’dan atayın

    private Transform contentRoot;
    private Button closeButton;
    private GameObject panelRoot;
    private GameObject mainMenuRoot;

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
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RefreshReferences();
        BindCloseButton();
    }

    private void FindStaticReferences()
    {
        GameObject leaderboardMenu = GameObject.Find("GlobalScoreboardPanel");
        if (leaderboardMenu != null)
        {
            Transform canvas = leaderboardMenu.transform.Find("Canvas");
            if (canvas != null)
            {
                panelRoot = canvas.gameObject;

                // 🎯 ScrollView/Viewport/Content yolunu doğrudan bul
                contentRoot = canvas.Find("ScrollView/Viewport/Content");
                if (contentRoot != null)
                {
                    Debug.Log("[GlobalScoreboardMenuUI] Content root found under ScrollView/Viewport!");
                }
                else
                {
                    Debug.LogError("[GlobalScoreboardMenuUI] Content not found under ScrollView/Viewport!");
                }
            }
            else
            {
                Debug.LogError("[GlobalScoreboardMenuUI] Canvas not found under GlobalScoreboardPanel.");
            }
        }
        else
        {
            Debug.LogError("[GlobalScoreboardMenuUI] GlobalScoreboardPanel not found in scene.");
        }

        mainMenuRoot = GameObject.Find("MainMenu");
    }

    private void BindCloseButton()
    {
        if (panelRoot == null)
            return;

        // Panel içindeki ilk Button’u Close butonu olarak kabul ediyoruz
        closeButton = panelRoot.GetComponentInChildren<Button>(true);
        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(TogglePanel);
        }
        else
        {
            Debug.LogError("[GlobalScoreboardMenuUI] CloseButton not found inside Canvas.");
        }
    }

    public void RefreshReferences()
    {
        if (panelRoot == null || panelRoot.Equals(null))
            FindStaticReferences();

        if (closeButton == null || closeButton.Equals(null))
            BindCloseButton();

        if (mainMenuRoot == null || mainMenuRoot.Equals(null))
            mainMenuRoot = GameObject.Find("MainMenu");
    }

    public void ShowLeaderboard()
    {
        PopulateScores();
        if (panelRoot != null)
            panelRoot.SetActive(true);
        if (mainMenuRoot != null)
            mainMenuRoot.SetActive(false);
    }

    private void PopulateScores()
    {
        ClearItems();

        if (scoreItemPrefab == null)
        {
            Debug.LogError("[GlobalScoreboardMenuUI] scoreItemPrefab is null! Assign it via Inspector.");
            return;
        }

        if (contentRoot == null)
        {
            Debug.LogError("[GlobalScoreboardMenuUI] contentRoot is null! Check hierarchy: GlobalScoreboardPanel/Canvas/ScrollView/Viewport/Content");
            return;
        }

        if (LeaderboardApiClient.Instance != null)
        {
            StartCoroutine(LeaderboardApiClient.Instance.FetchScores(OnScoresFetched));
        }
        else
        {
            Debug.LogError("[GlobalScoreboardMenuUI] LeaderboardApiClient instance not found.");
        }
    }

    private void OnScoresFetched(List<GlobalScoreEntry> scores)
    {
        // Skorları büyükten küçüğe sırala
        scores.Sort((a, b) => b.score.CompareTo(a.score));

        // Oyuncu adı
        string currentPlayerName = PlayerPrefs.GetString("PlayerName", "Player");

        // Oyuncunun global index’i (0-based)
        int playerIndex = scores.FindIndex(s => s.playerName == currentPlayerName);

        // Gösterilecek indexleri topla: ilk 3 + oyuncunun +/- 2 komşusu
        HashSet<int> displayIndexes = new HashSet<int>();

        // İlk 3
        for (int i = 0; i < Mathf.Min(3, scores.Count); i++)
            displayIndexes.Add(i);

        // Oyuncunun çevresi
        if (playerIndex != -1)
        {
            for (int i = playerIndex - 2; i <= playerIndex + 2; i++)
            {
                if (i >= 0 && i < scores.Count)
                    displayIndexes.Add(i);
            }
        }

        // Sadece seçilmiş indexleri oluştur
        for (int i = 0; i < scores.Count; i++)
        {
            if (!displayIndexes.Contains(i))
                continue;

            var entry = scores[i];
            GameObject item = Instantiate(scoreItemPrefab, contentRoot);

            // ⬇️ Prefab içi alan isimleri: RankText / NameText / ScoreText
            var rankText = item.transform.Find("RankText")?.GetComponent<TMP_Text>();
            var nameText = item.transform.Find("NameText")?.GetComponent<TMP_Text>();
            var scoreText = item.transform.Find("ScoreText")?.GetComponent<TMP_Text>();

            if (rankText == null || nameText == null || scoreText == null)
            {
                Debug.LogError("[GlobalScoreboardMenuUI] scoreItemPrefab children must contain RankText, NameText, ScoreText TMP_Texts.");
                continue;
            }

            // Rank = i + 1 (0-based index → 1-based sıra)
            rankText.text = (i + 1).ToString();
            nameText.text = entry.playerName;
            scoreText.text = entry.score.ToString();

            // Kendi satırın tam opak, diğerleri soluk
            if (entry.playerName != currentPlayerName)
            {
                MakeItemFaded(item);
            }
            else
            {
                // İstersen kendi satırını hafifçe vurgula (opsiyonel):
                // var cg = item.GetComponent<CanvasGroup>() ?? item.AddComponent<CanvasGroup>();
                // cg.alpha = 1f;
            }
        }
    }

    private void MakeItemFaded(GameObject item)
    {
        float fadedAlpha = 0.4f;

        foreach (var text in item.GetComponentsInChildren<TMP_Text>(true))
        {
            Color c = text.color;
            c.a = fadedAlpha;
            text.color = c;
        }
    }

    private void ClearItems()
    {
        if (contentRoot == null) return;

        for (int i = contentRoot.childCount - 1; i >= 0; i--)
        {
            Destroy(contentRoot.GetChild(i).gameObject);
        }
    }

    public void TogglePanel()
    {
        if (panelRoot == null)
            return;

        bool opening = !panelRoot.activeSelf;

        if (opening)
        {
            PopulateScores();
            panelRoot.SetActive(true);
            if (mainMenuRoot != null) mainMenuRoot.SetActive(false);
        }
        else
        {
            panelRoot.SetActive(false);
            if (mainMenuRoot != null) mainMenuRoot.SetActive(true);
            ClearItems();
        }
    }
}