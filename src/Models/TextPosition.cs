namespace fyn.Models
{
  public class TextPosition
    {
        public int Line { get; set; }
        public int Col { get; set; }
        public TextPosition(int line, int col)
        {
            Line = line;
            Col = col;
        }
    }
}
