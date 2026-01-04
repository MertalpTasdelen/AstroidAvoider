using System.Collections;
using UnityEngine;

public static class HapticsManager
{
    private class Runner : MonoBehaviour { }

    private static Runner runner;

    private static Runner GetRunner()
    {
        if (runner != null)
            return runner;

        var go = new GameObject("HapticsManager");
        go.hideFlags = HideFlags.HideInHierarchy;
        Object.DontDestroyOnLoad(go);
        runner = go.AddComponent<Runner>();
        return runner;
    }

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

    public static void VibratePulses(int pulseCount, float intervalSeconds = 0.12f)
    {
        if (!GameSettings.GetHapticsEnabled())
            return;

        if (pulseCount <= 1)
        {
            Vibrate();
            return;
        }

        GetRunner().StartCoroutine(VibrateRoutine(pulseCount, intervalSeconds));
    }

    private static IEnumerator VibrateRoutine(int pulseCount, float intervalSeconds)
    {
        pulseCount = Mathf.Clamp(pulseCount, 1, 5);
        intervalSeconds = Mathf.Clamp(intervalSeconds, 0.05f, 0.5f);

        for (int i = 0; i < pulseCount; i++)
        {
            Vibrate();
            if (i < pulseCount - 1)
                yield return new WaitForSecondsRealtime(intervalSeconds);
        }
    }
}
