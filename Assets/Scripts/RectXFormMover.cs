using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class RectXFormMover : MonoBehaviour
{
    public Vector3 StartPosition;
    public Vector3 OnScreenPosition;
    public Vector3 EndPosition;

    public float TimeToMove = 1f;

    RectTransform _rectXform;
    bool _isMoving = false;

    private void Awake()
    {
        _rectXform = GetComponent<RectTransform>();
    }


    void Move(Vector3 startPos, Vector3 endPos, float timeToMove)
    {
        if(!_isMoving)
        {
            StartCoroutine(MoveRoutine(startPos, endPos, timeToMove));
        }
    }

    IEnumerator MoveRoutine(Vector3 startPos, Vector3 endPos, float timeToMove)
    {
        if(_rectXform!=null)
        {
            _rectXform.anchoredPosition = startPos;
        }

        bool reachedDestination = false;
        float elapsedTime = 0f;
        _isMoving = true;

        while(!reachedDestination)
        {
            if(Vector3.Distance(_rectXform.anchoredPosition,endPos)<0.01f)
            {
                reachedDestination = true;
                break;
            }
            elapsedTime += Time.deltaTime;

            float t = Mathf.Clamp(elapsedTime / timeToMove, 0f, 1f);
            t = t * t * t * (t * (t * 6 - 15) + 10);
            if (_rectXform != null)
                _rectXform.anchoredPosition = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }
        _isMoving = false;
    }

    public void MoveOn()
    {
        Move(StartPosition, OnScreenPosition, TimeToMove);
    }

    public void MoveOff()
    {
        Move(OnScreenPosition, EndPosition, TimeToMove);
    }
}
