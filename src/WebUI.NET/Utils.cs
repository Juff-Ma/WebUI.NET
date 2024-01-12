/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

//TODO: Add Comments

// Ignore Spelling: Utils Malloc Tls Pem App

using System;
#if NET7_0_OR_GREATER
using System.Runtime.CompilerServices;
#endif
using System.Runtime.InteropServices;

namespace WebUI.NET
{
#if NET7_0_OR_GREATER
    public static partial class Utils
    {
#else
    public static class Utils
    {
#endif
#if NET7_0_OR_GREATER
        private static partial class Natives
        {
            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_wait")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial void Wait();

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_exit")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial void Exit();

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_clean")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial void Clean();

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_delete_all_profiles")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial void DeleteProfiles();

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_set_default_root_folder")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            [return: MarshalAs(UnmanagedType.I1)]
            public static partial bool SetDefaultRootFolder(string folder);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_set_timeout")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial void SetTimeout(UIntPtr seconds);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_encode")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial string Encode(string content);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_decode")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial string Decode(string content);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_malloc")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial IntPtr Malloc(UIntPtr size);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_free")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial void Free(IntPtr pointer);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_set_tls_certificate")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            [return: MarshalAs(UnmanagedType.I1)]
            public static partial bool SetTlsCertificate(string certPem, string privateKeyPem);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_interface_is_app_running")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            [return: MarshalAs(UnmanagedType.I1)]
            public static partial bool IsAppRunning();
        }
#else
        private static class Natives
        {
            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_wait")]
            public static extern void Wait();

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_exit")]
            public static extern void Exit();

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_clean")]
            public static extern void Clean();

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_delete_all_profiles")]
            public static extern void DeleteProfiles();

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_set_default_root_folder")]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern bool SetDefaultRootFolder(string folder);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_set_timeout")]
            public static extern void SetTimeout(UIntPtr seconds);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_encode")]
            public static extern string Encode(string content);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_decode")]
            public static extern string Decode(string content);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_malloc")]
            public static extern IntPtr Malloc(UIntPtr size);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_free")]
            public static extern void Free(IntPtr pointer);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_set_tls_certificate")]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern bool SetTlsCertificate(string certPem, string privateKeyPem);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_interface_is_app_running")]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern bool IsAppRunning();
        }
#endif

    }
}
