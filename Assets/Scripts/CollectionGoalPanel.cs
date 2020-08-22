using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectionGoalPanel : MonoBehaviour
{
    public CollectionGoal CollectionGoal;
    public Text PiecesLeftText;
    public Image PrefabImage;


    void Start()
    {
        SetupPanel();
    }

    public void SetupPanel()
    {
        if (CollectionGoal!=null && PiecesLeftText!=null && PrefabImage!=null)
        {
            SpriteRenderer prefabSprite = CollectionGoal.PrefabToCollect.GetComponent<SpriteRenderer>();
            if(PrefabImage!=null)
            {
                PrefabImage.sprite = prefabSprite.sprite;
                PrefabImage.color = prefabSprite.color;
            }
            PiecesLeftText.text = CollectionGoal.numberToCollect.ToString();
        }
    }

    public void UpdatePanel()
    {
        if (CollectionGoal != null && PiecesLeftText != null)
        {
            PiecesLeftText.text = CollectionGoal.numberToCollect.ToString();
        }
    }
}
