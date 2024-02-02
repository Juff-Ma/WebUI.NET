// Ignore Spelling: Api

using System.Runtime.InteropServices;
using WebUI;

Window window = new();

ulong handleId = 0;

handleId = window.RegisterEventHandler((@event, _, handlerId) =>
{
    if (handleId == handlerId)
    {
        var browser = @event.Window.GetBrowserChildProcess();
        NativeApi.Natives.MessageBox(browser.MainWindowHandle,
            "This is a message box attached to the Browser window",
            "Message", 0);
    }

    return null;
}, "show");

window.Show("index.html");

Utils.WaitForExit();

namespace NativeApi
{
    internal static partial class Natives
    {
        [LibraryImport("User32.dll", EntryPoint = "MessageBoxW", StringMarshalling = StringMarshalling.Utf16)]
        public static partial int MessageBox(nint hWnd, string lpText, string lpCaption, uint uType);
    }
}