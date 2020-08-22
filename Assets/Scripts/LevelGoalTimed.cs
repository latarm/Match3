using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGoalTimed : LevelGoal
{
    public override void Start()
    {
        LevelCounter = LevelCounter.Timer;
        base.Start();
    }
    public override bool IsGameOver()
    {
        int maxScore = ScoreGoals[ScoreGoals.Length - 1];

        if (ScoreManager.Instance.CurrentScore >= maxScore)
            return true;

        return TimeLeft <= 0;
    }
    public override bool IsWinner()
    {
        if (ScoreManager.Instance != null)
        {
            return ScoreManager.Instance.CurrentScore >= ScoreGoals[0];
        }
        return false;
    }
}
