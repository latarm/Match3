﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BoardDeadlock : MonoBehaviour
{
    List<GamePiece> GetRowOrColumnList(GamePiece[,] allPieces, int x, int y, int listLength=3, bool checkRow=true)
    {
        int width = allPieces.GetLength(0);
        int height = allPieces.GetLength(1);

        List<GamePiece> piecesList = new List<GamePiece>();

        for (int i = 0; i < listLength; i++)
        {
            if (checkRow)
            {
                if (x + i < width && y < height && allPieces[x + i, y] != null)
                {
                    piecesList.Add(allPieces[x + i, y]);
                }
            }
            else
            {
                if (x < width && y + i < height && allPieces[x, y + i] != null)
                {
                    piecesList.Add(allPieces[x, y + i]);
                }
            }
        }
        return piecesList;
    }
    List<GamePiece> GetMinimumMatches(List<GamePiece> gamePieces, int minForMatch = 2)
    {
        List<GamePiece> matches = new List<GamePiece>();

        var groups = gamePieces.GroupBy(n => n.MatchValue);

        foreach (var group in groups)
        {
            if (group.Count() >= minForMatch && group.Key != MatchValue.None)
                matches = group.ToList();
        }
        return matches;
    }
    List<GamePiece> GetNeighbors(GamePiece[,] allPieces,int x,int y)
    {
        int width = allPieces.GetLength(0);
        int height = allPieces.GetLength(1);

        List<GamePiece> neighbors = new List<GamePiece>();

        Vector2[] searchDirections = new Vector2[4]
        {
            new Vector2(1f,0),
            new Vector2(-1f,0),
            new Vector2(0f,1f),
            new Vector2(0,-1f),
        };

        foreach (Vector2 direction in searchDirections)
        {
            if (x + (int)direction.x >= 0 && x + (int)direction.x < width && y + (int)direction.y >= 0 && y + (int)direction.y < height)
            {
                if(allPieces[x+(int)direction.x,y+(int)direction.y]!=null)
                {
                    if (!neighbors.Contains(allPieces[x + (int)direction.x, y + (int)direction.y]))
                        neighbors.Add(allPieces[x + (int)direction.x, y + (int)direction.y]);
                }
            }
        }
        return neighbors;
    }
    bool HasMoveAt(GamePiece[,] allPieces, int x, int y, int listLength = 3, bool checkRow = true)
    {
        List<GamePiece> pieces = GetRowOrColumnList(allPieces, x, y, listLength, checkRow);
        List<GamePiece> matches = GetMinimumMatches(pieces, listLength - 1);

        GamePiece unmatchedPiece = null;

        if(pieces!=null && matches !=null)
        {
            if(pieces.Count==listLength&& matches.Count==listLength-1)
            {
                unmatchedPiece = pieces.Except(matches).FirstOrDefault();
            }
            if(unmatchedPiece!=null)
            {
                List<GamePiece> neighbors = GetNeighbors(allPieces, unmatchedPiece.XIndex, unmatchedPiece.YIndex);
                neighbors = neighbors.Except(matches).ToList();
                neighbors = neighbors.FindAll(n => n.MatchValue == matches[0].MatchValue);

                matches = matches.Union(neighbors).ToList();
            }
            if(matches.Count>=listLength)
            {
                string rowColStr = (checkRow) ? " row " : " column ";
                //Debug.Log("====AVAILABLE MOVE====");
                //Debug.Log("Move "+ matches[0].MatchValue+ " piece to "+ unmatchedPiece.XIndex+ ","+ unmatchedPiece.YIndex+ " to form matching " + rowColStr);
                return true;
            }
        }
        return false;
    }
    public bool IsDeadlocked(GamePiece[,] allPieces, int listLength=3)
    {
        int width = allPieces.GetLength(0);
        int height = allPieces.GetLength(1);

        bool isDeadlocked = true;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if(HasMoveAt(allPieces,i,j, listLength,true)|| HasMoveAt(allPieces,i,j, listLength,false))
                {
                    isDeadlocked = false;
                }
            }
        }
        if(isDeadlocked)
        {
            Debug.Log("===== BOARD DEADLOCKED =====");
        }
        return isDeadlocked;
    }

}
