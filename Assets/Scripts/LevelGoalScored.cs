using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGoalScored : LevelGoal
{
    public override void Start()
    {
        LevelCounter = LevelCounter.Moves;
        base.Start();
    }

    public override bool IsGameOver()
    {
        int maxScore = ScoreGoals[ScoreGoals.Length - 1];
        if (ScoreManager.Instance.CurrentScore >= maxScore)
            return true;

        return MovesLeft == 0;
    }

    public override bool IsWinner()
    {
        if(ScoreManager.Instance!=null)
        {
            return ScoreManager.Instance.CurrentScore >= ScoreGoals[0];
        }
        return false;
    }
}
