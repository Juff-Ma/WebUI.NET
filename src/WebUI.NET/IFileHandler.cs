/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER
#nullable enable
#endif

namespace WebUI
{
    /// <summary>
    /// Interface representing a handler for dynamic files/content, should not be called manually
    /// </summary>
    public interface IFileHandler
    {
        /// <summary>
        /// Handler for dynamic files
        /// </summary>
        /// <param name="path">the path of the file currently being loaded starting with an initial '/'</param>
        /// <returns>
        /// a <see cref="byte"/> array if the file is dynamic containing the contents of the file
        /// otherwise returns null
        /// </returns>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER
        byte[]? GetFile(string path);
#else
        byte[] GetFile(string path);
#endif
    }
}
