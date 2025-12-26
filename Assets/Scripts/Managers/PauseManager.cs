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

    private void OnDestroy()
    {
        // Oyun kapanırken time scale'i sıfırla
        Time.timeScale = 1f;
    }
}
