using System.Text;

namespace fyn.Models
{
	public class Document
	{
		public bool IsDirty { get; private set; }

		// FIXME this might come back to bite me in the ass later but this is set to public readonly for now
		public readonly List<StringBuilder> Lines;
		public string? FilePath;

		// same here
		public readonly TextPosition CursorPosition;
		public Selection? SelectionRange;

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
				// split by words and remove double spaces if needed
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

		public void HandleCursorLeft(bool withShift = false)
		{
			// store the original position before doing anything else
			// create a new object to make sure its unaffected by anything 
			var originalPosition = new TextPosition(CursorPosition.Line, CursorPosition.Col);

			if (CursorPosition.Col > 0)
			{
				CursorPosition.Col--;
			}
			else if (CursorPosition.Line > 0)
			{
				// move to the end of the previous line
				CursorPosition.Line--;
				CursorPosition.Col = Lines[CursorPosition.Line].Length;
			}

			// handle the selection logic
			if (withShift)
			{
				// if we are starting a brand new selection
				if (SelectionRange == null)
				{
					// the selection is from the original position to the new one
					// create a new textposition just to be safe
					var newPosition = new TextPosition(CursorPosition.Line, CursorPosition.Col);
					SelectionRange = new Selection(originalPosition, newPosition);
				}
				// if we are extending an existing selection
				else
				{
					// just update the end point to the new cursor position
					SelectionRange.End = new TextPosition(CursorPosition.Line, CursorPosition.Col);
				}
			}
			else // if shift was not held
			{
				// clear any existing selection
				SelectionRange = null;
			}
		}

		public void HandleCursorRight(bool withShift = false)
		{
			// store the original position before doing anything else
			// create a new object to make sure its unaffected by anything
			var originalPosition = new TextPosition(CursorPosition.Line, CursorPosition.Col);

			var line = Lines[CursorPosition.Line];
			if (CursorPosition.Col < line.Length)
			{
				CursorPosition.Col += 1;
			}
			// if at the end of a line but not the last line, wrap to the next one
			else if (CursorPosition.Col == line.Length && CursorPosition.Line < Lines.Count - 1)
			{
				CursorPosition.Line += 1;
				CursorPosition.Col = 0;
			}

			// handle the selection logic
			if (withShift)
			{
				// if we are starting a brand new selection
				if (SelectionRange == null)
				{
					// the selection is from the original position to the new one
					// create a new textposition just to be safe
					var newPosition = new TextPosition(CursorPosition.Line, CursorPosition.Col);
					SelectionRange = new Selection(originalPosition, newPosition);
				}
				// if we are extending an existing selection
				else
				{
					// just update the end point to the new cursor position
					SelectionRange.End = new TextPosition(CursorPosition.Line, CursorPosition.Col);
				}
			}
			else // if shift was not held
			{
				// clear any existing selection
				SelectionRange = null;
			}
		}

		public void HandleCursorDown(bool withShift = false)
		{
			// store the original position before doing anything else
			// create a new object to make sure its unaffected by anything
			var originalPosition = new TextPosition(CursorPosition.Line, CursorPosition.Col);

			// only move down if not on the last line
			if (CursorPosition.Line < Lines.Count - 1)
			{
				CursorPosition.Line += 1;
				int nextLineLength = Lines[CursorPosition.Line].Length;
				// clamp the column to the length of the new line
				if (CursorPosition.Col > nextLineLength)
				{
					CursorPosition.Col = nextLineLength;
				}
			}

			if (withShift)
			{
				// if we are starting a brand new selection
				if (SelectionRange == null)
				{
					// the selection is from the original position to the new one
					// create a new textposition just to be safe
					var newPosition = new TextPosition(CursorPosition.Line, CursorPosition.Col);
					SelectionRange = new Selection(originalPosition, newPosition);
				}
				// if we are extending an existing selection
				else
				{
					// just update the end point to the new cursor position
					SelectionRange.End = new TextPosition(CursorPosition.Line, CursorPosition.Col);
				}
			}
			else // if shift was not held
			{
				// clear any existing selection
				SelectionRange = null;
			}
		}

		public void HandleCursorUp(bool withShift = false)
		{
			// store the original position before doing anything else
			// create a new object to make sure its unaffected by anything
			var originalPosition = new TextPosition(CursorPosition.Line, CursorPosition.Col);

			// only move up if not on the first line
			if (CursorPosition.Line > 0)
			{
				CursorPosition.Line -= 1;
				int prevLineLength = Lines[CursorPosition.Line].Length;
				// clamp the column to the length of the new line
				if (CursorPosition.Col > prevLineLength)
				{
					CursorPosition.Col = prevLineLength;
				}
			}

			if (withShift)
			{
				// if we are starting a brand new selection
				if (SelectionRange == null)
				{
					// the selection is from the original position to the new one
					// create a new textposition just to be safe
					var newPosition = new TextPosition(CursorPosition.Line, CursorPosition.Col);
					SelectionRange = new Selection(originalPosition, newPosition);
				}
				// if we are extending an existing selection
				else
				{
					// just update the end point to the new cursor position
					SelectionRange.End = new TextPosition(CursorPosition.Line, CursorPosition.Col);
				}
			}
			else // if shift was not held
			{
				// clear any existing selection
				SelectionRange = null;
			}
		}

		// delete the word before the cursor
		public void HandleCtrlBackspace()
		{
			// if at the very start of the document do nothing
			if (CursorPosition.Line == 0 && CursorPosition.Col == 0)
				return;

			var line = Lines[CursorPosition.Line];
			int col = CursorPosition.Col;
			int lineIdx = CursorPosition.Line;

			// if at the start of a line (but not first line) join with previous line
			if (col == 0 && lineIdx > 0)
			{
				var prevLine = Lines[lineIdx - 1];
				int prevLen = prevLine.Length;
				prevLine.Append(line);
				Lines.RemoveAt(lineIdx);
				CursorPosition.Line -= 1;
				CursorPosition.Col = prevLen;
				IsDirty = true;
				// set up to delete word in the joined line
				line = prevLine;
				col = prevLen;
				lineIdx--;
			}

			// find the start of the previous word
			int start = col;
			// skip any whitespace to the left of the cursor
			while (start > 0 && char.IsWhiteSpace(line[start - 1]))
				start--;
			// skip non-whitespace (the word itself)
			while (start > 0 && !char.IsWhiteSpace(line[start - 1]))
				start--;

			int lengthToRemove = col - start;
			if (lengthToRemove > 0)
			{
				line.Remove(start, lengthToRemove);
				CursorPosition.Col = start;
				IsDirty = true;
			}
		}

		public void HandleTab()
		{
			var tab = "    ";
			var line = Lines[CursorPosition.Line];
			if (CursorPosition.Col > line.Length)
			{
				CursorPosition.Col = line.Length;
			}
			line.Insert(CursorPosition.Col, tab);
			CursorPosition.Col += 4;
			IsDirty = true;

		}
	}
}
