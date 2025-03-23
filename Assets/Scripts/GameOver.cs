using UnityEngine;
using UnityEngine.SceneManagement; 
using TMPro;

public class GameOverHandler : MonoBehaviour
{
    [SerializeField] private TMP_Text gameOverText;
    [SerializeField] private ScoreSystem scoreSystem;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private AstreoidSpawner astreidSpwaner;
    public void EndGame()
    {
        astreidSpwaner.enabled = false;

        //PAUSE THE SCORE WHEN THE PLAYER CRASH
        scoreSystem.PauseScore();

        gameOverText.text = "Game Over\nScore: " + Mathf.FloorToInt(scoreSystem.GetScore()).ToString();
        //DIPLAY THE SCORE ON THE UI
        scoreSystem.gameObject.SetActive(false);

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
