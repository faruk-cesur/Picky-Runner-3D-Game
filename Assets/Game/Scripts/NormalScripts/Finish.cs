using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Finish : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponentInParent<PlayerController>();
        if (player)
        {
            Debug.Log("win");
            GameManager.Instance.CurrentGameState = GameState.FinishGame;
        }
    }
}