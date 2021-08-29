using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Prepare,
    MainGame,
    GameOver,
    FinishGame,
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private GameState _currentGameState;

    public GameState CurrentGameState
    {
        get { return _currentGameState; }
        set
        {
            switch (value)
            {
                case GameState.Prepare:
                    break;
                case GameState.MainGame:
                    break;
                case GameState.GameOver:
                    break;
                case GameState.FinishGame:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }

            _currentGameState = value;
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }

        CurrentGameState = GameState.Prepare;
    }

    public void StartGame()
    {
        CurrentGameState = GameState.MainGame;
        UIManager.Instance.GamePlay();
        AnimationController.Instance.ActivateRunAnim();
    }

    public void RestartGame()
    {
        UIManager.Instance.RetryButton();
    }

    public void NextLevel()
    {
        UIManager.Instance.NextLevelButton();
    }

    public void GameOver()
    {
        StartCoroutine(UIManager.Instance.DurationGameOverUI());
    }

    public void Win()
    {
        UIManager.Instance.FinishGamePanel();
    }
}