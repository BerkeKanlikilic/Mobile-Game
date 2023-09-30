using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private Slider progressBar;

    private void Start()
    {
        string sceneToLoad = PlayerPrefs.GetString("sceneToLoad");
        LoadSceneAsync(sceneToLoad);
    }

    private void LoadSceneAsync(string sceneToLoad)
    {
        StartCoroutine(LoadAsyncRoutine(sceneToLoad));
    }

    private System.Collections.IEnumerator LoadAsyncRoutine(string sceneName)
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);

        while (!asyncOperation.isDone)
        {
            float progress = Mathf.Clamp01(asyncOperation.progress / 0.9f);
            progressBar.value = progress;
            yield return null;
        }
    }
}