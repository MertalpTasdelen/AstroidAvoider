using UnityEngine;

public class GameInitializerManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        if (ScoreSystem.Instance != null)
        {
            // ScoreSystem.Instance.ResetScore();
            // ScoreSystem.Instance.StartTimer();
        }

    }

}
