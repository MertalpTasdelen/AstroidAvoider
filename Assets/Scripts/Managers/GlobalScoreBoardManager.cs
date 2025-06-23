using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GlobalScoreBoardManager : MonoBehaviour
{
    public static GlobalScoreBoardManager Instance;

    private const string SaveKey = "GlobalScoreBoard";
    private List<GlobalScoreEntry> globalScores = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadScores();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddScore(string playerName, int score)
    {
        Debug.Log($"[LEADERBOARD] AddScore called for {playerName} → {score}");

        GlobalScoreEntry existing = globalScores.FirstOrDefault(e => e.playerName == playerName);
        if (existing != null)
        {
            if (score > existing.score)
                existing.score = score;
        }
        else
        {
            globalScores.Add(new GlobalScoreEntry { playerName = playerName, score = score });
        }

        SaveScores();
    }

    public List<GlobalScoreEntry> GetTopScores(int topN = 10)
    {
        return globalScores.OrderByDescending(e => e.score).Take(topN).ToList();
    }

    private void SaveScores()
    {
        string json = JsonUtility.ToJson(new GlobalScoreListWrapper(globalScores));
        PlayerPrefs.SetString(SaveKey, json);
        PlayerPrefs.Save();
    }

    private void LoadScores()
    {
        if (PlayerPrefs.HasKey(SaveKey))
        {
            string json = PlayerPrefs.GetString(SaveKey);
            GlobalScoreListWrapper wrapper = JsonUtility.FromJson<GlobalScoreListWrapper>(json);
            globalScores = wrapper.scores;
        }
    }

    [System.Serializable]
    private class GlobalScoreListWrapper
    {
        public List<GlobalScoreEntry> scores;

        public GlobalScoreListWrapper(List<GlobalScoreEntry> scores)
        {
            this.scores = scores;
        }
    }
}
