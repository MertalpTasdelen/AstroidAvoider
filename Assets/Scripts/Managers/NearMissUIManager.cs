using UnityEngine;
using TMPro;
using System.Collections;

public class NearMissUIManager : MonoBehaviour
{
    public static NearMissUIManager Instance;
    [SerializeField] private TMP_Text nearMissText;

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        Instance = this;

        canvasGroup = nearMissText.GetComponent<CanvasGroup>();
        if (canvasGroup != null)
            canvasGroup.alpha = 0f;
    }

    public void ShowNearMiss()
    {
        StopAllCoroutines();
        StartCoroutine(FlashNearMiss());
    }

    private IEnumerator FlashNearMiss()
    {
        if (canvasGroup == null) yield break;

        nearMissText.text = "Near Miss!";
        canvasGroup.alpha = 1f;

        yield return new WaitForSeconds(1.2f);

        canvasGroup.alpha = 0f;
    }
}
