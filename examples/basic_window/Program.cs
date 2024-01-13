using WebUI;

Window window = new();
window.Show("""
<html>
    <script src="webui.js"></script> 
    <head>
        <title>WebUI</title>
    </head>
    <body>
        <h1>WebUI</h1>
        <p>It works!</p>
    </body>
</html>
""");

Utils.WaitForExit();