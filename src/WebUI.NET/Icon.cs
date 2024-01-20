/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System;

namespace WebUI
{
    public sealed class Icon
    {
        public enum IconType
        {
            Svg,
            Png,
            Jpeg,
            Ico,
            Gif
        }

        internal byte[] Data { get; }
        internal string SvgData { get; }

        public IconType Type { get; }

        public Icon(byte[] data, IconType iconType)
        {
            Type = iconType;
            Data = data;
        }

        public Icon(string svgString)
        {
            SvgData = svgString;
        }

        internal static string IconTypeToMimeType(IconType type)
        {
            switch (type)
            {
                case IconType.Svg:
                    return "image/svg+xml";
                case IconType.Png:
                    return "image/png";
                case IconType.Jpeg:
                    return "image/jpeg";
                case IconType.Ico:
                    return "image/x-icon";
                case IconType.Gif:
                    return "image/gif";
                default:
                    throw new ArgumentException("IconType has to exist", nameof(type));
            }
        }
    }
}
