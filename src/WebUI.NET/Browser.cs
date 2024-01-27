/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

namespace WebUI
{
    /// <summary>
    /// Enum representing the Browser to be opened
    /// </summary>
    public enum Browser : uint
    {
        /// <summary>
        /// Open no Browser window
        /// </summary>
        /// <remarks>
        /// This is useful for testing or if you want to build a semi-network application
        /// </remarks>
        NoBrowser = 0,
        /// <summary>
        /// Open the browser recommended by WebUI
        /// </summary>
        AnyBrowser = 1,
        /// <summary>
        /// Open a Chrome window
        /// </summary>
        Chrome = 2,
        /// <summary>
        /// Open a Firefox window
        /// </summary>
        Firefox = 3,
        /// <summary>
        /// Open an Edge window
        /// </summary>
        Edge = 4,
        /// <summary>
        /// Open a Safari window
        /// </summary>
        /// <remarks>
        /// currently only works in a normal browser window and not app mode and is therefore not really recommended
        /// </remarks>
        Safari = 5,
        /// <summary>
        /// Open a Chromium window
        /// </summary>
        Chromium = 6,
        /// <summary>
        /// Opera browser, currently not implemented
        /// </summary>
        /// <remarks>
        /// Will still open as fall-back in a normal Window if no other browser is available (like every unsupported browser),
        /// this however is buggy and not recommended. <br/>
        /// However, this option will still not work, user should use <see cref="AnyBrowser"/> instead
        /// </remarks>
        Opera = 7,
        /// <summary>
        /// Open a Brave window
        /// </summary>
        Brave = 8,
        /// <summary>
        /// Open a Vivaldi window
        /// </summary>
        Vivaldi = 9,
        /// <summary>
        /// Open an Epic browser window
        /// </summary>
        Epic = 10,
        /// <summary>
        /// Open a Yandex window
        /// </summary>
        Yandex = 11,
        /// <summary>
        /// Open a window of a recommended Chromium based browser
        /// </summary>
        ChromiumBased = 12
    }
}
