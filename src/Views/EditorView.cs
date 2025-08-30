using System.Reflection.Metadata;
using Terminal.Gui;
using fyn.Models;

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

        for (int y = 0; y < this.Bounds.Height; y++)
        {
            if (y >= Document.Lines.Count)
            {
                break;
            }
            Move(0, y);
            var lineText = Document.Lines[y];
            Driver.AddStr(lineText.ToString());
        }
        Move(Document.CursorPosition.Col, Document.CursorPosition.Line);
    }

    public override bool ProcessKey(KeyEvent keyEvent)
    {
        if (Document == null)
        {
            return false;
        }

        bool handled = true;

        switch (keyEvent.Key)
        {
            case Key.Enter:
                Document.HandleEnter();
                break;

            case Key.Backspace:
                Document.HandleBackspace();
                break;

            case Key.CursorLeft:
                Document.HandleCursorLeft();
                break;

            case Key.CursorRight:
                Document.HandleCursorRight();
                break;

            case Key.CursorUp:
                Document.HandleCursorUp();
                break;

            case Key.CursorDown:
                Document.HandleCursorDown();
                break;

            // check for backspace with ctrl modifier
            case Key.Backspace | Key.CtrlMask:
                Document.HandleCtrlBackspace();
                break;

            // we have to check for ctrlW since terminals love using that for ctrlBackspace also
            case Key.W | Key.CtrlMask:
                Document.HandleCtrlBackspace();
                break;

            case Key.Tab:
                Document.HandleTab();
                break;


            // add other special keys above here as new cases

            // this has to be uppercase otherwise something TERRIBLE will happen
            case Key.Q | Key.CtrlMask:
                Application.RequestStop();
                break;
            
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
        // tell editorview that it needs to be redrawn
        if (handled)
        {
            SetNeedsDisplay();
        }
        return handled;
    }
}