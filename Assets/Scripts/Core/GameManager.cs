using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("References")] [SerializeField]
    private LevelLoader levelLoader;

    public LevelManager levelManager { get; private set; }

    [Header("Debug")] [SerializeField] private bool debug = false;
    [SerializeField] private int levelToLoad;

    private static GameManager _instance;

    public static GameManager Instance => _instance;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    private void Start()
    {
        if (!debug) levelToLoad = LevelDataManager.Instance.FindNextLevel();
        levelManager = levelLoader.LoadLevel(levelToLoad);
    }

    public int GetCurrentLevel()
    {
        return levelToLoad;
    }

    public void Back()
    {
        SceneManager.LoadScene("MainScene");
    }
}