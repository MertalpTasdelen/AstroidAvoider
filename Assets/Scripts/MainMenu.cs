using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private AchievementMenuUI achievementsUI;
    [SerializeField] private GlobalScoreboardMenuUI globalScoreboardUI;
    [SerializeField] private SettingsMenuUI settingsMenuUI;

    [Header("UI Root")]
    [Tooltip("If set, this object will be hidden when Settings is open. If empty, will try to find a child named 'Canvas'.")]
    [SerializeField] private GameObject mainMenuCanvasRoot;


    public void ShowMainMenu()
    {
        gameObject.SetActive(true);
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        TryAutoAssignAchievementUI();
        TryAutoAssignLeaderboardUI();
        TryAutoAssignSettingsUI();

        TryAutoAssignMainMenuCanvasRoot();
        HookSettingsVisibility();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        TryAutoAssignAchievementUI();
        TryAutoAssignLeaderboardUI();
        TryAutoAssignSettingsUI();

        TryAutoAssignMainMenuCanvasRoot();
        HookSettingsVisibility();
    }

    public void OpenSettings()
    {
        if (settingsMenuUI == null || settingsMenuUI.Equals(null))
        {
            TryAutoAssignSettingsUI();
        }

        settingsMenuUI?.TogglePanel();
    }

    private void HookSettingsVisibility()
    {
        if (settingsMenuUI == null || settingsMenuUI.Equals(null))
        {
            return;
        }

        settingsMenuUI.VisibilityChanged -= OnSettingsVisibilityChanged;
        settingsMenuUI.VisibilityChanged += OnSettingsVisibilityChanged;

        // Ensure correct state on load.
        OnSettingsVisibilityChanged(settingsMenuUI.IsOpen);
    }

    private void OnSettingsVisibilityChanged(bool isSettingsOpen)
    {
        if (mainMenuCanvasRoot == null)
        {
            TryAutoAssignMainMenuCanvasRoot();
        }

        if (mainMenuCanvasRoot != null)
        {
            mainMenuCanvasRoot.SetActive(!isSettingsOpen);
        }
    }

    public void OpenGlobalLeaderboard()
    {
        if (globalScoreboardUI == null)
        {
            GameObject prefab = Resources.Load<GameObject>("Prefabs/GlobalScoreboardMenuUI");
            GameObject instance = Instantiate(prefab);
            globalScoreboardUI = instance.GetComponent<GlobalScoreboardMenuUI>();
        }

        globalScoreboardUI.RefreshReferences();
        globalScoreboardUI.ShowLeaderboard();
    }


    private void TryAutoAssignAchievementUI()
    {
        if (achievementsUI == null || achievementsUI.Equals(null))
        {
            achievementsUI = FindFirstObjectByType<AchievementMenuUI>();
            if (achievementsUI != null)
            {
                achievementsUI.RefreshReferences();
                Debug.Log("[MainMenu] Auto-assigned AchievementMenuUI");
            }
            else
            {
                Debug.LogError("[MainMenu] AchievementMenuUI not found in scene!");
            }
        }
    }

    private void TryAutoAssignLeaderboardUI()
    {
        if (globalScoreboardUI == null || globalScoreboardUI.Equals(null))
        {
            globalScoreboardUI = FindFirstObjectByType<GlobalScoreboardMenuUI>();
            if (globalScoreboardUI != null)
            {
                Debug.Log("[MainMenu] Auto-assigned GlobalScoreboardMenuUI");
            }
            else
            {
                Debug.LogError("[MainMenu] GlobalScoreboardMenuUI not found in scene!");
            }
        }
    }

    private void TryAutoAssignSettingsUI()
    {
        if (settingsMenuUI == null || settingsMenuUI.Equals(null))
        {
            settingsMenuUI = FindFirstObjectByType<SettingsMenuUI>();
            if (settingsMenuUI != null)
            {
                Debug.Log("[MainMenu] Auto-assigned SettingsMenuUI");
            }
            else
            {
                Debug.LogWarning("[MainMenu] SettingsMenuUI not found in scene.");
            }
        }
    }

    private void TryAutoAssignMainMenuCanvasRoot()
    {
        if (mainMenuCanvasRoot != null)
        {
            return;
        }

        Transform canvas = transform.Find("Canvas");
        if (canvas != null)
        {
            mainMenuCanvasRoot = canvas.gameObject;
        }
        else
        {
            // Fallback: hide the whole menu object if no Canvas child exists.
            mainMenuCanvasRoot = gameObject;
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void OpenAchievements()
    {
        if (achievementsUI != null)
        {
            achievementsUI.RefreshReferences(); // 🔑 Key fix for first time issue
            achievementsUI.TogglePanel();
        }
        else
        {
            Debug.LogError("[MainMenu] Achievements UI reference is missing!");
        }
    }
}
