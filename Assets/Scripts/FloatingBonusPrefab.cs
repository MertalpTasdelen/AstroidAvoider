// FloatingBonusPrefab.cs
using UnityEngine;

public class FloatingBonusPrefab : MonoBehaviour
{
    [SerializeField] private float lifetime = 1.2f;
    [SerializeField] private Vector3 floatDirection = new Vector3(0, 60f, 0);
    [SerializeField] private float floatSpeed = 1f;

    private RectTransform rectTransform;
    private Vector3 initialPosition;
    private float timer;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        initialPosition = rectTransform.anchoredPosition;
    }

    void Update()
    {
        timer += Time.deltaTime;
        rectTransform.anchoredPosition = initialPosition + floatDirection * floatSpeed * timer;

        if (timer > lifetime)
        {
            Destroy(gameObject);
        }
    }
}
