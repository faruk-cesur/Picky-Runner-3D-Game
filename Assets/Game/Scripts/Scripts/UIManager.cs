using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public Slider energySlider;
    public GameObject energySliderObject;
    public List<GameObject> yellowStars;

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
                break;
            case GameState.GameOver:
                break;
            case GameState.FinishGame:
                break;
        }
    }

    public void PrepareGame()
    {
        energySliderObject.SetActive(false);
        prepareScene.SetActive(true);
        gamePlayScene.SetActive(false);
        gameOverScene.SetActive(false);
        finalScene.SetActive(false);
    }

    public void GamePlay()
    {
        energySliderObject.SetActive(true);
        prepareScene.SetActive(false);
        gamePlayScene.SetActive(true);
        gameOverScene.SetActive(false);
        finalScene.SetActive(false);
    }

    public void GameOverPanel()
    {
        energySliderObject.SetActive(false);
        prepareScene.SetActive(false);
        gamePlayScene.SetActive(false);
        gameOverScene.SetActive(true);
        finalScene.SetActive(false);
    }

    public void FinishGamePanel()
    {
        energySliderObject.SetActive(false);
        prepareScene.SetActive(false);
        gamePlayScene.SetActive(false);
        gameOverScene.SetActive(false);
        finalScene.SetActive(true);
    }

    public void EnergySliderStars()
    {
        for (int i = 0; i < energySlider.value; i++)
        {
            yellowStars[i].SetActive(true);
        }

        for (int i = 5; i > energySlider.value; i--)
        {
            yellowStars[i - 1].SetActive(false);
        }
    }
}