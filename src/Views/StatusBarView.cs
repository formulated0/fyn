using Terminal.Gui;

public class StatusBarView : View
{
    public StatusBarView()
    {
        X = 0;
        Y = Pos.AnchorEnd(1);
        Width = Dim.Fill();
        Height = 1;
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
        Driver.AddStr("statusBar");
    }
}
