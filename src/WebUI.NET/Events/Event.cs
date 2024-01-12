/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

//TODO: Add Comments

using System;
using System.Runtime.InteropServices;

namespace WebUI.Events
{
    public class Event
    {
#if !NET7_0_OR_GREATER
        internal static partial class Natives
        {
            
        }
#else
        private static class Natives
        {
            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_interface_get_int_at")]
            [return: MarshalAs(UnmanagedType.I8)]
            public static extern long GetInt(UIntPtr windowHandle, UIntPtr eventId, UIntPtr index);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_interface_get_string_at")]
            public static extern string GetString(UIntPtr windowHandle, UIntPtr eventId, UIntPtr index);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_interface_get_bool_at")]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool GetBool(UIntPtr windowHandle, UIntPtr eventId, UIntPtr index);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_interface_get_size_at")]
            public static extern UIntPtr GetSize(UIntPtr windowHandle, UIntPtr eventId, UIntPtr index);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_interface_set_response")]
            public static extern void Return(UIntPtr windowHandle, UIntPtr eventId, string content);
        }
#endif
    }
}
