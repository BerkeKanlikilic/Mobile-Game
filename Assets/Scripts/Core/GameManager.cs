using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LevelLoader levelLoader;
    public LevelManager levelManager { get; private set; }
    
    [Header("Debug")]
    [SerializeField] private int levelToLoad = 1;

    private void Start() {
        levelManager = levelLoader.LoadLevel(levelToLoad);
    }
}
