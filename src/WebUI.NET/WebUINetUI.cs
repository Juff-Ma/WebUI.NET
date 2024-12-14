using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WebUI
{
   
    public class DomEvent
    {

        public string currentTargetId { get; set; }
        public string originalTargetId { get; set; }
        [JsonConverter(typeof(MilisecondEpochConverter))]
        public DateTime timestamp { get; set; }
        public string type { get; set; }
        public Dictionary<string, string> additionalProps; //map of additional property values requested

    }
    public enum CommonEventTypes { load, unload, error, resize, scroll, focus, blur, click, dblclick, mousedown, mouseup, mouseover, mouseout, mousemove, input, keydown, keypress, keyup, submit, change, DOMNodeInserted, DOMNodeRemoved, DOMSubtreeModified, DOMNodeInsertedIntoDocument, DOMNodeRemovedFromDocument, DOMContentLoaded, hashchange, beforeunload }
    public static class WebUINetUI
    {

        public class EventOpts
        {
            public bool capture { get; set; }
            public bool once { get; set; }
            public string abortKey { get; set; }

        }
        private const int startId = 5432;
        private static volatile int curFuncId = startId;
        // CommonEventTypes
        public static void AddEventListener(this Window window, Action<DomEvent> OnFired, CommonEventTypes eventType, String domId, EventOpts additionalOpts = null, params KeyValuePair<string, string>[] additionalEventCaptureProps) => window.AddEventListener(OnFired, eventType.ToString(), domId, additionalOpts, additionalEventCaptureProps);

        public static void AddEventListener(this Window window, Action<DomEvent> OnFired, String eventType, String domId, EventOpts additionalOpts = null, params KeyValuePair<string, string>[] additionalEventCaptureProps)
        {

            //addCSEventListener(type, elementID, csFuncID, additionalProps, options, abortKey)
            var addlDict = additionalEventCaptureProps?.Length > 0 ?
            Newtonsoft.Json.JsonConvert.SerializeObject( additionalEventCaptureProps.ToDictionary(x => x.Key, x => x.Value)) : null;
            var abortKey = additionalOpts?.abortKey;
            if (abortKey != null)
                additionalOpts.abortKey = null;
            var curId = window._RegisterBoundFunction(OnCalled: (str) => OnFired(Newtonsoft.Json.JsonConvert.DeserializeObject<DomEvent[]>(str)[0]), registerFunc: false);
            //window.InvokeJavascriptMethod($"{ourJSClass}.addCSEventListener", eventType, domId, curId, addlDict, additionalOpts, abortKey);
            window.ScriptEvaluateMethod<string>($"{ourJSClass}.addCSEventListener", eventType, domId, curId, addlDict, additionalOpts, abortKey);//not awiating but at least will be in background task exception
        }
        private static async void JavascriptFuncCallback(this Window window, int csFuncId, int jsCallId, string jsonOfArgs)
        {
            if (!csFuncIdToFired.TryGetValue(csFuncId, out var info))
                throw new Exception($"The JS func callback handler was passed an invalid function id {csFuncId}"); ;
            if (info.normFunc != null)
            {
                Window.UseSpecificDispatcher.InvokeWithVoid(() => info.normFunc(jsonOfArgs));
                return;
            }
            var res = await Window.UseSpecificDispatcher.InvokeWithReturnType(() => info.taskFunc(jsonOfArgs));

            await window.ScriptEvaluateMethod<string>($"{ourJSClass}.setCSFunctionResult", jsCallId, res);//we don't use invoke so we can get the error
            //window.InvokeJavascriptMethod("setCSFunctionResult", jsCallId, res);

        }
        private const string ourJSClass = $"window.WebUINet";
        public static void SetDebug(this Window window, bool debugging = true)
        {
            var jsDebug = debugging ? "true" : "false";
            window.InvokeJavaScript($@"{ourJSClass}.DEBUG_MODE={jsDebug};
window.webui.setLogging({jsDebug});
");
        }
        public static void RegisterBoundFunction(this Window window, String RegisteredName, Func<string, Task<string>> OnCalled) => window._RegisterBoundFunction(RegisteredName, OnCalledTask: OnCalled);
        public static void RegisterBoundFunction(this Window window, String RegisteredName, Action<string> OnCalled) => window._RegisterBoundFunction(RegisteredName, OnCalled);

        private static int _RegisterBoundFunction(this Window window, String RegisteredName = null, Action<string> OnCalled = null, Func<string, Task<string>> OnCalledTask = null, bool registerFunc = true)
        {
            var curId = Interlocked.Increment(ref curFuncId);
            csFuncIdToFired[curId] = new RegisteredFunc { taskFunc = OnCalledTask, normFunc = OnCalled };
            if (registerFunc)
            {
                //we don't use invoke so we can get the error
                window.ScriptEvaluateMethod<string>($"{ourJSClass}.addCSFunction", curId, RegisteredName, OnCalledTask != null); //not awaiting but exception will at least show in logs just no ST
                //window.InvokeJavascriptMethod("WebUINet.addCSFunction", curId, RegisteredName, OnCalledTask != null);
            }
            if (curId == startId + 1)
            {
                window.RegisterEventHandler(RawFuncCallback, "webuiNet_Callback");
            }
            return curId;
        }
        public static void InitOurBridge(this Window window)
        {
            OurJSBytes = Encoding.UTF8.GetBytes(File.ReadAllText("webui_net.js"));
            window.SetFileHandler((path) => path == "/webui_net.js" ? OurJSBytes : null);
        }
        private static byte[] OurJSBytes;
        private static object RawFuncCallback(WebUI.Events.Event evt, string element, ulong handlerId)
        {
            evt.Window.JavascriptFuncCallback((int)evt.GetNumber(0), (int)evt.GetNumber(1), evt.GetString(2));
            return null;
        }

        private class RegisteredFunc
        {
            public Func<string, Task<string>> taskFunc;
            public Action<string> normFunc;
        }
        private static ConcurrentDictionary<int, RegisteredFunc> csFuncIdToFired = new();
    }
}
