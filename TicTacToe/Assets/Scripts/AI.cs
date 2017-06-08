using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.AI_Game_Model

//============================================================================//
//                             CONTENTS                                       //
//============================================================================//
//  1  -  Object Fields
//  2  -  Methods
//  3  -  Internal Classes (the AI's game model & algorithm):
//        3.1  -  Move
//        3.2  -  Side
//        3.3  -  Game
//        3.4  -  MiniMax
//

{
    public class AI : MonoBehaviour
    {

//============================================================================//
//                        1  -  OBJECT FIELDS                                 //
//============================================================================//
        public Text[] buttonList;
        private GameController gameController;

//============================================================================//
//                        2  -  METHODS                                       //
//============================================================================//
        public void SetGameControllerReference(GameController controller)
        {
            gameController = controller;
        }

        public int PickNextMove()
        {
            Game currentGame = new Game(buttonList[0].text, buttonList[1].text, buttonList[2].text, buttonList[3].text, buttonList[4].text, buttonList[5].text, buttonList[6].text, buttonList[7].text, buttonList[8].text, gameController.GetWhosTurn());
            Move wrappedMoveToDo = MiniMax.generateBestMove(currentGame, 9);
            int moveToDo = translateMove(wrappedMoveToDo);
            return moveToDo;
        }

        private int translateMove(Move move)
        {
            if ((move.row == 0) && (move.column == 0))
            {
                return 0;
            }
            else if ((move.row == 0) && (move.column == 1))
            {
                return 1;
            }
            else if ((move.row == 0) && (move.column == 2))
            {
                return 2;
            }
            else if ((move.row == 1) && (move.column == 0))
            {
                return 3;
            }
            else if ((move.row == 1) && (move.column == 1))
            {
                return 4;
            }
            else if ((move.row == 1) && (move.column == 2))
            {
                return 5;
            }
            else if ((move.row == 2) && (move.column == 0))
            {
                return 6;
            }
            else if ((move.row == 2) && (move.column == 1))
            {
                return 7;
            }
            else
            {
                return 8;
            }
        }

//============================================================================//
//                      3  -  INTERNAL CLASSES                                //
//============================================================================//

//----------------------------------------------------------------------------//
//                      3.1  -  MOVE                                          //
//----------------------------------------------------------------------------//
        struct Move : IEquatable<Move>
        {
            internal int row;
            internal int column;

            //                PERFORMANCE BOILERPLATE
            //-----------------------------------------------------------
            public bool Equals(Move other) { return this.row == other.row && this.column == other.column; }

            public override bool Equals(System.Object obj)
            {
                // Check for null values and compare run-time types.
                if (obj == null || GetType() != obj.GetType())
                    return false;

                Move other = (Move)obj;
                return this.row == other.row && this.column == other.column;
            }

            public override int GetHashCode() { return this.row ^ this.column; }
            public static bool operator ==(Move m1, Move m2) { return m1.Equals(m2); }
            public static bool operator !=(Move m1, Move m2) { return !m1.Equals(m2); }
        }

//-------------------------------------------------------------------------------//
//                            3.2  -  SIDE                                       //
//-------------------------------------------------------------------------------//
        enum Side { X, O }

//-------------------------------------------------------------------------------//
//                            3.3  -  GAME                                       //
//-------------------------------------------------------------------------------//

        class Game
        {
            private Side?[,] board = new Side?[3, 3];
            private Side currentPlayer;
            private List<Move> MoveHistory = new List<Move>(9);

            
            //             PUBLIC METHODS
            //-----------------------------------------------------------
            
            internal Side GetCurrentPlayer() { return currentPlayer; }
            
            internal bool IsGameOver()
            {
                return xWon() || oWon() || noWon();
            }
            
            internal int GetUtility(Side plyr)
            {
                if (plyr == Side.X)
                {
                    if (xWon()) return 1;
                    else if (oWon()) return -1;
                    else return 0;
                }
                else
                {
                    if (oWon()) return 1;
                    else if (xWon()) return -1;
                    else return 0;
                }
            }
            
            internal List<Move> GetLegalMoves()
            {
                List<Move> listOfMoves = new List<Move>(9);
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        if (board[i, j] == null)
                        {
                            Move move;
                            move.row = i;
                            move.column = j;
                            listOfMoves.Add(move);
                        }
                    }
                }
                return listOfMoves;
            }
            
            internal Game DoMove(Move m)
            {
                board[m.row, m.column] = GetCurrentPlayer();
                MoveHistory.Add(m);
                switchPlayer();
                return this;
            }
            
            internal void UndoMove()
            {
                Move lastMove = MoveHistory[MoveHistory.Count - 1];
                board[lastMove.row, lastMove.column] = null;
                MoveHistory.RemoveAt(MoveHistory.Count - 1);
                switchPlayer();
            }
            
            internal Move GetLastMove() { return MoveHistory[MoveHistory.Count - 1]; }

            
            //                 PRIVATE METHODS                                   
            //-------------------------------------------------------
            
            private void switchPlayer()
            {
                if (currentPlayer == Side.X) { currentPlayer = Side.O; }
                else { currentPlayer = Side.X; }
            }



            private bool noWon()
            {
                bool openSpaces = false;
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        if (this.board[i, j] == null) { openSpaces = true; break; }
                    }
                }
                return !openSpaces;
            }


            private bool xWon()
            {
                Side?[,] b = board;
                return ((b[0, 0] == Side.X && b[0, 1] == Side.X && b[0, 2] == Side.X) ||
                        (b[1, 0] == Side.X && b[1, 1] == Side.X && b[1, 2] == Side.X) ||
                        (b[2, 0] == Side.X && b[2, 1] == Side.X && b[2, 2] == Side.X) ||

                        (b[0, 0] == Side.X && b[1, 0] == Side.X && b[2, 0] == Side.X) ||
                        (b[0, 1] == Side.X && b[1, 1] == Side.X && b[2, 1] == Side.X) ||
                        (b[0, 2] == Side.X && b[1, 2] == Side.X && b[2, 2] == Side.X) ||

                        (b[0, 0] == Side.X && b[1, 1] == Side.X && b[2, 2] == Side.X) ||
                        (b[0, 2] == Side.X && b[1, 1] == Side.X && b[2, 0] == Side.X));
            }

            private bool oWon()
            {
                Side?[,] b = board;
                return ((b[0, 0] == Side.O && b[0, 1] == Side.O && b[0, 2] == Side.O) ||
                        (b[1, 0] == Side.O && b[1, 1] == Side.O && b[1, 2] == Side.O) ||
                        (b[2, 0] == Side.O && b[2, 1] == Side.O && b[2, 2] == Side.O) ||

                        (b[0, 0] == Side.O && b[1, 0] == Side.O && b[2, 0] == Side.O) ||
                        (b[0, 1] == Side.O && b[1, 1] == Side.O && b[2, 1] == Side.O) ||
                        (b[0, 2] == Side.O && b[1, 2] == Side.O && b[2, 2] == Side.O) ||

                        (b[0, 0] == Side.O && b[1, 1] == Side.O && b[2, 2] == Side.O) ||
                        (b[0, 2] == Side.O && b[1, 1] == Side.O && b[2, 0] == Side.O));
            }

            
            //                    CONSTRUCTOR
            //-----------------------------------------------------------
            internal Game(string space0, string space1, string space2,
                             string space3, string space4, string space5,
                             string space6, string space7, string space8,
                             string whosTurn)
            {

                switch (space0)
                {
                    case "":
                        this.board[0, 0] = null;
                        break;
                    case "X":
                        this.board[0, 0] = Side.X;
                        break;
                    default:
                        this.board[0, 0] = Side.O;
                        break;
                }

                switch (space1)
                {
                    case "":
                        this.board[0, 1] = null;
                        break;
                    case "X":
                        this.board[0, 1] = Side.X;
                        break;
                    default:
                        this.board[0, 1] = Side.O;
                        break;
                }

                switch (space2)
                {
                    case "":
                        this.board[0, 2] = null;
                        break;
                    case "X":
                        this.board[0, 2] = Side.X;
                        break;
                    default:
                        this.board[0, 2] = Side.O;
                        break;
                }

                switch (space3)
                {
                    case "":
                        this.board[1, 0] = null;
                        break;
                    case "X":
                        this.board[1, 0] = Side.X;
                        break;
                    default:
                        this.board[1, 0] = Side.O;
                        break;
                }

                switch (space4)
                {
                    case "":
                        this.board[1, 1] = null;
                        break;
                    case "X":
                        this.board[1, 1] = Side.X;
                        break;
                    default:
                        this.board[1, 1] = Side.O;
                        break;
                }

                switch (space5)
                {
                    case "":
                        this.board[1, 2] = null;
                        break;
                    case "X":
                        this.board[1, 2] = Side.X;
                        break;
                    default:
                        this.board[1, 2] = Side.O;
                        break;
                }

                switch (space6)
                {
                    case "":
                        this.board[2, 0] = null;
                        break;
                    case "X":
                        this.board[2, 0] = Side.X;
                        break;
                    default:
                        this.board[2, 0] = Side.O;
                        break;
                }

                switch (space7)
                {
                    case "":
                        this.board[2, 1] = null;
                        break;
                    case "X":
                        this.board[2, 1] = Side.X;
                        break;
                    default:
                        this.board[2, 1] = Side.O;
                        break;
                }

                switch (space8)
                {
                    case "":
                        this.board[2, 2] = null;
                        break;
                    case "X":
                        this.board[2, 2] = Side.X;
                        break;
                    default:
                        this.board[2, 2] = Side.O;
                        break;
                }

                switch (whosTurn)
                {
                    case "X":
                        this.currentPlayer = Side.X;
                        break;
                    case "O":
                        this.currentPlayer = Side.O;
                        break;
                }
            }
        }


//----------------------------------------------------------------------------//
//                            3.4  -  MINIMAX                                 //
//----------------------------------------------------------------------------//
        private static class MiniMax
        {

            
            //          helper MOVEVALUEPAIR struct                  
            //-------------------------------------------------------
            private struct moveValuePair : IEquatable<moveValuePair>
            {
                internal Move move;
                internal int value;

                //                 PERFORMANCE BOILERPLACE
                //--------------------------------------------------------------
                public bool Equals(moveValuePair other)
                {
                    return this.move == other.move && this.value == other.value;
                }

                public override bool Equals(System.Object obj)
                {
                    // Check for null values and compare run-time types.
                    if (obj == null || GetType() != obj.GetType())
                        return false;

                    moveValuePair mvp = (moveValuePair)obj;
                    return (this.move == mvp.move) && (this.value == mvp.value);
                }

                public override int GetHashCode()
                {
                    return this.move.row ^ this.move.column ^ this.value;
                }

                public static bool operator ==(moveValuePair mvp1, moveValuePair mvp2)
                {
                    return mvp1.Equals(mvp2);
                }
                public static bool operator !=(moveValuePair mvp1, moveValuePair mvp2)
                {
                    return !mvp1.Equals(mvp2);
                }
            }



            //================================================================================//
            //                         public ALPHABETAMINIMAX()                              //
            //================================================================================//
            //  INPUT:  GAMESTATE, a TicTacToe game instance
            //          MAXDEPTH, the maximum depth at which to perform the search
            //  OUTPUT: a MOVE that is the best move to do, given the depth at which it looks
            //  USER NOTES:  1) the range of possible scores in the game must be within the range:
            //                  [-99999,99999].
            //               2) the depth at which you want to search must be > 0

            internal static Move generateBestMove(Game gameState, int maxDepth)
            {

                //initialize alpha & beta
                int alpha = -99999;
                int beta = 99999;

                //initialize AI as NPC
                Side aiSide = gameState.GetCurrentPlayer();

                //initialize BESTMOVEVALUEPAIR to a reference value of a -1,-1 move and -99999 value
                moveValuePair bestMoveValuePair;
                bestMoveValuePair.move.row = -1;
                bestMoveValuePair.move.column = -1;
                bestMoveValuePair.value = -99999;

                //do each move
                List<Move> legalMoves = gameState.GetLegalMoves();
                int legalMovesCount = legalMoves.Count;
                int i;
                for (i = 0; i < legalMovesCount; i++)
                {
                    moveValuePair mvp = minMove(gameState.DoMove(legalMoves[i]), aiSide, maxDepth, 1, alpha, beta);
                    bestMoveValuePair = (mvp.value > bestMoveValuePair.value) ? mvp : bestMoveValuePair;
                    gameState.UndoMove();
                    alpha = Math.Max(alpha, bestMoveValuePair.value);
                }

                //Console.WriteLine("The value of the move the AI picked is: " + bestMoveFound.value);
                return bestMoveValuePair.move;
            }





            
            //                 PRIVATE (HELPER) METHODS                                  
            //-----------------------------------------------------------------



            //   private static maxMove()
            //======================================
            private static moveValuePair maxMove(Game gameState, Side aiSide, int maxDepth, int currDepth, int alpha, int beta)
            {
                //check if game is over or at maximum depth
                if (gameState.IsGameOver() || currDepth == maxDepth)
                {
                    moveValuePair mvp;
                    mvp.move = gameState.GetLastMove();
                    mvp.value = gameState.GetUtility(aiSide);
                    return mvp;
                }

                //initialize VAL to a reference value of a -1,-1 move and -99999
                moveValuePair val;
                val.move.row = -1;
                val.move.column = -1;
                val.value = -99999;

                //do each move
                List<Move> listOfLegalMoves = gameState.GetLegalMoves();
                int numberOfLegalMoves = listOfLegalMoves.Count;
                int i;
                for (i = 0; i < numberOfLegalMoves; i++)
                {
                    moveValuePair mvp = minMove(gameState.DoMove(listOfLegalMoves[i]), aiSide, maxDepth, currDepth + 1, alpha, beta);
                    val = (mvp.value > val.value) ? mvp : val;
                    gameState.UndoMove();
                    val.value = alpha = Math.Max(alpha, val.value);
                    if (alpha >= beta) { break; }
                }

                val.move = gameState.GetLastMove();
                return val;
            }



            //   private static minMove()
            //======================================
            private static moveValuePair minMove(Game gameState, Side aiSide, int maxDepth, int currDepth, int alpha, int beta)
            {

                //check if game is over or at maximum depth
                if (gameState.IsGameOver() || currDepth == maxDepth)
                {
                    moveValuePair mvp;
                    mvp.move = gameState.GetLastMove();
                    mvp.value = gameState.GetUtility(aiSide);
                    return mvp;
                }

                //initialize VAL to a reference value of a -1,-1 move and +99999
                moveValuePair val;
                val.move.row = -1;
                val.move.column = -1;
                val.value = 99999;

                //do each move
                List<Move> listOfLegalMoves = gameState.GetLegalMoves();
                int numberOfLegalMoves = listOfLegalMoves.Count;
                int i;
                for (i = 0; i < numberOfLegalMoves; i++)
                {
                    moveValuePair mvp = maxMove(gameState.DoMove(listOfLegalMoves[i]), aiSide, maxDepth, currDepth + 1, alpha, beta);
                    val = (mvp.value < val.value) ? mvp : val;
                    gameState.UndoMove();
                    val.value = beta = Math.Min(beta, val.value);
                    if (alpha >= beta) { break; }
                }

                val.move = gameState.GetLastMove();
                return val;
            }


        }
    }
}