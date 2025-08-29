using System.Text;

namespace fyn.Models
{
    public class Document
    {
        public bool IsDirty { get; private set; }

        // this might come back to bite me in the ass but this is set to public readonly for now
        public readonly List<StringBuilder> Lines;
        public string FilePath;

        // same here
        public readonly TextPosition CursorPosition;
        private Selection SelectionRange;

        public Document()
        {
            Lines = new List<StringBuilder>();
            Lines.Add(new StringBuilder());

            CursorPosition = new TextPosition(0, 0);

            FilePath = null;
            IsDirty = false;
            SelectionRange = null;

        }

        public int GetLineCount()
        {
            return Lines.Count();
        }

        public int GetWordCount()
        {
            int wordCount = 0;
            foreach (var line in Lines)
            {
                string[] wordsOnLine = line.ToString().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                wordCount += wordsOnLine.Length;
            }
            return wordCount;
        }

        public void InsertCharacter(char c)
        {
            var line = Lines[CursorPosition.Line];
            if (CursorPosition.Col > line.Length)
            {
                CursorPosition.Col = line.Length;
            }
            line.Insert(CursorPosition.Col, c);
            CursorPosition.Col += 1;
            IsDirty = true;
        }

        public void HandleEnter()
        {
            var line = Lines[CursorPosition.Line];
            string endOfLineText = line.ToString().Substring(CursorPosition.Col);
            line.Remove(CursorPosition.Col, line.Length - CursorPosition.Col);
            var newLine = new StringBuilder(endOfLineText);
            Lines.Insert(CursorPosition.Line + 1, newLine);
            CursorPosition.Line += 1;
            CursorPosition.Col = 0;
            IsDirty = true;
        }

        public void HandleBackspace()
        {
            if (CursorPosition.Col > 0)
            {
                var line = Lines[CursorPosition.Line];
                line.Remove(CursorPosition.Col - 1, 1);
                CursorPosition.Col -= 1;
                IsDirty = true;
            }
            else if (CursorPosition.Col == 0 && CursorPosition.Line > 0)
            {
                var line = Lines[CursorPosition.Line];
                var prevLine = Lines[CursorPosition.Line - 1];
                int prevLineLength = prevLine.Length;
                prevLine.Append(line);
                Lines.Remove(line);
                CursorPosition.Line -= 1;
                CursorPosition.Col = prevLineLength;
                IsDirty = true; 
            }
        }
    }
}
