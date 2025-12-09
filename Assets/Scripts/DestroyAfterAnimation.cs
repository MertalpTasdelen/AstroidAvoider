using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
public class DestroyAfterAnimation : MonoBehaviour
{
    private Animator animator;
    private AudioSource audioSource;
    private bool hasTriggeredDestroy = false;

    void Awake()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        // animasyon tamamlandığında bir kez çalıştır
        if (!hasTriggeredDestroy && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        {
            hasTriggeredDestroy = true;
            // ses kaynağı varsa ses süresi kadar gecikmeli yok et
            float delay = 0f;
            if (audioSource != null && audioSource.clip != null)
            {
                delay = audioSource.clip.length - audioSource.time;
            }
            Destroy(gameObject, Mathf.Max(0f, delay));
        }
    }
}
