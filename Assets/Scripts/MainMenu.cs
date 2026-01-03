using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private AchievementMenuUI achievementsUI;
    [SerializeField] private GlobalScoreboardMenuUI globalScoreboardUI;
    [SerializeField] private SettingsMenuUI settingsMenuUI;


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
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        TryAutoAssignAchievementUI();
        TryAutoAssignLeaderboardUI();
        TryAutoAssignSettingsUI();
    }

    public void OpenSettings()
    {
        if (settingsMenuUI == null || settingsMenuUI.Equals(null))
        {
            TryAutoAssignSettingsUI();
        }

        settingsMenuUI?.TogglePanel();
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
