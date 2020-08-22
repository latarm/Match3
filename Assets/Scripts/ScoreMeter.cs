using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class ScoreMeter : MonoBehaviour
{
    public Slider Slider;
    public ScoreStar[] ScoreStars = new ScoreStar[3];
    LevelGoal _levelGoal;
    int _maxScore;

    private void Awake()
    {
        Slider = GetComponent<Slider>();
    }

    public void SetupStars(LevelGoal levelGoal)
    {
        if(levelGoal==null)
        {
            Debug.LogWarning("SCOREMETER invalid level goal");
            return;
        }

        _levelGoal = levelGoal;

        _maxScore = _levelGoal.ScoreGoals[_levelGoal.ScoreGoals.Length - 1];
        float sliderWidth = Slider.GetComponent<RectTransform>().rect.width;

        if(_maxScore>0)
        {
            for (int i = 0; i < levelGoal.ScoreGoals.Length; i++)
            {
                if(ScoreStars[i]!=null)
                {
                    float newX = (sliderWidth * levelGoal.ScoreGoals[i] / _maxScore) - (sliderWidth * 0.5f);
                    RectTransform starRectXform = ScoreStars[i].GetComponent<RectTransform>();
                    if(starRectXform!=null)
                    {
                        starRectXform.anchoredPosition = new Vector2(newX, starRectXform.anchoredPosition.y);
                    }

                }
            }
        }
    }

    public void UpdateScoreMeter(int score, int starCount)
    {
        if(_levelGoal!=null)
        {
            Slider.value = (float)score / (float)_maxScore;
        }

        for (int i = 0; i < starCount; i++)
        {
            if(ScoreStars[i]!=null)
            {
                ScoreStars[i].Activate();
            }
        }
    }
}
