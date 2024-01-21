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
    public interface IEventHandler
    {
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER
        object? HandleEvent(Event @event, string element, ulong handlerId);
#else
        object HandleEvent(Event @event, string element, ulong handlerId);
#endif
    }
}
