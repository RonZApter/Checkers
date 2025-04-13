using System;

namespace Checkers
{
    public class Piece
    {
        private string color;
        private bool isQueen;

        public static bool IsInBounds(int row, int col)
        {
            return row >= 0 && row < Board.BoardSize && col >= 0 && col < Board.BoardSize;
        }

        public Piece(string color)
        {
            this.color = color;
            this.IsQueen = false;
        }

        public Piece()
        {
            color = "-";
            IsQueen = false;
        }

        public void Promote()
        {
            IsQueen = true;
            if (color == "W")
                Color = "w";
            else if (color == "B")
                Color = "b";
        }

        public string Color
        {
            get { return color; }
            set { color = value; }
        }

        public bool IsQueen
        {
            get { return isQueen; }
            set { isQueen = value; }
        }
    }
}
