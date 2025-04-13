using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Checkers
{
    public class Move
    {
        public int startRow { get; }
        public int startCol { get; }
        public int destRow { get; }
        public int destCol { get; }
        public int midRow { get; }
        public int midCol { get; }
        public bool isCapture { get; }
        public bool isLegal { get; }

        public bool makesQueen { get; set; } = false;

        public Move(string? start, string? dest)
        {
            int sRow, sCol, dRow, dCol;

            if (!format(start, out sRow, out sCol) || !format(dest, out dRow, out dCol))
            {
                Console.WriteLine("Invalid move. Try again.");
                isLegal = false;
                return;
            }

            startRow = sRow;
            startCol = sCol;
            destRow = dRow;
            destCol = dCol;
            midRow = (startRow + destRow) / 2;
            midCol = (startCol + destCol) / 2;
            isCapture = CaptureCheck();
            isLegal = LegalCheck();
            if (!isLegal) { isCapture = false; }
        }


        public Move(int startRow, int startCol, int destRow, int destCol)
        {
            this.startRow = startRow;
            this.startCol = startCol;
            this.destRow = destRow;
            this.destCol = destCol;
            midRow = (startRow + destRow) / 2;
            midCol = (startCol + destCol) / 2;
            isCapture = CaptureCheck();
            isLegal = LegalCheck();
            if (!isLegal) { isCapture = false; }
        }

        private static bool format(string input, out int row, out int col)
        {
            row = col = -1;
            if (string.IsNullOrWhiteSpace(input) || input.Length < 2)
                return false;

            col = char.ToLower(input[0]) - 'a';
            if (col < 0 || col > 7)
                return false;

            if (!int.TryParse(input[1].ToString(), out row) || row < 1 || row > 8)
                return false;

            row--;
            return true;
        }


        public bool checkQueen()
        {
            string playerColor = Game.HumanTurn ? "W" : "B";

            if (isLegal)
            {
                if(playerColor=="B")
                {
                    if (destRow == 7)
                    {
                        return true;
                    }
                }
                if (playerColor == "W")
                {
                    if (destRow == 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }













        public bool CaptureCheck()
        {
            string playerColor = Game.HumanTurn ? "W" : "B";
            string opponentColor = Game.HumanTurn ? "B" : "W";
      

            if (!Piece.IsInBounds(destRow, destCol) || !Piece.IsInBounds(startRow, startCol))
                return false;

            Piece piece = Board.board[startRow, startCol];

            if (!piece.Color.ToUpper().Equals(playerColor))
                return false;

            bool isQueen = piece.IsQueen;

            if (!isQueen && ((playerColor == "W" && destRow > startRow) || (playerColor == "B" && destRow < startRow)))
                return false;

            if (Math.Abs(destRow - startRow) == 2 && Math.Abs(destCol - startCol) == 2)
            {
                return Board.board[midRow, midCol].Color.ToUpper() == opponentColor;
            }

            return false;
        }

        public bool LegalCheck()
        {
            if (!(Piece.IsInBounds(startCol, startRow) && Piece.IsInBounds(destRow, destCol))) return false;

            Piece[,] board = Board.GetBoard();
            string currentPlayerColor = Game.HumanTurn ? "W" : "B";
            string opponentColor = Game.HumanTurn ? "B" : "W";
            bool isQueen = board[startRow, startCol].IsQueen;
            int direction = Game.HumanTurn ? -1 : 1;

            if (board[startRow, startCol].Color.ToUpper() != currentPlayerColor) return false;

            if (isQueen)
            {
                if (!isCapture && Math.Abs(destRow - startRow) == 1 && Math.Abs(destCol - startCol) == 1 && board[destRow, destCol].Color == "-")
                    return true;

                if (isCapture && Math.Abs(destRow - startRow) == 2 && Math.Abs(destCol - startCol) == 2)
                {
                    int midRow = (startRow + destRow) / 2;
                    int midCol = (startCol + destCol) / 2;

                    if (board[midRow, midCol].Color.ToUpper() == opponentColor && board[destRow, destCol].Color == "-")
                        return true;
                }
            }
            else
            {
                if ((currentPlayerColor == "W" && destRow > startRow) ||
                    (currentPlayerColor == "B" && destRow < startRow))
                    return false;

                if (!isCapture && Math.Abs(destRow - startRow) == 1 && Math.Abs(destCol - startCol) == 1 && board[destRow, destCol].Color == "-")
                    return true;

                if (isCapture && destRow == startRow + (2 * direction) && Math.Abs(destCol - startCol) == 2)
                {
                    int midRow = (startRow + destRow) / 2;
                    int midCol = (startCol + destCol) / 2;

                    if (board[midRow, midCol].Color.ToUpper() == opponentColor && board[destRow, destCol].Color == "-")
                        return true;
                }
            }

            return false;
        }

        public bool Apply()
        {
            if (!isLegal) return false;

            string currentPlayerColor = Game.HumanTurn ? "W" : "B";
            Board.GetBoard()[destRow, destCol] = Board.GetBoard()[startRow, startCol];
            Board.GetBoard()[startRow, startCol] = new Piece("-");

            if (isCapture)
                Board.GetBoard()[midRow, midCol] = new Piece("-");

            makesQueen = checkQueen();
            if(this.makesQueen)
            {
                Board.board[destRow, destCol].Promote();
            }


            return true;
        }

        public override string ToString()
        {
            char startColChar = (char)(startCol + 'a');
            char endColChar = (char)(destCol + 'a');
            return $"{startColChar}{startRow + 1},{endColChar}{destRow + 1}";
        }
    }
}
