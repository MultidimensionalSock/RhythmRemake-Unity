using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;

[Serializable]
public struct emotions
{
    public Sprite perfect;
    public Sprite doingGood;
    public Sprite couldBeBetter;
    public Sprite doingBad;
    public Sprite missedNote;
    public Sprite dying;
}

public class UIController : MonoBehaviour
{
    GameObject[] scoreRankingText; //perfect, great, good, etc
    TextMeshProUGUI comboCountText; //how many notes hit in a row
    TextMeshProUGUI songPercentageText;
    TextMeshProUGUI scoreText;
    Image emotionImage;
    Image HealthPercentage;
    [SerializeField] HitAreaFunctionality hitArea;
    float playerScore = 0;
    int playerCombo = 0;
    float playerHealth;
    [SerializeField] float maxHealth;
    string textToDiplay;
    [SerializeField] emotions emotionState;


    int perfect = 0;
    int great = 0;
    int good = 0;
    int bad = 0;
    int miss = 0;

    //need a way to find the max score and compare it to current score 
    //need to keep track of your score.

    private void OnEnable()
    {
        hitArea.ScoreEvent += ScoreRecieved;
        List<GameObject> list = new List<GameObject>();
        for (int i = 0; i < transform.GetChild(0).childCount; i++)
        {
            list.Add(transform.GetChild(0).GetChild(i).gameObject) ;
        }
        scoreRankingText = list.ToArray();
        comboCountText = transform.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>();
        songPercentageText = transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        scoreText = transform.GetChild(3).GetComponent<TextMeshProUGUI>();
        emotionImage = transform.GetChild(5).GetChild(0).GetComponent<Image>();
        HealthPercentage = transform.GetChild(4).GetComponent<Image>();
    }

    void ScoreRecieved(Scores score)
    {
        int indexScore = 0;
        switch (score)
        {
            case Scores.Perfect:
                perfect++;
                //textToDiplay = "Perfect";
                indexScore = 0;
                scoreRankingText[0].gameObject.SetActive(true);
                playerHealth += 2;
                playerCombo++;
                playerScore += 500f * ((playerCombo / 100) + 1);
                break;
            case Scores.Great:
                great++;
                //textToDiplay = "Great";
                indexScore = 1;
                playerHealth += 1;
                playerCombo++;
                playerScore += 250f * ((playerCombo / 100) + 1);
                break;
            case Scores.Good:
                good++;
                //textToDiplay = "Good";
                indexScore = 2;
                playerCombo = 0;
                playerScore += 100f;
                break;
            case Scores.Bad:
                bad++;
                //textToDiplay = "Bad";
                indexScore = 3;
                playerHealth -= 1;
                playerCombo = 0;
                break;
            case Scores.Miss:
                miss++;
                //textToDiplay = "Miss";
                indexScore = 4;
                playerCombo = 0;
                playerHealth -= 2;
                break;
        }

        //set all text on UI
        scoreText.text = playerScore.ToString();
        HideAllTextUI();
        scoreRankingText[indexScore].SetActive(true);
        comboCountText.text = playerCombo.ToString();
        HealthPercentage.fillAmount = CalculateHealth();
        EmotionReact(score);
        songPercentageText.text = PercentageCalculate().ToString();
        
    }

    void EmotionReact(Scores score)
    {
        switch (score)
        {
            case Scores.Perfect:
                if (playerHealth > maxHealth * 0.8)
                {
                    emotionImage.overrideSprite = emotionState.perfect;
                }
                else
                {
                    emotionImage.overrideSprite = emotionState.doingGood;
                }
                return;
            case Scores.Great:
                if (playerHealth > maxHealth * 0.3)
                {
                    emotionImage.overrideSprite = emotionState.doingGood;
                }
                else
                {
                    emotionImage.overrideSprite = emotionState.couldBeBetter;
                }
                return;
            case Scores.Good:
                if (playerHealth > maxHealth * 0.3)
                {
                    emotionImage.overrideSprite = emotionState.couldBeBetter;
                    
                }
                else
                {
                    emotionImage.overrideSprite = emotionState.doingBad;
                }
                return;
            case Scores.Bad:
                if (playerHealth > maxHealth * 0.3)
                {
                    emotionImage.overrideSprite = emotionState.doingBad;
                }
                else
                {
                    emotionImage.overrideSprite = emotionState.missedNote;
                }
                return;
            case Scores.Miss:
                if (playerHealth > maxHealth * 0.3)
                {
                    emotionImage.overrideSprite = emotionState.missedNote;
                }
                else
                {
                    emotionImage.overrideSprite = emotionState.dying;
                }
                return;
            default:
                return;
        }
    }
        
    

    float CalculateHealth()
    {
        return (playerHealth / maxHealth) * 0.25f;
    }

    float PercentageCalculate()
    {
        
        return 0f;
    }

    void HideAllTextUI()
    {
        foreach (GameObject scoreTex in scoreRankingText)
        {
            scoreTex.SetActive(false);
        }
    }
}
