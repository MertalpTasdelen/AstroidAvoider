using UnityEngine;
using TMPro;

public class ScoreSystem : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private float scoreMultiplier;
    private float score;
    private bool isCrashed;

    public float AddScore()
    {
        if(!isCrashed){
            score += Time.deltaTime * scoreMultiplier;
        }
        return score;
    }

    public void PauseScore()
    {
        isCrashed = true;
    }

    public float GetScore(){
        return score;
    }

    void Update()
    {
        scoreText.text = Mathf.FloorToInt(AddScore()).ToString();
    }
}

