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

using System.Windows.Controls;
using System.Windows.Input;

namespace Tidal.UI
{
    /// <summary>
    /// A control that floats over the other controls and shows information
    /// about how to play the game.
    /// </summary>
    public partial class HowToPlayControl : UserControl
    {
        public HowToPlayControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// If we mouse over the little showing tab, show the whole thing
        /// </summary>
        private void HowToPlayBg_MouseEnter(object sender, MouseEventArgs e)
        {
            // TODO: Animate
            HowToControl.SetValue(Canvas.LeftProperty, SHOWNX);
        }

        /// <summary>
        /// If the mouse leaves the area, hide ourselves again
        /// </summary>
        private void HowToPlayBg_MouseLeave(object sender, MouseEventArgs e)
        {
            // TODO: Animate
            HowToControl.SetValue(Canvas.LeftProperty, HIDDENX);
        }

        /// <summary>
        /// Absorb clicks made in the control
        /// </summary>
        private void HowToPlayBg_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        /// <summary>
        /// Absorb clicks made in the control
        /// </summary>
        private void HowToPlayBg_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        /// <summary>
        /// Where to put this control when hidden
        /// </summary>
        public const double HIDDENX = -476;

        /// <summary>
        /// Where to put this control when shown
        /// </summary>
        public const double SHOWNX = 0;
    }
}
