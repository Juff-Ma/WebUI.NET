/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

// Ignore Spelling: Utils Malloc Tls Pem App

using System;
using System.IO;

#if NET5_0_OR_GREATER
#nullable enable
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
#endif

#if NET7_0_OR_GREATER
using System.Runtime.CompilerServices;
#endif
using System.Runtime.InteropServices;

namespace WebUI
{
    /// <summary>
    /// Utility Class for non specific functionality
    /// </summary>
#if NET7_0_OR_GREATER
    public static partial class Utils
    {
#else
    public static class Utils
    {
#endif
        /// <summary>
        /// Waits for all Windows to close or for <see cref="ForceExit()"/> to be called
        /// </summary>
        public static void WaitForExit()
        {
            Natives.WebUIWait();
        }

        /// <summary>
        /// Closes all Windows
        /// </summary>
        public static void ForceExit()
        {
            Natives.WebUIExit();
        }

        /// <summary>
        /// Frees all memory resources used by WebUI, should be called after <see cref="WaitForExit()"/> and <see cref="DeleteAllProfiles()"/>
        /// </summary>
        public static void Clean()
        {
            Natives.WebUIClean();
        }

        /// <summary>
        /// Sets the maximum time WebUI will wait for the browser to startz
        /// </summary>
        /// <param name="seconds">The time in seconds</param>
        public static void SetStartupTimeout(uint seconds)
        {
            Natives.WebUISetTimeout(new UIntPtr(seconds));
        }

        /// <inheritdoc cref="SetStartupTimeout(uint)"/>
        /// <param name="timeout"><see cref="TimeSpan"/> representing the timeout, all units smaller than seconds will be ignored</param>
        public static void SetStartupTimeout(TimeSpan timeout)
        {
            SetStartupTimeout((uint)timeout.TotalSeconds);
        }

        /// <summary>
        /// Sets the default root folder to use for all Windows,
        /// should be called before any <see cref="Window.Show(string)"/> call
        /// </summary>
        /// <param name="folder">the folder to set the global root to</param>
        /// <exception cref="DirectoryNotFoundException">The specified folder does not exist or is invalid in another way</exception>
        public static void SetGlobalRootFolder(string folder)
        {
            if (!Natives.WebUISetDefaultRootFolder(folder))
            {
                throw new DirectoryNotFoundException("Specified folder does not exist or is invalid in another way");
            }
        }

        /// <param name="folder"><see cref="DirectoryInfo"/> containing the path to the new global root folder</param>
        /// <inheritdoc cref="SetGlobalRootFolder(string)"/>
        public static void SetGlobalRootFolder(DirectoryInfo folder)
        {
            SetGlobalRootFolder(folder.FullName);
        }

        /// <summary>
        /// Deletes all WebUI browser profiles
        /// </summary>
        public static void DeleteAllProfiles()
        {
            Natives.WebUIDeleteProfiles();
        }

        /// <summary>
        /// Native Base64 encoder from WebUI provided as a fall-back
        /// </summary>
        /// <param name="string">The <see cref="string"/> to be encoded</param>
        /// <returns>The encoded <see cref="string"/></returns>
        public static string EncodeBase64(string @string)
        {
            return Natives.WebUIEncode(@string);
        }

        /// <summary>
        /// Native Base64 decoder from WebUI provided as a fall-back
        /// </summary>
        /// <param name="string">The <see cref="string"/> to be decoded</param>
        /// <returns>The decoded <see cref="string"/></returns>
        public static string DecodeBase64(string @string)
        {
            return Natives.WebUIDecode(@string);
        }

        /// <summary>
        /// Internal allocator for WebUI, WebUI will automatically free if used correcly
        /// </summary>
        /// <param name="size">Size in bytes to be allocated</param>
        /// <returns>pointer to allocated memory</returns>
        internal static IntPtr Malloc(UIntPtr size)
        {
            return Natives.WebUIMalloc(size);
        }

        /// <summary>
        /// Frees WebUI memory
        /// </summary>
        /// <param name="ptr">pointer to allocated memory</param>
        internal static void Free(IntPtr ptr)
        {
            Natives.WebUIFree(ptr);
        }

        /// <summary>
        /// Checks if any Windows are currently open
        /// </summary>
        /// <returns><c>true</c> if at least 1 window is open otherwise <c>false</c></returns>
        public static bool AreWindowsOpen()
        {
            return Natives.WebUIIsAppRunning();
        }

        /// <summary>
        /// Sets a certificate for WebUI to use for secure connections. Must be called before <see cref="Window.Show(string)"/>
        /// </summary>
        /// <remarks>
        /// requires secure natives and will do nothing with normal natives.
        /// if not called when using secure natives WebUI will generate a self signed certificate
        /// </remarks>
        /// <param name="certificatePem">File name or contents of the public key/certificate in PEM format</param>
        /// <param name="privateKeyPem">File name or contents of the private key in unencrypted PEM format</param>
        /// <param name="loadFromFile">Set to <c>true</c> if contents should be loaded from file (default)
        /// or <c>false</c> if contents should be loaded from <paramref name="certificatePem"/> and <paramref name="privateKeyPem"/> directly
        /// </param>
        /// <returns><c>true</c> if successfully set or <c>false</c> if there was an Error</returns>
        public static bool SetCertificate(string certificatePem, string privateKeyPem, bool loadFromFile = true)
        {
            if (!loadFromFile)
            {
                return Natives.WebUISetTlsCertificate(certificatePem, privateKeyPem);
            }

            return Natives.WebUISetTlsCertificate(File.ReadAllText(certificatePem), File.ReadAllText(privateKeyPem));
        }

        /// <param name="certificatePem"><see cref="FileInfo"/> containing the path to the public key/certificate in PEM format</param>
        /// <param name="privateKeyPem"><see cref="FileInfo"/> containing the path to the private key in unencrypted PEM format</param>
        /// <inheritdoc cref="SetCertificate(string, string, bool)"/>
        public static bool SetCertificate(FileInfo certificatePem, FileInfo privateKeyPem)
        {
            return SetCertificate(certificatePem.FullName, privateKeyPem.FullName);
        }

#if NET5_0_OR_GREATER

        /// <param name="certificate">Unencrypted certificate as <see cref="X509Certificate2"/></param>
        /// <inheritdoc cref="SetCertificate(string, string, bool)"/>
        public static bool SetCertificate(X509Certificate2 certificate)
        {
            var key = GetKeyFromCertificate(certificate);

            if (key is null)
            {
                return false;
            }

            return SetCertificate(key);
        }

        /// <param name="password">Password to the <see cref="X509Certificate2"/></param>
        /// <param name="parameters"><see cref="PbeParameters"/> for decryption of certificate</param>
        /// <param name="certificate">Certificate as <see cref="X509Certificate2"/></param>
        /// <inheritdoc cref="SetCertificate(string, string, bool)"/>
        public static bool SetCertificate(X509Certificate2 certificate, ReadOnlySpan<byte> password, PbeParameters parameters)
        {
            var key = GetKeyFromCertificate(certificate);

            if (key is null)
            {
                return false;
            }

            return SetCertificate(key, password, parameters);
        }

        /// <inheritdoc cref="SetCertificate(X509Certificate2, ReadOnlySpan{byte}, PbeParameters)"/>
        public static bool SetCertificate(X509Certificate2 certificate, ReadOnlySpan<char> password, PbeParameters parameters)
        {
            var key = GetKeyFromCertificate(certificate);

            if (key is null)
            {
                return false;
            }

            return SetCertificate(key, password, parameters);
        }
#if NET7_0_OR_GREATER
        private static bool SetCertificate(AsymmetricAlgorithm key, ReadOnlySpan<byte> password, PbeParameters parameters)
        {
            try
            {
#if NET8_0_OR_GREATER
                string privateKey = key.ExportEncryptedPkcs8PrivateKeyPem(password, parameters);
#else
                byte[] privateBytes = key.ExportEncryptedPkcs8PrivateKey(password, parameters);
                string privateKey = PemEncoding.WriteString("PRIVATE KEY", privateBytes);
#endif
                string publicKey = key.ExportSubjectPublicKeyInfoPem();
                return SetCertificate(publicKey, privateKey, false);
            }
            catch (CryptographicException)
            {
                return false;
            }
        }

        private static bool SetCertificate(AsymmetricAlgorithm key, ReadOnlySpan<char> password, PbeParameters parameters)
        {
            try
            {
                string privateKey = key.ExportEncryptedPkcs8PrivateKeyPem(password, parameters);
                string publicKey = key.ExportSubjectPublicKeyInfoPem();
                return SetCertificate(publicKey, privateKey, false);
            }
            catch (CryptographicException)
            {
                return false;
            }
        }

        private static bool SetCertificate(AsymmetricAlgorithm key)
        {
            try
            {
                string privateKey = key.ExportPkcs8PrivateKeyPem();
                string publicKey = key.ExportSubjectPublicKeyInfoPem();
                return SetCertificate(publicKey, privateKey, false);
            }
            catch (CryptographicException)
            {
                return false;
            }
        }
#else
        private static bool SetCertificate(AsymmetricAlgorithm key, ReadOnlySpan<byte> password, PbeParameters parameters)
        {
            try
            {
                byte[] privateBytes = key.ExportEncryptedPkcs8PrivateKey(password, parameters);
                byte[] publicBytes = key.ExportSubjectPublicKeyInfo();
                return SetCertificate(privateBytes, publicBytes);
            }
            catch (CryptographicException)
            {
                return false;
            }
        }

        private static bool SetCertificate(AsymmetricAlgorithm key, ReadOnlySpan<char> password, PbeParameters parameters)
        {
            try
            {
                byte[] privateBytes = key.ExportEncryptedPkcs8PrivateKey(password, parameters);
                byte[] publicBytes = key.ExportSubjectPublicKeyInfo();
                return SetCertificate(privateBytes, publicBytes);
            }
            catch (CryptographicException)
            {
                return false;
            }
        }

        private static bool SetCertificate(AsymmetricAlgorithm key)
        {
            try
            {
                byte[] privateBytes = key.ExportPkcs8PrivateKey();
                byte[] publicBytes = key.ExportSubjectPublicKeyInfo();
                return SetCertificate(privateBytes, publicBytes);
            }
            catch (CryptographicException)
            {
                return false;
            }
        }

        private static bool SetCertificate(ReadOnlySpan<byte> privateBytes, ReadOnlySpan<byte> publicBytes)
        {
            string privateKey = new(PemEncoding.Write("PRIVATE KEY", privateBytes));
            string publicKey = new(PemEncoding.Write("PUBLIC KEY", publicBytes));

            return SetCertificate(publicKey, privateKey, false);
        }
#endif
        private static AsymmetricAlgorithm? GetKeyFromCertificate(X509Certificate2 certificate)
        {
            // returns the first found key format or null if none is found
            return (AsymmetricAlgorithm?)certificate.GetRSAPrivateKey() ??
                (AsymmetricAlgorithm?)certificate.GetECDsaPrivateKey() ??
                (AsymmetricAlgorithm?)certificate.GetECDiffieHellmanPrivateKey() ??
                certificate.GetDSAPrivateKey();
        }
#endif
#if NET7_0_OR_GREATER
        private static partial class Natives
        {
            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_wait")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial void WebUIWait();

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_exit")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial void WebUIExit();

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_clean")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial void WebUIClean();

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_delete_all_profiles")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial void WebUIDeleteProfiles();

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_set_default_root_folder")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            [return: MarshalAs(UnmanagedType.I1)]
            public static partial bool WebUISetDefaultRootFolder(string folder);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_set_timeout")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial void WebUISetTimeout(UIntPtr seconds);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_encode")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial string WebUIEncode(string content);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_decode")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial string WebUIDecode(string content);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_malloc")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial IntPtr WebUIMalloc(UIntPtr size);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_free")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            public static partial void WebUIFree(IntPtr pointer);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_set_tls_certificate")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            [return: MarshalAs(UnmanagedType.I1)]
            public static partial bool WebUISetTlsCertificate(string certPem, string privateKeyPem);

            [LibraryImport("webui-2", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "webui_interface_is_app_running")]
            [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
            [return: MarshalAs(UnmanagedType.I1)]
            public static partial bool WebUIIsAppRunning();
        }
#else
        private static class Natives
        {
            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_wait")]
            public static extern void WebUIWait();

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_exit")]
            public static extern void WebUIExit();

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_clean")]
            public static extern void WebUIClean();

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_delete_all_profiles")]
            public static extern void WebUIDeleteProfiles();

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_set_default_root_folder")]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern bool WebUISetDefaultRootFolder(string folder);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_set_timeout")]
            public static extern void WebUISetTimeout(UIntPtr seconds);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_encode")]
            public static extern string WebUIEncode(string content);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_decode")]
            public static extern string WebUIDecode(string content);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_malloc")]
            public static extern IntPtr WebUIMalloc(UIntPtr size);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_free")]
            public static extern void WebUIFree(IntPtr pointer);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_set_tls_certificate")]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern bool WebUISetTlsCertificate(string certPem, string privateKeyPem);

            [DllImport("webui-2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                ThrowOnUnmappableChar = false, BestFitMapping = false,
                EntryPoint = "webui_interface_is_app_running")]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern bool WebUIIsAppRunning();
        }
#endif

    }
}
