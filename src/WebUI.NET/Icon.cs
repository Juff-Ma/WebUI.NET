/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

// Ignore Spelling: svg

using System;

namespace WebUI
{
    /// <summary>
    /// Class representing a Window Icon (favicon) of a WebUI Window
    /// </summary>
    public sealed class Icon
    {
        /// <summary>
        /// possible formats of an icon
        /// </summary>
        public enum IconType
        {
            /// <summary>
            /// Text/XML based SVG icon
            /// </summary>
            Svg,
            /// <summary>
            /// PNG icon
            /// </summary>
            Png,
            /// <summary>
            /// JPEG/JPG icon
            /// </summary>
            Jpeg,
            /// <summary>
            /// Windows ICO/x-icon
            /// </summary>
            Ico,
            /// <summary>
            /// GIF icon
            /// </summary>
            Gif
        }

        internal byte[] Data { get; }
        internal string SvgData { get; }

        /// <summary>
        /// Format used by the Icon represented as <see cref="IconType"/>
        /// </summary>
        public IconType Type { get; }

        /// <summary>
        /// creates a new <see cref="Icon"/> with the given <paramref name="data"/> and <see cref="IconType"/>
        /// </summary>
        /// <param name="data">the binary data of the icon</param>
        /// <param name="iconType">the type of the icon</param>
        public Icon(byte[] data, IconType iconType)
        {
            Type = iconType;
            Data = data;
        }

        /// <summary>
        /// Recommended way of creating an <see cref="Icon"/>.
        /// </summary>
        /// <param name="svgString">The text/xml data of the SVG icon</param>
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
