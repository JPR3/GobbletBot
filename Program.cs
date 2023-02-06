using System.Diagnostics;

namespace GobbletBot
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Gobblet!\r\nType 'help' for a list of commands");
            Program prgm = new Program();
        }

        BoardState currentState;
        List<object> commandList;
        Command HELP;
        Command<string> INFO;
        Command RULES;
        Command CLS;
        Command HVH;
        Command<int, string> HVC;
        Command<int, int> CVC;
        Command<int, int, int> CVC2;
        Command<int, int> TOURNEY;
        public Program()
        {
            currentState = new BoardState();
            HELP = new Command("help", "shows command list, as well as how to play", "help", () =>
            {
                for (int i = 0; i < commandList.Count; i++)
                {
                    CommandBase commandBase = commandList[i] as CommandBase;
                    string output = $"{commandBase.commandName} - {commandBase.commandDescription}";
                    Console.WriteLine(output);
                }
                Console.WriteLine("\r\nTo play a turn, you first enter the starting position of the move.\r\n" +
                    "The board squares are numbered from left to right, top to bottom, starting at 0 and ending with 15.\r\n" +
                    "A starting position of 16 indicates a move from the pieces in your hand - you then specify the size of piece to play.\r\n" +
                    "Lastly, select an ending position for the piece. Remember you can cover smaller pieces!\r\n" +
                    "The board consits of 16 2-digit spaces - '00' indicates an empty space on the board or in the hand.\r\n" +
                    "A filled space consists of a letter to show the piece's color and a number showing the size (e.g. W4, B2, etc.)\r\n" +
                    "Above the board is Black's hand, and below is White's.\r\n" +
                    "\r\nFor general rules of play, use the 'rules' command.");
            });
            INFO = new Command<string>("info", "to show the syntax of a command, use 'info <command>'", "info <command>", (x) =>
            {
                foreach (object c in commandList)
                {
                    CommandBase commandBase = c as CommandBase;
                    if (commandBase != null && commandBase.commandName == x.ToLower())
                    {
                        Console.WriteLine($"{commandBase.commandName} - {commandBase.commandDescription} - {commandBase.commandSyntax}");
                    }

                }
            });
            RULES = new Command("rules", "displays the rules of Gobblet", "rules", () =>
            {
                Console.WriteLine("\r\nOBJECTIVE: Be the first player to align four of your Gobblets horizontally, vertically, or diagonally.\r\n" +
                    "At the start of the game, each player has three stacks of four Gobblets each, with only the largest Gobblets showing.\r\n" +
                    "On a player's turn, they may choose to either play a Gobblet from their hand or move one of their already placed pieces.\r\n" +
                    "Gobblets can be moved to any empty space, or used to cover smaller Gobblets of either color.\r\n" +
                    "If a Gobblet covering a piece is moved, the revealed piece does not move along with the piece being moved.\r\n" +
                    "When playing from the hand, only the top-most Gobblet of a stack may be played.\r\n" +
                    "Even if a player's hand is depleted completely, play continues using the pieces on the board.");
            });
            CLS = new Command("cls", "clears the console", "cls", () =>
            {
                Console.Clear();
            });
            HVH = new Command("hvh", "play a game between two human players", "hvh", () =>
            {
                Console.Clear();
                PlayHvH();
                currentState = new BoardState();
                Console.WriteLine();
            });
            HVC = new Command<int, string>("hvc", "play a game agains the computer", "hvc <depth> <computer color>", (x, y) =>
            {
                string lowerY = y.ToLower();
                if (!(lowerY == "w" || lowerY == "b"))
                {
                    Console.WriteLine("Must enter 'w' or 'b' for computer color");
                    return;
                }
                Console.Clear();
                PlayHvC(x, lowerY == "w");
                currentState = new BoardState();
                Console.WriteLine();
            });
            CVC = new Command<int, int>("cvc", "simulate a game between two computers", "cvc <depth> <turns>", (x, y) =>
            {
                Console.Clear();
                PlayCvC(x, y);
                currentState = new BoardState();
                Console.WriteLine();
            });
            CVC2 = new Command<int, int, int>("cvc", "simulate a game between two computers of different depths", "cvc <depth1> <depth2> <turns>", (x, y, z) =>
            {
                Console.Clear();
                PlayCvC(x, y, z);
                currentState = new BoardState();
                Console.WriteLine();
            });
            TOURNEY = new Command<int, int>("tourney", "run a tournament to see how different depths in a range compare", "tourney <minDepth> <maxDepth>", (x, y) =>
            {
                Console.Clear();
                PlayTourney(x, y);
                currentState = new BoardState();
                Console.WriteLine();
            });
            commandList = new List<object>
            {
                HELP,
                INFO,
                RULES,
                CLS,
                HVH,
                HVC,
                CVC,
                CVC2,
                TOURNEY
            };
            ReadCommands();
        }
        
        Move GetHumanMove()
        {
            while(true)
            {
                int startPos = -1;
                int pieceSize = -1;
                int endPos = -1;
                //Get user input
                string turnName = currentState.whiteTurn ? "White" : "Black";
                Console.WriteLine($"{turnName}'s turn");
                Console.WriteLine("Move from where? (16 for hand)");
                while(startPos < 0)
                {
                    string input = Console.ReadLine();
                    if(!string.IsNullOrWhiteSpace(input))
                    {
                        if (int.TryParse(input, out int num))
                        {
                            if(num >= 0)
                            {
                                startPos = num;
                            }
                            else
                            {
                                Console.WriteLine("Starting postition must be a positive number!");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Must enter a number!");
                        }
                    }
                    
                }
                if (startPos > 15)
                {
                    Console.WriteLine("Place which piece size?");
                    while (pieceSize < 0)
                    {
                        string input = Console.ReadLine();
                        if (!string.IsNullOrWhiteSpace(input))
                        {
                            if (int.TryParse(input, out int num))
                            {
                                if (num > 0 && num < 5)
                                {
                                    pieceSize = num;
                                }
                                else
                                {
                                    Console.WriteLine("Piece size must be between 0 and 4!");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Must enter a number!");
                            }
                        }

                    }
                }
                else
                {
                    pieceSize = currentState.pieces[startPos].size;
                }
                Console.WriteLine("Move to where?");
                while (endPos < 0)
                {
                    string input = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(input))
                    {
                        if (int.TryParse(input, out int num))
                        {
                            if (num >= 0 && num < 16)
                            {
                                endPos = num;
                            }
                            else
                            {
                                Console.WriteLine("Ending postition must be a positive number less than 16!");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Must enter a number!");
                        }
                    }

                }
                Move nextMove = new Move(startPos, endPos, pieceSize);
                if (ValidateMove(currentState, ref nextMove))
                {
                    return nextMove;
                }
                Console.WriteLine("Invalid move, try again!");
            }
            
        }

        Move GetComputerMove(int depth)
        {
            Move[] childMoves = currentState.FindMoves();
            if(currentState.whiteTurn) //Maximizing player (white)
            {
                ScoredMove maxEval = new ScoredMove(new Move(), int.MinValue);
                foreach(Move m in childMoves)
                {
                    ScoredMove eval = Minimax(currentState, m, depth - 1, int.MinValue, int.MaxValue, false);
                    maxEval = ScoredMove.Max(maxEval, eval);
                    
                }
                return maxEval.move;
            }
            else
            {
                ScoredMove minEval = new ScoredMove(new Move(), int.MaxValue);
                foreach(Move m in childMoves)
                {
                    ScoredMove eval = Minimax(currentState, m, depth - 1, int.MinValue, int.MaxValue, true);
                    minEval = ScoredMove.Min(minEval, eval);
                    
                }
                return minEval.move;
            }
            
        }
        ScoredMove Minimax(BoardState oldPosition, Move move, int depth, int alpha, int beta, bool maximizingPlayer)
        {
            BoardState newPosition = new BoardState(oldPosition, move);
            bool isWin = newPosition.IsWin();
            if (depth == 0 || isWin)
            {
                int points = newPosition.EvalPoints();
                if(isWin)
                {
                    points *= (depth + 1);
                }
                return new ScoredMove(move, points);
            }
            Move[] childMoves = newPosition.FindMoves();
            if (maximizingPlayer)
            {
                int maxEval = int.MinValue;
                foreach(Move m in childMoves)
                {
                    int eval = Minimax(newPosition, m, depth - 1, alpha, beta, false).score;
                    maxEval = Math.Max(maxEval, eval);
                    alpha = Math.Max(alpha, eval);
                    if (beta <= alpha) { break; }
                }
                return new ScoredMove(move, maxEval);
            }
            else
            {
                int minEval = int.MaxValue;
                foreach(Move m in childMoves)
                {
                    int eval = Minimax(newPosition, m, depth - 1, alpha, beta, true).score;
                    minEval = Math.Min(minEval, eval);
                    beta = Math.Min(beta, eval);
                    if (beta <= alpha) { break; }
                }
                return new ScoredMove(move, minEval);
            }
        }
        private void PrintBoard()
        {
            Console.Clear();
            //Black Hand
            for (int i = 0; i < 3; i++)
            {
                Piece outPiece = currentState.blackHand[i];
                if(outPiece.size == 0)
                {
                    Console.Write("00 ");
                }
                else
                {
                    string colorChar = (outPiece.color == Piece.Color.White) ? "W" : "B";
                    Console.Write(colorChar + outPiece.size + " ");
                }
            }
            Console.WriteLine("\n");
            //Board
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    Piece outPiece = currentState.pieces[(i * 4) + j];
                    if (outPiece.color == Piece.Color.Empty)
                    {
                        Console.Write("00 ");
                    }
                    else
                    {
                        string colorChar = (outPiece.color == Piece.Color.White) ? "W" : "B";
                        Console.Write(colorChar + outPiece.size + " ");
                    }

                }
                Console.Write("\r\n");
            }
            Console.WriteLine();
            //White Hand
            for (int i = 0; i < 3; i++)
            {
                Piece outPiece = currentState.whiteHand[i];
                if (outPiece.size == 0)
                {
                    Console.Write("00 ");
                }
                else
                {
                    string colorChar = (outPiece.color == Piece.Color.White) ? "W" : "B";
                    Console.Write(colorChar + outPiece.size + " ");
                }
            }
            
        }
        
        bool CheckWinner(int boardPoints)
        {
            if (Math.Abs(boardPoints) == 100)
            {
                string winner = (boardPoints > 0) ? "White" : "Black";
                Console.WriteLine($"\r\n{winner} wins!");
                return true;
            }
            return false;
        }
        void PlayHvH()
        {
            PrintBoard();
            Console.WriteLine();
            while (true)
            {
                Move nextMove = GetHumanMove();
                currentState = new BoardState(currentState, nextMove);

                int boardPoints = currentState.EvalPoints();
                PrintBoard();
                Console.WriteLine("\n");
                Console.WriteLine($"Points: {boardPoints}");
                if(currentState.IsWin())
                {
                    Console.WriteLine("Winner!");
                    return;
                }

            }

        }

        

        void PlayHvC(int depth, bool compFirst)
        {
            int boardPoints;
            if(!compFirst)
            {
                PrintBoard();
                Console.WriteLine();
                Move humanMove = GetHumanMove();
                currentState = new BoardState(currentState, humanMove);
                PrintBoard();
                boardPoints = currentState.EvalPoints();
                Console.WriteLine("\n");
                Console.WriteLine($"Points: {boardPoints}");
            }
            while (true)
            {
                //Computer
                string moveData;
                Stopwatch timer = new Stopwatch();
                timer.Start();
                Move computerMove = GetComputerMove(depth);
                timer.Stop();
                if (computerMove.startPos == 16)
                {
                    moveData = $"from hand ";
                }
                else
                {
                    moveData = $"from {computerMove.startPos} ";
                }
                moveData += $"to {computerMove.endPos}";
                currentState = new BoardState(currentState, computerMove);
                PrintBoard();
                boardPoints = currentState.EvalPoints();
                Console.WriteLine("\n");
                Console.WriteLine($"Points: {boardPoints}");
                TimeSpan timeTaken = timer.Elapsed;
                Console.WriteLine("Time taken: " + timeTaken.ToString(@"m\:ss\.fff"));
                Console.WriteLine("Computer played " + moveData);
                if (CheckWinner(boardPoints)) { return; }

                //Human
                Move humanMove = GetHumanMove();
                currentState = new BoardState(currentState, humanMove);
                PrintBoard();
                boardPoints = currentState.EvalPoints();
                Console.WriteLine("\n");
                Console.WriteLine($"Points: {boardPoints}");
                if (CheckWinner(boardPoints)) { return; }
            }
        }

        void PlayCvC(int depth, int numTurns)
        {
            bool isWinner = false;
            int turnsCompleted = 0;
            Stopwatch timer = new Stopwatch();
            timer.Start();
            while(turnsCompleted < numTurns)
            {
                Move computerMove = GetComputerMove(depth);
                currentState = new BoardState(currentState, computerMove);
                string loadSymbol;
                if(turnsCompleted % 4 == 2 || turnsCompleted % 4 == 3) { loadSymbol = "..."; }
                else if(turnsCompleted % 4 == 1) { loadSymbol = ".."; }
                else { loadSymbol = ".";}
                Console.Clear();
                Console.WriteLine("Calculating" + loadSymbol);
                Console.WriteLine($"{((turnsCompleted * 100f) / (numTurns - 1)).ToString("0.0")}% complete");
                int boardPoints = currentState.EvalPoints();
                turnsCompleted++;
                if(CheckWinner(boardPoints))
                {
                    isWinner = true;
                    break;
                }
            }
            timer.Stop();
            TimeSpan elapsedTime = timer.Elapsed;
            Console.WriteLine($"{turnsCompleted} turns were played, lasting {elapsedTime.ToString(@"hh\:mm\:ss")}");
            if (!isWinner)
            {
                Console.WriteLine("It's a draw!");
            }

        }
        void PlayCvC(int depth1, int depth2, int numTurns)
        {
            bool isWinner = false;
            int turnsCompleted = 0;
            Stopwatch timer = new Stopwatch();
            timer.Start();
            while (turnsCompleted < numTurns)
            {
                int currentDepth = (turnsCompleted % 2 == 0) ? depth1 : depth2;
                Move computerMove = GetComputerMove(currentDepth);
                currentState = new BoardState(currentState, computerMove);
                string loadSymbol;
                if (turnsCompleted % 4 == 2 || turnsCompleted % 4 == 3) { loadSymbol = "..."; }
                else if (turnsCompleted % 4 == 1) { loadSymbol = ".."; }
                else { loadSymbol = "."; }
                Console.Clear();
                Console.WriteLine("Calculating" + loadSymbol);
                Console.WriteLine($"{((turnsCompleted * 100f) / (numTurns - 1)).ToString("0.0")}% complete");
                int boardPoints = currentState.EvalPoints();
                turnsCompleted++;
                if (CheckWinner(boardPoints))
                {
                    isWinner = true;
                    break;
                }
            }
            timer.Stop();
            TimeSpan elapsedTime = timer.Elapsed;
            Console.WriteLine($"{turnsCompleted} turns were played, lasting {elapsedTime.ToString(@"hh\:mm\:ss")}");
            if (!isWinner)
            {
                Console.WriteLine("It's a draw!");
            }

        }

        string TourneyCvC(int depth1, int depth2)
        {
            int turnsCompleted = 0;
            while(turnsCompleted < 500)
            {
                int currentDepth = (turnsCompleted % 2 == 0) ? depth1 : depth2;
                Move computerMove = GetComputerMove(currentDepth);
                currentState = new BoardState(currentState, computerMove);
                int boardPoints = currentState.EvalPoints();
                turnsCompleted++;
                if(boardPoints >= 100)
                {
                    return "W";
                }
                if(boardPoints <= -100)
                {
                    return "B";
                }
            }
            return "T";
        }
        void PlayTourney(int minDepth, int maxDepth)
        {
            Console.Clear();
            //Header
            Console.Write(" ");
            for(int j = minDepth; j <= maxDepth; j++)
            {
                Console.Write($" {j}");
            }
            Console.Write(" <- White has this depth\r\n");
            Stopwatch timer = new Stopwatch();
            timer.Start();
            for (int i = 0; i < maxDepth; i++)
            {
                Console.Write($"{i + 1} ");
                for(int j = 0; j < maxDepth; j++)
                {
                    Console.Write(TourneyCvC(j + minDepth, i + minDepth) + " ");
                    currentState = new BoardState();
                }
                Console.WriteLine();
            }
            timer.Stop();
            TimeSpan elapsedTime = timer.Elapsed;
            Console.WriteLine($"\r\nTournament lasted {elapsedTime:hh\\:mm\\:ss}");
        }

        public static bool ValidateMove(BoardState board, ref Move move)
        {

            if (move.pieceSize <= board.pieces[move.endPos].size) { return false; }
            if (move.startPos < 16) { return (board.pieces[move.startPos].color == Piece.Color.White) == board.whiteTurn; }
            if (move.handInd == -1)
            {
                Piece[] activeHand = board.whiteTurn ? board.whiteHand : board.blackHand;
                int pieceSize = move.pieceSize;
                int handInd = activeHand.FirstIndexMatch(p => p.size == pieceSize);
                move.handInd = handInd;
            }
            return move.handInd != -1;
        }

        public void ReadCommands()
        {
            while(true)
            {
                Console.WriteLine("Enter command...");
                string input = Console.ReadLine();
                if(input != null)
                {
                    InterpretCommand(input.ToLower());
                }
            }
        }
        public void InterpretCommand(string cmd)
        {
            string[] args = cmd.Split(' ');
            for (int i = 0; i < commandList.Count; i++)
            {
                CommandBase commandBase = commandList[i] as CommandBase;

                if (args[0] == (commandBase.commandId))
                {
                    if (commandList[i] as Command != null && args.Length == 1)
                    {
                        Console.WriteLine();
                        (commandList[i] as Command).Invoke();
                        Console.WriteLine();
                    }
                    else if (commandList[i] as Command<int> != null && args.Length == 2)
                    {
                        Console.WriteLine();
                        (commandList[i] as Command<int>).Invoke(int.Parse(args[1]));
                        Console.WriteLine();
                    }
                    else if (commandList[i] as Command<string> != null && args.Length == 2)
                    {
                        Console.WriteLine();
                        (commandList[i] as Command<string>).Invoke(args[1]);
                        Console.WriteLine();
                    }
                    else if (commandList[i] as Command<int, int> != null && args.Length == 3)
                    {
                        Console.WriteLine();
                        (commandList[i] as Command<int, int>).Invoke(int.Parse(args[1]), int.Parse(args[2]));
                        Console.WriteLine();
                    }
                    else if (commandList[i] as Command<int, string> != null && args.Length == 3)
                    {
                        Console.WriteLine();
                        (commandList[i] as Command<int, string>).Invoke(int.Parse(args[1]), args[2]);
                        Console.WriteLine();
                    }
                    else if (commandList[i] as Command<int, int, int> != null && args.Length == 4)
                    {
                        Console.WriteLine();
                        (commandList[i] as Command<int, int, int>).Invoke(int.Parse(args[1]), int.Parse(args[2]), int.Parse(args[3]));
                        Console.WriteLine();
                    }
                }
            }
        }

    }
}