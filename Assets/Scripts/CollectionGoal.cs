using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectionGoal : MonoBehaviour
{
    public GamePiece PrefabToCollect;

    [Range(1, 50)]
    public int numberToCollect = 5;

    SpriteRenderer _spriteRenderer;

    private void Start()
    {
        if(PrefabToCollect!=null)
        {
            _spriteRenderer = PrefabToCollect.GetComponent<SpriteRenderer>();
        }
    }

    public void CollectPiece(GamePiece piece)
    {
        if(piece!=null)
        {
            SpriteRenderer sprite = piece.GetComponent<SpriteRenderer>();
            if(_spriteRenderer.sprite == sprite.sprite&& PrefabToCollect.MatchValue==piece.MatchValue)
            {
                numberToCollect--;
                numberToCollect = Mathf.Clamp(numberToCollect, 0, numberToCollect);
            }
        }
    }
}
 