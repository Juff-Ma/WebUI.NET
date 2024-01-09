/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

//TODO: implement

using System;
using System.Runtime.InteropServices;

namespace WebUI
{
    public class Window
    {
#if NET7_0_OR_GREATER
        internal static partial class Natives
        {
            
        }
#else   
        internal static class Natives
        {

        }
#endif
    }
}