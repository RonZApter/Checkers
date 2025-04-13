using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.Json;
using static System.Collections.Specialized.BitVector32;

namespace Checkers
{
    public class AI : Player
    {
        public AI()
        {
            Name = "AI";
            isHuman = false;
        }



        
    
        public static Dictionary<string, Move> mandatoryMoves;
        public static Dictionary<string, Move > regularMoves;

        private static Dictionary<string, Dictionary<string, double>> QTable = new();
       
        private static double alpha;  // Learning rate
        private static double gamma;  // Discount factor
        private static double epsilon;  // Exploration rate



        
        public static void Train()
        {
            string state = GetBoardState();
            Move move = ChooseMove();

            if (!move.Apply()) return;
            string nextState = GetBoardState();
            if (Board.IsWinningState() || Board.IsLosingState())
            {
                Console.WriteLine("Game Over.");
                return;
            }
            double reward = GetReward(move);

            UpdateQValue(state, move.ToString(), reward, nextState);
        }

        private static void UpdateQValue(string state, string action, double reward, string nextState)
        {
            if (!QTable.ContainsKey(state))
                QTable[state] = new Dictionary<string, double>();

            if (!QTable[state].ContainsKey(action))
                QTable[state][action] = 0.0;

            double maxNextQ = QTable.ContainsKey(nextState) ? QTable[nextState].Values.Max() : 0;

            QTable[state][action] = QTable[state][action] + alpha * (reward + gamma * maxNextQ - QTable[state][action]);
        }
       
        
        
        
        
        
 
        public static Move ChooseMove()
        {
            GetAvailableMoves(out var regularMoves, out var mandatoryMoves);

            Dictionary<string, Move> possibleMoves = mandatoryMoves.Count > 0 ? mandatoryMoves : regularMoves;
            if (possibleMoves.Count == 0) return null;

            string state = GetBoardState();

            if (!QTable.ContainsKey(state) || new Random().NextDouble() < epsilon)
            {
                // Exploration-> Random move.
                if (!QTable.ContainsKey(state) || new Random().NextDouble() < epsilon)
                {
                    return possibleMoves.Values.ElementAt(new Random().Next(possibleMoves.Count));
                }
            }

            // Exploitation-> Best Qvalue.
            Move bestMove = null;
            double bestValue = double.MinValue;

            foreach (Move move in possibleMoves.Values)
            {

                double value = 0;

                if (QTable[state].ContainsKey(move.ToString()))
                {
                    value = QTable[state][move.ToString()];
                }

                if (value > bestValue)
                {
                    bestValue = value;
                    bestMove = move;
                }
            }

            return bestMove;
        }

        private static double GetReward(Move move)
        {
            if (move.isCapture) return 10;  
            if (Board.IsWinningState()) return 100; 
            if (Board.IsLosingState()) return -100;  
            if (move.makesQueen) return 90;

            return -0.1;  
        }


        private static string GetBoardState()
        {
            Piece[,] board = Board.GetBoard();
            string state = "";

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    state += board[i, j].Color;
                }
            }
            return state;
        }

        public static void GetAvailableMoves(out Dictionary<string, Move> regular, out Dictionary<string, Move> mandatory)
        {
            regular = new Dictionary<string, Move>();
            mandatory = new Dictionary<string, Move>();

            Piece[,] board = Board.GetBoard();
            string currentColor = Game.HumanTurn ? "W" : "B";
            string opponentColor = currentColor == "W" ? "B" : "W";

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (board[i, j].Color.ToUpper() != currentColor || (i + j) % 2 != 1)
                        continue;

                    int[] rowSteps = { -1, -1, 1, 1 };
                    int[] colSteps = { -1, 1, -1, 1 };


                    for (int k = 0; k < 4; k++)
                    {
                        int destRow = i + rowSteps[k];
                        int destCol = j + colSteps[k];

                        if (!Piece.IsInBounds(destRow, destCol))
                            continue;

                        if (board[destRow, destCol].Color == "-")
                        {
                            Move move = new Move(i, j, destRow, destCol);
                            if (move.isLegal)
                            {
                                regular.Add(move.ToString(), move);
                            }
                        }
                    }

                    int[] rowCaptureSteps = { -2, -2, 2, 2 };
                    int[] colCaptureSteps = { -2, 2, -2, 2 };

                    for (int k = 0; k < 4; k++)
                    {
                        int destRow = i + rowCaptureSteps[k];
                        int destCol = j + colCaptureSteps[k];

                        if (!Piece.IsInBounds(destRow, destCol))
                            continue;

                        if (board[destRow, destCol].Color == "-")
                        {
                            Move move = new Move(i, j, destRow, destCol);
                            if (move.isCapture && board[move.midRow, move.midCol].Color ==opponentColor)
                            {
                                mandatory.Add(move.ToString(), move);
                            }

                        }
                    }
                }
            }
        }
        public static void SaveQTable()
        {
            string json = JsonSerializer.Serialize(QTable);

            File.WriteAllText("QTable.json", json);

            Console.WriteLine("QTable saved to QTable.json");  // Debugging message
        }

        public static void LoadQTable()
        {

            if (File.Exists("QTable.json"))
            {
                QTable = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, double>>>(File.ReadAllText("QTable.json"));
                //Console.WriteLine("QTable loaded: " + JsonSerializer.Serialize(QTable));  
                // Console.WriteLine("QTable.json created!");
                // Console.WriteLine(Directory.GetCurrentDirectory());

                return;
            }
            string json = JsonSerializer.Serialize(QTable, new JsonSerializerOptions { WriteIndented = true });
         //   File.WriteAllText("QTable.json", json);

        }

        public static void LoadParameters()
        {
            if (File.Exists("parameters.json"))
            {
                string json = File.ReadAllText("parameters.json");
                var parameters = JsonSerializer.Deserialize<Dictionary<string, double>>(json);
                if (parameters != null)
                {
                    alpha = parameters.ContainsKey("alpha") ? parameters["alpha"] : 0.1;
                    gamma = parameters.ContainsKey("gamma") ? parameters["gamma"] : 0.9;
                    epsilon = parameters.ContainsKey("epsilon") ? parameters["epsilon"] : 0.2;
                }
            }
            else
            {
                alpha = 0.1;
                gamma = 0.9;
                epsilon = 0.2;
                SaveParameters();
            }
        }

  public static void SaveParameters()
{
    alpha = Math.Max(alpha * 0.9999, 0.01);  
    gamma = Math.Min(gamma * 0.9999, 0.8);  
    epsilon = Math.Max(epsilon * 0.9999, 0.05); 

    var parameters = new Dictionary<string, double>
    {
        { "alpha", alpha },
        { "gamma", gamma },
        { "epsilon", epsilon }
    };

    string json = JsonSerializer.Serialize(parameters, new JsonSerializerOptions { WriteIndented = true });
    File.WriteAllText("parameters.json", json);
}






    }
}
