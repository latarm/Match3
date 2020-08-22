using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : GamePiece
{
    public bool ClearedByBomb = false;
    public bool ClearedAtBottom = true;

    private void Start()
    {
        MatchValue = MatchValue.None;
    }
}
