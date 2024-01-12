/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

//TODO: implement

using System;
using System.Runtime.InteropServices;
using System.Text;
using WebUI.Events;

namespace WebUI
{
    public class Window
    {
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

        //TODO: implement net7+
#if !NET7_0_OR_GREATER
        internal static partial class Natives
        {

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
                [MarshalAs(UnmanagedType.LPArray), Out] byte[] data, UIntPtr length);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_script")]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern bool Run(UIntPtr windowHandle, string function,
                [MarshalAs(UnmanagedType.LPStr)] StringBuilder data, UIntPtr length);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_set_runtime")]
            public static extern void SetRuntime(UIntPtr windowHandle,
                [MarshalAs(UnmanagedType.SysUInt)] Runtime runtime);
        }
#endif
    }
}
