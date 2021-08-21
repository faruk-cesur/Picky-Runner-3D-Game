﻿using System.Collections;
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

    public IEnumerator ActivateJumpAnim()
    {
        animator.SetBool("running",false);
        animator.SetBool("jump",true);
        yield return new WaitForSeconds(1f);
        animator.SetBool("running",true);
        animator.SetBool("jump",false);
    }
    
    public IEnumerator ActivateSlideAnim()
    {
        animator.SetBool("running",false);
        animator.SetBool("slide",true);
        yield return new WaitForSeconds(1f);
        animator.SetBool("running",true);
        animator.SetBool("slide",false);
    }

    public IEnumerator ActivateWallBreakAnim()
    {
        animator.SetBool("running", false);
        animator.SetBool("wallstrike", true);
        yield return new WaitForSeconds(2f);
        animator.SetBool("running", true);
        animator.SetBool("wallstrike", false);
    }
}
