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
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct Event
    {
        private UIntPtr _windowHandle;
        [MarshalAs(UnmanagedType.SysUInt)]
        private EventType _type;
        private string _elementId;
        private UIntPtr _eventNumber;
        private UIntPtr _bindId;


#if NET7_0_OR_GREATER
        internal static partial class Natives
        {
            
        }
#else
        private static class Natives
        {
            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_get_int_at")]
            [return: MarshalAs(UnmanagedType.I8)]
            public static extern long GetInt(ref Event @event, UIntPtr index);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_get_int")]
            [return: MarshalAs(UnmanagedType.I8)]
            public static extern long GetInt(ref Event @event);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_get_string_at")]
            public static extern string GetString(ref Event @event, UIntPtr index);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_get_string")]
            public static extern string GetString(ref Event @event);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_get_bool_at")]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool GetBool(ref Event @event, UIntPtr index);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_get_bool")]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool GetBool(ref Event @event);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_get_size_at")]
            public static extern UIntPtr GetSize(ref Event @event, UIntPtr index);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_get_size")]
            public static extern UIntPtr GetSize(ref Event @event);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_return_int")]
            public static extern void Return(ref Event @event,
                [MarshalAs(UnmanagedType.I8)] long @int);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_return_string")]
            public static extern void Return(ref Event @event, string @string);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_return_bool")]
            public static extern void Return(ref Event @event,
                [MarshalAs(UnmanagedType.U1)] bool @bool);
        }
#endif
    }
}
