using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LevelGoal : Singleton<LevelGoal>
{
    public int ScoreStars = 0;
    public int[] ScoreGoals = new int[3] { 1000, 2000, 3000 };
    public int MovesLeft = 30;
    public int TimeLeft = 60;
    public LevelCounter LevelCounter = LevelCounter.Moves;

    int _maxTime;

    public virtual void  Start()
    {
        Init();
        if (LevelCounter == LevelCounter.Timer)
        {
            _maxTime = TimeLeft;
            if (UIManager.Instance != null && UIManager.Instance.Timer != null)
                UIManager.Instance.Timer.InitTimer(TimeLeft);
        }
    }

    void Init()
    {
        ScoreStars = 0;

        for (int i = 1; i < ScoreGoals.Length; i++)
        {
            if(ScoreGoals[i]<ScoreGoals[i-1])
            {
                Debug.Log("LEVELGOAL Setup score goals in increasing order!");
            }
        }
    }

    int UpdateScore(int score)
    {
        for (int i = 0; i < ScoreGoals.Length; i++)
        {
            if(score<ScoreGoals[i])
            {
                return i;
            }
        }
        return ScoreGoals.Length;
    }

    public void UpdateScoreStars(int score)
    {
        ScoreStars = UpdateScore(score);
    }

    public abstract bool IsWinner();
    public abstract bool IsGameOver();

    public void StartCountdown()
    {
        StartCoroutine(CountdownRoutine());
    }
    IEnumerator CountdownRoutine()
    {
        while (TimeLeft > 0)
        {
            yield return new WaitForSeconds(1f);
            TimeLeft--;
            if (UIManager.Instance != null && UIManager.Instance.Timer != null)
                UIManager.Instance.Timer.UpdateTimer(TimeLeft);
        }
    }
    public void AddTime(int timeValue)
    {
        TimeLeft += timeValue;
        TimeLeft = Mathf.Clamp(TimeLeft, 0, _maxTime);
        if (UIManager.Instance != null && UIManager.Instance.Timer != null)
        {
            UIManager.Instance.Timer.UpdateTimer(TimeLeft);
        }
    }
}
