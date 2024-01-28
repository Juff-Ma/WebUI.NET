/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System;
#if NET7_0_OR_GREATER
using System.Runtime.CompilerServices;
#endif
using System.Runtime.InteropServices;

namespace WebUI.Events
{
    /// <summary>
    /// Class representing a WebUI event
    /// </summary>
#if NET7_0_OR_GREATER
    public sealed partial class Event
    {
#else
    public sealed class Event
    {
#endif
        private readonly IntPtr _windowId;
        private readonly UIntPtr _eventId;

        /// <summary>
        /// The type of the event
        /// </summary>
        public EventType Type { get; }

        internal Event(IntPtr windowId, UIntPtr eventId, EventType type)
        {
            _windowId = windowId;
            _eventId = eventId;

            Type = type;
        }

        /// <summary>
        /// Gets the associated <see cref="WebUI.Window"/>
        /// </summary>
        // Why Visual Studio? Why? (VS will show messages if i don't do this)
#if NET5_0_OR_GREATER
        public Window Window
        {
            get => new(_windowId, false);
        }
#else
        public Window Window
        {
            get => new Window(_windowId, false);
        }
#endif

        /// <summary>
        /// Throws if the <see cref="WebUI.Window"/> handle is invalid
        /// </summary>
        /// <exception cref="InvalidOperationException">The native Window handle was invalid</exception>
        private void ThrowOnInvalidHandle()
        {
            if (!Window.IsHandleValid(_windowId))
            {
                throw new InvalidOperationException("Window handle is invalid.");
            }
        }

        /// <summary>
        /// Gets the a string argument from the event
        /// </summary>
        /// <returns>the string</returns>
        /// <inheritdoc cref="ThrowOnInvalidHandle"/>
        public string GetString() => GetString(0U);

        /// <inheritdoc cref="GetString()"/>
        /// <param name="index">The index of the argument</param>
        public string GetString(uint index)
        {
            ThrowOnInvalidHandle();

            // doing this so automatic marshal doesn't free the pointer cause that causes WebUI to shit itself
            var ptr = Natives.WebUIGet(_windowId, _eventId, new UIntPtr(index));

            // using try-catch as this could lead to an overflow if it isn't a string
            try
            {
                return Marshal.PtrToStringAnsi(ptr);
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets a number argument from the event
        /// </summary>
        /// <returns>the number as <see cref="long"/></returns>
        /// <inheritdoc cref="ThrowOnInvalidHandle"/>
        public long GetNumber() => GetNumber(0U);

        /// <inheritdoc cref="GetNumber()"/>
        /// <param name="index">The index of the argument</param>
        public long GetNumber(uint index)
        {
            ThrowOnInvalidHandle();
            return Natives.WebUIGetInt(_windowId, _eventId, new UIntPtr(index));
        }

        /// <summary>
        /// Gets a boolean argument from the event
        /// </summary>
        /// <returns>the boolean</returns>
        /// <inheritdoc cref="ThrowOnInvalidHandle"/>
        public bool GetBool() => GetBool(0U);

        /// <inheritdoc cref="GetBool()"/>
        /// <param name="index">The index of the argument</param>
        public bool GetBool(uint index)
        {
            ThrowOnInvalidHandle();
            return Natives.WebUIGetBool(_windowId, _eventId, new UIntPtr(index));
        }

        /// <summary>
        /// Gets the raw data of an argument from the event
        /// </summary>
        /// <returns>the data as <see cref="byte"/> array</returns>
        /// <inheritdoc cref="ThrowOnInvalidHandle"/>
        public byte[] GetRaw() => GetRaw(0U);

        /// <inheritdoc cref="GetRaw()"/>
        /// <param name="index">The index of the argument</param>
        public byte[] GetRaw(uint index)
        {
            ThrowOnInvalidHandle();

            var uindex = new UIntPtr(index);
            ulong size = Natives.WebUIGetSize(_windowId, _eventId, uindex).ToUInt64();

            byte[] buffer = new byte[size];
            var ptr = Natives.WebUIGet(_windowId, _eventId, uindex);

            try
            {
                Marshal.Copy(ptr, buffer, 0, (int)size);
            }
            catch
            {
                // if this happens size is probably to big for an Int32
                // so we just return an empty buffer
            }

            return buffer;
        }

        internal void ReturnValue(string value)
        {
            ThrowOnInvalidHandle();
            Natives.WebUIReturn(_windowId, _eventId, value);
        }

#if NET7_0_OR_GREATER
        private static partial class Natives
        {
            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_interface_get_int_at")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            [return: MarshalAs(UnmanagedType.I8)]
            public static partial long WebUIGetInt(IntPtr windowHandle, UIntPtr eventId, UIntPtr index);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_interface_get_string_at")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial IntPtr WebUIGet(IntPtr windowHandle, UIntPtr eventId, UIntPtr index);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_interface_get_bool_at")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            [return: MarshalAs(UnmanagedType.U1)]
            public static partial bool WebUIGetBool(IntPtr windowHandle, UIntPtr eventId, UIntPtr index);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_interface_get_size_at")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial UIntPtr WebUIGetSize(IntPtr windowHandle, UIntPtr eventId, UIntPtr index);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_interface_set_response")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial void WebUIReturn(IntPtr windowHandle, UIntPtr eventId, string content);
        }
#else
        private static class Natives
        {
            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_interface_get_int_at")]
            [return: MarshalAs(UnmanagedType.I8)]
            public static extern long WebUIGetInt(IntPtr windowHandle, UIntPtr eventId, UIntPtr index);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_interface_get_string_at")]
            public static extern IntPtr WebUIGet(IntPtr windowHandle, UIntPtr eventId, UIntPtr index);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_interface_get_bool_at")]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool WebUIGetBool(IntPtr windowHandle, UIntPtr eventId, UIntPtr index);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_interface_get_size_at")]
            public static extern UIntPtr WebUIGetSize(IntPtr windowHandle, UIntPtr eventId, UIntPtr index);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_interface_set_response")]
            public static extern void WebUIReturn(IntPtr windowHandle, UIntPtr eventId, string content);
        }
#endif
    }
}
