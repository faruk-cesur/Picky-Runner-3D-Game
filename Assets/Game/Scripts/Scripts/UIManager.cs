using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public Slider specialPowerSlider;

    [SerializeField] private GameObject prepareScene, gamePlayScene, gameOverScene, finalScene;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(Instance);
        }
    }

    void Update()
    {
        switch (GameManager.Instance.CurrentGameState)
        {
            case GameState.Prepare:
                PrepareGame();
                break;
            case GameState.MainGame:
                GamePlay();
                break;
            case GameState.GameOver:
                break;
            case GameState.FinishGame:
                break;
        }
    }

    public void PrepareGame()
    {
        prepareScene.SetActive(true);
        gamePlayScene.SetActive(false);
        gameOverScene.SetActive(false);
        finalScene.SetActive(false);
    }

    public void GamePlay()
    {
        prepareScene.SetActive(false);
        gamePlayScene.SetActive(true);
        gameOverScene.SetActive(false);
        finalScene.SetActive(false);
    }

    public void GameOverPanel()
    {
        prepareScene.SetActive(false);
        gamePlayScene.SetActive(false);
        gameOverScene.SetActive(true);
        finalScene.SetActive(false);
    }

    public void FinishGamePanel()
    {
        prepareScene.SetActive(false);
        gamePlayScene.SetActive(false);
        gameOverScene.SetActive(false);
        finalScene.SetActive(true);
    }
}