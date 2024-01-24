/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

//TODO: Add Comments

namespace WebUI.Events
{
    public enum EventType : ulong
    {
        Disconnect = 0,
        Connect = 1,
        Click = 2,
        Navigation = 3,
        Callback = 4
    }
}
