using System;

namespace Checkers
{
    public class Board
    {
        public const int BoardSize = 8;

        private static Piece[,] b = new Piece[8, 8];
        public static Piece[,] board
        {
            get { return b; }
            set { b = value; }
        }

        static Board()
        {
            InitializeBoard();
        }

        public static Piece[,] GetBoard()
        {
            return b;
        }

        public static void PrintBoard()
        {
            Console.Clear();
            Console.WriteLine(Game.HumanTurn ? "+" : "-");
            Console.Write("   ");
            for (char i = 'a'; i <= 'h'; i++)
            {
                Console.Write(i + " ");
            }
            Console.WriteLine();

            for (int row = 0; row < 8; row++)
            {
                Console.Write((row + 1) + "  ");
                for (int col = 0; col < 8; col++)
                {
                    Console.Write(board[row, col].Color.PadRight(2));
                }
                Console.WriteLine();
            }
        }

        public static bool IsWinningState()
        {
            return CountPieces("B") == 0;
        }

        public static bool IsLosingState()
        {
            return CountPieces("W") == 0;
        }

        private static int CountPieces(string color)
        {
            int count = 0;
            for (int i = 0; i < BoardSize; i++)
            {
                for (int j = 0; j < BoardSize; j++)
                {
                    if (board[i, j].Color.ToUpper() == color)
                        count++;
                }
            }
            return count;
        }

        public static void InitializeBoard()
        {
            for (int row = 0; row < BoardSize; row++)
            {
                for (int col = 0; col < BoardSize; col++)
                {
                    if ((row + col) % 2 == 1)
                    {
                        if (row < 3)
                        {
                            board[row, col] = new Piece("B");
                        }
                        else if (row > 4)
                        {
                            board[row, col] = new Piece("W");
                        }
                        else
                        {
                            board[row, col] = new Piece("-");
                        }
                    }
                    else
                    {
                        board[row, col] = new Piece();
                    }
                }
            }
        }
    }
}
