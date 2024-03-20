using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using WebUI;
using Windows.Win32;
using Windows.Win32.UI.Input.KeyboardAndMouse;
using kvp=System.Collections.Generic.KeyValuePair<string,string>;

namespace WpfControllerApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            window?.Close();
        }

        WebUI.Window window;
        public class WinDispatcher : WebUI.Window.IInvokerHelper
        {
            private Dispatcher dispatcher;
            public WinDispatcher(Dispatcher dispatcher)
            {
                this.dispatcher = dispatcher;
            }
            public T InvokeWithReturnType<T>(Func<T> action) => dispatcher.Invoke<T>(action);

            public void InvokeWithVoid(Action action) => dispatcher.Invoke(action);
        }
        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            WebUI.Window.UseSpecificDispatcher = new WinDispatcher(Dispatcher);

            window = new(new WebUI.WindowProperties { Width = 640, Height = 480, X = 800, Y = 50 });
            var evts = window.RegisterDefaultEventHandler();

            //evts.OnDisconnect += (_) =>
            //{
            //    window = null;
            //    Dispatcher.InvokeAsync(() => Close());
            //};
            
            window.InitOurBridge();
            //window.Show("<html><head><script src='webui.js'></script></head><body>Hi Bob</body></html>");
            //await Task.Delay(2000);
            //btnDevTools_Click(null, null);
            // await Task.Delay(2000);

            evts.OnConnect += (window) =>
            {
                //this.window = window;
                NewConnect();
            };
            
            

            window.Show("index.html");

            //window.RegisterBoundFunction(() => MyTest_function);
        }
        private async void NewConnect()
        {
            window.SetDebug();
            window.AddEventListener(JSClickedButton, CommonEventTypes.click, "TestButton");
            window.RegisterBoundFunction("MyTest_function", RawMyTestCalled);
            window.AddEventListener(JSDblClickAny,CommonEventTypes.dblclick,"window",null,new kvp[]{new("button","button"), new("X Pos","clientX"), new("Y Pos","clientY")  });

        }

        private void JSDblClickAny(DomEvent evt)
        {
            LogItem($"DoubleClick event: " + Newtonsoft.Json.JsonConvert.SerializeObject(evt, Newtonsoft.Json.Formatting.Indented).Replace("\n","\t\n"));
        }

        private Task<string> RawMyTestCalled(string serialized)
        {
            var args = Newtonsoft.Json.JsonConvert.DeserializeObject<string[]>(serialized);
            return MyTest_function(args[0], args[1], double.Parse(args[2]));
        }

        private async Task<string> MyTest_function(String arg1, String arg2, double arg3)
        {
            var str = $"I am called: {arg1}({arg1.GetType()}) and {arg2}({arg2.GetType()}) {arg3}({arg3.GetType()})";
            LogItem(str);
            await Task.Delay(500);
            return "Nice Call " + arg1;
        }
        private void JSClickedButton(DomEvent evt)
        {
            LogItem($"Got a click from JS for: {evt.originalTargetId}");
        }

        private void LogItem(object val, [CallerArgumentExpression(nameof(val))] string expression = "unknown")
        {
            var valStr = val switch
            {
                String[] sarr => String.Join(", ", sarr),
                double or decimal or float or char or bool or int or string => val.ToString(),
                _ => "Serialized as: " + Newtonsoft.Json.JsonConvert.SerializeObject(val, Newtonsoft.Json.Formatting.None)
            };
            txtLog.Text += $"{expression} => {valStr.Replace("\r","")} ({val.GetType()})\n";
        }
        private async void btnTest1_Click(object sender, RoutedEventArgs e)
        {
            LogItem(await window.ScriptEvaluate<double>("return 5.143123;"));
            LogItem(await window.ScriptEvaluate<double>("return getDouble();"));
            LogItem(await window.ScriptEvaluate<String>("return getStrWArgs('arg1','arg2');"));
            LogItem(await window.ScriptEvaluateMethod<String>("getStrWArgs", "arg1", 1234.5678));
            LogItem(await window.ScriptEvaluateMethod<String[]>("getStrArr"));
            LogItem(await window.ScriptEvaluateMethod<ComplexObj>("getComplexObj"));
        }
        public class ComplexObj
        {
            public int id;
            public class ComplexObjSub
            {
                public string name;
                public string val;
            }
            public ComplexObjSub subObj;
            public object[] subArr;
        }
        private async void SendF12()
        {
            var mainWindHandle = window.GetBrowserChildProcess().MainWindowHandle;//main process null?
            PInvoke.SetForegroundWindow(new(mainWindHandle));
            PInvoke.PostMessage(new(mainWindHandle), PInvoke.WM_KEYDOWN, new Windows.Win32.Foundation.WPARAM((nuint)VIRTUAL_KEY.VK_F12), default);
            await Task.Delay(50);
            PInvoke.PostMessage(new(mainWindHandle), PInvoke.WM_KEYUP, new Windows.Win32.Foundation.WPARAM((nuint)VIRTUAL_KEY.VK_F12), default);
        }

        private void btnDevTools_Click(object sender, RoutedEventArgs e)
        {
            SendF12();
        }
    }
}
