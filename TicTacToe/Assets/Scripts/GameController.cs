using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Timers;
using Assets.Scripts.AI_Game_Model;



[System.Serializable]
public class Player
{
    public Image panel;
    public Text text;
    public Button button;
}



[System.Serializable]
public class PlayerColor
{
    public Color panelColor;
    public Color textColor;
}



public class GameController : MonoBehaviour {

    public Text[] buttonList;
    public GameObject gameOverPanel;
    public Text gameOverText;
    public GameObject restartButton;
    public Player playerX;
    public Player playerO;
    public PlayerColor activePlayerColor;
    public PlayerColor inactivePlayerColor;
    public GameObject startInfo;
    public GameObject goFirst;
    public GameObject goSecond;
    public AI ai;

    private int moveCount;
    private string currentPlayer;
    private string humanSide;
    private string AIside;
    
    private void Awake()
    {
        SetGameControllerReferenceOnButtons();
        SetGameControllerReferenceOnAI();
        gameOverPanel.SetActive(false);
        moveCount = 0;
        restartButton.SetActive(false);
        SetPlayerButtons(true);
        RenderOrderButtons(false);
    }


    public string GetWhosTurn() { return currentPlayer; }


    void StartGame()
    {
        SetBoardInteractable(true);
        if (currentPlayer == AIside)
        {
            //if AI goes first, we want the first move to be random
            //to add variety to the game. This is no problem, because
            //if both players are playing optimally, it will always draw
            //no matter which initial position is selected as long as it
            //is the middle or a corner
            System.Random rng = new System.Random();
            int[] possibleFirstMoves = new int[5] { 0, 2, 4, 6, 8 };
            int randomNumber = rng.Next(5);
            ExecuteAIMove(possibleFirstMoves[randomNumber]);
        }
    }

    void SetBoardInteractable(bool toggle)
    {
        for (int i = 0; i < buttonList.Length; i++)
        {
            buttonList[i].GetComponentInParent<Button>().interactable = toggle;
        }
    }

    void SetPlayerColors(Player newPlayer, Player oldPlayer)
    {
        newPlayer.panel.color = activePlayerColor.panelColor;
        newPlayer.text.color = activePlayerColor.textColor;
        oldPlayer.panel.color = inactivePlayerColor.panelColor;
        oldPlayer.text.color = inactivePlayerColor.textColor;
    }

    public void SetHumanPlayer(string choice)
    {
        humanSide = choice;
        if (humanSide == "X")
        {
            AIside = "O";
            SetPlayerColors(playerX, playerO);
        }
        else
        {
            AIside = "X";
            SetPlayerColors(playerO, playerX);
        }
        SetPlayerButtons(false);
        startInfo.SetActive(false);
        RenderOrderButtons(true);
    }

    void RenderOrderButtons(bool toggle)
    {
        goFirst.SetActive(toggle);
        goSecond.SetActive(toggle);
    }

    public void SetOrder(string firstOrSecond)
    {
        if (firstOrSecond == "first")
        {
            currentPlayer = humanSide;
        }
        else
        {
            currentPlayer = AIside;
        }
        if (currentPlayer == "X")
        {
            SetPlayerColors(playerX, playerO);
        } else
        {
            SetPlayerColors(playerO, playerX);
        }
        RenderOrderButtons(false);
        StartGame();
    }
   

    void SetPlayerButtons(bool toggle)
    {
        playerX.button.interactable = toggle;
        playerO.button.interactable = toggle;
    }

    void SetPlayerColorsInactive()
    {
        playerX.panel.color = inactivePlayerColor.panelColor;
        playerX.text.color = inactivePlayerColor.textColor;
        playerO.panel.color = inactivePlayerColor.panelColor;
        playerO.text.color = inactivePlayerColor.textColor;
    }

    void SetGameControllerReferenceOnButtons()
    {
        for (int i = 0; i <buttonList.Length; i++)
        {
            buttonList[i].GetComponentInParent<GridSpace>().SetGameControllerReference(this);
        }
    }

    void SetGameControllerReferenceOnAI()
    {
        ai.SetGameControllerReference(this);
    }

    void SetGameOverText(string value)
    {
        gameOverPanel.SetActive(true);
        gameOverText.text = value;
    }
    

    void ChangeSides()
    {
        //change the player-side
        currentPlayer = (currentPlayer == "X") ? "O" : "X";

        //update the colors on the player panels
        if (currentPlayer == "X")
        {
            SetPlayerColors(playerX, playerO);
        } else
        {
            SetPlayerColors(playerO, playerX);
        }
    }

    public void EndTurn()
    {

        moveCount++;

        //top row
        if (buttonList[0].text == currentPlayer &&
            buttonList[1].text == currentPlayer &&
            buttonList[2].text == currentPlayer) 
        {
            GameOver(currentPlayer);
        } else

        //middle row
        if (buttonList[3].text == currentPlayer &&
            buttonList[4].text == currentPlayer &&
            buttonList[5].text == currentPlayer)
        {
            GameOver(currentPlayer);
        } else

        //bottom row
        if (buttonList[6].text == currentPlayer &&
            buttonList[7].text == currentPlayer &&
            buttonList[8].text == currentPlayer)
        {
            GameOver(currentPlayer);
        } else

        //left column
        if (buttonList[0].text == currentPlayer &&
            buttonList[3].text == currentPlayer &&
            buttonList[6].text == currentPlayer)
        {
            GameOver(currentPlayer);
        } else

        //middle column
        if (buttonList[1].text == currentPlayer &&
            buttonList[4].text == currentPlayer &&
            buttonList[7].text == currentPlayer)
        {
            GameOver(currentPlayer);
        } else

        //right column
        if (buttonList[2].text == currentPlayer &&
            buttonList[5].text == currentPlayer &&
            buttonList[8].text == currentPlayer)
        {
            GameOver(currentPlayer);
        } else

        // \ diagonal
        if (buttonList[0].text == currentPlayer &&
            buttonList[4].text == currentPlayer &&
            buttonList[8].text == currentPlayer)
        {
            GameOver(currentPlayer);
        } else

        // / diagonal
        if (buttonList[2].text == currentPlayer &&
            buttonList[4].text == currentPlayer &&
            buttonList[6].text == currentPlayer)
        {
            GameOver(currentPlayer);
        } else

        if (moveCount >= 9)
        {
            GameOver("draw");
        }
        // else go onto the next move
        else
        {
            ChangeSides();
            if (currentPlayer == AIside)
            {
                ExecuteAIMove();
            }
        }
    }

    void ExecuteAIMove()
    {
        int buttonChoice = ai.PickNextMove();
        buttonList[buttonChoice].GetComponentInParent<GridSpace>().SetSpace();
    }

    void ExecuteAIMove(int buttonChoice)
    {
        buttonList[buttonChoice].GetComponentInParent<GridSpace>().SetSpace();
    }

    //======================================================
    //     This is the random ExecuteAIMove() info
    //void ExecuteAIMove()
    //{
       // List<int> possibleMoves = new List<int>();
        //for (int i = 0; i < buttonList.Length; i++)
        //{
       //     //is buttonList[].text the field I am looking for?
        //    if (buttonList[i].text == "")
       //     {
     //           possibleMoves.Add(i);
     //       }
     //   }
    //    int choice = Random.Range(0, possibleMoves.Count - 1);
    //    int buttonChoice = possibleMoves[choice];
    //    buttonList[buttonChoice].GetComponentInParent<GridSpace>().SetSpace();
    //}

    void GameOver(string winningPlayer)
    {
        SetBoardInteractable(false);
        
        if (winningPlayer == "draw")
        {
            SetGameOverText("It's a draw!");
            SetPlayerColorsInactive();
        }
        else
        {
            SetGameOverText(winningPlayer + " Won!");
        }
        restartButton.SetActive(true);
    }

    public void RestartGame()
    {
        moveCount = 0;
        gameOverPanel.SetActive(false);
        restartButton.SetActive(false);
        SetPlayerButtons(true);
        SetPlayerColorsInactive();

        for (int i = 0; i < buttonList.Length; i++)
        {
            buttonList[i].text = "";
        }
        startInfo.SetActive(true);
    }

    
}
