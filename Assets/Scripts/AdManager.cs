using UnityEngine;
using UnityEngine.Advertisements;

public class AdManager : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [SerializeField] private string andrioidGameID;
    [SerializeField] private string iosGameID;
    [SerializeField] private bool testMode = true;

    [SerializeField] private string androidAdUnitID;
    [SerializeField] private string iosAdUnitID;
    public static AdManager Instance { get; private set; }
    private string gameID;
    private string adUnitID;
    private GameOverHandler gameOverHandler;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }else
        {
            InitiliazeAds();
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

    public void OnUnityAdsAdLoaded(string placementId)
    {
        Advertisement.Show(placementId, this);
        Debug.Log($"Unity Ads Loaded: {placementId}");
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        Debug.Log($"Unity Ads Failed to Load: {adUnitID} - {error.ToString()} - {message}");
    }
    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        Debug.Log($"Unity Ads Show Failed: {adUnitID} - {error.ToString()} - {message}");
    }

    public void OnUnityAdsShowStart(string placementId)
    {
        Debug.Log($"Unity Ads Show Started: {adUnitID}");
    }
    public void OnUnityAdsShowClick(string placementId)
    {
        Debug.Log($"Unity Ads Show Clicked: {adUnitID}");
    }
    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        if(placementId == adUnitID && showCompletionState == UnityAdsShowCompletionState.COMPLETED)
        {
            gameOverHandler.ContinueGame();
        }
    }

    public void ShowAd(GameOverHandler gameOverHandler)
    {
        this.gameOverHandler = gameOverHandler;

        Advertisement.Load(adUnitID, this);
    }
}
