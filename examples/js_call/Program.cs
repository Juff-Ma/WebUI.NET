using System.Text;
using WebUI;

Window window = new();
window.Show("index.html");

for (int i = 1; i <= 100; i++)
{
    await Task.Delay(100);
    window.InvokeJavaScript("setText", Encoding.UTF8.GetBytes($"Invoke number {i}"));
}

Utils.WaitForExit();