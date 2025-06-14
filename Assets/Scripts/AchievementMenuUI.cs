using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

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
        // BindCloseButton();
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
        {
            panelRoot.SetActive(false);        
        }
        
        BindCloseButton();
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RefreshReferences();
        BindCloseButton();  // Also re-bind button in case scene reloads
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

                Transform contentTransform = null;
                foreach (var t in panelRoot.GetComponentsInChildren<Transform>(true))
                {
                    if (t.name == "Content")
                    {
                        contentTransform = t;
                        break;
                    }
                }

                if (contentTransform != null)
                {
                    contentContainer = contentTransform;
                }
                else
                {
                    Debug.LogError("[AchievementMenuUI] Content not found under Canvas.");
                }
            }
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
            Debug.LogError("[AchievementMenuUI] CloseButton not found inside Canvas.");
        }
    }

    public void RefreshReferences()
    {
        // Allow RefreshReferences to safely recover if objects get destroyed & reloaded

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

        if (panelRoot.activeSelf)
        {
            panelRoot.SetActive(false);
            if (mainMenuRoot != null)
                mainMenuRoot.SetActive(true);
            ClearAchievements();
        }
        else
        {
            panelRoot.SetActive(true);
            if (mainMenuRoot != null)
                mainMenuRoot.SetActive(false);
            PopulateAchievements();
        }
    }

    private void PopulateAchievements()
    {
        ClearAchievements();

        List<AchievementData> achievements = AchievementManager.Instance.GetAchievements(false);

        foreach (var data in achievements)
        {
            GameObject item = Instantiate(achievementItemPrefab, contentContainer);

            var titleText = item.transform.Find("TitleText").GetComponent<TextMeshProUGUI>();
            var descriptionText = item.transform.Find("DescriptionText").GetComponent<TextMeshProUGUI>();
            var starIcon = item.transform.Find("Icon").GetComponent<Image>();
            var canvasGroup = item.GetComponent<CanvasGroup>();

            titleText.text = data.title;
            descriptionText.text = data.description;

            if (data.isCompleted)
            {
                canvasGroup.alpha = 1f;
                starIcon.color = Color.yellow;
            }
            else
            {
                canvasGroup.alpha = 0.4f;
                starIcon.color = Color.white;
            }
        }
    }

    private void ClearAchievements()
    {
        if (contentContainer == null) return;

        foreach (Transform child in contentContainer)
        {
            Destroy(child.gameObject);
        }
    }
}
