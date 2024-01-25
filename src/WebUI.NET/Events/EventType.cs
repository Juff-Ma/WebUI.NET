/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

namespace WebUI.Events
{
    /// <summary>
    /// Possible event types
    /// </summary>
    public enum EventType : ulong
    {
        /// <summary>
        /// Window Disconnected
        /// </summary>
        Disconnect = 0,
        /// <summary>
        /// Window Connected
        /// </summary>
        Connect = 1,
        /// <summary>
        /// User Clicked
        /// </summary>
        Click = 2,
        /// <summary>
        /// User navigated use <see cref="Event.GetString()"/> to get the URL navigated to
        /// </summary>
        Navigation = 3,
        /// <summary>
        /// Callback from JavaScript to C# code
        /// </summary>
        Callback = 4
    }
}
