using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : Singleton<ScoreManager>
{
    public int CurrentScore { get; private set; } = 0;
    int _counterValue = 0;
    int _increment = 5;

    public Text scoreText;
    public float countTime = 1f;

    private void Start()
    {
        UpdateScoreText(CurrentScore);
    }

    public void UpdateScoreText(int scoreValue)
    {
        if(scoreText!=null)
        {
            scoreText.text = scoreValue.ToString();
        }
    }

    public void AddScore(int value)
    {
        CurrentScore += value;
        StartCoroutine(CountScoreRoutine());
    }

    IEnumerator CountScoreRoutine()
    {
        int iteration = 0;

        while(_counterValue<CurrentScore&&iteration<100000)
        {
            _counterValue += _increment;
            UpdateScoreText(_counterValue);
            iteration++;
            yield return null;
        }
        _counterValue = CurrentScore;
        UpdateScoreText(CurrentScore);

    }
}
