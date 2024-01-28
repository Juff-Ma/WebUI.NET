using System.Text;
using WebUI;

Window window = new();
ulong handlerid = 0;
handlerid = window.RegisterEventHandler((@event, _, id) =>
{
    if (handlerid == id)
    {
        var window = @event.Window;
        byte[] buffer = new byte[1024];

        window.InvokeJavaScript($"return \"with handler id: \" + {handlerid}", ref buffer, 10);

        string result = Encoding.UTF8.GetString(buffer);

        window.InvokeJavaScript(
            "var button = document.getElementById(\"replace\");" +
            "var text = document.createElement(\"p\");" +
            $"text.innerHTML = \"Successfully replaced {result}!\";" +
            "button.parentNode.replaceChild(text, button);");
    }
    return null;
}, "replace");

window.Show("index.html");

Utils.WaitForExit();