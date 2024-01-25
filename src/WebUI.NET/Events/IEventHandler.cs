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
    /// Interface representing an event handler
    /// </summary>
    public interface IEventHandler
    {
        /// <summary>
        /// Event handler method, should not be called manually
        /// </summary>
        /// <param name="event">the <see cref="Event"/> to be handled</param>
        /// <param name="element">the element that caused the Event, empty string if generic event</param>
        /// <param name="handlerId">the id of the requested handler, 
        /// should be checked if equal with id retrieved from <see cref="Window.RegisterEventHandler(IEventHandler)"/></param>
        /// <returns>the return value if <see cref="Event.Type"/> equals <see cref="EventType.Callback"/>.
        /// If no return value or other event type returns null</returns>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER
        object? HandleEvent(Event @event, string element, ulong handlerId);
#else
        object HandleEvent(Event @event, string element, ulong handlerId);
#endif
    }
}
