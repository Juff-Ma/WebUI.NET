using WebUI;

namespace BasicWindow
{
    internal static class Program
    {
        public static void Main()
        {
            var window = new Window();
            window.Show("<html><script src=\"webui.js\"></script> <head><title>WebUI</title></head><body><h1>WebUI</h1><p>It works!</p></body></html>");

            Utils.WaitForExit();
        }
    }
}