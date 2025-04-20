using UnityEngine;
using TMPro;
using System.Collections;

public class NearMissUIManager : MonoBehaviour
{
    public static NearMissUIManager Instance;
    [SerializeField] private TMP_Text nearMissText;

    private void Awake()
    {
        Instance = this;
        nearMissText.gameObject.SetActive(false);
    }

    public void ShowNearMiss()
    {
        StopAllCoroutines();
        StartCoroutine(FlashNearMiss());
    }

    private IEnumerator FlashNearMiss()
    {
        nearMissText.gameObject.SetActive(true);
        nearMissText.text = "Near Miss!";
        yield return new WaitForSeconds(1.2f);
        nearMissText.gameObject.SetActive(false);
    }
}
