using System.Text;
using WebUI;

Window window = new();

int count = 1;

window.RegisterFileHandler((path) =>
{
    if (path == "/dynamic.html")
    {
        return Encoding.ASCII.GetBytes($"""
            <html>
                <script src="webui.js"></script> 
                <head>
                    <title>Dynamic</title>
                </head>
                <body>
                    <h1>Dynamic {count++}</h1>
                    <a href="/dynamic.html">Refresh</a>
                </body>
            </html>
            """);
    }

    return null;
});

window.Show("""
<html>
    <script src="webui.js"></script> 
    <head>
        <title>Start</title>
    </head>
    <body>
        <h1>Dynamic Content Test</h1>
        <a href="/dynamic.html">Dynamic</a>
    </body>
</html>
""");

Utils.WaitForExit();