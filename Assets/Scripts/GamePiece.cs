using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class GamePiece : MonoBehaviour
{
    #region Fields

    private bool _isMoving = false;
    private Board _board;

    public int XIndex;
    public int YIndex;
    public InterpolationType Interpolation = InterpolationType.SmootherStep;
    public MatchValue MatchValue = MatchValue.Blue;
    public int ScoreValue = 20;
    public AudioClip ClearSound;
    
    #endregion

    #region Methods

    public void Init(Board board)
    {
        _board = board;
    }
    public void SetCoord(float x, float y)
    {
        XIndex = (int)x;
        YIndex = (int)y;
    }
    public void Move(int destinationX, int destinationY, float timeToMove)
    {
        if (!_isMoving)
            StartCoroutine(MoveCoroutine(new Vector3(destinationX, destinationY, 0), timeToMove));
    }
    IEnumerator MoveCoroutine(Vector3 destination, float timeToMove)
    {
        Vector3 startPosition = transform.position;
        bool reachedDestination = false;
        float elapsedTime = 0f;
        _isMoving = true;

        while (!reachedDestination)
        {
            if (Vector3.Distance(transform.position, destination) < 0.01f)
            {
                reachedDestination = true;
                if (_board != null)
                    _board.PlaceGamePiece(this, (int)destination.x, (int)destination.y);

                break;
            }

            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp(elapsedTime / timeToMove, 0f, 1f);

            switch (Interpolation)
            {
                case InterpolationType.Linear:
                    break;
                case InterpolationType.EaseOut:
                    t = Mathf.Sin(t * Mathf.PI * 0.5f);
                    break;
                case InterpolationType.EaseIn:
                    t = 1 - Mathf.Cos(t * Mathf.PI * 0.5f);
                    break;
                case InterpolationType.SmoothStep:
                    t = t * t * (3 - 2 * t);
                    break;
                case InterpolationType.SmootherStep:
                    t = t * t * t * (t * (t * 6 - 15) + 10);
                    break;
            }

            transform.position = Vector3.Lerp(startPosition, destination, t);
            yield return null;
        }
        _isMoving = false;
    }
    public void ChangeColor(GamePiece pieceToMatch)
    {
        SpriteRenderer rendererToChange = GetComponent<SpriteRenderer>();

        Color colorToMatch = Color.clear;

        if (pieceToMatch != null)
        {
            SpriteRenderer rendererToMatch = pieceToMatch.GetComponent<SpriteRenderer>();
            if (rendererToMatch != null && rendererToChange != null)
            {
                rendererToChange.color = rendererToMatch.color;
            }
            MatchValue = pieceToMatch.MatchValue;
        }
        #endregion
    }
}
