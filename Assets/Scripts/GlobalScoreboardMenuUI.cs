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

                foreach (var t in panelRoot.GetComponentsInChildren<Transform>(true))
                {
                    if (t.name == "Content")
                        contentRoot = t;
                }

                if (contentRoot == null)
                    Debug.LogError("[GlobalScoreboardMenuUI] Content not found under Canvas.");
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

        List<GlobalScoreEntry> topScores = GlobalScoreBoardManager.Instance.GetTopScores();

        foreach (var entry in topScores)
        {
            GameObject item = Instantiate(scoreItemPrefab, contentRoot);
            item.transform.Find("Canvas/NameText").GetComponent<TMP_Text>().text = entry.playerName;
            item.transform.Find("Canvas/ScoreText").GetComponent<TMP_Text>().text = entry.score.ToString();
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
