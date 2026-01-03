using UnityEngine;

public static class GameSettings
{
    public const string KeySensitivity = "settings_sensitivity";
    public const string KeyHapticsEnabled = "settings_haptics_enabled";
    public const string KeyMasterVolume = "settings_master_volume";
    public const string KeyControlSide = "settings_control_side";

    public const float DefaultSensitivity = 1f;
    public const float MinSensitivity = 0.5f;
    public const float MaxSensitivity = 2f;

    public const float DefaultMasterVolume = 1f;

    public enum ControlSide
    {
        Left = 0,
        Right = 1,
    }

    public static float GetSensitivity()
    {
        float value = PlayerPrefs.GetFloat(KeySensitivity, DefaultSensitivity);
        return Mathf.Clamp(value, MinSensitivity, MaxSensitivity);
    }

    public static void SetSensitivity(float value)
    {
        float clamped = Mathf.Clamp(value, MinSensitivity, MaxSensitivity);
        PlayerPrefs.SetFloat(KeySensitivity, clamped);
        PlayerPrefs.Save();
    }

    public static bool GetHapticsEnabled()
    {
        return PlayerPrefs.GetInt(KeyHapticsEnabled, 1) == 1;
    }

    public static void SetHapticsEnabled(bool enabled)
    {
        PlayerPrefs.SetInt(KeyHapticsEnabled, enabled ? 1 : 0);
        PlayerPrefs.Save();
    }

    public static float GetMasterVolume()
    {
        float value = PlayerPrefs.GetFloat(KeyMasterVolume, DefaultMasterVolume);
        return Mathf.Clamp01(value);
    }

    public static void SetMasterVolume(float value)
    {
        PlayerPrefs.SetFloat(KeyMasterVolume, Mathf.Clamp01(value));
        PlayerPrefs.Save();
    }

    public static ControlSide GetControlSide()
    {
        int raw = PlayerPrefs.GetInt(KeyControlSide, (int)ControlSide.Left);
        if (raw != (int)ControlSide.Left && raw != (int)ControlSide.Right)
        {
            raw = (int)ControlSide.Left;
        }
        return (ControlSide)raw;
    }

    public static void SetControlSide(ControlSide side)
    {
        PlayerPrefs.SetInt(KeyControlSide, (int)side);
        PlayerPrefs.Save();
    }
}
