using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// - Backend'i değiştirmeden çalışır.
/// - /achievements/global endpoint'inden başarımların tanımlarını (id, title, description, target_amount...) bir kez indirir ve cache'ler.
/// - /achievements/player/{playerName} ile kullanıcının ilerlemesini çeker.
/// - UI için: id → title / description / target lookupları ve güvenli yardımcı metodlar sağlar.
/// </summary>
public class AchievementApiClient : MonoBehaviour
{
    public static AchievementApiClient Instance;

    [SerializeField] private string apiBaseUrl = "https://api.yeninesilevim.com";

    // Değiştirmiyoruz: mevcut endpoint'ler
    private string postUrl => $"{apiBaseUrl}/achievements/progress";
    private string getPlayerUrl(string playerName) => $"{apiBaseUrl}/achievements/player/{playerName}";
    private string getGlobalUrl => $"{apiBaseUrl}/achievements/global";

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

    // -------------------- PAYLOAD & DTO'lar --------------------
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
    private class PlayerAchievementListWrapper
    {
        public List<PlayerAchievement> items;
    }

    [Serializable]
    public class AchievementDef
    {
        public string id;
        public string title;
        public string description;
        public int target_amount;
        public string type;
        public bool is_daily;
    }

    // -------------------- CACHE --------------------
    private readonly Dictionary<string, AchievementDef> defsById = new Dictionary<string, AchievementDef>();
    private bool defsLoaded = false;
    private bool defsLoading = false;

    /// <summary>
    /// Global tanımları bir kez indirir ve cache'ler.
    /// UI veya başka sınıflar bu metodun ardından GetTitle/GetDescription/GetTarget gibi yardımcıları güvenle kullanabilir.
    /// </summary>
    public IEnumerator EnsureDefinitionsLoaded(Action onReady = null)
    {
        if (defsLoaded) { onReady?.Invoke(); yield break; }
        if (defsLoading)
        {
            // Bir başka yer zaten indiriyorsa kısa bir bekleme döngüsü
            while (defsLoading) yield return null;
            onReady?.Invoke(); yield break;
        }

        defsLoading = true;

        using (UnityWebRequest request = UnityWebRequest.Get(getGlobalUrl))
        {
            request.downloadHandler = new DownloadHandlerBuffer();
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogWarning($"[AchievementApiClient] Global achievements fetch failed: {request.responseCode} - {request.error}");
                // Yine de UI akmasın diye 'loaded' işaretliyoruz; bulunamayan id'ler için graceful fallback yapacağız.
                defsLoaded = true;
                defsLoading = false;
                onReady?.Invoke();
                yield break;
            }

            try
            {
                // /achievements/global -> JSON array
                string json = request.downloadHandler.text;
                AchievementDef[] list = JsonHelper.FromJsonArray<AchievementDef>(json);
                defsById.Clear();
                if (list != null)
                {
                    foreach (var def in list)
                    {
                        if (def != null && !string.IsNullOrEmpty(def.id))
                            defsById[def.id] = def;
                    }
                }
                defsLoaded = true;
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[AchievementApiClient] Global JSON parse error: {ex.Message}");
                defsLoaded = true; // fallback'li ilerleyelim
            }
            finally
            {
                defsLoading = false;
                onReady?.Invoke();
            }
        }
    }

    // -------------------- Yardımcı Erişimciler --------------------
    public string GetTitleFor(string achievementId)
    {
        if (!string.IsNullOrEmpty(achievementId) && defsById.TryGetValue(achievementId, out var def))
            return string.IsNullOrWhiteSpace(def.title) ? achievementId : def.title;
        // fallback: id yaz (backend ön yüzü bozulmasın)
        return achievementId;
    }

    public string GetDescriptionFor(string achievementId)
    {
        if (!string.IsNullOrEmpty(achievementId) && defsById.TryGetValue(achievementId, out var def))
            return def.description ?? "";
        return "";
    }

    public int? GetTargetFor(string achievementId)
    {
        if (!string.IsNullOrEmpty(achievementId) && defsById.TryGetValue(achievementId, out var def))
            return def.target_amount;
        return null;
    }

    // -------------------- Progress gönder --------------------
    public void SubmitProgress(string achievementId, int amount = 1)
    {
        string playerName = PlayerPrefs.GetString("PlayerName", "Player");
        var payload = new ProgressPayload(playerName, achievementId, amount);
        StartCoroutine(SendProgressRequest(JsonUtility.ToJson(payload)));
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
                Debug.LogError($"[AchievementApiClient] POST Failed: {request.responseCode} - {request.error}");
            else
                Debug.Log("[AchievementApiClient] Progress submitted!");
        }
    }

    // -------------------- Oyuncu başarımları --------------------
    public IEnumerator FetchPlayerAchievements(string playerName, Action<List<PlayerAchievement>> onResult)
    {
        string url = getPlayerUrl(playerName);

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.downloadHandler = new DownloadHandlerBuffer();
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"[AchievementApiClient] GET Failed: {request.responseCode} - {request.error}");
                onResult?.Invoke(new List<PlayerAchievement>());
                yield break;
            }

            string json = request.downloadHandler.text;
            try
            {
                // Backend dizi döndürüyor → wrapper içine sarıyoruz.
                var wrapper = JsonUtility.FromJson<PlayerAchievementListWrapper>("{\"items\":" + json + "}");
                onResult?.Invoke(wrapper?.items ?? new List<PlayerAchievement>());
            }
            catch (Exception ex)
            {
                Debug.LogError($"[AchievementApiClient] JSON Parse Error: {ex.Message}");
                onResult?.Invoke(new List<PlayerAchievement>());
            }
        }
    }

    // -------------------- Küçük JSON array helper --------------------
    private static class JsonHelper
    {
        [Serializable] private class Wrapper<T> { public T[] Items; }
        public static T[] FromJsonArray<T>(string json)
        {
            // Unity'nin JsonUtility'si düz dizileri sevmiyor; dolayısıyla sarıyoruz.
            string wrapped = "{\"Items\":" + json + "}";
            var w = JsonUtility.FromJson<Wrapper<T>>(wrapped);
            return w?.Items;
        }
    }
}
