using System;
using System.Collections.Generic;
using UnityEngine;

public enum AchievementType
{
    TimeSurvived,
    NearMiss,
    AsteroidsDodged,
    DifficultyReached,
    CoinsCollected,
    SessionsPlayed
}

[Serializable]
public class AchievementData
{
    public string id;
    public string title;
    public string description;
    public int targetAmount;
    public int currentAmount;
    public bool isCompleted;
    public bool isDaily;
    public AchievementType type;
}

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager Instance;

    [SerializeField] private List<AchievementData> achievements = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadAchievements();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ReportProgress(AchievementType type, int amount = 1)
    {
        foreach (var achievement in achievements)
        {
            if (achievement.type != type || achievement.isCompleted) continue;

            achievement.currentAmount += amount;

            if (achievement.currentAmount >= achievement.targetAmount)
            {
                achievement.currentAmount = achievement.targetAmount;
                achievement.isCompleted = true;
                OnAchievementCompleted(achievement);
            }
        }

        SaveAchievements();
    }

    private void OnAchievementCompleted(AchievementData achievement)
    {
        Debug.Log($"🏆 Achievement Completed: {achievement.title}");
        // TODO: Trigger feedback system (UI, SFX, etc.)
    }

    public List<AchievementData> GetAchievements(bool onlyDaily = false)
    {
        return achievements.FindAll(a => a.isDaily == onlyDaily);
    }

    private void LoadAchievements()
    {
        // TODO: Load from PlayerPrefs/JSON. This is a temporary hardcoded list.
        achievements = new List<AchievementData>
        {
            new AchievementData { id = "survive_60", title = "Stay Alive!", description = "Survive 60 seconds", targetAmount = 60, isDaily = true, type = AchievementType.TimeSurvived },
            new AchievementData { id = "near_3", title = "Close Call", description = "Perform 3 near misses", targetAmount = 3, isDaily = true, type = AchievementType.NearMiss },
            new AchievementData { id = "total_asteroids", title = "Dodger", description = "Avoid 1000 asteroids", targetAmount = 1000, isDaily = false, type = AchievementType.AsteroidsDodged },
            new AchievementData { id = "difficulty_10", title = "Elite Pilot", description = "Reach difficulty level 10", targetAmount = 10, isDaily = false, type = AchievementType.DifficultyReached },
        };
    }

    private void SaveAchievements()
    {
        // TODO: Save logic (PlayerPrefs or JSON)
    }
}
