using WebUI;

Window window = new();
ulong handlerid = 0;
handlerid = window.RegisterEventHandler((@event, _, id) =>
{
    if (handlerid == id)
    {
        var window = @event.Window;
        window.InvokeJavaScript("""
            var button = document.getElementById("replace");
            var text = document.createElement("p");
            text.innerHTML = "Successfully replaced!";
            button.parentNode.replaceChild(text, button);
            """);
    }
    return null;
}, "replace");

window.Show("index.html");

Utils.WaitForExit();