using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gameplay : MonoBehaviour
{
    //Parameter variables
    [SerializeField] GameObject blackTile;
    [SerializeField] GameObject redTile;
    [SerializeField] GameObject leftTopCorner;
    [SerializeField] GameObject rightBottomCorner;
    Vector3 leftTopPosition;
    Vector3 rightBottomPosition;

    //State variables
    Player currentPlayerMove = Player.PLAYER_ONE;
    GameObject currentTile;
    bool cursorOutsideOfBoard = true;

    // Start is called before the first frame update
    private void Start()
    {
        currentTile = Instantiate(blackTile);

        leftTopPosition = leftTopCorner.transform.position;
        rightBottomPosition = rightBottomCorner.transform.position;
    }

    // Update is called once per frame
    private void Update()
    {
        UpdateCurrentTilesPosition();
        IfMouseIsClickedRenderNewTile();
    }

    private void IfMouseIsClickedRenderNewTile()
    {
        if (Input.GetMouseButtonDown(0) && !cursorOutsideOfBoard)
        {
            switch (currentPlayerMove)
            {
                case Player.PLAYER_ONE:
                    currentTile = Instantiate(redTile, GetMousePositionInUnits(), transform.rotation);
                    currentPlayerMove = Player.PLAYER_TWO;
                    break;
                case Player.PLAYER_TWO:
                    currentTile = Instantiate(blackTile, GetMousePositionInUnits(), transform.rotation);
                    currentPlayerMove = Player.PLAYER_ONE;
                    break;
            }
        }
    }

    private void UpdateCurrentTilesPosition()
    {
        var mousePosInUnits = GetMousePositionInUnits();

        var newXPosition = RoundGivenCoordinate(mousePosInUnits.x);
        var newYPosition = RoundGivenCoordinate(mousePosInUnits.y);

        if (CursorInGameBoard(newXPosition, newYPosition))
        {
            currentTile.SetActive(false);
            cursorOutsideOfBoard = true;
        }
        else
        {
            currentTile.SetActive(true);
            cursorOutsideOfBoard = false;
        }

        currentTile.transform.position = new Vector3(newXPosition, newYPosition, -5);
    }

    private bool CursorInGameBoard(float x, float y)
    {
        return x + 0.25 < leftTopCorner.transform.position.x || 
               x - 0.25 > rightBottomCorner.transform.position.x || 
               y - 0.25 > leftTopCorner.transform.position.y || 
               y + 0.25 < rightBottomCorner.transform.position.y;
    }

    private float RoundGivenCoordinate(double coordinate)
    {
        if (coordinate >= 0)
        {
            double mainPart = Math.Floor(coordinate);
            double secondaryPart = coordinate % 1;
            if (secondaryPart > 0.5)
            {
                return (float)(mainPart + 0.75);
            }
            else
            {
                return (float)(mainPart + 0.25);
            }
        }
        else
        {
            double mainPart = Math.Ceiling(coordinate);
            double secondaryPart = Math.Abs(coordinate % 1);
            
            if (secondaryPart > 0.5)
            {
                return (float)(mainPart - 0.75);
            }
            else
            {
                return (float)(mainPart - 0.25);
            }
        }

    }

    private Vector3 GetMousePositionInUnits()
    {
        var mousePosition = Input.mousePosition;
        return Camera.main.ScreenToWorldPoint(mousePosition);
    }
}
