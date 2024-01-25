/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

namespace WebUI
{
    /// <summary>
    /// Enum representing possible WebUI JavaScript runtimes
    /// </summary>
    public enum Runtime : uint
    {
        /// <summary>
        /// No runtime at all
        /// </summary>
        None = 0,
        /// <summary>
        /// The Deno JavaScript runtime
        /// </summary>
        Deno = 1,
        /// <summary>
        /// The NodeJS JavaScript runtime
        /// </summary>
        NodeJS = 2
    }
}
