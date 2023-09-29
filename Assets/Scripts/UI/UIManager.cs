using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text moveCountTxt;
    [SerializeField] private List<GameObject> obsObjects = new List<GameObject>();
    [SerializeField] private List<TMP_Text> obsTexts = new List<TMP_Text>();

    private int moveCount;

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
            if(counts[i] == 0)
            {
                if(firstCheck) obsObjects[i].SetActive(false);
                else obsTexts[i].text = counts[i].ToString();
            } else {
                obsTexts[i].text = counts[i].ToString();
            }
        }
    }

}
