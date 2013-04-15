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
using Tidal.Game;

namespace Tidal.UI
{
    /// <summary>
    /// Root control to handle switching between screens
    /// </summary>
    public partial class RootContainer : UserControl
    {
        /// <summary>
        /// Constructor will default to showing the title screen
        /// </summary>
        public RootContainer()
        {
            Stats = new GameStats();

            InitializeComponent();

            this.SwitchControl(new TitleScreen(this));
        }

        /// <summary>
        /// Replace all contents with new contents
        /// </summary>
        /// <param name="newControl">New control to become the exclusive view</param>
        public void SwitchControl(UserControl newControl)
        {
            LayoutRoot.Children.Clear();
            if (newControl != null)
            {
                Height = newControl.Height;
                Width = newControl.Width;
                LayoutRoot.Children.Add(newControl);
            }
        }

        /// <summary>
        /// Keeps track of win/loss statistics between games
        /// </summary>
        public GameStats Stats { get; set; }
    }
}
