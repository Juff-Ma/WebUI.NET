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
    public static class WindowExtensions
    {
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

            if (!string.IsNullOrEmpty(properties.RootFolder))
            {
                window.SetRootFolder(properties.RootFolder);
            }

            if (!string.IsNullOrEmpty(properties.ProxyServer))
            {
                window.SetProxyServer(properties.ProxyServer);
            }
#else
            if (!(properties.Icon is null))
            {
                window.SetIcon(properties.Icon);
            }

            if (!(properties.RootFolder is null) && properties.RootFolder == "")
            {
                window.SetRootFolder(properties.RootFolder);
            }

            if (!(properties.ProxyServer is null) && properties.ProxyServer == "")
            {
                window.SetProxyServer(properties.ProxyServer);
            }
#endif
        }

        public static DefaultEventHandler RegisterDefaultEventHandler(this Window window)
        {
            var handler = new DefaultEventHandler();
            handler.UsedHandlerId = window.RegisterEventHandler(handler);

            return handler;
        }
    }
}