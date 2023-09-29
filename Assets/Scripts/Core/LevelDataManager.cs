using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LevelState
{
    Locked = 0,
    Unlocked = 1,
    Built = 2
}

public class LevelDataManager : MonoBehaviour
{
    private static LevelDataManager _instance;

    // Ensures that LevelDataManager is a singleton.
    public static LevelDataManager Instance => _instance;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    public void SetLevelState(int levelNumber, LevelState state)
    {
        PlayerPrefs.SetInt("LevelState_" + levelNumber, (int)state);
        PlayerPrefs.Save();
    }

    public LevelState GetLevelState(int levelNumber)
    {
        int stateValue = PlayerPrefs.GetInt("LevelState_" + levelNumber, (int)LevelState.Locked);
        return (LevelState)stateValue;
    }
}
