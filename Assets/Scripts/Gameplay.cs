using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    float distanceBetweenTiles = 0.5f;
    float halfOfDistanceBetweenTiles;

    //State variables
    Player currentPlayerMove = Player.PLAYER_ONE;
    GameObject currentTile;
    bool cursorOutsideOfBoard = true;
    List<GameObject> allTilesInGame;
    bool gameOver = false;

    // Start is called before the first frame update
    private void Start()
    {
        currentTile = Instantiate(blackTile);
        SetCurrentTileOpacity(0.5f);
        allTilesInGame = new List<GameObject>();
        halfOfDistanceBetweenTiles = (float)Math.Round(distanceBetweenTiles / 2, 2);

        leftTopPosition = leftTopCorner.transform.position;
        rightBottomPosition = rightBottomCorner.transform.position;
    }

    // Update is called once per frame
    private void Update()
    {
        UpdateCurrentTilesPosition();
        IfMouseIsClickedRenderNewTile();
        if (CheckIfAnyPlayerWon() == true)
        {
            gameOver = true;
            string winner = currentPlayerMove == Player.PLAYER_ONE ? Player.PLAYER_TWO.ToString() : Player.PLAYER_ONE.ToString();
            print(winner + " won");
        }
    }

    private void IfMouseIsClickedRenderNewTile()
    {
        if (Input.GetMouseButtonDown(0) && !cursorOutsideOfBoard && !TileWasPlacedInMousePosition())
        {
            SetCurrentTileOpacity(1f);
            switch (currentPlayerMove)
            {
                case Player.PLAYER_ONE:
                    InstantiateNewTile(redTile);
                    break;
                case Player.PLAYER_TWO:
                    InstantiateNewTile(blackTile);
                    break;
            }
        }
    }

    private void InstantiateNewTile(GameObject newTile)
    {
        allTilesInGame.Add(currentTile);
        currentTile = Instantiate(newTile, GetMousePositionInUnits(), transform.rotation);
        SetCurrentTileOpacity(0.5f);
        currentPlayerMove = currentPlayerMove == Player.PLAYER_ONE ? Player.PLAYER_TWO : Player.PLAYER_ONE;
    }

    private void UpdateCurrentTilesPosition()
    {
        var mousePosInUnits = GetMousePositionInUnits();

        var newXPosition = RoundGivenCoordinate(mousePosInUnits.x);
        var newYPosition = RoundGivenCoordinate(mousePosInUnits.y);

        if (CursorInGameBoard(newXPosition, newYPosition) || TileWasPlacedInMousePosition())
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
        float dif = halfOfDistanceBetweenTiles;
        return x + dif < leftTopCorner.transform.position.x ||
               x - dif > rightBottomCorner.transform.position.x ||
               y - dif > leftTopCorner.transform.position.y ||
               y + dif < rightBottomCorner.transform.position.y;
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

    private bool TileWasPlacedInMousePosition()
    {
        foreach (var tile in allTilesInGame)
        {
            if (tile.transform.position == currentTile.transform.position) return true;
        }
        return false;
    }

    private void SetCurrentTileOpacity(float opacity)
    {
        switch (currentTile.tag)
        {
            case "RedTile":
                currentTile.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, opacity);
                break;
            case "BlackTile":
                currentTile.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, opacity);
                break;
        }
    }

    private bool CheckIfAnyPlayerWon()
    {
        if (allTilesInGame.Count >= 9)
        {
            var lastTile = allTilesInGame[allTilesInGame.Count - 1];
            var lastTileTag = currentTile.tag == "RedTile" ? "BlackTile" : "RedTile";

            bool resultOfCheck = false;

            for (int i = 4; i >= 0; i--)
            {
                Vector3 leftPos = lastTile.transform.position + new Vector3(distanceBetweenTiles * -i, 0, 0);
                Vector3 leftTopPos = lastTile.transform.position + new Vector3(distanceBetweenTiles * -i, distanceBetweenTiles * i, 0);
                Vector3 topPos = lastTile.transform.position + new Vector3(0, distanceBetweenTiles * i);
                Vector3 rightTopPos = lastTile.transform.position + new Vector3(distanceBetweenTiles * i, distanceBetweenTiles * i, 0);

                resultOfCheck = resultOfCheck || CheckFiveInRow(1, 0, leftPos, lastTileTag) || CheckFiveInRow(1, -1, leftTopPos, lastTileTag) || CheckFiveInRow(0, -1, topPos, lastTileTag) || CheckFiveInRow(-1, -1, rightTopPos, lastTileTag);
            }

            return resultOfCheck; 
        }
        return false;
    }

    private bool CheckFiveInRow(float deltaX, float deltaY, Vector3 position, string lastTileTag)  // -1 deltaX to check left and -1 deltaY to check bottom. 1 to check right and top side
    {                                                                                              // to check diagonals - both deltas 1 or -1
       

        for (int i = 0; i <= 4; i++)
        {
            var currentDelta = distanceBetweenTiles * i;

            if (!CheckIfTileIsOnGivenCoordinates(position.x + (currentDelta * deltaX), position.y + (currentDelta * deltaY), lastTileTag))
            {
                return false;
            }
            else
            {
                if (i == 4) return true;
            }
        }
        return false;
    }

    private bool CheckIfTileIsOnGivenCoordinates(float posX, float posY, string tag)
    {
        foreach (var tile in allTilesInGame)
        {
            if (tile.transform.position.x == posX && tile.transform.position.y == posY && tile.tag == tag)
            {
                return true;
            }
        }

        return false;
    }
}
