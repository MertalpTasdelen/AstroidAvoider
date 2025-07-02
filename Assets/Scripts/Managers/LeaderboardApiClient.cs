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

    [System.Serializable]
    private class ScoreListWrapper
    {
        public List<GlobalScoreEntry> scores;
    }
}
