/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER
#nullable enable
#endif

namespace WebUI
{
    public struct WindowProperties
    {
        public bool Fullscreen { get; set; }
        public bool Hidden { get; set; }

        public uint? Width { get; set; }
        public uint? Height { get; set; }

        public uint? X { get; set; }
        public uint? Y { get; set; }

        public uint? Port { get; set; }

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER
        public Icon? Icon { get; set; }

        public string? RootFolder { get; set; }

        public string? ProxyServer { get; set; }
#else
        public Icon Icon { get; set; }

        public string RootFolder { get; set; }

        public string ProxyServer { get; set; }
#endif
        public WindowProperties(bool fullscreen = false, bool hidden = false)
        {
            Fullscreen = fullscreen;
            Hidden = hidden;

            Width = null;
            Height = null;

            X = null;
            Y = null;

            Port = null;

            Icon = null;

            RootFolder = null;

            ProxyServer = null;
        }
    }
}
