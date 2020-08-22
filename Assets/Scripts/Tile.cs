using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(SpriteRenderer))]
public class Tile : MonoBehaviour
{
    #region Fields

    private Board _board;
    private SpriteRenderer _spriteRenderer;

    public int XIndex;
    public int YIndex;

    public TileType TileType = TileType.Normal;
    public int BreakableValue = 0;
    public Sprite[] BreakableSprites;
    public Color Normalcolor;

    #endregion

    #region UnityMethods

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnMouseDown()
    {
        if (_board != null)
            _board.ClickTile(this);
    }
    private void OnMouseEnter()
    {
        if (_board != null)
            _board.DragToTile(this);
    }
    private void OnMouseUp()
    {
        if (_board != null)
            _board.ReleaseTile();
    }
    
    #endregion

    #region Methods

    public void Init(int x, int y, Board board)
    {
        XIndex = x;
        YIndex = y;
        _board = board;

        if (TileType == TileType.Breakable)
        {
            if (BreakableSprites[BreakableValue] != null)
            {
                _spriteRenderer.sprite = BreakableSprites[BreakableValue];
            }
        }
    }

    public void BreakTile()
    {
        if(TileType!=TileType.Breakable)
        {
            return;
        }

        StartCoroutine(BreakTileRoutine());
    }

    IEnumerator BreakTileRoutine()
    {
        BreakableValue = Mathf.Clamp(--BreakableValue, 0, BreakableValue);

        yield return new WaitForSeconds(0.25f);

        if(BreakableSprites[BreakableValue]!=null)
        {
            _spriteRenderer.sprite = BreakableSprites[BreakableValue];
        }
        if(BreakableValue==0)
        {
            TileType = TileType.Normal;
            _spriteRenderer.color = Normalcolor;
        }
    }

    #endregion
}
