/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

//TODO: Add Comments

using System;
using System.Runtime.InteropServices;

namespace WebUI.Events
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    internal struct Event
    {
        public UIntPtr WindowHandle;
        [MarshalAs(UnmanagedType.SysUInt)]
        public EventType Type;
        public string ElementId;
        public UIntPtr EventNumber;
        public UIntPtr BindId;
    }
}
