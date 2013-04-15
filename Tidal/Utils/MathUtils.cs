//////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2013, Pete Vasiliauskas <pete@nofailgames.com>
//
// Permission to use, copy, modify, and/or distribute this software for any
// purpose with or without fee is hereby granted, provided that the above
// copyright notice and this permission notice appear in all copies.
//
// THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL WARRANTIES
// WITH REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED WARRANTIES OF
// MERCHANTABILITY AND FITNESS. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY
// SPECIAL, DIRECT, INDIRECT, OR CONSEQUENTIAL DAMAGES OR ANY DAMAGES
// WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN AN
// ACTION OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS ACTION, ARISING OUT OF OR
// IN CONNECTION WITH THE USE OR PERFORMANCE OF THIS SOFTWARE.
//////////////////////////////////////////////////////////////////////////////

using System.Windows;

namespace Tidal.Utils
{
    /// <summary>
    /// Extension methods for dealing with rectangles
    /// </summary>
    public static class MathUtils
    {
        /// <summary>
        /// Returns true if rContainee is completely within rContainer.
        /// Same size and position rectangles would return true.
        /// </summary>
        /// <param name="rContainer">The rectangle container</param>
        /// <param name="rContainee">The rectangle to check if its in the container</param>
        /// <returns>true if within the container, false if not</returns>
        public static bool Contains(this Rect rContainer, Rect rContainee)
        {
            bool bContains = true;
            bContains &= rContainer.Contains(new Point(rContainee.Left, rContainee.Top));
            bContains &= rContainer.Contains(new Point(rContainee.Right, rContainee.Bottom));
            return bContains;
        }
    }
}
