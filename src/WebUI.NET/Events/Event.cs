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
    internal partial class Event
    {
#else
    internal class Event
    {
#endif
#if NET7_0_OR_GREATER
        private static partial class Natives
        {
            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_interface_get_int_at")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            [return: MarshalAs(UnmanagedType.I8)]
            public static partial long WebUIGetInt(UIntPtr windowHandle, UIntPtr eventId, UIntPtr index);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_interface_get_string_at")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial string WebUIGetString(UIntPtr windowHandle, UIntPtr eventId, UIntPtr index);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_interface_get_bool_at")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            [return: MarshalAs(UnmanagedType.U1)]
            public static partial bool WebUIGetBool(UIntPtr windowHandle, UIntPtr eventId, UIntPtr index);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_interface_get_size_at")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial UIntPtr WebUIGetSize(UIntPtr windowHandle, UIntPtr eventId, UIntPtr index);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_interface_set_response")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial void WebUIReturn(UIntPtr windowHandle, UIntPtr eventId, string content);
        }
#else
        private static class Natives
        {
            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_interface_get_int_at")]
            [return: MarshalAs(UnmanagedType.I8)]
            public static extern long WebUIGetInt(UIntPtr windowHandle, UIntPtr eventId, UIntPtr index);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_interface_get_string_at")]
            public static extern string WebUIGetString(UIntPtr windowHandle, UIntPtr eventId, UIntPtr index);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_interface_get_bool_at")]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool WebUIGetBool(UIntPtr windowHandle, UIntPtr eventId, UIntPtr index);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_interface_get_size_at")]
            public static extern UIntPtr WebUIGetSize(UIntPtr windowHandle, UIntPtr eventId, UIntPtr index);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_interface_set_response")]
            public static extern void WebUIReturn(UIntPtr windowHandle, UIntPtr eventId, string content);
        }
#endif
    }
}
