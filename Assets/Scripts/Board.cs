using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(BoardDeadlock),typeof(BoardShuffler))]

public class Board : MonoBehaviour
{
    #region Fields

    [SerializeField] private int _width;
    [SerializeField] private int _height;
    [SerializeField] private int _borderSize;
  

    private bool _playerInputEnabled = true;
    private Tile _clickedTile;
    private Tile _targetTile;
    private Tile[,] _allTiles;
    private GamePiece[,] _allGamePieces;
    private ParticleManager _particlManager;
    [Space(20)]
    public GameObject TilePrefab;
    public GameObject TileNormalPrefab;
    public GameObject TileObstaclePrefab;
    [Space(20)]
    public GameObject[] AdjacentBombPrefabs;
    public GameObject[] ColumnBombPrefabs;
    public GameObject[] RowBombPrefabs;
    public GameObject ColorBombPrefab;
    [Space(20)]
    public int MaxCollectibles = 3;
    public int CollectibleCount = 0;

    [Range(0, 1)]
    public float ChanceForCollectible = 0.1f;
    public GameObject[] CollectiblePrefabs;

    GameObject _clickedTileBomb;
    GameObject _targetTileBomb;

    [Space(20)]
    public float SwapTime;
    public GameObject[] GamePiecesPrefabs;
    public int FillYOffset = 10;
    public float FillMoveTime = 0.5f;
    [Space(20)]
    public StartingObject[] StartingTiles;
    public StartingObject[] StartingGamePieces;

    public bool IsRefilling = false;
    BoardDeadlock _boardDeadlock;
    BoardShuffler _boardShuffler;

    [SerializeField] int _scoreMultiplier = 0;

    [System.Serializable]
    public class StartingObject
    {
        public GameObject TilePrefab;
        public int x;
        public int y;
        public int z;

    }

    #endregion

    #region UnityMethods

    private void Start()
    {
        _allTiles = new Tile[_width, _height];
        _allGamePieces = new GamePiece[_width, _height];
        _particlManager = GameObject.FindGameObjectWithTag("ParticleManager").GetComponent<ParticleManager>();
        _boardDeadlock = GetComponent<BoardDeadlock>();
        _boardShuffler = GetComponent<BoardShuffler>();
    }


    #endregion

    #region Methods

    public void SetupBoard()
    {
        SetupTiles();
        SetupGamePieces();
        List<GamePiece> startingCollectibles = FindAllCollectibles();
        CollectibleCount = startingCollectibles.Count;

        SetupCamera();
        FillBoard(FillYOffset, FillMoveTime);
    }
    void MakeTile(GameObject prefab, int x, int y, int z = 1)
    {
        if (prefab != null && IsWithinBounds(x, y))
        {
            GameObject tile = Instantiate(prefab, new Vector3(x, y, z), Quaternion.identity) as GameObject;
            tile.name = "Tile (" + x + "," + y + ")";
            _allTiles[x, y] = tile.GetComponent<Tile>();
            tile.transform.parent = transform;
            _allTiles[x, y].Init(x, y, this);
        }
    }
    void MakeGamePiece(GameObject prefab, int x, int y, int falseYOffset = 0, float moveTime = 0.1f)
    {
        if (prefab != null && IsWithinBounds(x, y))
        {
            prefab.GetComponent<GamePiece>().Init(this);
            PlaceGamePiece(prefab.GetComponent<GamePiece>(), x, y);

            if (falseYOffset != 0)
            {
                prefab.transform.position = new Vector3(x, y + falseYOffset, 0);
                prefab.GetComponent<GamePiece>().Move(x, y, moveTime);
            }

            prefab.transform.parent = transform;
        }
    }
    GameObject MakeBomb(GameObject prefab, int x, int y)
    {
        if(prefab!=null&& IsWithinBounds(x,y))
        {
            GameObject bomb = Instantiate(prefab, new Vector3(x, y, 0), Quaternion.identity) as GameObject;
            bomb.GetComponent<Bomb>().Init(this);
            bomb.GetComponent<Bomb>().SetCoord(x, y);
            bomb.transform.parent = transform;
            return bomb;
        }
        return null;
    }
    void SetupTiles()
    {
        foreach (StartingObject sTile in StartingTiles)
        {
            if (sTile != null)
            {
                MakeTile(sTile.TilePrefab, sTile.x, sTile.y, sTile.z);
            }

        }

        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                if (_allTiles[i, j] == null)
                {
                    MakeTile(TileNormalPrefab, i, j);
                }
            }
        }
    }
    void SetupGamePieces()
    {
        foreach (StartingObject sPiece in StartingGamePieces)
        {
            if (sPiece != null)
            {
                GameObject piece = Instantiate(sPiece.TilePrefab, new Vector3(sPiece.x, sPiece.y, 0), Quaternion.identity) as GameObject;
                MakeGamePiece(piece, sPiece.x, sPiece.y, FillYOffset, FillMoveTime);
            }

        }
    }
    void SetupCamera()
    {
        Camera.main.transform.position = new Vector3((float)(_width - 1) / 2f, (float)(_height - 1) / 2f, -10f);

        float aspectRatio = (float)Screen.width / (float)Screen.height;

        float verticalSize = (float)_height / 2f + (float)_borderSize;

        float horizontalSize = ((float)_width / 2f + (float)_borderSize) / aspectRatio;

        Camera.main.orthographicSize = (verticalSize > horizontalSize) ? verticalSize : horizontalSize;

    }
    GameObject GetRandomGameObject(GameObject[] objectArray)
    {
        int randomIdx = Random.Range(0, objectArray.Length);
        if (objectArray[randomIdx] == null)
        {
            Debug.LogWarning("BOARD:  " + randomIdx + "does not contain a valid GameObject!");
        }
        return objectArray[randomIdx];
    }
    GameObject GetRandomGamePiece()
    {
        return GetRandomGameObject(GamePiecesPrefabs);
    }
    GameObject GetRandomCollectible()
    {
        return GetRandomGameObject(CollectiblePrefabs);
    }
    public void PlaceGamePiece(GamePiece gamePiece, int x, int y)
    {
        if (gamePiece == null)
        {
            Debug.LogWarning("BOARD:  Invalid GamePiece!");
            return;
        }

        gamePiece.transform.position = new Vector3(x, y, 0);
        gamePiece.transform.rotation = Quaternion.identity;

        if (IsWithinBounds(x, y))
        {
            _allGamePieces[x, y] = gamePiece;
        }

        gamePiece.SetCoord(x, y);
    }
    bool IsWithinBounds(int x, int y)
    {
        return (x >= 0 && x < _width && y >= 0 && y < _height);
    }
    GamePiece FillRandomGamePieceAt(int x, int y, int falseYOffset = 0, float moveTime = 0.1f)
    {
        if (IsWithinBounds(x, y))
        {
            GameObject randomPiece = Instantiate(GetRandomGamePiece(), Vector3.zero, Quaternion.identity) as GameObject;
            MakeGamePiece(randomPiece, x, y, falseYOffset, moveTime);
            return randomPiece.GetComponent<GamePiece>();
        }
        return null;
    }
    GamePiece FillRandomCollectibleAt(int x, int y, int falseYOffset = 0, float moveTime = 0.1f)
    {
        if (IsWithinBounds(x, y))
        {
            GameObject randomPiece = Instantiate(GetRandomCollectible(), Vector3.zero, Quaternion.identity) as GameObject;
            MakeGamePiece(randomPiece, x, y, falseYOffset, moveTime);
            return randomPiece.GetComponent<GamePiece>();
        }
        return null;
    }
    void FillBoardFromList(List<GamePiece> gamePieces)
    {
        Queue<GamePiece> unusedPieces = new Queue<GamePiece>(gamePieces);
        int maxIterations = 100;
        int iterations = 0;

        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                if(_allGamePieces[i,j]==null&&_allTiles[i,j].TileType!=TileType.Obstacle)
                {
                    _allGamePieces[i, j] = unusedPieces.Dequeue();
                    iterations = 0;
                    while(HasMatchOnFill(i,j))
                    {
                        unusedPieces.Enqueue(_allGamePieces[i, j]);
                        _allGamePieces[i, j] = unusedPieces.Dequeue();
                        iterations++;
                        if(iterations>=maxIterations)
                        {
                            break;
                        }
                    }
                }
            }
        }
    }
    void FillBoard(int falseYOffset = 0, float moveTime = 0.1f)
    {
        int maxInterations = 100;
        int iterations = 0;

        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                if (_allGamePieces[i, j] == null && _allTiles[i, j].TileType != TileType.Obstacle)
                {
                    
                    if (j == _height - 1 && CanAddCollectible())
                    {
                        FillRandomCollectibleAt(i, j, falseYOffset, moveTime);
                        CollectibleCount++;
                    }
                    else
                    {
                        FillRandomGamePieceAt(i, j, falseYOffset, moveTime);
                        iterations = 0;

                        while (HasMatchOnFill(i, j))
                        {
                            ClearPieceAt(i, j);
                            FillRandomGamePieceAt(i, j, falseYOffset, moveTime);
                            iterations++;

                            if (iterations >= maxInterations)
                            {
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
    bool HasMatchOnFill(int x, int y, int minLength = 3)
    {
        List<GamePiece> leftMatches = FindMatches(x, y, new Vector2(-1, 0), minLength);
        List<GamePiece> downwardMatches = FindMatches(x, y, new Vector2(0, -1), minLength);

        if (leftMatches == null)
        {
            leftMatches = new List<GamePiece>();
        }

        if (downwardMatches == null)
        {
            downwardMatches = new List<GamePiece>();
        }

        return (leftMatches.Count > 0 || downwardMatches.Count > 0);

    }
    public void ClickTile(Tile tile)
    {
        if (_clickedTile == null)
        {
            _clickedTile = tile;
            //Debug.Log("clicked tile: " + tile.name);
        }
    }
    public void DragToTile(Tile tile)
    {
        if (_clickedTile != null && IsNextTo(tile, _clickedTile))
        {
            _targetTile = tile;
        }
    }
    public void ReleaseTile()
    {
        if (_clickedTile != null && _targetTile != null)
        {
            SwitchTiles(_clickedTile, _targetTile);
        }

        _clickedTile = null;
        _targetTile = null;
    }
    void SwitchTiles(Tile clickedTile, Tile targetTile)
    {
        StartCoroutine(SwitchTilesRoutine(clickedTile, targetTile));
    }
    IEnumerator SwitchTilesRoutine(Tile clickedTile, Tile targetTile)
    {
        if (_playerInputEnabled && !GameManager.Instance.IsGameOver)
        {
            GamePiece clickedPiece = _allGamePieces[clickedTile.XIndex, clickedTile.YIndex];
            GamePiece targetPiece = _allGamePieces[targetTile.XIndex, targetTile.YIndex];

            if (targetPiece != null && clickedPiece != null)
            {
                clickedPiece.Move(targetTile.XIndex, targetTile.YIndex, SwapTime);
                targetPiece.Move(clickedTile.XIndex, clickedTile.YIndex, SwapTime);

                yield return new WaitForSeconds(SwapTime);

                List<GamePiece> clickedPieceMatches = FindMatchesAt(clickedTile.XIndex, clickedTile.YIndex);
                List<GamePiece> targetPieceMatches = FindMatchesAt(targetTile.XIndex, targetTile.YIndex);
                List<GamePiece> ColorMatches = new List<GamePiece>();

                if (IsColorBomb(clickedPiece) && !IsColorBomb(targetPiece))
                {
                    clickedPiece.MatchValue = targetPiece.MatchValue;
                    ColorMatches = FindAllMatchValue(clickedPiece.MatchValue);
                }
                else if (!IsColorBomb(clickedPiece) && IsColorBomb(targetPiece))
                {
                    targetPiece.MatchValue = clickedPiece.MatchValue;
                    ColorMatches = FindAllMatchValue(targetPiece.MatchValue);
                }
                else if (IsColorBomb(clickedPiece) && IsColorBomb(targetPiece))
                {
                    foreach (GamePiece piece in _allGamePieces)
                    {
                        if (!ColorMatches.Contains(piece))
                        {
                            ColorMatches.Add(piece);
                        }
                    }
                }

                if (targetPieceMatches.Count == 0 && clickedPieceMatches.Count == 0 && ColorMatches.Count == 0)
                {
                    clickedPiece.Move(clickedTile.XIndex, clickedTile.YIndex, SwapTime);
                    targetPiece.Move(targetTile.XIndex, targetTile.YIndex, SwapTime);
                }
                else
                {
                    yield return new WaitForSeconds(SwapTime);

                    Vector2 swipeDirection = new Vector2(targetTile.XIndex - clickedTile.XIndex, targetTile.YIndex - clickedTile.YIndex);
                    _clickedTileBomb = DropBomb(clickedTile.XIndex, clickedTile.YIndex, swipeDirection, clickedPieceMatches);
                    _targetTileBomb = DropBomb(targetTile.XIndex, targetTile.YIndex, swipeDirection, targetPieceMatches);

                    if (_clickedTileBomb != null && targetPiece != null)
                    {
                        GamePiece clickedBombPiece = _clickedTileBomb.GetComponent<GamePiece>();
                        if (!IsColorBomb(clickedBombPiece))
                            clickedBombPiece.ChangeColor(targetPiece);
                    }
                    if (_targetTileBomb != null && clickedPiece != null)
                    {
                        GamePiece targetBombPiece = _targetTileBomb.GetComponent<GamePiece>();
                        if (!IsColorBomb(targetBombPiece))
                            targetBombPiece.ChangeColor(clickedPiece);
                    }

                    List<GamePiece> pieceToClear = clickedPieceMatches.Union(targetPieceMatches).ToList().Union(ColorMatches).ToList();

                    yield return StartCoroutine(ClearAndRefillBoardRoutine(pieceToClear));

                    if (GameManager.Instance != null)
                    {
                        GameManager.Instance.UpdateMoves();
                    }
                }
            }
        }
    }
    bool IsNextTo(Tile start, Tile end)
    {
        if (Mathf.Abs(start.XIndex - end.XIndex) == 1 && start.YIndex == end.YIndex)
        {
            return true;
        }

        if (Mathf.Abs(start.YIndex - end.YIndex) == 1 && start.XIndex == end.XIndex)
        {
            return true;
        }

        return false;
    }
    List<GamePiece> FindMatches(int startX, int startY, Vector2 searchDirection, int minLength = 3)
    {
        List<GamePiece> matches = new List<GamePiece>();

        GamePiece startPiece = null;

        if (IsWithinBounds(startX, startY))
        {
            startPiece = _allGamePieces[startX, startY];
        }

        if (startPiece != null)
        {
            matches.Add(startPiece);
        }

        else
        {
            return null;
        }

        int nextX;
        int nextY;

        int maxValue = (_width > _height) ? _width : _height;

        for (int i = 1; i < maxValue - 1; i++)
        {
            nextX = startX + (int)Mathf.Clamp(searchDirection.x, -1, 1) * i;
            nextY = startY + (int)Mathf.Clamp(searchDirection.y, -1, 1) * i;

            if (!IsWithinBounds(nextX, nextY))
            {
                break;
            }

            GamePiece nextPiece = _allGamePieces[nextX, nextY];

            if (nextPiece == null)
            {
                break;
            }
            else
            {
                if (nextPiece.MatchValue == startPiece.MatchValue && !matches.Contains(nextPiece)&& nextPiece.MatchValue!=MatchValue.None)
                {
                    matches.Add(nextPiece);
                }

                else
                {
                    break;
                }
            }
        }

        if (matches.Count >= minLength)
        {
            return matches;
        }

        return null;

    }
    List<GamePiece> FindVerticalMatches(int startX, int startY, int minLength = 3)
    {
        List<GamePiece> upwardMatches = FindMatches(startX, startY, new Vector2(0, 1), 2);
        List<GamePiece> downwardMatches = FindMatches(startX, startY, new Vector2(0, -1), 2);

        if (upwardMatches == null)
        {
            upwardMatches = new List<GamePiece>();
        }

        if (downwardMatches == null)
        {
            downwardMatches = new List<GamePiece>();
        }

        var combinedMatches = upwardMatches.Union(downwardMatches).ToList();

        return (combinedMatches.Count >= minLength) ? combinedMatches : null;

    }
    List<GamePiece> FindHorizontalMatches(int startX, int startY, int minLength = 3)
    {
        List<GamePiece> rightMatches = FindMatches(startX, startY, new Vector2(1, 0), 2);
        List<GamePiece> leftMatches = FindMatches(startX, startY, new Vector2(-1, 0), 2);

        if (rightMatches == null)
        {
            rightMatches = new List<GamePiece>();
        }

        if (leftMatches == null)
        {
            leftMatches = new List<GamePiece>();
        }

        var combinedMatches = rightMatches.Union(leftMatches).ToList();

        return (combinedMatches.Count >= minLength) ? combinedMatches : null;

    }
    List<GamePiece> FindMatchesAt(int x, int y, int minLength = 3)
    {
        List<GamePiece> horizMatches = FindHorizontalMatches(x, y, minLength);
        List<GamePiece> vertMatches = FindVerticalMatches(x, y, minLength);

        if (horizMatches == null)
        {
            horizMatches = new List<GamePiece>();
        }

        if (vertMatches == null)
        {
            vertMatches = new List<GamePiece>();
        }
        var combinedMatches = horizMatches.Union(vertMatches).ToList();
        return combinedMatches;
    }
    List<GamePiece> FindMatchesAt(List<GamePiece> gamePieces, int minLength = 3)
    {
        List<GamePiece> matches = new List<GamePiece>();

        foreach (GamePiece piece in gamePieces)
        {
            matches = matches.Union(FindMatchesAt(piece.XIndex, piece.YIndex, minLength)).ToList();
        }
        return matches;

    }
    List<GamePiece> FindAllMatches()
    {
        List<GamePiece> combinedMatches = new List<GamePiece>();

        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                var matches = FindMatchesAt(i, j);
                combinedMatches = combinedMatches.Union(matches).ToList();
            }
        }
        return combinedMatches;
    }
    void HighlightTileOff(int x, int y)
    {
        if (_allTiles[x, y].TileType != TileType.Breakable)
        {
            SpriteRenderer spriteRenderer = _allTiles[x, y].GetComponent<SpriteRenderer>();
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0);
        }
    }
    void HighlightTileOn(int x, int y, Color col)
    {
        if (_allTiles[x, y].TileType != TileType.Breakable)
        {
            SpriteRenderer spriteRenderer = _allTiles[x, y].GetComponent<SpriteRenderer>();
            spriteRenderer.color = col;
        }
    }
    void HighlightMatchesAt(int x, int y)
    {
        HighlightTileOff(x, y);
        var combinedMatches = FindMatchesAt(x, y);
        if (combinedMatches.Count > 0)
        {
            foreach (GamePiece piece in combinedMatches)
            {
                HighlightTileOn(piece.XIndex, piece.YIndex, piece.GetComponent<SpriteRenderer>().color);
            }
        }
    }
    void HighlightMatches()
    {
        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                HighlightMatchesAt(i, j);

            }
        }
    }
    void HighlightPieces(List<GamePiece> gamePieces)
    {
        foreach (GamePiece piece in gamePieces)
        {
            if (piece != null)
            {
                HighlightTileOn(piece.XIndex, piece.YIndex, piece.GetComponent<SpriteRenderer>().color);
            }
        }
    }
    void ClearPieceAt(int x, int y)
    {
        GamePiece pieceToClear = _allGamePieces[x, y];

        if (pieceToClear != null)
        {
            _allGamePieces[x, y] = null;
            Destroy(pieceToClear.gameObject);
        }

        //HighlightTileOff(x,y);
    }
    void ClearBoard()
    {
        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                ClearPieceAt(i, j);
                if(_particlManager!=null)
                {
                    _particlManager.ClearPieceFXAt(i, j);
                }
            }
        }
    }
    void ClearPieceAt(List<GamePiece> gamePieces, List<GamePiece> bombedPieces)
    {
        foreach (GamePiece piece in gamePieces)
        {
            if (piece != null)
            {
                ClearPieceAt(piece.XIndex, piece.YIndex);
                int bonus = 0;
                if (gamePieces.Count >= 4)
                {
                    bonus = 20;
                }
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.ScorePoints(piece, _scoreMultiplier, bonus);

                    TimeBonus timeBonus = piece.GetComponent<TimeBonus>();

                    if (timeBonus != null)
                    {
                        GameManager.Instance.AddTime(timeBonus.BonusValue);
                    }

                    GameManager.Instance.UpdateCollectionGoals(piece);

                }

                if (_particlManager != null)
                {
                    if (bombedPieces.Contains(piece))
                    {
                        _particlManager.BombFXAt(piece.XIndex, piece.YIndex);
                    }
                    else
                    {
                        _particlManager.ClearPieceFXAt(piece.XIndex, piece.YIndex);
                    }
                }
            }
        }
    }
    void BreakTileAt(int x, int y)
    {
        Tile tileToBreak = _allTiles[x, y];

        if (tileToBreak != null && tileToBreak.TileType == TileType.Breakable)
        {
            if (_particlManager != null)
            {
                _particlManager.BreakTileFXAt(tileToBreak.BreakableValue, x, y, 0);
            }

            tileToBreak.BreakTile();
        }
    }
    void BreakTileAt(List<GamePiece> gamePieces)
    {
        foreach (GamePiece piece in gamePieces)
        {
            if (piece != null)
            {
                BreakTileAt(piece.XIndex, piece.YIndex);
            }
        }
    }
    List<GamePiece> CollapseColumn(int column, float collapseTime = 0.1f)
    {
        List<GamePiece> movingPieces = new List<GamePiece>();

        for (int i = 0; i < _height - 1; i++)
        {
            if (_allGamePieces[column, i] == null && _allTiles[column, i].TileType != TileType.Obstacle)
            {
                for (int j = i + 1; j < _height; j++)
                {
                    if (_allGamePieces[column, j] != null)
                    {
                        _allGamePieces[column, j].Move(column, i, collapseTime * (j - i));

                        _allGamePieces[column, i] = _allGamePieces[column, j];
                        _allGamePieces[column, i].SetCoord(column, i);

                        if (!movingPieces.Contains(_allGamePieces[column, i]))
                        {
                            movingPieces.Add(_allGamePieces[column, i]);
                        }

                        _allGamePieces[column, j] = null;

                        break;
                    }
                }
            }
        }
        return movingPieces;
    }
    List<GamePiece> CollapseColumn(List<GamePiece> gamePieces)
    {
        List<GamePiece> movingPieces = new List<GamePiece>();

        List<int> columnsToCollapse = GetColumns(gamePieces);

        foreach (int column in columnsToCollapse)
        {
            movingPieces = movingPieces.Union(CollapseColumn(column)).ToList();
        }

        return movingPieces;
    }
    List<GamePiece> CollapseColumn(List<int> columnsToCollapse)
    {
        List<GamePiece> movingPieces = new List<GamePiece>();
        foreach (int column in columnsToCollapse)
        {
            movingPieces = movingPieces.Union(CollapseColumn(column)).ToList();
        }
        return movingPieces;
    }
    List<int> GetColumns(List<GamePiece> gamePieces)
    {
        List<int> columns = new List<int>();

        foreach (GamePiece piece in gamePieces)
        {
            if (piece != null)
                if (!columns.Contains(piece.XIndex))
                {
                    columns.Add(piece.XIndex);
                }
        }

        return columns;
    }
    void ClearAndRefillBoard(List<GamePiece> gamePieces)
    {
        StartCoroutine(ClearAndRefillBoardRoutine(gamePieces));
    }
    IEnumerator ClearAndRefillBoardRoutine(List<GamePiece> gamePieces)
    {
        _playerInputEnabled = false;
        IsRefilling = true;

        List<GamePiece> matches = gamePieces;

        _scoreMultiplier = 0;

        do
        {
            _scoreMultiplier++;

            yield return StartCoroutine(ClearAndCollapseRoutine(matches));

            
            yield return null;

            yield return StartCoroutine(RefillRoutine());

            matches = FindAllMatches();

            yield return new WaitForSeconds(0.2f);

        }
        while (matches.Count != 0);

        if(_boardDeadlock.IsDeadlocked(_allGamePieces))
        {
            yield return new WaitForSeconds(1f);
            //ClearBoard();
            yield return StartCoroutine(ShuffleBoardRoutine());
            yield return new WaitForSeconds(1f);
            //yield return StartCoroutine(RefillRoutine());
        }

        _playerInputEnabled = true;
        IsRefilling = false;

    }
    IEnumerator ClearAndCollapseRoutine(List<GamePiece> gamePieces)
    {
        List<GamePiece> movingPieces = new List<GamePiece>();
        List<GamePiece> matches = new List<GamePiece>();

        yield return new WaitForSeconds(0.2f);

        bool isFinished = false;

        while (!isFinished)
        {
            List<GamePiece> bombedPieces = GetBombedPieces(gamePieces);
            gamePieces = gamePieces.Union(bombedPieces).ToList();

            bombedPieces = GetBombedPieces(gamePieces);
            gamePieces = gamePieces.Union(bombedPieces).ToList();

            List<GamePiece> collectedPieces = FindCollectiblesAt(0, true);

            List<GamePiece> allCollectibles = FindAllCollectibles();
            List<GamePiece> blockers = gamePieces.Intersect(allCollectibles).ToList();
            collectedPieces = collectedPieces.Union(blockers).ToList();

            CollectibleCount -= collectedPieces.Count;

            gamePieces = gamePieces.Union(collectedPieces).ToList();

            List<int> collumnsToCollapse= GetColumns(gamePieces);

            ClearPieceAt(gamePieces, bombedPieces);
            BreakTileAt(gamePieces);

            if(_clickedTileBomb !=null)
            {
                ActivateBomb(_clickedTileBomb);
                _clickedTileBomb = null;
            }
            if(_targetTileBomb!=null)
            {
                ActivateBomb(_targetTileBomb);
                _targetTileBomb = null;
            }

            yield return new WaitForSeconds(0.25f);

            movingPieces = CollapseColumn(collumnsToCollapse);
            while (!IsCollapsed(movingPieces))
            {
                yield return null;
            }
            yield return new WaitForSeconds(0.2f);

            matches = FindMatchesAt(movingPieces);
            collectedPieces = FindCollectiblesAt(0, true);
            matches = matches.Union(collectedPieces).ToList();

            if (matches.Count == 0)
            {
                isFinished = true;
                break;
            }
            else
            {
                _scoreMultiplier++;
                if(SoundManager.Instance!=null)
                {
                    SoundManager.Instance.PlayBonusSound();
                }
                yield return StartCoroutine(ClearAndCollapseRoutine(matches));
            }
        }
        yield return null;
    }
    IEnumerator RefillRoutine()
    {
        FillBoard(FillYOffset, FillMoveTime);

        yield return null;

    }
    bool IsCollapsed(List<GamePiece> gamePieces)
    {
        foreach (GamePiece piece in gamePieces)
        {
            if (piece != null)
            {
                if (piece.transform.position.y - piece.YIndex > 0.001f)
                {
                    return false;
                }
                if (piece.transform.position.x - piece.XIndex > 0.001f)
                {
                    return false;
                }
            }
        }
        return true;
    }
    List<GamePiece> GetRowPieces(int row)
    {
        List<GamePiece> gamePieces = new List<GamePiece>();

        for (int i = 0; i < _width; i++)
        {
            if (_allGamePieces[i, row] != null)
            {
                gamePieces.Add(_allGamePieces[i, row]);
            }
        }
        return gamePieces;
    }
    List<GamePiece> GetColumnPieces(int column)
    {
        List<GamePiece> gamePieces = new List<GamePiece>();

        for (int i = 0; i < _height; i++)
        {
            if (_allGamePieces[column, i] != null)
            {
                gamePieces.Add(_allGamePieces[column, i]);
            }
        }
        return gamePieces;
    }
    List<GamePiece> GetAdjacentPieces(int x, int y, int offset = 1)
    {
        List<GamePiece> gamePieces = new List<GamePiece>();

        for (int i = x - offset; i <= x + offset; i++)
        {
            for (int j = y - offset; j <= y + offset; j++)
            {
                if (IsWithinBounds(i, j))
                {
                    gamePieces.Add(_allGamePieces[i, j]);
                }

            }
        }

        return gamePieces;
    }
    List<GamePiece> GetBombedPieces(List<GamePiece> gamePieces)
    {
        List<GamePiece> allPiecesToClear = new List<GamePiece>();

        foreach (GamePiece piece in gamePieces)
        {
            if (piece != null)
            {
                List<GamePiece> piecesToClear = new List<GamePiece>();

                Bomb bomb = piece.GetComponent<Bomb>();

                if (bomb != null)
                {
                    switch (bomb.BombType)
                    {
                        case BombType.Column:
                            piecesToClear = GetColumnPieces(bomb.XIndex);
                            break;
                        case BombType.Row:
                            piecesToClear = GetRowPieces(bomb.YIndex);
                            break;
                        case BombType.Adjacent:
                            piecesToClear = GetAdjacentPieces(bomb.XIndex, bomb.YIndex, 1);
                            break;
                        case BombType.Color:

                            break;
                    }

                    allPiecesToClear = allPiecesToClear.Union(piecesToClear).ToList();
                    allPiecesToClear = RemoveCollectibles(allPiecesToClear);
                }
            }
        }

        return allPiecesToClear;
    }
    bool IsCornerMatch(List<GamePiece> gamePieces)
    {
        bool vertical = false;
        bool horizontal = false;
        int xStart = -1;
        int yStart = -1;

        foreach (GamePiece piece in gamePieces)
        {
            if(piece!=null)
            {
                if(xStart==-1||yStart==-1)
                {
                    xStart = piece.XIndex;
                    yStart = piece.YIndex;
                    continue;
                }
                if (piece.XIndex != xStart && piece.YIndex == yStart)
                    horizontal = true;
                if (piece.XIndex == xStart && piece.YIndex != yStart)
                    vertical = true;
            }
        }
        return horizontal && vertical;
    }
    GameObject DropBomb(int x, int y, Vector2 SwapDirection, List<GamePiece> gamePieces)
    {
        GameObject bomb = null;

        MatchValue matchValue = MatchValue.None;
        if(gamePieces!=null)
        {
            matchValue = FindMatchValue(gamePieces);
        }


        if (gamePieces.Count >= 5 && matchValue != MatchValue.None)
        {
            if (IsCornerMatch(gamePieces))
            {
                GameObject adjacentBomb = FindGamePieceByMatchValue(AdjacentBombPrefabs, matchValue);

                if (adjacentBomb != null)
                {
                    bomb = MakeBomb(adjacentBomb, x, y);
                }
            }
            else
            {
                if (ColorBombPrefab != null)
                {
                    bomb = MakeBomb(ColorBombPrefab, x, y);
                }
            }
        }
        else if (gamePieces.Count == 4 && matchValue != MatchValue.None)
        {

            if (SwapDirection.x != 0)
            {
                GameObject rowBomb = FindGamePieceByMatchValue(RowBombPrefabs, matchValue);

                if (rowBomb != null)
                {
                    bomb = MakeBomb(rowBomb, x, y);
                }
            }
            else
            {
                GameObject columnBomb = FindGamePieceByMatchValue(ColumnBombPrefabs, matchValue);

                if (columnBomb != null)
                {
                    bomb = MakeBomb(columnBomb, x, y);
                }
            }
        }                 
        return bomb;
    }
    void ActivateBomb(GameObject bomb)
    {
        int x = (int)bomb.transform.position.x;
        int y = (int)bomb.transform.position.y;

        if(IsWithinBounds(x,y))
        {
            _allGamePieces[x, y] = bomb.GetComponent<GamePiece>();
        }
    }
    List<GamePiece> FindAllMatchValue(MatchValue mValue)
    {
        List<GamePiece> foundPieces = new List<GamePiece>();
        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                if (_allGamePieces[i, j] != null && _allGamePieces[i, j].MatchValue == mValue)
                {
                    foundPieces.Add(_allGamePieces[i, j]);
                }
            }
        }
        return foundPieces;
    }
    bool IsColorBomb(GamePiece gamePiece)
    {
        Bomb bomb = gamePiece.GetComponent<Bomb>();
        if(bomb!=null)
        {
            return bomb.BombType == BombType.Color;
        }
        return false;
    }
    List<GamePiece> FindCollectiblesAt(int row, bool clearedAtBottomOnly = false)
    {
        List<GamePiece> foundCollectibles = new List<GamePiece>();
        for (int i = 0; i < _width; i++)
        {
            if(_allGamePieces[i,row]!=null)
            {
                Collectible collectibleComponent = _allGamePieces[i, row].GetComponent<Collectible>();
                if(collectibleComponent!=null)
                {
                    if (!clearedAtBottomOnly || (clearedAtBottomOnly && collectibleComponent.ClearedAtBottom))
                    {
                        foundCollectibles.Add(_allGamePieces[i, row]);
                    }
                }
            }
        }
        return foundCollectibles;
    }
    List<GamePiece> FindAllCollectibles()
    {
        List<GamePiece> foundCollectibles = new List<GamePiece>();
        for (int i = 0; i < _height; i++)
        {
            List<GamePiece> collectibleRow = FindCollectiblesAt(i);
            foundCollectibles = foundCollectibles.Union(collectibleRow).ToList();
        }
        return foundCollectibles;
    }
    bool CanAddCollectible()
    {
        return Random.Range(0f, 1f) <= ChanceForCollectible && CollectiblePrefabs.Length > 0
            && CollectibleCount < MaxCollectibles;
    }
    List<GamePiece> RemoveCollectibles(List<GamePiece> bombedPieces)
    {
        List<GamePiece> collectiblePieces = FindAllCollectibles();
        List<GamePiece> piecesToRemove = new List<GamePiece>();

        foreach (GamePiece piece in collectiblePieces)
        {
            Collectible collectibleComponent = piece.GetComponent<Collectible>();
            if(collectibleComponent!=null)
            {
                if(!collectibleComponent.ClearedByBomb)
                {
                    piecesToRemove.Add(piece);
                }
            }
        }
        return bombedPieces.Except(piecesToRemove).ToList();
    }
    MatchValue FindMatchValue(List<GamePiece> gamePieces)
    {
        foreach (GamePiece piece in gamePieces)
        {
            if(piece!=null)
            {
                return piece.MatchValue;
            }
        }
        return MatchValue.None;
    }
    GameObject FindGamePieceByMatchValue(GameObject[] gamePiecePrefabs, MatchValue matchValue)
    {
        if(matchValue==MatchValue.None)
        {
            return null;
        }
        foreach (GameObject pref in gamePiecePrefabs)
        {
            GamePiece gamePiece = pref.GetComponent<GamePiece>();
            if(gamePiece != null)
            {
                if (gamePiece.MatchValue==matchValue)
                {
                    return pref;
                }
            }
        }
        return null;
    }
    public void TestDeadlock()
    {
        _boardDeadlock.IsDeadlocked(_allGamePieces, 3);
    }
    public void ShuffleBoard()
    {
        if(_playerInputEnabled)
        {
            StartCoroutine(ShuffleBoardRoutine());
        }
    }
    IEnumerator ShuffleBoardRoutine()
    {
        List<GamePiece> allPieces = new List<GamePiece>();
        foreach (GamePiece piece in _allGamePieces)
        {
            allPieces.Add(piece);
        }
        while (!IsCollapsed(allPieces))
        {
            yield return null;
        }

        List<GamePiece> normalPieces = _boardShuffler.RemoveNormalPieces(_allGamePieces);
        _boardShuffler.ShuffleList(normalPieces);
        FillBoardFromList(normalPieces);
        _boardShuffler.MovePieces(_allGamePieces, SwapTime);
        List<GamePiece> matches = FindAllMatches();
        StartCoroutine(ClearAndRefillBoardRoutine(matches));
    }

    #endregion

}