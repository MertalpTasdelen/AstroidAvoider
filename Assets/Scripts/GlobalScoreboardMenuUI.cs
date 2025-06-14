using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class GlobalScoreboardMenuUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject scoreItemPrefab;
    [SerializeField] private Transform contentRoot;
    [SerializeField] private Button closeButton;
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private GameObject mainMenuRoot;

    private void Start()
    {
        panelRoot.SetActive(false);
        closeButton.onClick.AddListener(TogglePanel);
    }

    public void ShowLeaderboard()
    {
        PopulateScores();
        panelRoot.SetActive(true);
        mainMenuRoot.SetActive(false);
    }

    private void PopulateScores()
    {
        ClearItems();

        List<GlobalScoreEntry> topScores = GlobalScoreBoardManager.Instance.GetTopScores();

        foreach (var entry in topScores)
        {
            GameObject item = Instantiate(scoreItemPrefab, contentRoot);
            item.transform.Find("NameText").GetComponent<TMP_Text>().text = entry.playerName;
            item.transform.Find("ScoreText").GetComponent<TMP_Text>().text = entry.score.ToString();
        }
    }

    private void ClearItems()
    {
        foreach (Transform child in contentRoot)
        {
            Destroy(child.gameObject);
        }
    }

    private void TogglePanel()
    {
        panelRoot.SetActive(false);
        mainMenuRoot.SetActive(true);
        ClearItems();
    }
}
