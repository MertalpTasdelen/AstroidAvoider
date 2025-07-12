using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AchievementApiClient : MonoBehaviour
{
    public static AchievementApiClient Instance;

    [SerializeField] private string apiBaseUrl = "https://api.yeninesilevim.com";

    private string postUrl => $"{apiBaseUrl}/achievements/progress";
    private string getUrl(string playerName) => $"{apiBaseUrl}/achievements/player/{playerName}";

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

    [Serializable]
    public class ProgressPayload
    {
        public string player_name;
        public string achievement_id;
        public int progress;

        public ProgressPayload(string name, string id, int amount)
        {
            player_name = name;
            achievement_id = id;
            progress = amount;
        }
    }

    [Serializable]
    public class PlayerAchievement
    {
        public string achievement_id;
        public int current_amount;
        public bool is_completed;
    }

    [Serializable]
    public class PlayerAchievementListWrapper
    {
        public List<PlayerAchievement> items;
    }

    // ✅ SUNUCUYA BAŞARIM İLERLEMESİ GÖNDER
    public void SubmitProgress(string achievementId, int amount = 1)
    {
        string playerName = PlayerPrefs.GetString("PlayerName", "Player");
        ProgressPayload payload = new ProgressPayload(playerName, achievementId, amount);
        string json = JsonUtility.ToJson(payload);
        StartCoroutine(SendProgressRequest(json));
    }

    private IEnumerator SendProgressRequest(string jsonBody)
    {
        using (UnityWebRequest request = new UnityWebRequest(postUrl, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"[AchievementApiClient] POST Failed: {request.responseCode} - {request.error}");
            }
            else
            {
                Debug.Log("[AchievementApiClient] Progress submitted!");
            }
        }
    }

    // ✅ OYUNCUNUN BAŞARIMLARINI GETİR
    public IEnumerator FetchPlayerAchievements(string playerName, Action<List<PlayerAchievement>> onResult)
    {
        string url = getUrl(playerName);

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.downloadHandler = new DownloadHandlerBuffer();

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"[AchievementApiClient] GET Failed: {request.responseCode} - {request.error}");
                onResult?.Invoke(new List<PlayerAchievement>());
            }
            else
            {
                string json = request.downloadHandler.text;

                try
                {
                    PlayerAchievementListWrapper wrapper = JsonUtility.FromJson<PlayerAchievementListWrapper>("{\"items\":" + json + "}");
                    onResult?.Invoke(wrapper.items);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[AchievementApiClient] JSON Parse Error: {ex.Message}");
                    onResult?.Invoke(new List<PlayerAchievement>());
                }
            }
        }
    }
}
