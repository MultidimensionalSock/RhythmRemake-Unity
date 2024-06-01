using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Windows.Speech;

public class UIController : MonoBehaviour
{
    TextMeshProUGUI scoreRankingText; //perfect, great, good, etc
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
        scoreRankingText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        comboCountText = transform.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>();
        songPercentageText = transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        scoreText = transform.GetChild(3).GetComponent<TextMeshProUGUI>();
        emotionImage = transform.GetChild(5).GetChild(1).GetComponent<Image>();
    }

    void ScoreRecieved(Scores score)
    {
        switch (score)
        {
            case Scores.Perfect:
                perfect++;
                textToDiplay = "Perfect";
                playerHealth += 2;
                playerCombo++;
                playerScore += 500f * ((playerCombo / 100) + 1);
                break;
            case Scores.Great:
                great++;
                textToDiplay = "Great";
                playerHealth += 1;
                playerCombo++;
                playerScore += 250f * ((playerCombo / 100) + 1);
                break;
            case Scores.Good:
                good++;
                textToDiplay = "Good";
                playerCombo = 0;
                playerScore += 100f;
                break;
            case Scores.Bad:
                bad++;
                textToDiplay = "Bad";
                playerHealth -= 1;
                playerCombo = 0;
                break;
            case Scores.Miss:
                miss++;
                textToDiplay = "Miss";
                playerCombo = 0;
                playerHealth -= 2;
                break;
        }

        //set all text on UI
        Debug.Log(playerScore);
        scoreText.text = playerScore.ToString();
        scoreRankingText.text = textToDiplay;
        comboCountText.text = playerCombo.ToString();
        //emotionImage.image = EmotionReact(score);
        songPercentageText.text = PercentageCalculate().ToString();
        
    }

    Texture EmotionReact(Scores score)
    {
        return null;
    }

    float PercentageCalculate()
    {
        //reduce to 2 decimal places
        return 0f;
    }
}
