using UnityEngine;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Image pauseButtonImage;

    [Header("Button Sprites")]
    [SerializeField] private Sprite pauseSprite;
    [SerializeField] private Sprite playSprite;

    [Header("Audio (optional)")]
    [Tooltip("Only these music sources will be paused/unpaused when the game is paused.")]
    [SerializeField] private AudioSource[] musicSources;

    private bool isPaused = false;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        // Oyun başlarken pause panelini kapat
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        TryAutoAssignMusicSources();

        // Pause butonuna listener ekle
        if (pauseButton != null)
        {
            pauseButton.onClick.AddListener(TogglePause);
        }

        // Başlangıçta pause sprite'ı göster
        UpdateButtonSprite();
    }

    public void TogglePause()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f; // Oyunu durdur

        PauseMusic();
        
        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
        }

        UpdateButtonSprite();

        // Ses efekti eklenebilir
        // AudioManager.Instance?.PlayPauseSound();
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f; // Oyunu devam ettir

        ResumeMusic();
        
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        UpdateButtonSprite();

        // Ses efekti eklenebilir
        // AudioManager.Instance?.PlayResumeSound();
    }

    private void UpdateButtonSprite()
    {
        if (pauseButtonImage != null)
        {
            if (isPaused && playSprite != null)
            {
                pauseButtonImage.sprite = playSprite;
            }
            else if (!isPaused && pauseSprite != null)
            {
                pauseButtonImage.sprite = pauseSprite;
            }
        }
    }

    public bool IsPaused()
    {
        return isPaused;
    }

    private void TryAutoAssignMusicSources()
    {
        if (musicSources != null && musicSources.Length > 0)
        {
            return;
        }

        // Best-effort: if scene has a dedicated music player, hook it automatically.
        var gameMusic = FindFirstObjectByType<GameMusicPlayer>();
        if (gameMusic != null)
        {
            var src = gameMusic.GetComponent<AudioSource>();
            if (src != null)
            {
                musicSources = new[] { src };
                return;
            }
        }

        var menuMusic = FindFirstObjectByType<MenuMusicPlayer>();
        if (menuMusic != null)
        {
            var src = menuMusic.GetComponent<AudioSource>();
            if (src != null)
            {
                musicSources = new[] { src };
            }
        }
    }

    private void PauseMusic()
    {
        if (musicSources == null)
        {
            return;
        }

        for (int i = 0; i < musicSources.Length; i++)
        {
            var src = musicSources[i];
            if (src != null && src.isPlaying)
            {
                src.Pause();
            }
        }
    }

    private void ResumeMusic()
    {
        if (musicSources == null)
        {
            return;
        }

        for (int i = 0; i < musicSources.Length; i++)
        {
            var src = musicSources[i];
            if (src != null)
            {
                src.UnPause();
            }
        }
    }

    private void OnDestroy()
    {
        // Oyun kapanırken time scale'i sıfırla
        Time.timeScale = 1f;

        // In case destroyed while paused.
        ResumeMusic();
    }
}
