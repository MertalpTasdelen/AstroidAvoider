using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class AchievementMenuUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject achievementItemPrefab;
    [SerializeField] private Transform contentContainer;
    [SerializeField] private GameObject panelRoot;

    private void Start()
    {
        panelRoot.SetActive(false); // Panel ilk başta kapalı
    }

    public void TogglePanel()
    {
        if (panelRoot.activeSelf)
        {
            panelRoot.SetActive(false);
            ClearAchievements();
        }
        else
        {
            panelRoot.SetActive(true);
            PopulateAchievements();
        }
    }

    private void PopulateAchievements()
    {
        ClearAchievements();

        List<AchievementData> achievements = AchievementManager.Instance.GetAchievements(false);

        Debug.Log($"Number of achievements: {achievements.Count}");

        foreach (AchievementData data in achievements)
        {
            GameObject item = Instantiate(achievementItemPrefab, contentContainer);
            TextMeshProUGUI titleText = item.transform.Find("TitleText").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI descriptionText = item.transform.Find("DescriptionText").GetComponent<TextMeshProUGUI>();
            Image checkmark = item.transform.Find("Checkmark").GetComponent<Image>();
            CanvasGroup canvasGroup = item.GetComponent<CanvasGroup>();

            titleText.text = data.title;
            descriptionText.text = data.description;
            checkmark.enabled = data.isCompleted;
            canvasGroup.alpha = data.isCompleted ? 1f : 0.4f;
        }
    }

    private void ClearAchievements()
    {
        foreach (Transform child in contentContainer)
        {
            Destroy(child.gameObject);
        }
    }
}
