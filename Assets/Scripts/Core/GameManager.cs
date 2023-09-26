using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LevelLoader levelLoader;
    
    [Header("Debug")]
    [SerializeField] private int levelToLoad = 1;

    private void Start() {
        levelLoader.LoadLevel(levelToLoad);
    }

}
