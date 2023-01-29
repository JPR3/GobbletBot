using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace GobbletBot
{

    public class BoardState
    {
        public Piece[] pieces { get; private set; }
        public Piece[] whiteHand { get; private set; }
        public Piece[] blackHand { get; private set; }
        public bool whiteTurn { get; private set; }
        public BoardState()
        {
            pieces = new Piece[16].Populate();
            whiteTurn = true;
            //This creates each player a nested stack of pieces in the way you have them in your hand at the beginning of the game
            whiteHand = new Piece[3].Populate(() => 
            new Piece(4, Piece.Color.White, new Piece(3, Piece.Color.White, new Piece(2, Piece.Color.White, new Piece(1, Piece.Color.White, new Piece())))));
            blackHand = new Piece[3].Populate(() =>
            new Piece(4, Piece.Color.Black, new Piece(3, Piece.Color.Black, new Piece(2, Piece.Color.Black, new Piece(1, Piece.Color.Black, new Piece())))));
        }

        public BoardState(BoardState parent, Move move) //Note: move is assumed to be valid
        {
            whiteTurn = parent.whiteTurn;
            pieces = new Piece[parent.pieces.Length];
            for(int i = 0; i < parent.pieces.Length; i++)
            {
                pieces[i] = parent.pieces[i].DeepCopy();
            }
            whiteHand = new Piece[parent.whiteHand.Length];
            for (int i = 0; i < parent.whiteHand.Length; i++)
            {
                whiteHand[i] = parent.whiteHand[i].DeepCopy();
            }
            blackHand = new Piece[parent.blackHand.Length];
            for (int i = 0; i < parent.blackHand.Length; i++)
            {
                blackHand[i] = parent.blackHand[i].DeepCopy();
            }
            if(move.startPos > -1) //Negative startpos indicates not to apply any change
            {
                PerformMove(move);
            }
        }

        private void PerformMove(Move move)
        {
            Piece startPiece;
            Piece[] activeHand;
            if (move.startPos < 16)
            {
                //Perform move from previous postition
                startPiece = pieces[move.startPos];
                pieces[move.startPos] = startPiece.coveredPiece;
            }
            else
            {
                //Perform move from hand
                activeHand = whiteTurn ? whiteHand : blackHand;
                startPiece = activeHand[move.handInd];
                activeHand[move.handInd] = startPiece.coveredPiece;

            }
            
            Piece endPiece = pieces[move.endPos];
            startPiece.coveredPiece = endPiece;
            pieces[move.endPos] = startPiece;
            
            whiteTurn = !whiteTurn;
        }

        public int EvalPoints()
        {
            int wPoints = 0;
            int bPoints = 0;
            //Check Horizontal
            for (int i = 0; i < 16; i+=4)
            {
                int numWhite = 0;
                int numBlack = 0;
                for(int j = i; j < i + 4; j++)
                {
                    if (pieces[j].color == Piece.Color.White)
                    {
                        numWhite++;
                    }
                    else if (pieces[j].color == Piece.Color.Black)
                    {
                        numBlack++;
                    }
                }
                if(Math.Abs(numWhite - numBlack) == 4)
                {
                    return (numWhite - numBlack) * 25;
                }
                if(numBlack == 0)
                {
                    wPoints += numWhite;
                }
                else if(numWhite == 0)
                {
                    bPoints += numBlack;
                }
            }
            //Check Vertical
            for (int i = 0; i < 4; i++)
            {
                int numWhite = 0;
                int numBlack = 0;
                for (int j = i; j < i + 16; j+=4)
                {
                    if (pieces[j].color == Piece.Color.White)
                    {
                        numWhite++;
                    }
                    else if (pieces[j].color == Piece.Color.Black)
                    {
                        numBlack++;
                    }
                }
                if (Math.Abs(numWhite - numBlack) == 4)
                {
                    return (numWhite - numBlack) * 25;
                }
                if (numBlack == 0)
                {
                    wPoints += numWhite;
                }
                else if (numWhite == 0)
                {
                    bPoints += numBlack;
                }
            }
            //Check Diagonal \
            #region CheckDiagonal \
            int numWhiteD1 = 0;
            int numBlackD1 = 0;
            for (int i = 0; i < 16; i+=5)
            {
                if (pieces[i].color == Piece.Color.White)
                {
                    numWhiteD1++;
                }
                else if (pieces[i].color == Piece.Color.Black)
                {
                    numBlackD1++;
                }
            }
            if (Math.Abs(numWhiteD1 - numBlackD1) == 4)
            {
                return (numWhiteD1 - numBlackD1) * 25;
            }
            if (numBlackD1 == 0)
            {
                wPoints += numWhiteD1;
            }
            else if (numWhiteD1 == 0)
            {
                bPoints += numBlackD1;
            }
            #endregion
            //Check Diagonal /
            #region CheckDiagonal /
            int numWhiteD2 = 0;
            int numBlackD2 = 0;
            for (int i = 3; i < 13; i += 3)
            {
                if (pieces[i].color == Piece.Color.White)
                {
                    numWhiteD2++;
                }
                else if (pieces[i].color == Piece.Color.Black)
                {
                    numBlackD2++;
                }
            }
            if (Math.Abs(numWhiteD2 - numBlackD2) == 4)
            {
                return (numWhiteD2 - numBlackD2) * 25;
            }
            if (numBlackD2 == 0)
            {
                wPoints += numWhiteD2;
            }
            else if (numWhiteD2 == 0)
            {
                bPoints += numBlackD2;
            }
            #endregion
            
            return wPoints - bPoints;
        }

        public bool IsWin()
        {
            for(int i = 0; i < 16; i += 4)
            {
                if (pieces[i].color == Piece.Color.Empty) { continue; }
                if (pieces[i].color == pieces[i + 1].color && pieces[i].color == pieces[i + 2].color && 
                    pieces[i].color == pieces[i + 3].color)
                {
                    return true;
                }
            }
            for(int i = 0; i < 4; i++)
            {
                if (pieces[i].color == Piece.Color.Empty) { continue; }
                if (pieces[i].color == pieces[i + 4].color && pieces[i].color == pieces[i + 8].color &&
                    pieces[i].color == pieces[i + 12].color)
                {
                    return true;
                }
            }
            return ((pieces[0].color != Piece.Color.Empty) && pieces[0].color == pieces[5].color && pieces[0].color == pieces[10].color && pieces[0].color == pieces[15].color) ||
                   ((pieces[3].color != Piece.Color.Empty) && pieces[3].color == pieces[6].color && pieces[3].color == pieces[9].color && pieces[3].color == pieces[12].color);
        }

        public Move[] FindMoves()
        {
            List<Move> foundMoves = new List<Move>();
            Piece[] activeHand = whiteTurn ? whiteHand : blackHand;
            //Moves from hand
            for (int i = 0; i < 3; i++)
            {
                for(int j = 0; j < 16; j++)
                {
                    Move nextMove = new Move(16, j, activeHand[i].size, i);
                    if(Program.ValidateMove(this, ref nextMove))
                    {
                        foundMoves.Add(nextMove);
                    }
                }
            }
            //Moves from board
            for(int i = 0; i < 16; i++)
            {
                if ((pieces[i].color == Piece.Color.White) == whiteTurn)
                {
                    for(int j = 0; j < 16; j++)
                    {
                        if (pieces[i].size > pieces[j].size)
                        {
                            foundMoves.Add(new Move(i, j, pieces[i].size));
                        }
                    }
                }
            }
            return foundMoves.ToArray();
        }
    }
}
