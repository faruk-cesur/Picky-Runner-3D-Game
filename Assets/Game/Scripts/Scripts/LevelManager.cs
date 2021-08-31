using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
     public static LevelManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public GameObject level1;
    public GameObject level2;
    public GameObject level3;
    
    public int currentLevel;

    private void Start()
    {
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
            currentLevel = 1;
            PlayerPrefs.SetInt("CurrentLevel", currentLevel);
            level1.SetActive(true);
            level3.SetActive(false);
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
