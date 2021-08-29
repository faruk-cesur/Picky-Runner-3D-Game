using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public PlayerController player;
    public Slider energySlider;
    public Slider distanceSlider;
    public GameObject energySliderObject;
    public GameObject distanceFinish;
    public TextMeshProUGUI currentGoldText;
    public TextMeshProUGUI earnedGoldText;
    public TextMeshProUGUI totalGoldText;
    public TextMeshProUGUI sliderLevelText;
    public List<GameObject> yellowStars;
    [HideInInspector] public int sliderLevel = 1;
    [HideInInspector] public int gold;

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

    private void Start()
    {
        SetGoldZeroOnStart();
        SetPlayerPrefs();
    }

    void Update()
    {
        switch (GameManager.Instance.CurrentGameState)
        {
            case GameState.Prepare:
                PrepareGame();
                UpdateGoldInfo();
                break;
            case GameState.MainGame:
                CalculateRoadDistance();
                EqualCurrentGold();
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

    private void CalculateRoadDistance()
    {
        distanceSlider.maxValue = distanceFinish.gameObject.transform.localPosition.z;
        distanceSlider.value = player.gameObject.transform.localPosition.z;
    }

    private void SetGoldZeroOnStart()
    {
        gold = 0;
    }

    private void EqualCurrentGold()
    {
        currentGoldText.text = gold.ToString();
    }

    public void UpdateGoldInfo()
    {
        earnedGoldText.text = currentGoldText.text;
        totalGoldText.text = PlayerPrefs.GetInt("TotalGold").ToString();
    }

    private void SetPlayerPrefs()
    {
        if (!PlayerPrefs.HasKey("TotalGold"))
        {
            PlayerPrefs.SetInt("TotalGold", gold);
        }

        if (!PlayerPrefs.HasKey("SliderLevel"))
        {
            PlayerPrefs.SetInt("SliderLevel", sliderLevel);
        }

        sliderLevelText.text = PlayerPrefs.GetInt("SliderLevel").ToString();
    }

    public IEnumerator DurationFinishUI()
    {
        yield return new WaitForSeconds(2f);
        FinishGamePanel();
    }

    public IEnumerator DurationGameOverUI()
    {
        yield return new WaitForSeconds(2f);
        GameOverPanel();
    }

    public void RetryButton()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }

    public void NextLevelButton()
    {
        // Bir sonraki level Instantiate edilecek. (Level Manager'dan method cagrilabilir) Simdilik Retry ekliyorum.
        RetryButton();
        PlayerPrefs.SetInt("SliderLevel", PlayerPrefs.GetInt("SliderLevel") + 1);
        sliderLevelText.text = PlayerPrefs.GetInt("SliderLevel").ToString();
    }
}