using UnityEngine;

[RequireComponent(typeof(Animator))]
public class DestroyAfterAnimation : MonoBehaviour
{
    private Animator animator;
    private bool hasPlayed = false;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!hasPlayed && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        {
            hasPlayed = true;
            Destroy(gameObject);
        }
    }
}
