using WebUI;
using WebUI.Events;

using Events;

Window window = new();

// the default event handler handles common events like Connect, Disconnect or Navigate and exposes them as C# events
// you can also create your own event handler for those if you register without an element name (string.Empty or "") and check event.Type
var defaultHandler = window.RegisterDefaultEventHandler();

defaultHandler.OnConnect += (_) => Console.WriteLine("Connected");

defaultHandler.OnDisconnect += (_) => Console.WriteLine("Disconnected");

ClickHandler handler = new();

ulong id = window.RegisterEventHandler(handler, "click");

handler.HandlerId = id;

window.Show("""
<html>
    <script src="webui.js"></script> 
    <head>
        <title>WebUI</title>
    </head>
    <body>
        <h1>Events Test</h1>
        <button id="click">Click</button>
    </body>
</html>
""");

Utils.WaitForExit();

namespace Events
{
    class ClickHandler : IEventHandler
    {
        private int _clicks = 1;

        // the handler id will never be 0 if everything goes well so it is a good default value
        public ulong HandlerId { get; set; } = 0;

        public object? HandleEvent(Event @event, string element, ulong handlerId)
        {
            // You have to always check if the handlerId matches, otherwise your logic could run multiple times.
            // if you register for all elements you would also have to check if the element matches
            if (HandlerId == handlerId)
            {
                Console.WriteLine($"Click {_clicks++}");
            }
            return null;
        }
    }
}