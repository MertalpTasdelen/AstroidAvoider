using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LeaderboardApiClient : MonoBehaviour
{
    public static LeaderboardApiClient Instance;
    [SerializeField] private string apiBaseUrl = "https://api.yeninesilevim.com"; // test için
    private string apiUrl => $"{apiBaseUrl}/scores/top";
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public IEnumerator FetchScores(System.Action<List<GlobalScoreEntry>> onResult)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(apiUrl))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"[LeaderboardApiClient] Failed to fetch scores: {request.responseCode} - {request.url} - {request.error}");
                onResult?.Invoke(new List<GlobalScoreEntry>());
            }
            else
            {
                string json = request.downloadHandler.text;
                ScoreListWrapper wrapper = JsonUtility.FromJson<ScoreListWrapper>(json);
                if (wrapper != null && wrapper.scores != null)
                    onResult?.Invoke(wrapper.scores);
                else
                    onResult?.Invoke(new List<GlobalScoreEntry>());
            }
        }
    }

    public IEnumerator SubmitScore(string playerName, int score)
    {
        int lastSentScore = PlayerPrefs.GetInt("LastSubmittedScore", 0);
        
        Debug.Log("[LeaderboardApiClient] Last submitted score: " + lastSentScore);


        if (score <= lastSentScore)
        {
            Debug.Log("[LeaderboardApiClient] Skor zaten gönderildi veya daha düşük, POST atlanıyor.");
            yield break;
        }

        var scoreData = new GlobalScoreEntry { playerName = playerName, score = score };
        string jsonData = JsonUtility.ToJson(scoreData);

        using (UnityWebRequest request = new UnityWebRequest("https://api.yeninesilevim.com/scores", "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"[LeaderboardApiClient] Failed to submit score: {request.responseCode} - {request.error}");
            }
            else
            {
                Debug.Log("[LeaderboardApiClient] Score submitted successfully!");
                PlayerPrefs.SetInt("LastSubmittedScore", score);
                PlayerPrefs.Save();
            }
        }
    }


    [System.Serializable]
    private class ScoreListWrapper
    {
        public List<GlobalScoreEntry> scores;
    }
}
