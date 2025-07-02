using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GlobalScoreboardMenuUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject scoreItemPrefab;  // ✅ Prefab'ı Inspector’dan assign edeceğiz

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

                // 🎯 Doğrudan içerik yolu
                contentRoot = canvas.Find("ScrollView/Viewport/Content");
                Debug.Log("[GlobalScoreboardMenuUI] Content root founded under ScrollView/Viewport!");


                if (contentRoot == null)
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
        panelRoot.SetActive(true);
        mainMenuRoot?.SetActive(false);
    }

    private void PopulateScores()
    {
        ClearItems();

        if (scoreItemPrefab == null)
        {
            Debug.LogError("[GlobalScoreboardMenuUI] scoreItemPrefab is null! Assign it via Inspector.");
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
        scores.Sort((a, b) => b.score.CompareTo(a.score));

        foreach (var entry in scores)
        {
            GameObject item = Instantiate(scoreItemPrefab, contentRoot);
            item.transform.Find("NameText").GetComponent<TMP_Text>().text = entry.playerName;
            item.transform.Find("ScoreText").GetComponent<TMP_Text>().text = entry.score.ToString();
        }
    }

    private void ClearItems()
    {
        foreach (Transform child in contentRoot)
        {
            Destroy(child.gameObject);
        }
    }

    public void TogglePanel()
    {
        if (panelRoot.activeSelf)
        {
            panelRoot.SetActive(false);
            mainMenuRoot?.SetActive(true);
            ClearItems();
        }
        else
        {
            PopulateScores();
            panelRoot.SetActive(true);
            mainMenuRoot?.SetActive(false);
        }
    }
}
