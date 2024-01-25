/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER
#nullable enable
#endif

using WebUI.Events;

namespace WebUI
{
    /// <summary>
    /// Class containing extension methods for <see cref="Window"/>
    /// </summary>
    public static class WindowExtensions
    {
        /// <summary>
        /// Applies the given <see cref="WindowProperties"/> to Window, should be called before <see cref="Window.Show(string)"/>
        /// </summary>
        /// <param name="window">The window to which the properties should be applied</param>
        /// <param name="properties">the <see cref="WindowProperties"/> to be applied</param>
        public static void ApplyWindowProperties(this Window window, WindowProperties properties)
        {
            window.SetFullscreen(properties.Fullscreen);
            window.SetHidden(properties.Hidden);

            if (properties.Width.HasValue && properties.Height.HasValue)
            {
                window.SetSize(properties.Width.Value, properties.Height.Value);
            }

            if (properties.X.HasValue && properties.Y.HasValue)
            {
                window.SetPosition(properties.X.Value, properties.Y.Value);
            }

            if (properties.Port.HasValue)
            {
                window.SetPort(properties.Port.Value);
            }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER
            if (properties.Icon is { })
            {
                window.SetIcon(properties.Icon);
            }
#else
            if (!(properties.Icon is null))
            {
                window.SetIcon(properties.Icon);
            }
#endif
            if (!string.IsNullOrEmpty(properties.RootFolder))
            {
                window.SetRootFolder(properties.RootFolder);
            }

            if (!string.IsNullOrEmpty(properties.ProxyServer))
            {
                window.SetProxyServer(properties.ProxyServer);
            }
        }

        /// <summary>
        /// Registers a new <see cref="DefaultEventHandler"/> containing C# friendly Events
        /// </summary>
        /// <param name="window">The window that the <see cref="DefaultEventHandler"/> should be registered for</param>
        /// <returns>the registered <see cref="DefaultEventHandler"/></returns>
        public static DefaultEventHandler RegisterDefaultEventHandler(this Window window)
        {
            var handler = new DefaultEventHandler();
            handler.UsedHandlerId = window.RegisterEventHandler(handler);

            return handler;
        }
    }
}