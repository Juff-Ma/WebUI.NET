using TextEditor;
using WebUI;

WindowProperties windowProperties = new()
{
    Width = 800,
    Height = 600,

    RootFolder = "ui"
};

TextEditorWindow window = new(windowProperties);
window.Show();

Utils.WaitForExit();
Utils.Clean();
