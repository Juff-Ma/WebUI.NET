/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

// Ignore Spelling: Fullscreen svg

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER
#nullable enable
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

#if NET7_0_OR_GREATER
using System.Runtime.CompilerServices;
#endif
using System.Runtime.InteropServices;
using WebUI.Events;

namespace WebUI
{
    /// <summary>
    /// Class representing a WebUI (browser) Window
    /// </summary>
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
        private delegate void EventCallback(IntPtr windowHandle,
            UIntPtr eventType, string element,
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
        internal static bool IsHandleValid(IntPtr handle)
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
            // I hate this, but VS won't shut up
#if NET7_0_OR_GREATER
            ObjectDisposedException.ThrowIf(_disposed, nameof(Window));
#else
            if (_disposed)
                throw new ObjectDisposedException(nameof(Window));
#endif
            if (_handle.IsInvalid)
                throw new InvalidOperationException("Window handle is invalid.");
        }

        /// <summary>
        /// A save wrapper for the native window handle, makes sure it won't leak memory if used correctly
        /// </summary>
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

        /// <summary>
        /// List of event callbacks, used for keeping the Garbage collector happy
        /// </summary>
        // why VS? Why? don't you get it that i have to do this in order to support stuff older than .NET5
#if NET5_0_OR_GREATER
        private readonly List<EventCallback> _callbacks = new();
#else
        private readonly List<EventCallback> _callbacks = new List<EventCallback>();
#endif

        /// <summary>
        /// file handler storing the dynamic content handler,  used for keeping the Garbage collector happy
        /// </summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER
        private FileHandler? _fileHandler = null;
#else
        private FileHandler _fileHandler = null;
#endif

        internal Window(IntPtr windowHandle, bool isMainInstance = true)
        {
            _handle = new WindowHandle(windowHandle, isMainInstance);
        }

        /// <summary>
        /// Creates a new Window instance
        /// </summary>
        public Window() : this(Natives.WebUINewWindow()) { }

        /// <summary>
        /// Creates a new Window instance with the specified window id
        /// </summary>
        /// <param name="windowId">the window id to be used</param>
        public Window(int windowId) : this(Natives.WebUINewWindow(new IntPtr(windowId))) { }

        /// <summary>
        /// Creates a new Window instance with the specified <see cref="WindowProperties"/>
        /// </summary>
        /// <param name="properties">properties to be applied to the Window</param>
        public Window(WindowProperties properties) : this()
        {
            this.ApplyWindowProperties(properties);
        }

        /// <summary>
        /// Creates a new Window instance with the specified <see cref="WindowProperties"/> and window id
        /// </summary>
        /// <inheritdoc cref="Window(WindowProperties)"/>
        /// <inheritdoc cref="Window(int)"/>
        public Window(int windowId, WindowProperties properties) : this(windowId)
        {
            this.ApplyWindowProperties(properties);
        }

        /// <summary>
        /// Gets an existing Window by its id
        /// </summary>
        /// <param name="id">the id to be searched for</param>
        /// <returns>the <see cref="Window"/> if found otherwise <c>null</c></returns>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER
        public static Window? GetWindowById(int id)
        {
#else
        public static Window GetWindowById(int id)
        {
#endif
            var handle = Natives.WebUICheckValidWindow(new IntPtr(id));

            //Return null if the window doesn't exist
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

        /// <summary>
        /// Sets if the window should be full screen
        /// </summary>
        /// <param name="value"><c>true</c> if should be fullscreen; otherwise <c>false</c></param>
        /// <inheritdoc cref="ThrowIfDisposedOrInvalid"/>
        public void SetFullscreen(bool value)
        {
            ThrowIfDisposedOrInvalid();
            Natives.WebUISetKiosk(_handle, value);
        }

        /// <summary>
        /// Sets if the window should be hidden <br/>
        /// Should be called before <see cref="Show(string)"/>
        /// </summary>
        /// <param name="value"><c>true</c> if should be hidden; otherwise <c>false</c></param>
        /// <inheritdoc cref="ThrowIfDisposedOrInvalid"/>
        public void SetHidden(bool value)
        {
            ThrowIfDisposedOrInvalid();
            Natives.WebUISetHidden(_handle, value);
        }

        /// <summary>
        /// Sets the <paramref name="width"/> and <paramref name="height"/> of the Window
        /// </summary>
        /// <param name="width">the windows width</param>
        /// <param name="height">the windows height</param>
        /// <inheritdoc cref="ThrowIfDisposedOrInvalid"/>
        public void SetSize(uint width, uint height)
        {
            ThrowIfDisposedOrInvalid();
            Natives.WebUISetSize(_handle, width, height);
        }

        /// <summary>
        /// Sets the Windows <paramref name="x"/> and <paramref name="y"/> position
        /// </summary>
        /// <param name="x">the windows X position</param>
        /// <param name="y">the windows Y position</param>
        /// <inheritdoc cref="ThrowIfDisposedOrInvalid"/>
        public void SetPosition(uint x, uint y)
        {
            ThrowIfDisposedOrInvalid();
            Natives.WebUISetPosition(_handle, x, y);
        }

        /// <summary>
        /// Sets the <paramref name="port"/> of WebUI should use
        /// </summary>
        /// <remarks>
        /// This is useful for building semi web-applications or using and external web-server for WebUI (determining the port of webui.js)
        /// </remarks>
        /// <param name="port">the port to be set</param>
        /// <returns><c>true</c> if port is available and usable by WebUI; otherwise <c>false</c></returns>
        /// <inheritdoc cref="ThrowIfDisposedOrInvalid"/>
        public bool SetPort(uint port)
        {
            ThrowIfDisposedOrInvalid();
            return Natives.WebUISetPort(_handle, new UIntPtr(port));
        }

        /// <summary>
        /// Sets a SVG icon for WebUI to use as Window icon
        /// </summary>
        /// <param name="svgString">the SVG in text/xml format</param>
        /// <inheritdoc cref="ThrowIfDisposedOrInvalid"/>
        public void SetIcon(string svgString)
        {
            ThrowIfDisposedOrInvalid();
            Natives.WebUISetIcon(_handle, svgString, "image/svg+xml");
        }

        /// <summary>
        /// Sets a <see cref="Icon"/> for WebUI to use as Window icon
        /// </summary>
        /// <remarks>
        /// Icons other than SVG are not recommended and should be used with caution. <br/>
        /// If you need to use another icon format consider serving it over dynamic file handlers or directly from the root directory
        /// </remarks>
        /// <param name="icon">the icon to be set</param>
        /// <inheritdoc cref="ThrowIfDisposedOrInvalid"/>
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

        /// <summary>
        /// Allow the Window to be accessed from a public network
        /// </summary>
        /// <param name="value"><c>true</c> if window should be accessible from public network, otherwise <c>false</c></param>
        /// <inheritdoc cref="ThrowIfDisposedOrInvalid"/>
        public void SetAllowPublicConnections(bool value)
        {
            ThrowIfDisposedOrInvalid();
            Natives.WebUISetPublic(_handle, value);
        }

        /// <summary>
        /// Gets if the window is shown
        /// </summary>
        /// <inheritdoc cref="ThrowIfDisposedOrInvalid"/>
        public bool IsShown
        {
            get
            {
                ThrowIfDisposedOrInvalid();
                return Natives.WebUIWindowIsShown(_handle);
            }
        }

        /// <summary>
        /// Gets the current URL of the browser Window or Navigates to it if getting set
        /// </summary>
        /// <inheritdoc cref="ThrowIfDisposedOrInvalid"/>
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

        /// <summary>
        /// Navigates to the specified <paramref name="url"/><br/>
        /// Requires full HTTP/HTTPS url
        /// </summary>
        /// <param name="url">the url to be navigated to</param>
        /// <inheritdoc cref="ThrowIfDisposedOrInvalid"/>
        public void Navigate(string url)
        {
            ThrowIfDisposedOrInvalid();
            Natives.WebUINavigate(_handle, url);
        }

        /// <summary>
        /// Gets a window id to be used in conjunction with <see cref="Window(int)"/> or <see cref="GetWindowById(int)"/>
        /// </summary>
        /// <returns>an available window id not yet associated to any <see cref="Window"/></returns>
        public static int GetNewWindowId()
        {
            return Natives.WebUIGetNewWindowId().ToInt32();
        }

        /// <summary>
        /// Opens the Window with the specified <paramref name="content"/>
        /// </summary>
        /// <remarks>
        /// Raw HTML should be enclosed in <![CDATA[ <html></html> ]]> tags. All content should include a reference to webui.js
        /// </remarks>
        /// <param name="content">the content as raw HTML or URL</param>
        /// <returns><c>true</c> if Window opened successfully; otherwise <c>false</c></returns>
        /// <inheritdoc cref="ThrowIfDisposedOrInvalid"/>
        public bool Show(string content)
        {
            ThrowIfDisposedOrInvalid();
            return Natives.WebUIShow(_handle, content);
        }

        /// <param name="content">the content as raw HTML or URL</param>
        /// <param name="browser">the <see cref="Browser"/> to be opened</param>
        /// <inheritdoc cref="Show(string)"/>
        public bool Show(string content, Browser browser)
        {
            ThrowIfDisposedOrInvalid();
            return Natives.WebUIShow(_handle, content, browser);
        }

        /// <summary>
        /// Closes the Window
        /// </summary>
        /// <inheritdoc cref="ThrowIfDisposedOrInvalid"/>
        public void Close()
        {
            ThrowIfDisposedOrInvalid();
            Natives.WebUIClose(_handle);
        }

        /// <summary>
        /// Disposes the native Window handle
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the native Window handle and runs the keep-alive code for event callbacks
        /// </summary>
        /// <param name="disposing">set to <c>false</c> if the Window handle shouldn't be disposed</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (_handle.IsInvalid || _handle.IsClosed)
            {
                return;
            }

            foreach (var callback in _callbacks)
            {
                GC.KeepAlive(callback);
            }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER
            if (_fileHandler is { })
#else
            if (!(_fileHandler is null))
#endif
            {
                GC.KeepAlive(_fileHandler);
            }

            if (disposing)
            {
                _handle.SetHandleAsInvalid();
                _handle.Dispose();
            }

            _disposed = true;
        }

        /// <summary>
        /// Fire and forget JavaScript invocation
        /// </summary>
        /// <param name="js">the JavaScript to be run</param>
        /// <inheritdoc cref="ThrowIfDisposedOrInvalid"/>
        public void InvokeJavaScript(string js)
        {
            ThrowIfDisposedOrInvalid();
            Natives.WebUIRun(_handle, js);
        }

        /// <summary>
        /// Runs JavaScript and stores the result in <paramref name="buffer"/>
        /// </summary>
        /// <param name="js">the JavaScript to be run</param>
        /// <param name="buffer">contains the result of the JavaScript</param>
        /// <param name="timeout">maximum waiting time in seconds</param>
        /// <returns><c>true</c> if JavaScript executed without errors; otherwise <c>false</c>.<br/>
        /// If <c>false</c>, <paramref name="buffer"/> contains the error
        /// </returns>
        /// <inheritdoc cref="ThrowIfDisposedOrInvalid"/>
        public bool InvokeJavaScript(string js, ref byte[] buffer, uint timeout)
        {
            return InvokeJavaScript(js, ref buffer, new UIntPtr(timeout));
        }

        /// <param name="js">the JavaScript to be run</param>
        /// <param name="buffer">contains the result of the JavaScript</param>
        /// <param name="timeout">
        /// maximum waiting time represented as <see cref="TimeSpan"/><br/>
        /// any unit smaller than seconds will be ignored
        /// </param>
        /// <inheritdoc cref="InvokeJavaScript(string, ref byte[], uint)"/>
        public bool InvokeJavaScript(string js, ref byte[] buffer, TimeSpan timeout)
        {
            return InvokeJavaScript(js, ref buffer, (uint)timeout.TotalSeconds);
        }

        private bool InvokeJavaScript(string js, ref byte[] buffer, UIntPtr timeout)
        {
            ThrowIfDisposedOrInvalid();
#if NET7_0_OR_GREATER
            return Natives.WebUIRun(_handle, js, timeout, ref buffer, new UIntPtr((uint)buffer.Length));
#else
            return Natives.WebUIRun(_handle, js, timeout, buffer, new UIntPtr((uint)buffer.Length));
#endif
        }

        /// <summary>
        /// Invokes a JavaScript function with the specified argument <paramref name="data"/>
        /// </summary>
        /// <param name="function">the name of the function to be invoked</param>
        /// <param name="data">the raw binary data to be used as argument for the function</param>
        /// <inheritdoc cref="ThrowIfDisposedOrInvalid"/>
        public void InvokeJavaScript(string function, byte[] data)
        {
            ThrowIfDisposedOrInvalid();
            Natives.WebUISendRaw(_handle, function, data, new UIntPtr((uint)data.Length));
        }

        /// <summary>
        /// Registers the specified <see cref="IEventHandler"/> as event handler
        /// </summary>
        /// <param name="eventHandler">the event handler</param>
        /// <returns>the unique handler id.</returns>
        /// <inheritdoc cref="ThrowIfDisposedOrInvalid"/>
        public ulong RegisterEventHandler(IEventHandler eventHandler) => RegisterEventHandler(eventHandler, string.Empty);

        /// <param name="eventHandler">the event handler</param>
        /// <param name="element">the element the <paramref name="eventHandler"/> should be registered for</param>
        /// <remarks>
        /// The event handler will only receive events that are related to the specified <paramref name="element"/>,
        /// notably <see cref="EventType.Click"/>
        /// </remarks>
        /// <inheritdoc cref="RegisterEventHandler(IEventHandler)"/>
        public ulong RegisterEventHandler(IEventHandler eventHandler, string element)
            => RegisterEventHandler(eventHandler.HandleEvent, element);

        /// <summary>
        /// Registers the specified callback as event handler
        /// </summary>
        /// <inheritdoc cref="RegisterEventHandler(IEventHandler)"/>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER
        public ulong RegisterEventHandler(Func<Event, string, ulong, object?> eventHandler)
#else
        public ulong RegisterEventHandler(Func<Event, string, ulong, object> eventHandler)
#endif
        => RegisterEventHandler(eventHandler, string.Empty);

        /// <summary>
        /// Registers the specified callback as event handler
        /// </summary>
        /// <inheritdoc cref="RegisterEventHandler(IEventHandler, string)"/>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER
        public ulong RegisterEventHandler(Func<Event, string, ulong, object?> eventHandler, string element)
#else
        public ulong RegisterEventHandler(Func<Event, string, ulong, object> eventHandler, string element)
#endif
        {
            ThrowIfDisposedOrInvalid();
#pragma warning disable IDE0039 // No i will not use a local function
            EventCallback callback = (IntPtr windowHandle,
                UIntPtr eventType,
                string elementName,
                UIntPtr eventId, UIntPtr bindId) =>
            {
                ulong intType = eventType.ToUInt64();
                if (!Enum.IsDefined(typeof(EventType), intType))
                {
                    return;
                }

                var @event = new Event(windowHandle, eventId, (EventType)intType);
                var value = eventHandler(@event, elementName, bindId.ToUInt64());
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER
                if (value is { })
#else
                if (!(value is null))
#endif
                {
                    @event.ReturnValue(value.ToString());
                }
            };

            _callbacks.Add(callback);

            return Natives.WebUIBind(_handle, element, callback).ToUInt64();
        }

        /// <summary>
        /// Sets the specified <see cref="IFileHandler"/> as the handler for dynamically loading files
        /// </summary>
        /// <param name="fileHandler">the dynamic file handler to be registered</param>
        /// <inheritdoc cref="ThrowIfDisposedOrInvalid"/>
        public void SetFileHandler(IFileHandler fileHandler) => SetFileHandler(fileHandler.GetFile);

        /// <summary>
        /// Sets the specified callback as the handler for dynamically loading files
        /// </summary>
        /// <inheritdoc cref="SetFileHandler(IFileHandler)"/>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER
        public void SetFileHandler(Func<string, byte[]?> fileHandler)
#else
        public void SetFileHandler(Func<string, byte[]> fileHandler)
#endif
        {
            ThrowIfDisposedOrInvalid();

            FileHandler callback = (string path, out int length) =>
            {
                var data = fileHandler(path);

                if (data is null)
                {
                    length = 0;
                    return IntPtr.Zero;
                }

                length = data.Length;

                // WebUI will free automatically if allocated using its malloc method
                IntPtr webuiCopy = Utils.Malloc(new UIntPtr((uint)data.Length));
                Marshal.Copy(data, 0, webuiCopy, data.Length);

                return webuiCopy;
            };

            _fileHandler = callback;

            Natives.WebUISetFileHandler(_handle, callback);
        }
#pragma warning restore IDE0039

        /// <summary>
        /// Sets the root folder for this Window <br/>
        /// needs to be called before any <see cref="Window.Show(string)"/>
        /// </summary>
        /// <param name="folder">the root folder that WebUI should server files from for this Window</param>
        /// <exception cref="DirectoryNotFoundException">The specified folder does not exist or is invalid in another way</exception>
        /// <inheritdoc cref="ThrowIfDisposedOrInvalid"/>
        public void SetRootFolder(string folder)
        {
            ThrowIfDisposedOrInvalid();
            if (!Natives.WebUISetRootFolder(_handle, folder))
            {
                throw new DirectoryNotFoundException("Specified folder does not exist or is invalid in another way");
            }
        }

        /// <param name="folder">
        /// the root folder that WebUI should server files from for this Window
        /// represented as <see cref="DirectoryInfo"/>
        /// </param>
        /// <inheritdoc cref="SetRootFolder(string)"/>
        public void SetRootFolder(DirectoryInfo folder)
        {
            SetRootFolder(folder.FullName);
        }

        /// <param name="name">the name of the browser profile</param>
        /// <param name="path">the path to the browser profile</param>
        /// <remarks>
        /// The Browser stores the user data and state of the browser in the profile directory. <br/>
        /// If both <paramref name="name"/> and <paramref name="path"/> are empty, the default profile is used.
        /// </remarks>
        /// <inheritdoc cref="SetBrowserProfile(BrowserProfile)"/>
        public void SetBrowserProfile(string name, string path)
        {
            ThrowIfDisposedOrInvalid();
            Natives.WebUISetProfile(_handle, name, path);
        }

        /// <summary>
        /// Sets the browser profile for this Window <br/>
        /// needs to be called before any <see cref="Window.Show(string)"/>
        /// </summary>
        /// <remarks>
        /// The Browser stores the user data and state of the browser in the profile directory.
        /// </remarks>
        /// <param name="profile">the Browser profile to be used</param>
        /// <inheritdoc cref="ThrowIfDisposedOrInvalid"/>
        public void SetBrowserProfile(BrowserProfile profile)
        {
            SetBrowserProfile(profile.Name, profile.Path);
        }

        /// <summary>
        /// Deletes the browser profile for this Window <br/>
        /// </summary>
        public void DeleteBrowserProfile()
        {
            ThrowIfDisposedOrInvalid();
            Natives.WebUIDeleteProfile(_handle);
        }

        /// <summary>
        /// Sets the proxy to be used for this Window
        /// </summary>
        /// <param name="proxy">the proxy</param>
        /// <inheritdoc cref="ThrowIfDisposedOrInvalid"/>
        public void SetProxyServer(string proxy)
        {
            ThrowIfDisposedOrInvalid();
            Natives.WebUISetProxy(_handle, proxy);
        }

        /// <summary>
        /// Sets WebUI's JavaScript runtime
        /// </summary>
        /// <param name="runtime">The JavaScript runtime to use</param>
        /// <inheritdoc cref="ThrowIfDisposedOrInvalid"/>
        public void SetJavaScriptRuntime(Runtime runtime)
        {
            ThrowIfDisposedOrInvalid();
            Natives.WebUISetRuntime(_handle, runtime);
        }

        /// <summary>
        /// Gets the browsers main process
        /// </summary>
        /// <returns>the main process of the browser as <see cref="Process"/></returns>
        /// <inheritdoc cref="ThrowIfDisposedOrInvalid"/>
        public Process GetBrowserMainProcess()
        {
            ThrowIfDisposedOrInvalid();
            int processId = Convert.ToInt32(Natives.WebUIGetParentProcessId(_handle).ToUInt64());
            return Process.GetProcessById(processId);
        }

        /// <summary>
        /// Gets the last child process of the browser
        /// </summary>
        /// <returns>the last child process of the browser as <see cref="Process"/></returns>
        /// <inheritdoc cref="ThrowIfDisposedOrInvalid"/>
        public Process GetBrowserChildProcess()
        {
            ThrowIfDisposedOrInvalid();
            int processId = Convert.ToInt32(Natives.WebUIGetChildProcessId(_handle));
            return Process.GetProcessById(processId);
        }
#if NET7_0_OR_GREATER
        private static partial class Natives
        {
            [LibraryImport(Utils.LibraryName, StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_new_window")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial IntPtr WebUINewWindow();

            [LibraryImport(Utils.LibraryName, StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_new_window_id")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial IntPtr WebUINewWindow(IntPtr windowId);

            [LibraryImport(Utils.LibraryName, StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_get_new_window_id")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial IntPtr WebUIGetNewWindowId();

            [LibraryImport(Utils.LibraryName, StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_interface_get_window_id")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial IntPtr WebUICheckValidWindow(IntPtr windowId);

            [LibraryImport(Utils.LibraryName, StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_interface_bind")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial UIntPtr WebUIBind(WindowHandle windowHandle, string element,
                [MarshalAs(UnmanagedType.FunctionPtr)] EventCallback callback);

            [LibraryImport(Utils.LibraryName, StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_show")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            [return: MarshalAs(UnmanagedType.I1)]
            public static partial bool WebUIShow(WindowHandle windowHandle, string content);

            [LibraryImport(Utils.LibraryName, StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_show_browser")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            [return: MarshalAs(UnmanagedType.I1)]
            public static partial bool WebUIShow(WindowHandle windowHandle, string content, UIntPtr browser);

            public static bool WebUIShow(WindowHandle windowHandle, string content, Browser browser) =>
                WebUIShow(windowHandle, content, (UIntPtr)browser);

            [LibraryImport(Utils.LibraryName, StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_set_kiosk")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial void WebUISetKiosk(WindowHandle windowHandle,
                [MarshalAs(UnmanagedType.I1)] bool status);

            [LibraryImport(Utils.LibraryName, StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_destroy")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial void WebUIDestroy(IntPtr windowHandle);

            [LibraryImport(Utils.LibraryName, StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_close")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial void WebUIClose(WindowHandle windowHandle);

            [LibraryImport(Utils.LibraryName, StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_set_root_folder")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            [return: MarshalAs(UnmanagedType.I1)]
            public static partial bool WebUISetRootFolder(WindowHandle windowHandle, string folder);

            [LibraryImport(Utils.LibraryName, StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_set_file_handler")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial void WebUISetFileHandler(WindowHandle windowHandle,
                [MarshalAs(UnmanagedType.FunctionPtr)] FileHandler handler);

            [LibraryImport(Utils.LibraryName, StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_is_shown")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            [return: MarshalAs(UnmanagedType.I1)]
            public static partial bool WebUIWindowIsShown(WindowHandle windowHandle);

            [LibraryImport(Utils.LibraryName, StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_set_icon")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial void WebUISetIcon(WindowHandle windowHandle, string icon, string iconType);

            [LibraryImport(Utils.LibraryName, StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_set_icon")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial void WebUISetIcon(WindowHandle windowHandle,
                [MarshalAs(UnmanagedType.LPArray)] in byte[] icon, string iconType);

            [LibraryImport(Utils.LibraryName, StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_send_raw")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial void WebUISendRaw(WindowHandle windowHandle, string function,
                IntPtr data, UIntPtr length);

            public static void WebUISendRaw(WindowHandle windowHandle, string function,
                in byte[] data, UIntPtr length)
            {
                GCHandle? pinnedDataPointer = null;
                try
                {
                    pinnedDataPointer = GCHandle.Alloc(data, GCHandleType.Pinned);

                    // hack, but works
                    if (pinnedDataPointer is { } notNullPointer)
                    {
                        WebUISendRaw(windowHandle, function, notNullPointer.AddrOfPinnedObject(), length);
                    }
                }
                catch
                {
                    // If an exception throws it will most likely be from Marshal.Copy and there will be nothing to handle
                }
                finally
                {
                    pinnedDataPointer?.Free();
                }
            }

            [LibraryImport(Utils.LibraryName, StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_set_hide")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial void WebUISetHidden(WindowHandle windowHandle,
                [MarshalAs(UnmanagedType.I1)] bool status);

            [LibraryImport(Utils.LibraryName, StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_set_size")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial void WebUISetSize(WindowHandle windowHandle,
                [MarshalAs(UnmanagedType.U4)] uint width,
                [MarshalAs(UnmanagedType.U4)] uint height);

            [LibraryImport(Utils.LibraryName, StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_set_position")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial void WebUISetPosition(WindowHandle windowHandle,
                [MarshalAs(UnmanagedType.U4)] uint x,
                [MarshalAs(UnmanagedType.U4)] uint y);

            [LibraryImport(Utils.LibraryName, StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_set_profile")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial void WebUISetProfile(WindowHandle windowHandle, string name, string path);

            [LibraryImport(Utils.LibraryName, StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_set_proxy")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial void WebUISetProxy(WindowHandle windowHandle, string proxyServer);

            [LibraryImport(Utils.LibraryName, StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_get_url")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial string WebUIGetUrl(WindowHandle windowHandle);

            [LibraryImport(Utils.LibraryName, StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_set_public")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial void WebUISetPublic(WindowHandle windowHandle,
                [MarshalAs(UnmanagedType.I1)] bool status);

            [LibraryImport(Utils.LibraryName, StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_navigate")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial void WebUINavigate(WindowHandle windowHandle, string url);

            [LibraryImport(Utils.LibraryName, StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_delete_profile")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial void WebUIDeleteProfile(WindowHandle windowHandle);

            [LibraryImport(Utils.LibraryName, StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_get_parent_process_id")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial UIntPtr WebUIGetParentProcessId(WindowHandle windowHandle);

            [LibraryImport(Utils.LibraryName, StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_get_child_process_id")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial UIntPtr WebUIGetChildProcessId(WindowHandle windowHandle);

            [LibraryImport(Utils.LibraryName, StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_set_port")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            [return: MarshalAs(UnmanagedType.I1)]
            public static partial bool WebUISetPort(WindowHandle windowHandle, UIntPtr port);

            [LibraryImport(Utils.LibraryName, StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_run")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial void WebUIRun(WindowHandle windowHandle, string javaScript);

            [LibraryImport(Utils.LibraryName, StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_script")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            [return: MarshalAs(UnmanagedType.I1)]
            public static partial bool WebUIRun(WindowHandle windowHandle, string javaScript, UIntPtr timeout,
                IntPtr dataPointer, UIntPtr length);

            // LibraryImportAttribute seems to  not work with this kind of buffer, so i just allocate it manually
            public static bool WebUIRun(WindowHandle windowHandle, string javaScript, UIntPtr timeout,
                ref byte[] dataPointer, UIntPtr length)
            {
                IntPtr buffer = Utils.Malloc(length);
                try
                {
                    var result = WebUIRun(windowHandle, javaScript, timeout, buffer, length);

                    Marshal.Copy(buffer, dataPointer, 0, (int)length);

                    return result;
                }
                catch
                {
                    return false;
                }
                finally
                {
                    Utils.Free(buffer);
                }
            }

            [LibraryImport(Utils.LibraryName, StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_set_runtime")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial void WebUISetRuntime(WindowHandle windowHandle, UIntPtr runtime);

            public static void WebUISetRuntime(WindowHandle windowHandle, Runtime runtime) =>
                WebUISetRuntime(windowHandle, (UIntPtr)runtime);
        }
#else
        private static class Natives
        {
            [DllImport(Utils.LibraryName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_new_window")]
            public static extern IntPtr WebUINewWindow();

            [DllImport(Utils.LibraryName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_new_window_id")]
            public static extern IntPtr WebUINewWindow(IntPtr windowId);

            [DllImport(Utils.LibraryName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_get_new_window_id")]
            public static extern IntPtr WebUIGetNewWindowId();

            [DllImport(Utils.LibraryName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_interface_get_window_id")]
            public static extern IntPtr WebUICheckValidWindow(IntPtr windowId);

            [DllImport(Utils.LibraryName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_interface_bind")]
            public static extern UIntPtr WebUIBind(WindowHandle windowHandle, string element,
                [MarshalAs(UnmanagedType.FunctionPtr)] EventCallback callback);

            [DllImport(Utils.LibraryName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_show")]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern bool WebUIShow(WindowHandle windowHandle, string content);

            [DllImport(Utils.LibraryName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_show_browser")]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern bool WebUIShow(WindowHandle windowHandle, string content,
                [MarshalAs(UnmanagedType.U4)] Browser browser);

            [DllImport(Utils.LibraryName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_set_kiosk")]
            public static extern void WebUISetKiosk(WindowHandle windowHandle,
                [MarshalAs(UnmanagedType.I1)] bool status);

            [DllImport(Utils.LibraryName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_destroy")]
            public static extern void WebUIDestroy(IntPtr windowHandle);

            [DllImport(Utils.LibraryName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_close")]
            public static extern void WebUIClose(WindowHandle windowHandle);

            [DllImport(Utils.LibraryName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_set_root_folder")]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern bool WebUISetRootFolder(WindowHandle windowHandle, string folder);

            [DllImport(Utils.LibraryName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_set_file_handler")]
            public static extern void WebUISetFileHandler(WindowHandle windowHandle,
                [MarshalAs(UnmanagedType.FunctionPtr)] FileHandler handler);

            [DllImport(Utils.LibraryName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_is_shown")]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern bool WebUIWindowIsShown(WindowHandle windowHandle);

            [DllImport(Utils.LibraryName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_set_icon")]
            public static extern void WebUISetIcon(WindowHandle windowHandle, string icon, string iconType);

            [DllImport(Utils.LibraryName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_set_icon")]
            public static extern void WebUISetIcon(WindowHandle windowHandle,
                [MarshalAs(UnmanagedType.LPArray), In] byte[] icon, string iconType);

            [DllImport(Utils.LibraryName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_send_raw")]
            public static extern void WebUISendRaw(WindowHandle windowHandle, string function,
                [MarshalAs(UnmanagedType.LPArray), In] byte[] data, UIntPtr length);

            [DllImport(Utils.LibraryName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_set_hide")]
            public static extern void WebUISetHidden(WindowHandle windowHandle,
                [MarshalAs(UnmanagedType.I1)] bool status);

            [DllImport(Utils.LibraryName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_set_size")]
            public static extern void WebUISetSize(WindowHandle windowHandle,
                [MarshalAs(UnmanagedType.U4)] uint width,
                [MarshalAs(UnmanagedType.U4)] uint height);

            [DllImport(Utils.LibraryName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_set_position")]
            public static extern void WebUISetPosition(WindowHandle windowHandle,
                [MarshalAs(UnmanagedType.U4)] uint x,
                [MarshalAs(UnmanagedType.U4)] uint y);

            [DllImport(Utils.LibraryName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_set_profile")]
            public static extern void WebUISetProfile(WindowHandle windowHandle, string name, string path);

            [DllImport(Utils.LibraryName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_set_proxy")]
            public static extern void WebUISetProxy(WindowHandle windowHandle, string proxyServer);

            [DllImport(Utils.LibraryName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_get_url")]
            public static extern string WebUIGetUrl(WindowHandle windowHandle);

            [DllImport(Utils.LibraryName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_set_public")]
            public static extern void WebUISetPublic(WindowHandle windowHandle,
                [MarshalAs(UnmanagedType.I1)] bool status);

            [DllImport(Utils.LibraryName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_navigate")]
            public static extern void WebUINavigate(WindowHandle windowHandle, string url);

            [DllImport(Utils.LibraryName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_delete_profile")]
            public static extern void WebUIDeleteProfile(WindowHandle windowHandle);

            [DllImport(Utils.LibraryName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_get_parent_process_id")]
            public static extern UIntPtr WebUIGetParentProcessId(WindowHandle windowHandle);

            [DllImport(Utils.LibraryName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_get_child_process_id")]
            public static extern UIntPtr WebUIGetChildProcessId(WindowHandle windowHandle);

            [DllImport(Utils.LibraryName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_set_port")]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern bool WebUISetPort(WindowHandle windowHandle, UIntPtr port);

            [DllImport(Utils.LibraryName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_run")]
            public static extern void WebUIRun(WindowHandle windowHandle, string javaScript);

            [DllImport(Utils.LibraryName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_script")]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern bool WebUIRun(WindowHandle windowHandle, string javaScript, UIntPtr timeout,
                [MarshalAs(UnmanagedType.LPArray), Out] byte[] data, UIntPtr length);

            [DllImport(Utils.LibraryName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_set_runtime")]
            public static extern void WebUISetRuntime(WindowHandle windowHandle,
                [MarshalAs(UnmanagedType.U4)] Runtime runtime);
        }
#endif
    }
}
