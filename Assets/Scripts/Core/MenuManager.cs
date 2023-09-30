using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [System.Serializable]
    public class LevelUI
    {
        public GameObject lockedGameObject;
        public GameObject unlockedGameObject;
        public GameObject builtGameObject;
        public Button buildButton;
    }

    [SerializeField] private LevelUI[] levelsUI;
    [SerializeField] private TMP_Text buttonText;
    [SerializeField] private GameObject starParticlePrefab;

    private static MenuManager _instance;

    public static MenuManager Instance => _instance;

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

    void Start()
    {
        UpdateNumberOfLevels();
        UpdateLevelButtons();
        UpdateNextLevelButton();
    }

    public void UpdateLevelButtons()
    {
        for (int i = 0; i < levelsUI.Length; i++)
        {
            if (i < 0 || i >= levelsUI.Length)
            {
                return;
            }

            int levelNumber = i + 1;
            LevelState state = LevelDataManager.Instance.GetLevelState(levelNumber);
            int justUnlocked = LevelDataManager.Instance.justUnlockedLevel;

            switch (state)
            {
                case LevelState.Locked:
                    EnableGameObject(levelsUI[i].lockedGameObject, levelsUI[i]);
                    break;
                case LevelState.Unlocked:

                    if (levelNumber == justUnlocked)
                    {
                        StartCoroutine(PlayUnlockAnimation(levelNumber,
                            () =>
                            {
                                EnableGameObject(levelsUI[levelNumber - 1].unlockedGameObject,
                                    levelsUI[levelNumber - 1]);
                            }));
                        LevelDataManager.Instance.justUnlockedLevel = -1;
                    }
                    else
                    {
                        EnableGameObject(levelsUI[i].unlockedGameObject, levelsUI[i]);
                    }

                    break;
                case LevelState.Built:
                    EnableGameObject(levelsUI[i].builtGameObject, levelsUI[i]);
                    break;
            }
        }
    }


    private void EnableGameObject(GameObject toEnable, LevelUI levelUI)
    {
        levelUI.lockedGameObject.SetActive(false);
        levelUI.unlockedGameObject.SetActive(false);
        levelUI.builtGameObject.SetActive(false);
        toEnable.SetActive(true);
    }

    public void UpdateNextLevelButton()
    {
        if (NextLevelExists())
        {
            buttonText.text = "Level " + LevelDataManager.Instance.FindNextLevel();
        }
        else
        {
            buttonText.text = "Finished";
        }
    }

    public bool NextLevelExists()
    {
        int nLvl = LevelDataManager.Instance.FindNextLevel();
        int lvlAmount = LevelDataManager.Instance.numberOfLevels;
        if (nLvl <= lvlAmount) return true;
        else return false;
    }

    public void UpdateNumberOfLevels()
    {
        LevelDataManager.Instance.SetNumberOfLevels(levelsUI.Length);
    }

    IEnumerator PlayUnlockAnimation(int levelNumber, System.Action onCompleted)
    {
        if (levelNumber < 1 || levelNumber > levelsUI.Length)
        {
            Debug.LogError("Invalid level number: " + levelNumber);
            yield break;
        }

        LevelUI levelUI = levelsUI[levelNumber - 1];

        float duration = 1.5f;
        GameObject lockedObj = levelUI.lockedGameObject;
        GameObject unlockedObj = levelUI.unlockedGameObject;
        Image lockedImage = lockedObj.GetComponent<Image>();
        CanvasGroup unlockedImage = unlockedObj.GetComponent<CanvasGroup>();

        yield return new WaitForSeconds(1f);

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            float alpha = Mathf.Lerp(1f, 0f, t / duration);
            lockedImage.color = new Color(lockedImage.color.r, lockedImage.color.g, lockedImage.color.b, alpha);
            yield return null;
        }

        lockedObj.SetActive(false);

        GameObject starParticle = Instantiate(starParticlePrefab, unlockedObj.transform.position, Quaternion.identity);

        yield return new WaitForSeconds(0.2f);

        unlockedObj.SetActive(true);
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            float alpha = Mathf.Lerp(0f, 1f, t / duration);
            unlockedImage.alpha = alpha;
            yield return null;
        }

        Destroy(starParticle, 2f);

        onCompleted?.Invoke();
    }

    IEnumerator BuildLevel(int levelNumber)
    {
        LevelUI levelUI = levelsUI[levelNumber - 1];
        GameObject builtObj = levelUI.builtGameObject;
        GameObject unlockedObj = levelUI.unlockedGameObject;

        unlockedObj.SetActive(false);

        builtObj.transform.parent.gameObject.SetActive(true);
        builtObj.SetActive(true);

        RectTransform maskRect = builtObj.transform.parent.GetComponent<RectTransform>();

        maskRect.sizeDelta = new Vector2(maskRect.sizeDelta.x, 0);

        // Calculate the final height and width of the mask
        float finalHeight = builtObj.GetComponent<RectTransform>().rect.height;
        float buildingWidth = builtObj.GetComponent<RectTransform>().rect.width;

        // Instantiate the particle system at the base of the building
        Vector3 particlePosition = builtObj.transform.position; // + new Vector3(0, -finalHeight / 2, 0);
        GameObject starParticle = Instantiate(starParticlePrefab, particlePosition, Quaternion.identity);

        // Adjust the particle system's width dynamically based on the building's width
        ParticleSystem.ShapeModule shapeModule = starParticle.GetComponent<ParticleSystem>().shape;
        shapeModule.scale = new Vector3(buildingWidth / 150f, shapeModule.scale.y, shapeModule.scale.z);

        float duration = 1.5f;
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            float newHeight = Mathf.Lerp(0, finalHeight, t / duration);
            maskRect.sizeDelta = new Vector2(maskRect.sizeDelta.x, newHeight);

            starParticle.transform.position =
                builtObj.transform.position + new Vector3(0, newHeight / 100f, 0);

            yield return null;
        }

        maskRect.sizeDelta = new Vector2(maskRect.sizeDelta.x, finalHeight);

        LevelDataManager.Instance.SetLevelState(levelNumber, LevelState.Built);
        UpdateLevelButtons();

        yield return new WaitForSeconds(1f);

        Destroy(starParticle);
    }

    public void OnNextLevelButtonClick()
    {
        if (NextLevelExists()) ChangeScene.LoadScene("GameScene");
    }

    public void OnBuildButtonClick(int levelNumber)
    {
        if (levelNumber < 1 || levelNumber > levelsUI.Length)
        {
            Debug.LogError("Invalid level number: " + levelNumber);
            return;
        }

        LevelState state = LevelDataManager.Instance.GetLevelState(levelNumber);

        if (state == LevelState.Unlocked)
        {
            StartCoroutine(BuildLevel(levelNumber));
        }
    }
}