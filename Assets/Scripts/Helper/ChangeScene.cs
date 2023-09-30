using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class ChangeScene
{
    public static void LoadScene(string sceneName)
    {
        PlayerPrefs.SetString("sceneToLoad", sceneName);
        SceneManager.LoadScene("LoadingScene");
    }
}
