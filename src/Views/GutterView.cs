using Terminal.Gui;

public class GutterView : View
{
    public GutterView()
    {
        X = 0;
        Y = 0;
        Width = 4;
        Height = Dim.Fill();
    }

    public override void Redraw(Rect bounds)
    {
        Driver.SetAttribute(ColorScheme.Normal);
        for (int y = 0; y < bounds.Height; y++)
        {
            Move(0, y);
            Driver.AddStr(new string(' ', bounds.Width));
        }
        Move(0, 0);
    }
}