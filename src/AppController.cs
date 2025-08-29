using Terminal.Gui;
using fyn.Models;


namespace fyn
{
    public class AppController
    {
		private Document _document;
        public void Run()
		{
			_document = new Document();

			Application.Init();
			var top = Application.Top;

			var statusBarView = new StatusBarView
			{
				X = 0,
				Y = Pos.AnchorEnd(1),
				Width = Dim.Fill(),
				Height = 1,
				ColorScheme = Colors.Base
			};

			var gutterView = new GutterView
			{
				X = 0,
				Y = Pos.At(0),
				Width = 4,
				Height = Dim.Fill(1),
				ColorScheme = Colors.Dialog
			};

			var editorView = new EditorView
			{
				X = Pos.Right(gutterView),
				Y = 0,
				Width = Dim.Fill(),
				Height = Dim.Fill(1),
				Document = _document,
				ColorScheme = Colors.Error
			};

			top.Add(gutterView, statusBarView, editorView);
			editorView.SetFocus();
			Application.Run();
			Application.Shutdown();
		}
    }
}
