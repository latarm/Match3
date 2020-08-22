using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGoalCollected : LevelGoal
{
    public CollectionGoal[] CollectionGoals;

    bool AreGoalsComplete(CollectionGoal[] goals)
    {
        foreach(CollectionGoal goal in goals)
        {
            if (goal == null || goals == null)
                return false;
            if (goals.Length == 0)
                return false;

            if(goal.numberToCollect!=0)
            {
                return false;
            }
        }
        return true;
    }
    public void UpdateGoals(GamePiece pieceToCheck)
    {
        if(pieceToCheck!=null)
        {
            foreach (CollectionGoal goal in CollectionGoals)
            {
                if(goal!=null)
                {
                    goal.CollectPiece(pieceToCheck);
                }
            }
        }
        UpdateUI();
    }
    public void UpdateUI()
    {
        if (UIManager.Instance != null)
            UIManager.Instance.UpdateCollectionGoalLayout();
      
    }
    public override bool IsWinner()
    {
        if(ScoreManager.Instance!=null)
        {
            return (ScoreManager.Instance.CurrentScore >= ScoreGoals[0] && AreGoalsComplete(CollectionGoals));
        }
        return false;
    }
    public override bool IsGameOver()
    {
        if(AreGoalsComplete(CollectionGoals)&& ScoreManager.Instance!=null)
        {
            int maxScore = ScoreGoals[ScoreGoals.Length - 1];
            if(ScoreManager.Instance.CurrentScore>=maxScore)
            {
                return true;
            }
        }
        if (LevelCounter == LevelCounter.Timer)
            return TimeLeft <= 0;
        else
            return MovesLeft <= 0;
    }
}
