/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

//TODO: implement

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

#if NET7_0_OR_GREATER
        private static partial class Natives
        {
            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_new_window")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial UIntPtr NewWindow();

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_new_window_id")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial UIntPtr NewWindow(UIntPtr windowId);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_get_new_window_id")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial UIntPtr GetNewWindowId();

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_interface_get_window_id")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial UIntPtr CheckValidWindow(UIntPtr windowId);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_interface_bind")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial UIntPtr Bind(UIntPtr windowHandle, string element,
                [MarshalAs(UnmanagedType.FunctionPtr)] EventCallback callback);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_show")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            [return: MarshalAs(UnmanagedType.I1)]
            public static partial bool Show(UIntPtr windowHandle, string content);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_show_browser")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            [return: MarshalAs(UnmanagedType.I1)]
            public static partial bool Show(UIntPtr windowHandle, string content, UIntPtr browser);

            public static bool Show(UIntPtr windowHandle, string content, Browser browser) =>
                Show(windowHandle, content, (UIntPtr)browser);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_set_kiosk")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial void SetKiosk(UIntPtr windowHandle,
                [MarshalAs(UnmanagedType.I1)] bool status);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_destroy")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial void Destroy(UIntPtr windowHandle);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_set_root_folder")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            [return: MarshalAs(UnmanagedType.I1)]
            public static partial bool SetRootFolder(UIntPtr windowHandle, string folder);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_set_file_handler")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial void SetFileHandler(UIntPtr windowHandle,
                [MarshalAs(UnmanagedType.FunctionPtr)] FileHandler handler);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_is_shown")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            [return: MarshalAs(UnmanagedType.I1)]
            public static partial bool WindowIsShown(UIntPtr windowHandle);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_set_icon")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial void SetIcon(UIntPtr windowHandle, string icon, string iconType);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_send_raw")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial void SendRaw(UIntPtr windowHandle, string function,
                [MarshalAs(UnmanagedType.LPArray)] byte[] data, UIntPtr length);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_set_hide")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial void SetHidden(UIntPtr windowHandle,
                [MarshalAs(UnmanagedType.I1)] bool status);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_set_size")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial void SetSize(UIntPtr windowHandle,
                [MarshalAs(UnmanagedType.U4)] uint width,
                [MarshalAs(UnmanagedType.U4)] uint height);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_set_position")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial void SetPosition(UIntPtr windowHandle,
                [MarshalAs(UnmanagedType.U4)] uint x,
                [MarshalAs(UnmanagedType.U4)] uint y);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_set_profile")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial void SetProfile(UIntPtr windowHandle, string name, string path);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_set_proxy")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial void SetProxy(UIntPtr windowHandle, string proxyServer);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_get_url")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial string GetUrl(UIntPtr windowHandle);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_set_public")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial void SetPublic(UIntPtr windowHandle,
                [MarshalAs(UnmanagedType.I1)] bool status);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_navigate")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial void Navigate(UIntPtr windowHandle, string url);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_delete_profile")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial void DeleteProfile(UIntPtr windowHandle);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_get_parent_process_id")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial UIntPtr GetParentProcessId(UIntPtr windowHandle);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_get_child_process_id")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial UIntPtr GetChildProcessId(UIntPtr windowHandle);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_set_port")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            [return: MarshalAs(UnmanagedType.I1)]
            public static partial bool SetPort(UIntPtr windowHandle, UIntPtr port);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_run")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial void Run(UIntPtr windowHandle, string javaScript);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_script")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            [return: MarshalAs(UnmanagedType.I1)]
            public static partial bool Run(UIntPtr windowHandle, string function,
                [MarshalAs(UnmanagedType.LPArray)] ref byte[] data, UIntPtr length);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_set_runtime")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial void SetRuntime(UIntPtr windowHandle, UIntPtr runtime);

            public static void SetRuntime(UIntPtr windowHandle, Runtime runtime) =>
                SetRuntime(windowHandle, (UIntPtr)runtime);
        }
#else
        private static class Natives
        {
            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_new_window")]
            public static extern UIntPtr NewWindow();

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_new_window_id")]
            public static extern UIntPtr NewWindow(UIntPtr windowId);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_get_new_window_id")]
            public static extern UIntPtr GetNewWindowId();

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_interface_get_window_id")]
            public static extern UIntPtr CheckValidWindow(UIntPtr windowId);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_interface_bind")]
            public static extern UIntPtr Bind(UIntPtr windowHandle, string element,
                [MarshalAs(UnmanagedType.FunctionPtr)] EventCallback callback);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_show")]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern bool Show(UIntPtr windowHandle, string content);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_show_browser")]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern bool Show(UIntPtr windowHandle, string content,
                [MarshalAs(UnmanagedType.SysUInt)] Browser browser);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_set_kiosk")]
            public static extern void SetKiosk(UIntPtr windowHandle,
                [MarshalAs(UnmanagedType.I1)] bool status);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_destroy")]
            public static extern void Destroy(UIntPtr windowHandle);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_set_root_folder")]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern bool SetRootFolder(UIntPtr windowHandle, string folder);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_set_file_handler")]
            public static extern void SetFileHandler(UIntPtr windowHandle,
                [MarshalAs(UnmanagedType.FunctionPtr)] FileHandler handler);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_is_shown")]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern bool WindowIsShown(UIntPtr windowHandle);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_set_icon")]
            public static extern void SetIcon(UIntPtr windowHandle, string icon, string iconType);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_send_raw")]
            public static extern void SendRaw(UIntPtr windowHandle, string function,
                [MarshalAs(UnmanagedType.LPArray)] byte[] data, UIntPtr length);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_set_hide")]
            public static extern void SetHidden(UIntPtr windowHandle,
                [MarshalAs(UnmanagedType.I1)] bool status);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_set_size")]
            public static extern void SetSize(UIntPtr windowHandle,
                [MarshalAs(UnmanagedType.U4)] uint width,
                [MarshalAs(UnmanagedType.U4)] uint height);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_set_position")]
            public static extern void SetPosition(UIntPtr windowHandle,
                [MarshalAs(UnmanagedType.U4)] uint x,
                [MarshalAs(UnmanagedType.U4)] uint y);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_set_profile")]
            public static extern void SetProfile(UIntPtr windowHandle, string name, string path);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_set_proxy")]
            public static extern void SetProxy(UIntPtr windowHandle, string proxyServer);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_get_url")]
            public static extern string GetUrl(UIntPtr windowHandle);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_set_public")]
            public static extern void SetPublic(UIntPtr windowHandle,
                [MarshalAs(UnmanagedType.I1)] bool status);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_navigate")]
            public static extern void Navigate(UIntPtr windowHandle, string url);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_delete_profile")]
            public static extern void DeleteProfile(UIntPtr windowHandle);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_get_parent_process_id")]
            public static extern UIntPtr GetParentProcessId(UIntPtr windowHandle);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_get_child_process_id")]
            public static extern UIntPtr GetChildProcessId(UIntPtr windowHandle);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_set_port")]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern bool SetPort(UIntPtr windowHandle, UIntPtr port);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_run")]
            public static extern void Run(UIntPtr windowHandle, string javaScript);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_script")]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern bool Run(UIntPtr windowHandle, string function,
                [MarshalAs(UnmanagedType.LPArray)] ref byte[] data, UIntPtr length);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_set_runtime")]
            public static extern void SetRuntime(UIntPtr windowHandle,
                [MarshalAs(UnmanagedType.SysUInt)] Runtime runtime);
        }
#endif
    }
}
