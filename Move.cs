using System.Runtime.CompilerServices;

namespace GobbletBot
{
    public struct Move
    {
        public readonly int startPos; //From the hand if 16
        public readonly int endPos;
        public readonly int pieceSize;
        public int handInd; //Only used if playing from the hand, set when move is validated

        public Move(int startPos, int endPos, int pieceSize)
        {
            this.startPos = startPos;
            this.endPos = endPos;
            this.pieceSize = pieceSize;
            handInd = -1;
        }

        public Move(int startPos, int endPos, int pieceSize, int handInd) : this(startPos, endPos, pieceSize)
        {
            this.handInd = handInd;
        }

        public Move()
        {
            startPos = -1;
            endPos = -1;
            pieceSize = -1;
            handInd = -1;
        }
    }

    public struct ScoredMove
    {
        public readonly Move move;
        public readonly int score;

        public ScoredMove(Move move, int score)
        {
            this.move = move;
            this.score = score;
        }

        public static ScoredMove Max(ScoredMove a, ScoredMove b)
        {
            return (a.score > b.score) ? a : b;
        }
        public static ScoredMove Min(ScoredMove a, ScoredMove b)
        {
            return (a.score < b.score) ? a : b;
        }
    }
}
