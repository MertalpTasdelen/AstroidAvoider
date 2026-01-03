using UnityEngine;
using UnityEngine.UI;

public class SettingsMenuUI : MonoBehaviour
{
    [Header("Panel Root")]
    [SerializeField] private GameObject panelRoot;

    [Header("Behavior")]
    [SerializeField] private bool startHidden = true;

    [Header("Controls")]
    [SerializeField] private Slider sensitivitySlider;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Toggle hapticsToggle;

    [Header("Apply Targets (optional)")]
    [SerializeField] private SettingsApplier settingsApplier;

    private bool isBinding;

    private void Awake()
    {
        if (panelRoot == null)
        {
            panelRoot = gameObject;
        }
    }

    private void Start()
    {
        BindUI();
        RefreshUIFromSettings();

        if (startHidden)
        {
            Hide();
        }
    }

    public void Show()
    {
        if (panelRoot != null)
        {
            panelRoot.SetActive(true);
        }

        RefreshUIFromSettings();
    }

    public void Hide()
    {
        if (panelRoot != null)
        {
            panelRoot.SetActive(false);
        }
    }

    public void TogglePanel()
    {
        if (panelRoot == null)
        {
            return;
        }

        bool next = !panelRoot.activeSelf;
        panelRoot.SetActive(next);

        if (next)
        {
            RefreshUIFromSettings();
        }
    }

    private void BindUI()
    {
        if (isBinding)
        {
            return;
        }

        isBinding = true;

        if (sensitivitySlider != null)
        {
            sensitivitySlider.onValueChanged.RemoveAllListeners();
            sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);
        }

        if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.RemoveAllListeners();
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        }

        if (hapticsToggle != null)
        {
            hapticsToggle.onValueChanged.RemoveAllListeners();
            hapticsToggle.onValueChanged.AddListener(OnHapticsChanged);
        }

        isBinding = false;
    }

    private void RefreshUIFromSettings()
    {
        if (sensitivitySlider != null)
        {
            sensitivitySlider.minValue = GameSettings.MinSensitivity;
            sensitivitySlider.maxValue = GameSettings.MaxSensitivity;
            sensitivitySlider.value = GameSettings.GetSensitivity();
        }

        if (volumeSlider != null)
        {
            volumeSlider.minValue = 0f;
            volumeSlider.maxValue = 1f;
            volumeSlider.value = GameSettings.GetMasterVolume();
        }

        if (hapticsToggle != null)
        {
            hapticsToggle.isOn = GameSettings.GetHapticsEnabled();
        }
    }

    private void OnSensitivityChanged(float value)
    {
        GameSettings.SetSensitivity(value);
        ApplySettings();
    }

    private void OnVolumeChanged(float value)
    {
        GameSettings.SetMasterVolume(value);
        ApplySettings();
    }

    private void OnHapticsChanged(bool enabled)
    {
        GameSettings.SetHapticsEnabled(enabled);
        ApplySettings();

        // Small confirmation vibration if enabling.
        if (enabled)
        {
            HapticsManager.Vibrate();
        }
    }

    private void OnRightSideControlsChanged(bool isRight)
    {
        GameSettings.SetControlSide(isRight ? GameSettings.ControlSide.Right : GameSettings.ControlSide.Left);
        ApplySettings();
    }

    private void ApplySettings()
    {
        // Always apply global volume.
        AudioListener.volume = GameSettings.GetMasterVolume();

        // Optional scene-specific applier.
        if (settingsApplier != null)
        {
            settingsApplier.ApplyAll();
        }
    }
}
