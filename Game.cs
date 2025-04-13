using Checkers;
using System;
using System.Text.Json;

namespace Checkers
{
    public class Game
    {




        public static Move? currentMove;
        public static Move lastMove;
        public static bool succesfulMove { get; set; }

        private static bool _humanTurn = true;
        public static bool HumanTurn
        {
            get { return _humanTurn; }
            set { _humanTurn = value; }
        }

        private static bool _gameOver = false;
        public static bool GameOver
        {
            get { return _gameOver; }
            set { _gameOver = value; }

        }

        public static void changeTurn()
        {
            HumanTurn = !HumanTurn;
        }


        public static void start()
        {


            GameOver = false;
            HumanTurn = true;
            succesfulMove = true;
            currentMove = null;
            lastMove = null;

            AI.LoadParameters();
            AI.LoadQTable();
            Board.InitializeBoard();

            while (!GameOver)
            {

                lastMove = currentMove;
                Board.PrintBoard();
                if (!succesfulMove)
                {
                    Console.WriteLine("Invalid move. Try again.");
                }

                AI.GetAvailableMoves(out AI.regularMoves, out AI.mandatoryMoves);


                if (!HumanTurn)
                {

                    Thread.Sleep(1000);
                    AI.Train();
                    succesfulMove = true;

                }

                else
                {

                    Console.WriteLine("Enter starting row:");
                    String startPos = Console.ReadLine();
                    Console.WriteLine("Enter starting column:");
                    string destPos = Console.ReadLine();



                    Move m = new Move(startPos, destPos);

                    currentMove = new Move(startPos, destPos);
                    succesfulMove = currentMove.Apply();
                }
                AI.GetAvailableMoves(out AI.regularMoves, out AI.mandatoryMoves);
         
                
                if (AI.mandatoryMoves.Count > 0 && currentMove.isCapture)   // not good enough need to check if the piece was also the one eating
                {


                    AI.SaveQTable();
                    AI.SaveParameters();
                }
                else if(succesfulMove)
                { 

                    
                    changeTurn();
                    


                    AI.SaveQTable();
                    AI.SaveParameters();
                }
                else
                {
                    Console.WriteLine("Invalid move. Try again.");
                    succesfulMove = false;
                }
            }

        }




        



        //    Console.WriteLine("non capturing moves:");
        //                foreach (Move move in AI.regularMoves.Values)
        //                {
        //                    Console.WriteLine(move);
        //                }
        //Console.WriteLine("Mandatory moves:");
        //                foreach (Move move in AI.mandatoryMoves.Values)
        //                {
        //                    Console.WriteLine(move);
        //                }









    }
}