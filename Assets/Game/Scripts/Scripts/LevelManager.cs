using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

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
    }

    public GameObject level1, level2, level3, level4, level5, level6, level7, level8;

    public int currentLevel;

    private void Start()
    {
        //PlayerPrefs.DeleteAll();
        SetLevelPlayerPrefs();
        CallLevel();
    }

    public void SetLevelPlayerPrefs()
    {
        if (!PlayerPrefs.HasKey("CurrentLevel"))
        {
            currentLevel = 1;
            PlayerPrefs.SetInt("CurrentLevel", currentLevel);
        }
    }


    public void CallLevel()
    {
        if (PlayerPrefs.GetInt("CurrentLevel") == 1)
        {
            currentLevel = 1;
            PlayerPrefs.SetInt("CurrentLevel", currentLevel);
            level1.SetActive(true);
        }

        if (PlayerPrefs.GetInt("CurrentLevel") == 2)
        {
            currentLevel = 2;
            PlayerPrefs.SetInt("CurrentLevel", currentLevel);
            level2.SetActive(true);
            level1.SetActive(false);
        }

        if (PlayerPrefs.GetInt("CurrentLevel") == 3)
        {
            currentLevel = 3;
            PlayerPrefs.SetInt("CurrentLevel", currentLevel);
            level3.SetActive(true);
            level2.SetActive(false);
        }

        if (PlayerPrefs.GetInt("CurrentLevel") == 4)
        {
            currentLevel = 4;
            PlayerPrefs.SetInt("CurrentLevel", currentLevel);
            level4.SetActive(true);
            level3.SetActive(false);
        }

        if (PlayerPrefs.GetInt("CurrentLevel") == 5)
        {
            currentLevel = 5;
            PlayerPrefs.SetInt("CurrentLevel", currentLevel);
            level5.SetActive(true);
            level4.SetActive(false);
        }

        if (PlayerPrefs.GetInt("CurrentLevel") == 6)
        {
            currentLevel = 6;
            PlayerPrefs.SetInt("CurrentLevel", currentLevel);
            level6.SetActive(true);
            level5.SetActive(false);
        }

        if (PlayerPrefs.GetInt("CurrentLevel") == 7)
        {
            currentLevel = 7;
            PlayerPrefs.SetInt("CurrentLevel", currentLevel);
            level7.SetActive(true);
            level6.SetActive(false);
        }

        if (PlayerPrefs.GetInt("CurrentLevel") == 8)
        {
            currentLevel = 8;
            PlayerPrefs.SetInt("CurrentLevel", currentLevel);
            level8.SetActive(true);
            level7.SetActive(false);
        }

        if (PlayerPrefs.GetInt("CurrentLevel") == 9)
        {
            currentLevel = 3;
            PlayerPrefs.SetInt("CurrentLevel", currentLevel);
            level3.SetActive(true);
            level8.SetActive(false);
        }
    }


    public void NextLevel()
    {
        currentLevel++;
        PlayerPrefs.SetInt("CurrentLevel", currentLevel);
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }
}