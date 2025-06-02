using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class AchievementMenuUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject achievementItemPrefab; // Bizim oluşturduğumuz prefab
    [SerializeField] private Transform contentContainer; // Scroll View içindeki Content
    [SerializeField] private GameObject panelRoot; // AchievementPanel kökü
    [SerializeField] private GameObject mainMenuRoot;

    private void Start()
    {
        Debug.Log("AchievementMenuUI Start called");
        panelRoot.SetActive(false);
    }

    public void TogglePanel()
    {
        Debug.Log("TogglePanel called");
        if (panelRoot.activeSelf)
        {
            Debug.Log("Panel is active, closing it");
            panelRoot.SetActive(false);
            mainMenuRoot.SetActive(true);
            ClearAchievements();
        }
        else
        {
            Debug.Log("Panel is not active, opening it");
            panelRoot.SetActive(true);
            mainMenuRoot.SetActive(false);
            PopulateAchievements();
        }
    }

    private void PopulateAchievements()
    {
        ClearAchievements();

        List<AchievementData> achievements = AchievementManager.Instance.GetAchievements(false);

        foreach (var data in achievements)
        {
            GameObject item = Instantiate(achievementItemPrefab, contentContainer);

            // --- Data Bağlama ---
            var titleText = item.transform.Find("TitleText").GetComponent<TextMeshProUGUI>();
            var descriptionText = item.transform.Find("DescriptionText").GetComponent<TextMeshProUGUI>();
            var starIcon = item.transform.Find("Icon").GetComponent<Image>();
            var canvasGroup = item.GetComponent<CanvasGroup>();

            titleText.text = data.title;
            descriptionText.text = data.description;

            if (data.isCompleted)
            {
                canvasGroup.alpha = 1f;
                starIcon.color = Color.yellow; // Başarılmış görevlerde yıldız sarı
            }
            else
            {
                canvasGroup.alpha = 0.4f;
                starIcon.color = Color.white; // Tamamlanmamış görevlerde yıldız beyaz
            }
        }
    }

    private void ClearAchievements()
    {
        foreach (Transform child in contentContainer)
        {
            Destroy(child.gameObject);
        }
    }
    
    public GameObject PanelRoot
    {
        get { return panelRoot; }
        set { panelRoot = value; }
    }

    public GameObject MainMenuRoot
    {
        get { return mainMenuRoot; }
        set { mainMenuRoot = value; }
    }

}
