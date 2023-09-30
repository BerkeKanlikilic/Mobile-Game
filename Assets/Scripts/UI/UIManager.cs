using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("References")] [SerializeField]
    private TMP_Text moveCountTxt;

    [SerializeField] private List<GameObject> obsObjects = new List<GameObject>();
    [SerializeField] private List<TMP_Text> obsTexts = new List<TMP_Text>();
    [Header("Win Screen References")]
    [SerializeField] private GameObject fireworksParticle;
    [SerializeField] private GameObject winBackground;
    [SerializeField] private GameObject starObj;

    [Header("Lose Screen References")]
    [SerializeField] private GameObject loseBackground;
    [SerializeField] private GameObject losePopup;

    private int moveCount;


    private static UIManager _instance;
    public static UIManager Instance => _instance;

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
        fireworksParticle.SetActive(false);
        winBackground.SetActive(false);
        loseBackground.SetActive(false);
        losePopup.SetActive(false);
    }

    public void UpdateMoveCount(int c)
    {
        moveCountTxt.text = c.ToString();
        moveCount = c;
    }

    public void DecreaseMoveCount()
    {
        moveCount--;
        moveCountTxt.text = moveCount.ToString();
    }

    public void UpdateObstacleCount(List<int> counts, Boolean firstCheck = false)
    {
        for (int i = 0; i < counts.Count; i++)
        {
            if (counts[i] == 0)
            {
                if (firstCheck) obsObjects[i].SetActive(false);
                else obsTexts[i].text = counts[i].ToString();
            }
            else
            {
                obsTexts[i].text = counts[i].ToString();
            }
        }
    }
    
    public void WinLoadingScene()
    {
        StartCoroutine(PlayWinAnimationAndLoadMenu());
    }

    public void LoseScene()
    {
        StartCoroutine(PlayLoseAnimation());
    }

    IEnumerator PlayWinAnimationAndLoadMenu()
    {
        winBackground.SetActive(true);
        CanvasGroup canvasGroup = winBackground.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        
        Vector3 orgScale = starObj.transform.localScale;
        starObj.transform.localScale = Vector3.zero;

        float duration = 1f;
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            float alpha = Mathf.Lerp(0f, 1f, t / duration);
            canvasGroup.alpha = alpha;

            starObj.transform.localScale = orgScale * alpha;
            yield return null;
        }
        
        fireworksParticle.SetActive(true);
        yield return new WaitForSeconds(6f);
        
        ChangeScene.LoadScene("MainScene");
    }
    
    IEnumerator PlayLoseAnimation()
    {
        loseBackground.SetActive(true);
        CanvasGroup canvasGroup = loseBackground.GetComponent<CanvasGroup>();
        
        float duration = 1f;
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            float alpha = Mathf.Lerp(0f, 1f, t / duration);
            canvasGroup.alpha = alpha;
            
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        losePopup.SetActive(true);
        Vector3 startPos = losePopup.transform.position + Vector3.up * 10;
        Vector3 endPos = losePopup.transform.position;
        
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            float newPos = Mathf.Lerp(startPos.y, endPos.y, t / duration);
            losePopup.transform.position = new Vector3(0, newPos, 0);
            
            yield return null;
        }
    }

    public void ReturnMainMenu()
    {
        ChangeScene.LoadScene("MainScene");
    }

    public void TryAgain()
    {
        ChangeScene.LoadScene("GameScene");
    }
}