/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

//TODO: Add Comments

using System;
#if NET7_0_OR_GREATER
using System.Runtime.CompilerServices;
#endif
using System.Runtime.InteropServices;

namespace WebUI.Events
{
#if NET7_0_OR_GREATER
    public sealed partial class Event
    {
#else
    public sealed class Event
    {
#endif
        private IntPtr _windowId;
        private UIntPtr _eventId;

        public EventType Type { get; }

        internal Event(IntPtr windowId, UIntPtr eventId, EventType type)
        {
            _windowId = windowId;
            _eventId = eventId;

            Type = type;
        }

        public Window Window
        {
            get => new Window(_windowId, false);
        }

        public string GetString() => GetString(0U);

        public string GetString(uint index)
        {
            return Natives.WebUIGetString(_windowId, _eventId, new UIntPtr(index));
        }

        public long GetNumber() => GetNumber(0U);

        public long GetNumber(uint index)
        {
            return Natives.WebUIGetInt(_windowId, _eventId, new UIntPtr(index));
        }

        public bool GetBool() => GetBool(0U);

        public bool GetBool(uint index)
        {
            return Natives.WebUIGetBool(_windowId, _eventId, new UIntPtr(index));
        }

        public ulong GetSize() => GetSize(0U);

        public ulong GetSize(uint index)
        {
            return Natives.WebUIGetSize(_windowId, _eventId, new UIntPtr(index)).ToUInt64();
        }

        internal void ReturnValue(string value)
        {
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
            public static partial string WebUIGetString(IntPtr windowHandle, UIntPtr eventId, UIntPtr index);

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
            public static extern string WebUIGetString(IntPtr windowHandle, UIntPtr eventId, UIntPtr index);

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
