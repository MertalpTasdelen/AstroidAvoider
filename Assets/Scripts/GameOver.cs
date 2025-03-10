using UnityEngine;
using UnityEngine.SceneManagement; 

public class GameOverHandler : MonoBehaviour
{
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private AstreoidSpawner astreidSpwaner;
    public void EndGame()
    {
        astreidSpwaner.enabled = false;

        gameOverUI.gameObject.SetActive(true);
    }  
    
    public void RestartGame()
    {
        // Load the game scene
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }

    public void ReturnToMainMenu()
    {
        // Load the main menu scene
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}
