using UnityEngine;

public class SettingsApplier : MonoBehaviour
{
    [Header("Optional")]
    [SerializeField] private ControlLayoutApplier controlLayoutApplier;

    private void Awake()
    {
        ApplyAll();
    }

    public void ApplyAll()
    {
        AudioListener.volume = GameSettings.GetMasterVolume();

        if (controlLayoutApplier != null)
        {
            controlLayoutApplier.Apply(GameSettings.GetControlSide());
        }
    }
}
