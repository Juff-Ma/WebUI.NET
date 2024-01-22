/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER
#nullable enable
#endif

namespace WebUI.Events
{
    public sealed class DefaultEventHandler : IEventHandler
    {
        public delegate void DefaultCallback(Window window);
        public delegate void ClickCallback(Window window, string elementId);
        public delegate void NavigationCallback(Window window, string url);

        public ulong UsedHandlerId { get; set; }

        public DefaultEventHandler()
        {
            UsedHandlerId = 0;
        }

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER
        public object? HandleEvent(Event @event, string element, ulong handlerId)
#else
        public object HandleEvent(Event @event, string element, ulong handlerId)
#endif
        {
            if (handlerId != UsedHandlerId)
            {
                return null;
            }

            switch (@event.Type)
            {
                case EventType.Connect:
                    HandleConnect(@event.Window); break;
                case EventType.Disconnect:
                    HandleDisconnect(@event.Window); break;
                case EventType.Click:
                    HandleClick(@event.Window, element); break;
                case EventType.Navigation:
                    HandleNavigation(@event.Window, @event.GetString()); break;
            }

            return null;
        }

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER
        public event DefaultCallback? OnConnect;
        public event DefaultCallback? OnDisconnect;
        public event ClickCallback? OnClick;
        public event NavigationCallback? OnNavigation;

        private void HandleConnect(Window window)
        {
            if (OnConnect is { })
            {
                OnConnect(window);
            }
        }

        private void HandleDisconnect(Window window)
        {
            if (OnDisconnect is { })
            {
                OnDisconnect(window);
            }
        }

        private void HandleClick(Window window, string elementId)
        {
            if (OnClick is { })
            {
                OnClick(window, elementId);
            }
        }

        private void HandleNavigation(Window window, string url)
        {
            if (OnNavigation is { })
            {
                OnNavigation(window, url);
            }
        }
#else
        public event DefaultCallback OnConnect;
        public event DefaultCallback OnDisconnect;
        public event ClickCallback OnClick;
        public event NavigationCallback OnNavigation;

        private void HandleConnect(Window window)
        {
            if (!(OnConnect is null))
            {
                OnConnect(window);
            }
        }

        private void HandleDisconnect(Window window)
        {
            if (!(OnDisconnect is null))
            {
                OnDisconnect(window);
            }
        }

        private void HandleClick(Window window, string elementId)
        {
            if (!(OnClick is null))
            {
                OnClick(window, elementId);
            }
        }

        private void HandleNavigation(Window window, string url)
        {
            if (!(OnNavigation is null))
            {
                OnNavigation(window, url);
            }
        }
#endif  
    }
}

