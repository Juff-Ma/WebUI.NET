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
    /// <summary>
    /// A default implementation of <see cref="IEventHandler"/> that exposes
    /// <see cref="EventType.Connect"/>, <see cref="EventType.Disconnect"/>, <see cref="EventType.Click"/> and <see cref="EventType.Navigation"/>
    /// as C# events
    /// </summary>
    public sealed class DefaultEventHandler : IEventHandler
    {
        /// <summary>
        /// Default callback with only <paramref name="window"/> as argument
        /// used for <see cref="OnConnect"/> and <see cref="OnDisconnect"/>
        /// </summary>
        /// <param name="window">the <see cref="Window"/> associated with the Event</param>
        public delegate void DefaultCallback(Window window);
        /// <summary>
        /// Callback for <see cref="OnClick"/> events
        /// </summary>
        /// <param name="window">the <see cref="Window"/> associated with the Event</param>
        /// <param name="elementId">the element the click originates from</param>
        public delegate void ClickCallback(Window window, string elementId);
        /// <summary>
        /// Callback used for <see cref="OnNavigation"/> events
        /// </summary>
        /// <param name="window">the <see cref="Window"/> associated with the Event</param>
        /// <param name="url">the URL the user/the app navigated to</param>
        public delegate void NavigationCallback(Window window, string url);

        /// <summary>
        /// The id returned by <see cref="Window.RegisterEventHandler(IEventHandler)"/> to be compared to <br/>
        /// Should not be set manually as <see cref="WindowExtensions.RegisterDefaultEventHandler(Window)"/> takes care of it
        /// </summary>
        public ulong UsedHandlerId { get; set; }

        /// <summary>
        /// Creates a new <see cref="DefaultEventHandler"/>
        /// </summary>
        public DefaultEventHandler()
        {
            UsedHandlerId = 0;
        }

        /// <inheritdoc />
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
        /// <summary>
        /// Window connect event
        /// </summary>
        public event DefaultCallback? OnConnect;
        /// <summary>
        /// Window disconnect event
        /// </summary>
        public event DefaultCallback? OnDisconnect;
        /// <summary>
        /// Window click event
        /// </summary>
        public event ClickCallback? OnClick;
        /// <summary>
        /// Window navigation event
        /// </summary>
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
        /// <summary>
        /// Window connect event
        /// </summary>
        public event DefaultCallback OnConnect;
        /// <summary>
        /// Window disconnect event
        /// </summary>
        public event DefaultCallback OnDisconnect;
        /// <summary>
        /// Window click event
        /// </summary>
        public event ClickCallback OnClick;
        /// <summary>
        /// Window navigation event
        /// </summary>
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