using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private GameOverHandler gameOverHandler;

    public void Crash()
    {
        // gameObject.SetActive(false);

        // gameOverHandler.EndGame();
        Debug.Log("Player has crashed!");
    }
}
