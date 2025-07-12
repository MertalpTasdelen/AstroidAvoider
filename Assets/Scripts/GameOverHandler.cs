using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class GameOverHandler : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private Button continueButton;
    [SerializeField] private TMP_Text gameOverText;
    [SerializeField] private ScoreSystem scoreSystem;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private AstreoidSpawner astreidSpwaner;
    public void EndGame()
    {
        astreidSpwaner.enabled = false;

        var scoreRef = scoreSystem != null ? scoreSystem : ScoreSystem.Instance;
        if (scoreRef != null)
        {
            scoreRef.PauseScore();

            int finalScore = Mathf.FloorToInt(scoreRef.GetScore());
            int highScore = scoreRef.GetHighScore();

            gameOverText.text = $"Game Over\nScore: {finalScore}\nHigh Score: {highScore}";

            string playerName = PlayerPrefs.GetString("PlayerName", "Player");

            if (LeaderboardApiClient.Instance != null)
            {
                StartCoroutine(LeaderboardApiClient.Instance.SubmitScore(playerName, highScore));
            }
            
            AchievementApiClient.Instance?.SubmitProgress("session_5", 1);
            AchievementApiClient.Instance?.SubmitProgress("session_20", 1);
        }
        else
        {
            gameOverText.text = "Game Over\nScore: 0\nHigh Score: 0";
        }

        gameOverUI.SetActive(true);
    }




    public void RestartGame()
    {
        ScoreSystem.Instance.ResetScore();
        ScoreSystem.Instance.StartTimer();
        DifficultyManager.Instance?.ResetDifficulty();
        // Load the game scene
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }

    public void CotinueButton()
    {
        // Load the game scene
        AdManager.Instance.ShowAd(this);
        continueButton.interactable = false;
    }

    public void ReturnToMainMenu()
    {
        // Load the main menu scene
        if (ScoreSystem.Instance != null)
        {
            Destroy(ScoreSystem.Instance.gameObject);
            DifficultyManager.Instance?.ResetDifficulty();

        }

        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    public void ContinueGame()
    {
        player.transform.position = Vector3.zero;
        player.SetActive(true);
        player.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;

        astreidSpwaner.enabled = true;
        gameOverUI.gameObject.SetActive(false);

        scoreSystem.gameObject.SetActive(true);
        scoreSystem.StartTimer();
    }
}
