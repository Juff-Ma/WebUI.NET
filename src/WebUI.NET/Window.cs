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
using System.Diagnostics;
using System.IO;

#if NET7_0_OR_GREATER
using System.Runtime.CompilerServices;
#endif
using System.Runtime.InteropServices;

using WebUI.Events;

namespace WebUI
{
#if NET7_0_OR_GREATER   
    public partial class Window : IDisposable
    {
#else
    public class Window : IDisposable
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

        /// <summary>
        /// Determines whether <paramref name="handle"/> is valid or not.
        /// </summary>
        /// <param name="handle">The handle to be checked.</param>
        /// <returns>
        ///   <c>true</c> if <paramref name="handle"/> is valid; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsHandleValid(IntPtr handle)
        {
            return handle != IntPtr.Zero;
        }

        /// <summary>
        /// Throws if disposed or Window handle is invalid.
        /// </summary>
        /// <exception cref="System.ObjectDisposedException">Window</exception>
        /// <exception cref="System.InvalidOperationException">Window handle is invalid.</exception>
        private void ThrowIfDisposedOrInvalid()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(Window));
            if (_handle.IsInvalid)
                throw new InvalidOperationException("Window handle is invalid.");
        }

        internal sealed class WindowHandle : SafeHandle
        {
            public WindowHandle(IntPtr handle, bool isOwner) : base(IntPtr.Zero, isOwner)
            {
                SetHandle(handle);
            }

            public override bool IsInvalid => !IsHandleValid(handle);

            protected override bool ReleaseHandle()
            {
                Natives.WebUIDestroy(handle);
                return !IsHandleValid(Natives.WebUICheckValidWindow(handle));
            }
        }

        private bool _disposed;

        private readonly WindowHandle _handle;

        public void ApplyWindowProperties(WindowProperties properties)
        {
            SetFullscreen(properties.Fullscreen);
            SetHidden(properties.Hidden);

            if (properties.Width.HasValue && properties.Height.HasValue)
            {
                SetSize(properties.Width.Value, properties.Height.Value);
            }

            if (properties.X.HasValue && properties.Y.HasValue)
            {
                SetPosition(properties.X.Value, properties.Y.Value);
            }

            if (properties.Port.HasValue)
            {
                SetPort(properties.Port.Value);
            }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER
            if (properties.Icon is { })
            {
                SetIcon(properties.Icon);
            }

            if (!string.IsNullOrEmpty(properties.RootFolder))
            {
                SetRootFolder(properties.RootFolder);
            }

            if (!string.IsNullOrEmpty(properties.ProxyServer))
            {
                SetProxyServer(properties.ProxyServer);
            }
#else
            if (!(properties.Icon is null))
            {
                SetIcon(properties.Icon);
            }

            if (!(properties.RootFolder is null) && properties.RootFolder == "")
            {
                SetRootFolder(properties.RootFolder);
            }

            if (!(properties.ProxyServer is null) && properties.ProxyServer == "")
            {
                SetProxyServer(properties.ProxyServer);
            }
#endif
        }

        internal Window(IntPtr windowHandle, bool isMainInstance = true)
        {
            _handle = new WindowHandle(windowHandle, isMainInstance);
        }

        public Window() : this(Natives.WebUINewWindow()) { }

        public Window(int windowId) : this(Natives.WebUINewWindow(new IntPtr(windowId))) { }

        public Window(WindowProperties properties) : this()
        {
            ApplyWindowProperties(properties);
        }

        public Window(int windowId, WindowProperties properties) : this(windowId)
        {
            ApplyWindowProperties(properties);
        }

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER
        public static Window? GetWindowById(int id)
        {
#else
        public static Window FromWindowId(int id)
        {
#endif
            //Return null if the window doesn't exist
            var handle = Natives.WebUICheckValidWindow(new IntPtr(id));

            if (!IsHandleValid(handle))
            {
                return null;
            }

            //if window exists return it as secondary instance
            return new Window(handle, false);
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="Window"/> class.
        /// </summary>
        ~Window() => Dispose(false);

        public void SetFullscreen(bool value)
        {
            ThrowIfDisposedOrInvalid();
            Natives.WebUISetKiosk(_handle, value);
        }

        public void SetHidden(bool value)
        {
            ThrowIfDisposedOrInvalid();
            Natives.WebUISetHidden(_handle, value);
        }

        public void SetSize(uint width, uint height)
        {
            ThrowIfDisposedOrInvalid();
            Natives.WebUISetSize(_handle, width, height);
        }

        public void SetPosition(uint x, uint y)
        {
            ThrowIfDisposedOrInvalid();
            Natives.WebUISetPosition(_handle, x, y);
        }

        public void SetPort(uint port)
        {
            ThrowIfDisposedOrInvalid();
            Natives.WebUISetPort(_handle, new UIntPtr(port));
        }

        public void SetIcon(string svgString)
        {
            ThrowIfDisposedOrInvalid();
            Natives.WebUISetIcon(_handle, svgString, "image/svg+xml");
        }

        public void SetIcon(Icon icon)
        {
            ThrowIfDisposedOrInvalid();
            if (icon.Type == Icon.IconType.Svg && icon.SvgData != string.Empty)
            {
                Natives.WebUISetIcon(_handle, icon.SvgData, "image/svg+xml");
                return;
            }

            Natives.WebUISetIcon(_handle, icon.Data, Icon.IconTypeToMimeType(icon.Type));
        }

        public bool IsShown
        {
            get
            {
                ThrowIfDisposedOrInvalid();
                return Natives.WebUIWindowIsShown(_handle);
            }
        }

        public string CurrentUrl
        {
            get
            {
                ThrowIfDisposedOrInvalid();
                return Natives.WebUIGetUrl(_handle);
            }
            set
            {
                Navigate(value);
            }
        }

        public void Navigate(string url)
        {
            ThrowIfDisposedOrInvalid();
            Natives.WebUINavigate(_handle, url);
        }

        public static int GetNewWindowId()
        {
            return Natives.WebUIGetNewWindowId().ToInt32();
        }

        public bool Show(string content)
        {
            ThrowIfDisposedOrInvalid();
            return Natives.WebUIShow(_handle, content);
        }

        public bool Show(string content, Browser browser)
        {
            ThrowIfDisposedOrInvalid();
            return Natives.WebUIShow(_handle, content, browser);
        }

        public void Close()
        {
            Natives.WebUIClose(_handle);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _handle.SetHandleAsInvalid();
                _handle.Dispose();
            }

            _disposed = true;
        }

        public void SetRootFolder(string folder)
        {
            ThrowIfDisposedOrInvalid();
            if (!Natives.WebUISetRootFolder(_handle, folder))
            {
                throw new DirectoryNotFoundException("Specified folder does not exist or is invalid in another way");
            }
        }

        public void SetRootFolder(DirectoryInfo folder)
        {
            SetRootFolder(folder.FullName);
        }

        public void SetBrowserProfile(string name, string path)
        {
            ThrowIfDisposedOrInvalid();
            Natives.WebUISetProfile(_handle, name, path);
        }

        public void SetBrowserProfile(BrowserProfile profile)
        {
            SetBrowserProfile(profile.Name, profile.Path);
        }

        public void DeleteBrowserProfile()
        {
            ThrowIfDisposedOrInvalid();
            Natives.WebUIDeleteProfile(_handle);
        }

        public void SetProxyServer(string proxy)
        {
            ThrowIfDisposedOrInvalid();
            Natives.WebUISetProxy(_handle, proxy);
        }

        public Process GetBrowserMainProcess()
        {
            ThrowIfDisposedOrInvalid();
            int processId = Convert.ToInt32(Natives.WebUIGetParentProcessId(_handle).ToUInt64());
            return Process.GetProcessById(processId);

        }

        public Process GetBrowserChildProcess()
        {
            ThrowIfDisposedOrInvalid();
            int processId = Convert.ToInt32(Natives.WebUIGetChildProcessId(_handle));
            return Process.GetProcessById(processId);
        }
#if NET7_0_OR_GREATER
        private static partial class Natives
        {
            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_new_window")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial IntPtr WebUINewWindow();

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_new_window_id")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial IntPtr WebUINewWindow(IntPtr windowId);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_get_new_window_id")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial IntPtr WebUIGetNewWindowId();

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_interface_get_window_id")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial IntPtr WebUICheckValidWindow(IntPtr windowId);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_interface_bind")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial UIntPtr WebUIBind(WindowHandle windowHandle, string element,
                [MarshalAs(UnmanagedType.FunctionPtr)] EventCallback callback);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_show")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            [return: MarshalAs(UnmanagedType.I1)]
            public static partial bool WebUIShow(WindowHandle windowHandle, string content);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_show_browser")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            [return: MarshalAs(UnmanagedType.I1)]
            public static partial bool WebUIShow(WindowHandle windowHandle, string content, UIntPtr browser);

            public static bool WebUIShow(WindowHandle windowHandle, string content, Browser browser) =>
                WebUIShow(windowHandle, content, (UIntPtr)browser);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_set_kiosk")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial void WebUISetKiosk(WindowHandle windowHandle,
                [MarshalAs(UnmanagedType.I1)] bool status);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_destroy")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial void WebUIDestroy(IntPtr windowHandle);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_close")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial void WebUIClose(WindowHandle windowHandle);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_set_root_folder")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            [return: MarshalAs(UnmanagedType.I1)]
            public static partial bool WebUISetRootFolder(WindowHandle windowHandle, string folder);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_set_file_handler")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial void WebUISetFileHandler(WindowHandle windowHandle,
                [MarshalAs(UnmanagedType.FunctionPtr)] FileHandler handler);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_is_shown")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            [return: MarshalAs(UnmanagedType.I1)]
            public static partial bool WebUIWindowIsShown(WindowHandle windowHandle);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_set_icon")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial void WebUISetIcon(WindowHandle windowHandle, string icon, string iconType);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_set_icon")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial void WebUISetIcon(WindowHandle windowHandle,
                [MarshalAs(UnmanagedType.LPArray)]in byte[] icon, string iconType);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_send_raw")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial void WebUISendRaw(WindowHandle windowHandle, string function,
                [MarshalAs(UnmanagedType.LPArray)] in byte[] data, UIntPtr length);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_set_hide")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial void WebUISetHidden(WindowHandle windowHandle,
                [MarshalAs(UnmanagedType.I1)] bool status);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_set_size")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial void WebUISetSize(WindowHandle windowHandle,
                [MarshalAs(UnmanagedType.U4)] uint width,
                [MarshalAs(UnmanagedType.U4)] uint height);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_set_position")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial void WebUISetPosition(WindowHandle windowHandle,
                [MarshalAs(UnmanagedType.U4)] uint x,
                [MarshalAs(UnmanagedType.U4)] uint y);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_set_profile")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial void WebUISetProfile(WindowHandle windowHandle, string name, string path);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_set_proxy")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial void WebUISetProxy(WindowHandle windowHandle, string proxyServer);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_get_url")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial string WebUIGetUrl(WindowHandle windowHandle);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_set_public")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial void WebUISetPublic(WindowHandle windowHandle,
                [MarshalAs(UnmanagedType.I1)] bool status);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_navigate")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial void WebUINavigate(WindowHandle windowHandle, string url);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_delete_profile")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial void WebUIDeleteProfile(WindowHandle windowHandle);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_get_parent_process_id")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial UIntPtr WebUIGetParentProcessId(WindowHandle windowHandle);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_get_child_process_id")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial UIntPtr WebUIGetChildProcessId(WindowHandle windowHandle);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_set_port")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            [return: MarshalAs(UnmanagedType.I1)]
            public static partial bool WebUISetPort(WindowHandle windowHandle, UIntPtr port);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_run")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial void WebUIRun(WindowHandle windowHandle, string javaScript);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_script")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            [return: MarshalAs(UnmanagedType.I1)]
            public static partial bool WebUIRun(WindowHandle windowHandle, string function,
                [MarshalAs(UnmanagedType.LPArray)] ref byte[] data, UIntPtr length);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_set_runtime")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial void WebUISetRuntime(WindowHandle windowHandle, UIntPtr runtime);

            public static void WebUISetRuntime(WindowHandle windowHandle, Runtime runtime) =>
                WebUISetRuntime(windowHandle, (UIntPtr)runtime);
        }
#else
        private static class Natives
        {
            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_new_window")]
            public static extern IntPtr WebUINewWindow();

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_new_window_id")]
            public static extern IntPtr WebUINewWindow(IntPtr windowId);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_get_new_window_id")]
            public static extern IntPtr WebUIGetNewWindowId();

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_interface_get_window_id")]
            public static extern IntPtr WebUICheckValidWindow(IntPtr windowId);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_interface_bind")]
            public static extern UIntPtr WebUIBind(WindowHandle windowHandle, string element,
                [MarshalAs(UnmanagedType.FunctionPtr)] EventCallback callback);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_show")]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern bool WebUIShow(WindowHandle windowHandle, string content);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_show_browser")]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern bool WebUIShow(WindowHandle windowHandle, string content,
                [MarshalAs(UnmanagedType.SysUInt)] Browser browser);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_set_kiosk")]
            public static extern void WebUISetKiosk(WindowHandle windowHandle,
                [MarshalAs(UnmanagedType.I1)] bool status);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_destroy")]
            public static extern void WebUIDestroy(IntPtr windowHandle);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_close")]
            public static extern void WebUIClose(WindowHandle windowHandle);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_set_root_folder")]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern bool WebUISetRootFolder(WindowHandle windowHandle, string folder);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_set_file_handler")]
            public static extern void WebUISetFileHandler(WindowHandle windowHandle,
                [MarshalAs(UnmanagedType.FunctionPtr)] FileHandler handler);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_is_shown")]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern bool WebUIWindowIsShown(WindowHandle windowHandle);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_set_icon")]
            public static extern void WebUISetIcon(WindowHandle windowHandle, string icon, string iconType);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_set_icon")]
            public static extern void WebUISetIcon(WindowHandle windowHandle,
                [MarshalAs(UnmanagedType.LPArray), In] byte[] icon, string iconType);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_send_raw")]
            public static extern void WebUISendRaw(WindowHandle windowHandle, string function,
                [MarshalAs(UnmanagedType.LPArray), In] byte[] data, UIntPtr length);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_set_hide")]
            public static extern void WebUISetHidden(WindowHandle windowHandle,
                [MarshalAs(UnmanagedType.I1)] bool status);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_set_size")]
            public static extern void WebUISetSize(WindowHandle windowHandle,
                [MarshalAs(UnmanagedType.U4)] uint width,
                [MarshalAs(UnmanagedType.U4)] uint height);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_set_position")]
            public static extern void WebUISetPosition(WindowHandle windowHandle,
                [MarshalAs(UnmanagedType.U4)] uint x,
                [MarshalAs(UnmanagedType.U4)] uint y);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_set_profile")]
            public static extern void WebUISetProfile(WindowHandle windowHandle, string name, string path);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_set_proxy")]
            public static extern void WebUISetProxy(WindowHandle windowHandle, string proxyServer);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_get_url")]
            public static extern string WebUIGetUrl(WindowHandle windowHandle);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_set_public")]
            public static extern void WebUISetPublic(WindowHandle windowHandle,
                [MarshalAs(UnmanagedType.I1)] bool status);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_navigate")]
            public static extern void WebUINavigate(WindowHandle windowHandle, string url);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_delete_profile")]
            public static extern void WebUIDeleteProfile(WindowHandle windowHandle);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_get_parent_process_id")]
            public static extern UIntPtr WebUIGetParentProcessId(WindowHandle windowHandle);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_get_child_process_id")]
            public static extern UIntPtr WebUIGetChildProcessId(WindowHandle windowHandle);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_set_port")]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern bool WebUISetPort(WindowHandle windowHandle, UIntPtr port);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_run")]
            public static extern void WebUIRun(WindowHandle windowHandle, string javaScript);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_script")]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern bool WebUIRun(WindowHandle windowHandle, string function,
                [MarshalAs(UnmanagedType.LPArray)] ref byte[] data, UIntPtr length);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_set_runtime")]
            public static extern void WebUISetRuntime(WindowHandle windowHandle,
                [MarshalAs(UnmanagedType.SysUInt)] Runtime runtime);
        }
#endif
    }
}
