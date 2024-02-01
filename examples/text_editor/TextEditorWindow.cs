namespace TextEditor;

using WebUI;
using WebUI.Events;

internal class TextEditorWindow : Window, IEventHandler
{
    private const string CloseButtonId = "__close-btn";
    private const string MainPage = "index.html";

    private readonly ulong _handleId;

    public object? HandleEvent(Event @event, string element, ulong handlerId)
    {
        if (element == CloseButtonId && handlerId == _handleId)
        {
            Close();
        }

        // if you close the Window or invalidate it in any other way during an Event you *MUST* return null
        // otherwise buggy behavior will occur and the program will probably crash
        return null;
    }

    public TextEditorWindow(WindowProperties properties) : base(properties)
    {
        // normally you would probably register the Event handler of the window itself for all events, but we only have one event
        _handleId = RegisterEventHandler(this, CloseButtonId);
    }

    public bool Show()
    {
        if (!Show(MainPage, Browser.ChromiumBased))
        {
            return Show(MainPage);
        }
        return true;
    }
}
