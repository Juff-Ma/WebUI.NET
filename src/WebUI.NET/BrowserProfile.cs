﻿/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System.IO;

namespace WebUI
{
    /// <summary>
    /// Represents a browser profile for storing the Information generated by WebUI
    /// </summary>
    public struct BrowserProfile
    {
        /// <summary>
        /// The Name of the Profile
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The path of the Profile
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Creates a new Browser Profile with its <paramref name="name"/> and <paramref name="path"/>
        /// </summary>
        /// <param name="name">a <see cref="string"/> representing the name of the Profile</param>
        /// <param name="path">the path of the Profile</param>
        public BrowserProfile(string name, string path)
        {
            Name = name;
            Path = path;
        }

        /// <summary>
        /// Creates a new Browser Profile with its <paramref name="name"/> and <paramref name="path"/>
        /// </summary>
        /// <param name="name">a <see cref="string"/> representing the name of the Profile</param>
        /// <param name="path">the path of the Profile represented as a <see cref="DirectoryInfo"/></param>
        public BrowserProfile(string name, DirectoryInfo path)
        {
            Name = name;
            Path = path.FullName;
        }
    }
}
