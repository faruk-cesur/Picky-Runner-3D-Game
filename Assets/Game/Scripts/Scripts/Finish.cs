using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Finish : MonoBehaviour
{
    public PlayerController player;

    [SerializeField] private Slider slider;
    [SerializeField] private Transform currentPlayerPos;

    private float playerStartPosZ, finishLinePosZ, totalPathLength;

    private void Start()
    {
        GetStartingPositions();
    }

    private void Update()
    {
        switch (GameManager.Instance.CurrentGameState)
        {
            case GameState.Prepare:
                break;
            case GameState.MainGame:
                CalculatePlayerDistance();
                break;
            case GameState.GameOver:
                break;
            case GameState.FinishGame:
                break;
        }
    }

    private void GetStartingPositions()
    {
        playerStartPosZ = currentPlayerPos.position.z;
        finishLinePosZ = transform.position.z;
        totalPathLength = finishLinePosZ - playerStartPosZ;
    }

    private void CalculatePlayerDistance()
    {
        float completedPathRatio = currentPlayerPos.position.z / totalPathLength;
        slider.value = completedPathRatio;
        if (completedPathRatio >= 1f)
        {
            player.PlayerSpeedDown();
            UIManager.Instance.FinishGamePanel();
            AnimationController.Instance.ActivateVictoryAnim();
            GameManager.Instance.CurrentGameState = GameState.FinishGame;
        }
    }
}