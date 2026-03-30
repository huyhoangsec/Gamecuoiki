using System.Collections;
using UnityEngine;

public class DestroyWhenAnimationFinished : MonoBehaviour
{
    private Animator animator;

    void OnEnable()
    {
        animator = GetComponent<Animator>();
        StartCoroutine("Deactivate");
    }

    IEnumerator Deactivate()
    {
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        gameObject.SetActive(false);
    }
}