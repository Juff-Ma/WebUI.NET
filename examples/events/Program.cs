using WebUI;

Window window = new();
window.Show("""
<html>
    <script src="webui.js"></script> 
    <head>
        <title>WebUI</title>
    </head>
    <body>
        <h1>Events Test</h1>
    </body>
</html>
""");

var defaultHandler = window.RegisterDefaultEventHandler();

defaultHandler.OnConnect += (_) => {
    Console.WriteLine("Connected");
};

defaultHandler.OnDisconnect += (_) => {
    Console.WriteLine("Disconnected");
};

Utils.WaitForExit();