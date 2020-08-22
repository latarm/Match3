using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(LevelGoal))]
public class GameManager : Singleton<GameManager>
{
    Board _board;

    bool _isReadyToBegin = false;
    bool _isGameOver = false;
    bool _isWinner = false;
    bool _isReadyToReload = false;

    public bool IsGameOver { get => _isGameOver; set => _isGameOver = value; }

    LevelGoal _levelGoal;
    LevelGoalCollected _levelGoalCollected;
    public LevelGoal LevelGoal { get => _levelGoal; }
   
    public override void Awake()
    {
        base.Awake();
        //_levelGoalTimed = GetComponent<LevelGoalTimed>();
        _levelGoal = GetComponent<LevelGoal>();
        _levelGoalCollected = GetComponent<LevelGoalCollected>();
        _board = FindObjectOfType<Board>().GetComponent<Board>();
    }

    private void Start()
    {
        if (UIManager.Instance != null)
        {
            if (UIManager.Instance.ScoreMeter != null)
            {
                UIManager.Instance.ScoreMeter.SetupStars(_levelGoal);
            }

            if (UIManager.Instance.LevelNameText != null)
            {
                Scene scene = SceneManager.GetActiveScene();
                UIManager.Instance.LevelNameText.text = scene.name;
            }
            if (_levelGoalCollected != null)
            {
                UIManager.Instance.EnableCollectionGoalLayout(true);
                UIManager.Instance.SetupCollectionGoalLayout(_levelGoalCollected.CollectionGoals);
            }
            else
            {
                UIManager.Instance.EnableCollectionGoalLayout(false);
            }

            bool useTimer = _levelGoal.LevelCounter == LevelCounter.Timer;
            UIManager.Instance.EnableTimer(useTimer);
            UIManager.Instance.EnableMovesCounter(!useTimer);
        }

        _levelGoal.MovesLeft++;
        UpdateMoves();

        StartCoroutine(ExecuteGameLoop());
    }
    public void UpdateMoves()
    {

        if (_levelGoal.LevelCounter == LevelCounter.Moves)
        {
            _levelGoal.MovesLeft--;
            if (UIManager.Instance!=null && UIManager.Instance.MovesLeftText != null)
            {
                UIManager.Instance.MovesLeftText.text = _levelGoal.MovesLeft.ToString();
            }
        }
    }
    IEnumerator ExecuteGameLoop()
    {
        yield return StartCoroutine(StartGameRoutine());
        yield return StartCoroutine(PlayGameRoutine());
        yield return StartCoroutine(WaitForBoardRoutine(0.5f));
        yield return StartCoroutine(EndGameRoutine());
    }
    public void BeginGame()
    {
        _isReadyToBegin = true;
    }
    IEnumerator StartGameRoutine()
    {
        if(UIManager.Instance.MessageWindow !=null)
        {
            UIManager.Instance.MessageWindow.GetComponent<RectXFormMover>().MoveOn();
            int maxGoal = _levelGoal.ScoreGoals.Length - 1;
            UIManager.Instance.MessageWindow.ShowScoreMessage(_levelGoal.ScoreGoals[maxGoal]);

            if(_levelGoal.LevelCounter==LevelCounter.Timer)
            {
                UIManager.Instance.MessageWindow.ShowTimedGoal(_levelGoal.TimeLeft);
            }
            else
            {
                UIManager.Instance.MessageWindow.ShowMovesGoal(_levelGoal.MovesLeft);
            }

            if(_levelGoalCollected!=null)
            {
                UIManager.Instance.MessageWindow.ShowCollectionGoal(true);

                GameObject goalLayout = UIManager.Instance.MessageWindow.collectionGoalLayout;
                if(goalLayout!=null)
                {
                    UIManager.Instance.SetupCollectionGoalLayout(_levelGoalCollected.CollectionGoals, goalLayout, 80);
                }
            }
        }


        while (!_isReadyToBegin)
        {
            yield return null;
        }
        if(UIManager.Instance!=null && UIManager.Instance.ScreenFader !=null)
        {
            UIManager.Instance.ScreenFader.FadeOff();
        }

        yield return new WaitForSeconds(0.5f);
        if(_board!=null)
        {
            _board.SetupBoard();
        }
    }
    IEnumerator PlayGameRoutine()
    {
        if(_levelGoal.LevelCounter==LevelCounter.Timer)
        {
            _levelGoal.StartCountdown();
        }

        while (!IsGameOver)
        {
            _isGameOver = _levelGoal.IsGameOver();
            _isWinner = _levelGoal.IsWinner();
            yield return null;
        }
    }
    IEnumerator EndGameRoutine()
    {
        _isReadyToReload = false;

        if (_isWinner)
        {
            ShowWinScreen();
        }
        else
        {
            ShowLoseScreen();
        }

        yield return new WaitForSeconds(1f);

        if (UIManager.Instance != null && UIManager.Instance.ScreenFader != null)
        {
            UIManager.Instance.ScreenFader.FadeOn();
        }
        
        while (!_isReadyToReload)
        {
            yield return null;
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        yield return null;
    }

    private void ShowLoseScreen()
    {
        if (UIManager.Instance != null && UIManager.Instance.MessageWindow != null)
        {
            UIManager.Instance.MessageWindow.GetComponent<RectXFormMover>().MoveOn();
            UIManager.Instance.MessageWindow.ShowLoseMessage();
            UIManager.Instance.MessageWindow.ShowCollectionGoal(false);

            string caption = "";

            if(_levelGoal.LevelCounter==LevelCounter.Timer)
            {
                caption = "Out of time";
            }
            else
            {
                caption = "Out of moves";
            }
           
            UIManager.Instance.MessageWindow.ShowGoalCaption(caption, 0, 70);

            if (UIManager.Instance.MessageWindow.LoseIcon != null)
            {
                UIManager.Instance.MessageWindow.ShowGoalImage(UIManager.Instance.MessageWindow.GoalFailedIcon);
            }
        }

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayLoseSound();
        }
    }

    private void ShowWinScreen()
    {
        if (UIManager.Instance != null && UIManager.Instance.MessageWindow != null)
        {
            UIManager.Instance.MessageWindow.GetComponent<RectXFormMover>().MoveOn();
            UIManager.Instance.MessageWindow.ShowWinMessage();
            UIManager.Instance.MessageWindow.ShowCollectionGoal(false);

            string scoreString = "you scored\n" + ScoreManager.Instance.CurrentScore.ToString() + " points";
            UIManager.Instance.MessageWindow.ShowGoalCaption(scoreString, 0, 70);

            if(UIManager.Instance.MessageWindow.GoalCompleteIcon!=null)
            {
                UIManager.Instance.MessageWindow.ShowGoalImage(UIManager.Instance.MessageWindow.GoalCompleteIcon);
            }
        }

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayWinSound();
        }
    }

    IEnumerator WaitForBoardRoutine(float delay = 0f)
    {
        if (_levelGoal.LevelCounter == LevelCounter.Timer && UIManager.Instance != null && UIManager.Instance.Timer != null)
        {
            UIManager.Instance.Timer.FadeOff();
            UIManager.Instance.Timer.Paused = true;
        }

        if(_board !=null)
        {
            yield return new WaitForSeconds(_board.SwapTime);
            while (_board.IsRefilling)
                yield return null;
        }
        yield return new WaitForSeconds(delay);
    }
    public void ReloadScene()
    {
        _isReadyToReload = true;
    }
    public void ScorePoints(GamePiece piece, int multiplier = 1, int bonus = 0)
    {
        if (piece != null)
        {
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.AddScore(piece.ScoreValue * multiplier + bonus);

                _levelGoal.UpdateScoreStars(ScoreManager.Instance.CurrentScore);
                if (UIManager.Instance != null)
                {
                    if (UIManager.Instance.ScoreMeter != null)
                    {
                        UIManager.Instance.ScoreMeter.UpdateScoreMeter(ScoreManager.Instance.CurrentScore, _levelGoal.ScoreStars);
                    }
                }
            }
            if (SoundManager.Instance != null && piece.ClearSound != null)
            {
                SoundManager.Instance.PlayClipAtPoint(piece.ClearSound, Vector3.zero, SoundManager.Instance.FxVolume);
            }
        }
    }
    public void AddTime(int timeValue)
    {
        if(_levelGoal.LevelCounter==LevelCounter.Timer)
        {
            _levelGoal.AddTime(timeValue);
        }
    }
    public void UpdateCollectionGoals(GamePiece pieceToCheck)
    {
        if(_levelGoalCollected!=null)
        {
            _levelGoalCollected.UpdateGoals(pieceToCheck);
        }
    }

}
