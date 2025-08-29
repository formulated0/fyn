using fyn.Models;

namespace fyn.Models;
    public class Selection
{
    public TextPosition Start { get; set; }
    public TextPosition End { get; set; }

    public Selection(TextPosition start, TextPosition end)
    {
        Start = start;
        End = end;
    }
}