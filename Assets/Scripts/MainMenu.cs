using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private AchievementMenuUI achievementsUI;

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
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        TryAutoAssignAchievementUI();
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
