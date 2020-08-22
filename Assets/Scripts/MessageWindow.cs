using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class MessageWindow : MonoBehaviour
{
    [Space(20)]
    public Image MessageImage;
    public Text MessageText;
    [Space(20)]
    public Sprite LoseIcon;
    public Sprite WinIcon;
    public Sprite GoalIcon;
    [Space(20)]
    public Sprite MovesIcon;
    public Sprite TimerIcon;
    public Sprite CollectIcon;
    [Space(20)]
    public Sprite GoalCompleteIcon;
    public Sprite GoalFailedIcon;
    [Space(20)]
    public Image GoalImage;
    public Text GoalText;
    [Space(20)]
    public Text ButtonText;
    [Space(20)]
    public GameObject collectionGoalLayout;

    public void ShowMessage(Sprite sprite=null, string message="", string buttonText="start")
    {
        if (MessageImage != null)
            MessageImage.sprite = sprite;
        if (MessageText != null)
            MessageText.text = message;
        if (buttonText != null)
            ButtonText.text = buttonText;
    }

    public void ShowScoreMessage(int scoreGoal)
    {
        string message = "Score goal\n" + scoreGoal.ToString();
        ShowMessage(GoalIcon, message, "start");
    }
    public void ShowWinMessage()
    {
        ShowMessage(WinIcon, "level\nCompleted", "ok");
    }
    public void ShowLoseMessage()
    {
        ShowMessage(LoseIcon, "level\nFailed", "ok");
    }
    public void ShowGoal(string caption="", Sprite icon = null)
    {
        if (caption != "")
        {
            ShowGoalCaption(caption);
        }
        if (icon != null)
        {
            ShowGoalImage(icon);
        }
    }
    public void ShowGoalCaption(string caption ="", int offsetX=0, int offsetY=0)
    {
        if ( GoalText!=null)
        {
            GoalText.text = caption;
            RectTransform rectXform = GoalText.GetComponent<RectTransform>();
            rectXform.anchoredPosition += new Vector2(offsetX, offsetY);
        }
    }
    public void ShowGoalImage(Sprite icon = null)
    {
        if (GoalImage != null)
        {
            GoalImage.gameObject.SetActive(true);
            GoalImage.sprite = icon;
        }
        if (icon == null)
        {
            GoalImage.gameObject.SetActive(false);
        }
    }
    public void ShowTimedGoal(int time)
    {
        string caption = time.ToString() + " seconds";
        ShowGoal(caption, TimerIcon);
    }
    public void ShowMovesGoal(int moves)
    {
        string caption = moves.ToString() + " moves";
        ShowGoal(caption, MovesIcon);
    }
    public void ShowCollectionGoal(bool state=true)
    {        
        if(collectionGoalLayout!=null)
        {
            collectionGoalLayout.SetActive(state);
        }
        if (state)
        {
            ShowGoal("", CollectIcon);
        }
    }


}
