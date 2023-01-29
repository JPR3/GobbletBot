namespace GobbletBot
{
    public class Piece
    {
        public enum Color
        {
            White,
            Black,
            Empty
        }

        public readonly int size;
        public readonly Color color;
        public Piece? coveredPiece;

        public Piece()
        {
            size = 0;
            color = Color.Empty;
            coveredPiece = null;
        }

        public Piece(int size, Color color, Piece? coveredPiece)
        {
            this.size = size;
            this.color = color;
            this.coveredPiece = coveredPiece;
        }

        public Piece DeepCopy()
        {
            Piece other = (Piece) this.MemberwiseClone();
            if(coveredPiece != null)
            {
                other.coveredPiece = coveredPiece.DeepCopy();
            }
            return other;
        }

        public override string ToString()
        {
            return $"Color: {color}, Size: {size}, Covered Piece ({coveredPiece})";
        }
    }
}
