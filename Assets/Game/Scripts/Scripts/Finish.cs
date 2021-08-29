using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Finish : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponentInParent<PlayerController>();
        if (player)
        {
            player.finishCam = true;
            player.PlayerSpeedDown();
            PlayerPrefs.SetInt("TotalGold", UIManager.Instance.gold + PlayerPrefs.GetInt("TotalGold"));
            AnimationController.Instance.ActivateVictoryAnim();
            UIManager.Instance.UpdateGoldInfo();
            StartCoroutine(UIManager.Instance.DurationFinishUI());
            GameManager.Instance.CurrentGameState = GameState.FinishGame;
        }
    }
}