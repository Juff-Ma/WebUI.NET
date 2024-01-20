/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System.IO;

namespace WebUI
{
    public struct BrowserProfile
    {
        public string Name { get; set; }
        public string Path { get; set; }

        public BrowserProfile(string name, string path)
        {
            Name = name;
            Path = path;
        }

        public BrowserProfile(string name, DirectoryInfo path)
        {
            Name = name;
            Path = path.FullName;
        }
    }
}
