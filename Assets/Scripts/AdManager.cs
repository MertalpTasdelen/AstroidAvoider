using UnityEngine;
using UnityEngine.Advertisements;

public class AdManager : MonoBehaviour, IUNITYAdsInitializationListener
{
    [SerializeField] private string andrioidGameID;
    [SerializeField] private string iosGameID;
    [SerializeField] private bool testMode = true;

    [SerializeField] private string androidAdUnitID;
    [SerializeField] private string iosAdUnitID;
    public static AdManager Instance { get; private set; }
    private string gameID;
    private string adUnitID;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void InitiliazeAds()
    {
        #if UNITY_IOS
            gameID = iosGameID;
            adUnitID = iosAdUnitID;
        #elif UNITY_ANDROID
            gameID = andrioidGameID;
            adUnitID = androidAdUnitID;
        #elif UNITY_EDITOR
            gameID = iosGameID;
            adUnitID = iosAdUnitID;
        #endif
    
        if(!Advertisement.isInitialized)
        {
            Advertisement.Initialize(gameID, testMode, this);
        }
    }

    public void OnInitializationComplete()
    {
        Debug.Log("Unity Ads initialization complete.");
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
    }
}
