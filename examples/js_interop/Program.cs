using System.Text;
using WebUI;

Window window = new();
ulong handlerid = 0;
handlerid = window.RegisterEventHandler((@event, _, id) =>
{
    if (handlerid == id)
    {
        var window = @event.Window;
        byte[] buffer = new byte[255];

        window.InvokeJavaScript($"return \"with handler id: \" + {handlerid}", ref buffer, 10);

        // remove null bytes from string, Encoding.UTF8.GetString() does not remove the null bytes
        // C *will* interpret them as string end and inevitable fuck with the system
        string result = Encoding.UTF8.GetString(buffer).TrimEnd('\0');

        window.InvokeJavaScript(
            "var button = document.getElementById(\"replace\");\n" +
            "var text = document.createElement(\"p\");\n" +
            $"text.innerHTML = \"Successfully replaced {result}!\";\n" +
            "button.parentNode.replaceChild(text, button);\n");
    }
    return null;
}, "replace");

window.Show("index.html");

Utils.WaitForExit();