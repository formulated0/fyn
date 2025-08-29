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

        switch (keyEvent.Key)
        {
            case Key.Enter:
                Document.HandleEnter();
                break;
            case Key.Backspace:
                Document.HandleBackspace();
                break;
            default:
                char character = (char)keyEvent.KeyValue;
                if (!char.IsControl(character))
                {
                    Document.InsertCharacter(character);
                }
                break;
        }
        // tell editorview that it needs to be redrawn
        this.SetNeedsDisplay();

        return true;
    }
}