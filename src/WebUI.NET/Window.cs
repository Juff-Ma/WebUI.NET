/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

//TODO: implement

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER
#nullable enable
#endif

using System;
#if NET7_0_OR_GREATER
using System.Runtime.CompilerServices;
#endif
using System.Runtime.InteropServices;
using WebUI.Events;

namespace WebUI
{
#if NET7_0_OR_GREATER   
    public partial class Window
    {
#else
    public class Window
    {
#endif
        [UnmanagedFunctionPointer(CallingConvention.Cdecl,
            BestFitMapping = false, ThrowOnUnmappableChar = false,
            CharSet = CharSet.Ansi)]
        private delegate void EventCallback(UIntPtr windowHandle,
            [MarshalAs(UnmanagedType.SysUInt)] EventType eventType, string element,
            UIntPtr eventId, UIntPtr bindId);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl,
            BestFitMapping = false, ThrowOnUnmappableChar = false,
            CharSet = CharSet.Ansi)]
        private delegate IntPtr FileHandler(string filename, out int length);

        private UIntPtr _handle;

        internal Window(UIntPtr windowHandle)
        {
            _handle = windowHandle;
        }

        public Window() : this(Natives.WebUINewWindow()) { }

        public Window(uint windowId) : this(Natives.WebUINewWindow(new UIntPtr(windowId))) { }

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER
        public static Window? GetWindowById(uint id)
        {
#else
        public static Window GetWindowById(uint id)
        {
#endif


            var handle = Natives.WebUICheckValidWindow(new UIntPtr(id));

            if (handle == UIntPtr.Zero)
            {
                return null;
            }

            return new Window(handle);
        }

        public static uint GetNewWindowId()
        {
            return Natives.WebUIGetNewWindowId().ToUInt32();
        }

        public bool Show(string content)
        {
            return Natives.WebUIShow(_handle, content);
        }

        public bool Show(string content, Browser browser)
        {
            return Natives.WebUIShow(_handle, content, browser);
        }
#if NET7_0_OR_GREATER
        private static partial class Natives
        {
            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_new_window")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial UIntPtr WebUINewWindow();

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_new_window_id")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial UIntPtr WebUINewWindow(UIntPtr windowId);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_get_new_window_id")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial UIntPtr WebUIGetNewWindowId();

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_interface_get_window_id")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial UIntPtr WebUICheckValidWindow(UIntPtr windowId);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_interface_bind")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial UIntPtr WebUIBind(UIntPtr windowHandle, string element,
                [MarshalAs(UnmanagedType.FunctionPtr)] EventCallback callback);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_show")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            [return: MarshalAs(UnmanagedType.I1)]
            public static partial bool WebUIShow(UIntPtr windowHandle, string content);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_show_browser")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            [return: MarshalAs(UnmanagedType.I1)]
            public static partial bool WebUIShow(UIntPtr windowHandle, string content, UIntPtr browser);

            public static bool WebUIShow(UIntPtr windowHandle, string content, Browser browser) =>
                WebUIShow(windowHandle, content, (UIntPtr)browser);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_set_kiosk")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial void WebUISetKiosk(UIntPtr windowHandle,
                [MarshalAs(UnmanagedType.I1)] bool status);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_destroy")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial void WebUIDestroy(UIntPtr windowHandle);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_set_root_folder")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            [return: MarshalAs(UnmanagedType.I1)]
            public static partial bool WebUISetRootFolder(UIntPtr windowHandle, string folder);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_set_file_handler")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial void WebUISetFileHandler(UIntPtr windowHandle,
                [MarshalAs(UnmanagedType.FunctionPtr)] FileHandler handler);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_is_shown")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            [return: MarshalAs(UnmanagedType.I1)]
            public static partial bool WebUIWindowIsShown(UIntPtr windowHandle);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_set_icon")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial void WebUISetIcon(UIntPtr windowHandle, string icon, string iconType);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_send_raw")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial void WebUISendRaw(UIntPtr windowHandle, string function,
                [MarshalAs(UnmanagedType.LPArray)] byte[] data, UIntPtr length);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_set_hide")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial void WebUISetHidden(UIntPtr windowHandle,
                [MarshalAs(UnmanagedType.I1)] bool status);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_set_size")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial void WebUISetSize(UIntPtr windowHandle,
                [MarshalAs(UnmanagedType.U4)] uint width,
                [MarshalAs(UnmanagedType.U4)] uint height);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_set_position")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial void WebUISetPosition(UIntPtr windowHandle,
                [MarshalAs(UnmanagedType.U4)] uint x,
                [MarshalAs(UnmanagedType.U4)] uint y);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_set_profile")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial void WebUISetProfile(UIntPtr windowHandle, string name, string path);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_set_proxy")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial void WebUISetProxy(UIntPtr windowHandle, string proxyServer);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_get_url")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial string WebUIGetUrl(UIntPtr windowHandle);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_set_public")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial void WebUISetPublic(UIntPtr windowHandle,
                [MarshalAs(UnmanagedType.I1)] bool status);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_navigate")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial void WebUINavigate(UIntPtr windowHandle, string url);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_delete_profile")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial void WebUIDeleteProfile(UIntPtr windowHandle);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_get_parent_process_id")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial UIntPtr WebUIGetParentProcessId(UIntPtr windowHandle);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_get_child_process_id")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial UIntPtr WebUIGetChildProcessId(UIntPtr windowHandle);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_set_port")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            [return: MarshalAs(UnmanagedType.I1)]
            public static partial bool WebUISetPort(UIntPtr windowHandle, UIntPtr port);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_run")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial void WebUIRun(UIntPtr windowHandle, string javaScript);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_script")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            [return: MarshalAs(UnmanagedType.I1)]
            public static partial bool WebUIRun(UIntPtr windowHandle, string function,
                [MarshalAs(UnmanagedType.LPArray)] ref byte[] data, UIntPtr length);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_set_runtime")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial void WebUISetRuntime(UIntPtr windowHandle, UIntPtr runtime);

            public static void WebUISetRuntime(UIntPtr windowHandle, Runtime runtime) =>
                WebUISetRuntime(windowHandle, (UIntPtr)runtime);
        }
#else
        private static class Natives
        {
            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_new_window")]
            public static extern UIntPtr WebUINewWindow();

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_new_window_id")]
            public static extern UIntPtr WebUINewWindow(UIntPtr windowId);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_get_new_window_id")]
            public static extern UIntPtr WebUIGetNewWindowId();

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_interface_get_window_id")]
            public static extern UIntPtr WebUICheckValidWindow(UIntPtr windowId);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_interface_bind")]
            public static extern UIntPtr WebUIBind(UIntPtr windowHandle, string element,
                [MarshalAs(UnmanagedType.FunctionPtr)] EventCallback callback);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_show")]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern bool WebUIShow(UIntPtr windowHandle, string content);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_show_browser")]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern bool WebUIShow(UIntPtr windowHandle, string content,
                [MarshalAs(UnmanagedType.SysUInt)] Browser browser);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_set_kiosk")]
            public static extern void WebUISetKiosk(UIntPtr windowHandle,
                [MarshalAs(UnmanagedType.I1)] bool status);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_destroy")]
            public static extern void WebUIDestroy(UIntPtr windowHandle);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_set_root_folder")]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern bool WebUISetRootFolder(UIntPtr windowHandle, string folder);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_set_file_handler")]
            public static extern void WebUISetFileHandler(UIntPtr windowHandle,
                [MarshalAs(UnmanagedType.FunctionPtr)] FileHandler handler);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_is_shown")]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern bool WebUIWindowIsShown(UIntPtr windowHandle);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_set_icon")]
            public static extern void WebUISetIcon(UIntPtr windowHandle, string icon, string iconType);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_send_raw")]
            public static extern void WebUISendRaw(UIntPtr windowHandle, string function,
                [MarshalAs(UnmanagedType.LPArray)] byte[] data, UIntPtr length);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_set_hide")]
            public static extern void WebUISetHidden(UIntPtr windowHandle,
                [MarshalAs(UnmanagedType.I1)] bool status);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_set_size")]
            public static extern void WebUISetSize(UIntPtr windowHandle,
                [MarshalAs(UnmanagedType.U4)] uint width,
                [MarshalAs(UnmanagedType.U4)] uint height);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_set_position")]
            public static extern void WebUISetPosition(UIntPtr windowHandle,
                [MarshalAs(UnmanagedType.U4)] uint x,
                [MarshalAs(UnmanagedType.U4)] uint y);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_set_profile")]
            public static extern void WebUISetProfile(UIntPtr windowHandle, string name, string path);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_set_proxy")]
            public static extern void WebUISetProxy(UIntPtr windowHandle, string proxyServer);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_get_url")]
            public static extern string WebUIGetUrl(UIntPtr windowHandle);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_set_public")]
            public static extern void WebUISetPublic(UIntPtr windowHandle,
                [MarshalAs(UnmanagedType.I1)] bool status);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_navigate")]
            public static extern void WebUINavigate(UIntPtr windowHandle, string url);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_delete_profile")]
            public static extern void WebUIDeleteProfile(UIntPtr windowHandle);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_get_parent_process_id")]
            public static extern UIntPtr WebUIGetParentProcessId(UIntPtr windowHandle);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_get_child_process_id")]
            public static extern UIntPtr WebUIGetChildProcessId(UIntPtr windowHandle);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_set_port")]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern bool WebUISetPort(UIntPtr windowHandle, UIntPtr port);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_run")]
            public static extern void WebUIRun(UIntPtr windowHandle, string javaScript);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_script")]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern bool WebUIRun(UIntPtr windowHandle, string function,
                [MarshalAs(UnmanagedType.LPArray)] ref byte[] data, UIntPtr length);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_set_runtime")]
            public static extern void WebUISetRuntime(UIntPtr windowHandle,
                [MarshalAs(UnmanagedType.SysUInt)] Runtime runtime);
        }
#endif
    }
}
