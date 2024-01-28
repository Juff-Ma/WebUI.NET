using System.Text;
using WebUI;

Window window = new();

ulong printHandlerId = 0;
// element is the name of the function when calling a backed function
printHandlerId = window.RegisterEventHandler((@event, _, handlerid) =>
{
    if (handlerid == printHandlerId)
    {
        // same as GetString(0)
        Console.WriteLine(@event.GetString());
    }
    return null;
}, "backendPrint");

ulong randomNumberHandlerID = 0;

randomNumberHandlerID = window.RegisterEventHandler((_, _, handlerid) =>
{
    if (handlerid == randomNumberHandlerID)
    {
        return Random.Shared.Next(1, 10001);
    }
    return null;
}, "randomNumber");

window.Show("index.html");

Console.WriteLine(window.CurrentUrl);

Utils.WaitForExit();