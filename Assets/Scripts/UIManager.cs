using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    public ScreenFader ScreenFader;
    public Text LevelNameText;
    public Text MovesLeftText;
    public ScoreMeter ScoreMeter;
    public MessageWindow MessageWindow;
    public GameObject MovesCounter;
    public Timer Timer;

    public GameObject CollectionGoalLayout;
    public int CollectionGoalBaseWidth = 125;

    CollectionGoalPanel[] _collectionGoalPanels;

    public override void Awake()
    {
        base.Awake();

        if (MessageWindow != null)
            MessageWindow.gameObject.SetActive(true);
        if (ScreenFader != null)
            ScreenFader.gameObject.SetActive(true);
    }
    public void SetupCollectionGoalLayout(CollectionGoal[] collectionGoals, GameObject goalLayout, int spacingWidth)
    {
        if(goalLayout != null && collectionGoals!=null && collectionGoals.Length!=0)
        {
            RectTransform rectTransform = goalLayout.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(collectionGoals.Length * spacingWidth, rectTransform.sizeDelta.y);

            CollectionGoalPanel[] panels = goalLayout.gameObject.GetComponentsInChildren<CollectionGoalPanel>();

            for (int i = 0; i < panels.Length; i++)
            {
                if(i<collectionGoals.Length&&collectionGoals[i]!=null)
                {
                    panels[i].gameObject.SetActive(true);
                    panels[i].CollectionGoal = collectionGoals[i];
                    panels[i].SetupPanel();
                }
                else
                {
                    panels[i].gameObject.SetActive(false);
                }
            }
        }
    }
    public void SetupCollectionGoalLayout(CollectionGoal[] collectionGoals)
    {
        SetupCollectionGoalLayout(collectionGoals, CollectionGoalLayout, CollectionGoalBaseWidth);
    }
    public void UpdateCollectionGoalLayout(GameObject goalLayout)
    {
        if(goalLayout!=null)
        {
            CollectionGoalPanel[] panels = goalLayout.GetComponentsInChildren<CollectionGoalPanel>();

            if(panels!=null && panels.Length!=0)
            {
                foreach (CollectionGoalPanel panel in panels)
                {
                    if(panel!=null && panel.isActiveAndEnabled)
                    {
                        panel.UpdatePanel();
                    }
                }
            }
        }
    }
    public void UpdateCollectionGoalLayout()
    {
        UpdateCollectionGoalLayout(CollectionGoalLayout);
    }
    public void EnableTimer(bool state)
    {
        if(Timer!=null)
        {
            Timer.gameObject.SetActive(state);
        }
    }
    public void EnableMovesCounter(bool state)
    {
        if (MovesCounter != null)
        {
            MovesCounter.gameObject.SetActive(state);
        }
    }
    public void EnableCollectionGoalLayout(bool state)
    {
        if (CollectionGoalLayout != null)
        {
            CollectionGoalLayout.gameObject.SetActive(state);
        }
    }

}
