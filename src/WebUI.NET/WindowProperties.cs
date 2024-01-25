/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

// Ignore Spelling: Fullscreen

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER
#nullable enable
#endif

namespace WebUI
{
    /// <summary>
    /// Class representing the properties of a <see cref="Window"/>
    /// </summary>
    public struct WindowProperties
    {
        /// <summary>
        /// Is the <see cref="Window"/> full screen
        /// </summary>
        public bool Fullscreen { get; set; }
        /// <summary>
        /// Is the <see cref="Window"/> hidden
        /// </summary>
        public bool Hidden { get; set; }

        /// <summary>
        /// The Width of the <see cref="Window"/><br/>
        /// <see cref="Height"/> needs to be set in order for this to work
        /// </summary>
        public uint? Width { get; set; }
        /// <summary>
        /// The Height of the <see cref="Window"/><br/>
        /// <see cref="Width"/> needs to be set in order for this to work
        /// </summary>
        public uint? Height { get; set; }

        /// <summary>
        /// The horizontal position of the <see cref="Window"/><br/>
        /// <see cref="Y"/> needs to be set in order for this to work
        /// </summary>
        public uint? X { get; set; }
        /// <summary>
        /// The vertical position of the <see cref="Window"/><br/>
        /// <see cref="X"/> needs to be set in order for this to work
        /// </summary>
        public uint? Y { get; set; }

        /// <summary>
        /// The Port the internal or external web server is running on
        /// </summary>
        public uint? Port { get; set; }

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER
        /// <summary>
        /// The Icon of the <see cref="Window"/>
        /// </summary>
        public Icon? Icon { get; set; }

        /// <summary>
        /// The root folder where the internal web server serves files
        /// </summary>
        public string? RootFolder { get; set; }

        /// <summary>
        /// Proxy server to be used by the <see cref="Window"/>
        /// </summary>
        public string? ProxyServer { get; set; }
#else
        /// <summary>
        /// The Icon of the <see cref="Window"/>
        /// </summary>
        public Icon Icon { get; set; }

        /// <summary>
        /// The root folder where the internal web server serves files
        /// </summary>
        public string RootFolder { get; set; }

        /// <summary>
        /// Proxy server to be used by the <see cref="Window"/>
        /// </summary>
        public string ProxyServer { get; set; }
#endif
        /// <summary>
        /// Creates new Window Properties
        /// </summary>
        /// <param name="fullscreen">should the <see cref="Window"/> be fullscreen</param>
        /// <param name="hidden">should the <see cref="Window"/> be hidden</param>
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
