using System.Reflection.Metadata;
using Terminal.Gui;
using fyn.Models;
using System.Data;

public class EditorView : View
{
	public fyn.Models.Document Document { get; set; }
	public EditorView()
	{
		X = 0;
		Y = 0;
		Width = Dim.Fill();
		Height = Dim.Fill();
		this.CanFocus = true;
	}

	public override void Redraw(Rect bounds)
	{
		base.Redraw(bounds);

		if (Document == null)
		{
			return;
		}

		TextPosition selectionStart = null;
		TextPosition selectionEnd = null;
		bool hasSelection = Document.SelectionRange != null;
		if (hasSelection)
		{
			// this line is so unreadable icl
			if (Document.SelectionRange.Start.Line < Document.SelectionRange.End.Line || (Document.SelectionRange.Start.Line == Document.SelectionRange.End.Line && Document.SelectionRange.Start.Col < Document.SelectionRange.End.Col))
			{
				selectionStart = Document.SelectionRange.Start;
				selectionEnd = Document.SelectionRange.End;
			}
			else
			{
				selectionStart = Document.SelectionRange.End;
				selectionEnd = Document.SelectionRange.Start;
			}
		}


		for (int y = 0; y < Bounds.Height; y++)
		{
			if (y >= Document.Lines.Count)
			{
				break;
			}
			Move(0, y);
			var lineText = Document.Lines[y];

			// only apply selection if there is a selection and this line is affected
			if (hasSelection && y >= selectionStart.Line && y <= selectionEnd.Line)
			{
				int lineLen = lineText.Length;
				int selStartCol = 0;
				int selEndCol = lineLen;

				if (selectionStart.Line == selectionEnd.Line)
				{
					// selection is within this line
					selStartCol = selectionStart.Col;
					selEndCol = selectionEnd.Col;
				}
				else if (y == selectionStart.Line)
				{
					// selection starts on this line
					selStartCol = selectionStart.Col;
					selEndCol = lineLen;
				}
				else if (y == selectionEnd.Line)
				{
					// selection ends on this line
					selStartCol = 0;
					selEndCol = selectionEnd.Col;
				}
				else
				{
					// whole line is selected
					selStartCol = 0;
					selEndCol = lineLen;
				}

				// draw unselected prefix
				if (selStartCol > 0)
				{
					Driver.SetAttribute(ColorScheme.Normal);
					Driver.AddStr(lineText.ToString().Substring(0, selStartCol));
				}
				// draw selected part
				if (selEndCol > selStartCol)
				{
					Driver.SetAttribute(ColorScheme.Focus);
					Driver.AddStr(lineText.ToString().Substring(selStartCol, selEndCol - selStartCol));
				}
				// draw unselected suffix
				if (selEndCol < lineLen)
				{
					Driver.SetAttribute(ColorScheme.Normal);
					Driver.AddStr(lineText.ToString().Substring(selEndCol));
				}
			}
			else
			{
				// No selection on this line, draw normally
				Driver.SetAttribute(ColorScheme.Normal);
				Driver.AddStr(lineText.ToString());
			}
		}
		Move(Document.CursorPosition.Col, Document.CursorPosition.Line);
	}

	public override bool ProcessKey(KeyEvent keyEvent)
	{
		if (Document == null) return false;

		bool handled = true;

		if (keyEvent.Key == (Key.Backspace | Key.CtrlMask) || keyEvent.Key == (Key.W | Key.CtrlMask))
		{
			Document.HandleCtrlBackspace();
		}
		// the Q has to be uppercase otherwise something TERRIBLE will happen
		else if (keyEvent.Key == (Key.Q | Key.CtrlMask))
		{
			Application.RequestStop();
		}
		else if (keyEvent.Key == (Key.CursorLeft | Key.CtrlMask))
		{
			// Document.HandleWordLeft(); // for the future
		}
		else if (keyEvent.Key == (Key.CursorRight | Key.CtrlMask))
		{
			// Document.HandleWordRight(); // for the future
		}
		else if (keyEvent.Key == (Key.CursorUp | Key.CtrlMask))
		{
			// Document.HandleWordUp(); // for the future
		}
		else if (keyEvent.Key == (Key.CursorDown | Key.CtrlMask))
		{
			// Document.HandleWordDown(); // for the future
		}

		// add other elseifs here for other shortcuts
		else
		{
			var baseKey = keyEvent.Key & ~Key.ShiftMask & ~Key.CtrlMask & ~Key.AltMask;

			switch (baseKey)
			{
				case Key.Enter:
					Document.HandleEnter();
					break;

				case Key.Backspace:
					Document.HandleBackspace();
					break;

				case Key.CursorLeft:
					Document.HandleCursorLeft(keyEvent.IsShift);
					break;

				case Key.CursorRight:
					Document.HandleCursorRight(keyEvent.IsShift);
					break;

				case Key.CursorUp:
					Document.HandleCursorUp(keyEvent.IsShift);
					break;

				case Key.CursorDown:
					Document.HandleCursorDown(keyEvent.IsShift);
					break;

				case Key.Tab:
					Document.HandleTab();
					break;


				// add other special keys above here as new cases

				// otherwise if just a normal key:
				default:
					char character = (char)keyEvent.KeyValue;
					if (!char.IsControl(character))
					{
						Document.InsertCharacter(character);
					}
					else
					{
						handled = false;
					}
					break;
			}
		}

		// tell editorview that it needs to be redrawn
		if (handled)
		{
			SetNeedsDisplay();
		}
		return handled;
	}
}