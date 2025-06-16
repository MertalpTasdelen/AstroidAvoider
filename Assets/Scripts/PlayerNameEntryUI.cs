using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class PlayerNameEntryUI : MonoBehaviour
{
    private GameObject panelRoot;
    private TMP_InputField nameInput;
    private Button submitButton;
    private TMP_Text titleText;
    private Action onCompleteCallback;

    private void Awake()
    {
        FindStaticReferences();
    }

    private void FindStaticReferences()
    {
        GameObject nameEntryMenu = GameObject.Find("PlayerNameEntryPanel");
        if (nameEntryMenu != null)
        {
            Transform canvas = nameEntryMenu.transform.Find("Canvas");
            if (canvas != null)
            {
                panelRoot = canvas.gameObject;

                nameInput = canvas.GetComponentInChildren<TMP_InputField>(true);
                submitButton = canvas.GetComponentInChildren<Button>(true);
                titleText = canvas.GetComponentInChildren<TMP_Text>(true);
            }
            else
            {
                Debug.LogError("[PlayerNameEntryUI] Canvas not found under PlayerNameEntryPanel.");
            }
        }
    }

    public void HidePanel()
    {
        if (panelRoot != null)
            panelRoot.SetActive(false);
    }
    public void RefreshReferences()
    {
        if (panelRoot == null)
            FindStaticReferences();

        if (submitButton != null)
        {
            submitButton.onClick.RemoveAllListeners();
            submitButton.onClick.AddListener(SavePlayerName);
        }
    }

    public void ShowInitialEntry(Action onComplete)
    {
        onCompleteCallback = onComplete;

        panelRoot.SetActive(true);
        nameInput.text = "";
    }

    private void SavePlayerName()
    {
        string enteredName = nameInput.text.Trim();

        if (!string.IsNullOrEmpty(enteredName))
        {
            PlayerPrefs.SetString("PlayerName", enteredName);
            PlayerPrefs.Save();
            Debug.Log($"[PlayerNameEntryUI] Player name saved: {enteredName}");

            panelRoot.SetActive(false);
            onCompleteCallback?.Invoke();
        }
    }
}
