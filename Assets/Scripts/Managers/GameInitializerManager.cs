using UnityEngine;

public class GameInitializerManager : MonoBehaviour
{
    private PlayerNameEntryUI playerNameEntryUI;
    private MainMenu mainMenu;

    void Start()
    {
        playerNameEntryUI = FindFirstObjectByType<PlayerNameEntryUI>();

        // ✅ MainMenu'yu bulana kadar biraz beklemek gerekiyor
        Invoke(nameof(FindMainMenuAfterDelay), 0.1f);
    }

    void FindMainMenuAfterDelay()
    {
        mainMenu = FindFirstObjectByType<MainMenu>();

        if (mainMenu != null)
            mainMenu.gameObject.SetActive(false);

        if (!PlayerPrefs.HasKey("PlayerName"))
        {
            if (playerNameEntryUI != null)
            {
                playerNameEntryUI.RefreshReferences();
                playerNameEntryUI.ShowInitialEntry(OnUsernameEntered);
            }
        }
        else
        {
            ShowMainMenu();
        }
    }

    private void OnUsernameEntered()
    {
        ShowMainMenu();
    }

    private void ShowMainMenu()
    {
        if (playerNameEntryUI != null)
            playerNameEntryUI.HidePanel();

        if (mainMenu != null)
        {
            mainMenu.gameObject.SetActive(true);

            // 🎯 Canvas'ı da aktif ediyoruz
            Transform canvas = mainMenu.transform.Find("Canvas");
            if (canvas != null)
            {
                canvas.gameObject.SetActive(true);
            }
        }
    }
}
