using UnityEngine;

public static class HapticsManager
{
    public static void Vibrate()
    {
        if (!GameSettings.GetHapticsEnabled())
        {
            return;
        }

#if UNITY_ANDROID || UNITY_IOS
        Handheld.Vibrate();
#endif
    }
}
