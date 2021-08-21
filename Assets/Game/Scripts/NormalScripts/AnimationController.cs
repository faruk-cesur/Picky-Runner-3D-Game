using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    public static AnimationController instance;

    [SerializeField] Animator animator;

    private void Awake()
    {
        if (instance != this)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    public void ActivateIdleAnim()
    {
        animator.SetBool("idle", true);
        animator.SetBool("running", false);
    }

    public void ActivateRunAnim()
    {
        animator.SetBool("idle", false);
        animator.SetBool("running", true);
    }

    public void ActivateDeathAnim()
    {
        animator.SetBool("running", false);
        animator.SetBool("death", true);
    }
    public void ActivateVictoryAnim()
    {
        animator.SetBool("running", false);
        animator.SetBool("victory", true);
    }
}
