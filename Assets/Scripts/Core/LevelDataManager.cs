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
    [HideInInspector] public int justUnlockedLevel = -1;

    public int numberOfLevels { get; private set; }

    [Header("Debug")] [SerializeField] private bool resetOnStart = false;
    [SerializeField] private bool useForcedState = false;
    [SerializeField] private int levelToForce = 1;
    [SerializeField] private LevelState forceStateOnStart = LevelState.Locked;

    private static LevelDataManager _instance;

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

            if (resetOnStart)
            {
                ResetAllLevels();
            }
            else if (useForcedState)
            {
                ForceLevelToState(levelToForce, forceStateOnStart);
            }
        }
    }

    public void SetLevelState(int levelNumber, LevelState state)
    {
        if (state == LevelState.Unlocked)
        {
            justUnlockedLevel = levelNumber;
        }

        // Debug.Log($"Saving Level {levelNumber} as {state}");
        PlayerPrefs.SetInt("LevelState_" + levelNumber, (int)state);
        PlayerPrefs.Save();
    }

    public LevelState GetLevelState(int levelNumber)
    {
        int stateValue = PlayerPrefs.GetInt("LevelState_" + levelNumber, (int)LevelState.Locked);
        return (LevelState)stateValue;
    }

    public int FindNextLevel()
    {
        int nextLevel = numberOfLevels + 1;
        for (int i = 1; i <= numberOfLevels; i++)
        {
            LevelState state = GetLevelState(i);
            if (state == LevelState.Locked)
            {
                nextLevel = i;
                break;
            }
        }

        return nextLevel;
    }

    private void ResetAllLevels()
    {
        for (int i = 1; i <= 10; i++)
        {
            SetLevelState(i, LevelState.Locked);
        }
    }

    private void ForceLevelToState(int level, LevelState state)
    {
        SetLevelState(level, state);
    }

    public void SetNumberOfLevels(int amount)
    {
        numberOfLevels = amount;
    }
}