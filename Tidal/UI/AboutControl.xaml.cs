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

using System;
using System.Reflection;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Controls;
using System.Windows.Input;

namespace Tidal.UI
{
    /// <summary>
    /// A control that floats over the other controls and shows information
    /// about the game.
    /// </summary>
    public partial class AboutControl : UserControl
    {
        public AboutControl()
        {
            InitializeComponent();

            VersionText.Text = "Tidal v1.x";
            try
            {
                string verText = "Tidal v";
                AssemblyName assemblyName = new AssemblyName(Assembly.GetExecutingAssembly().FullName);
                verText += assemblyName.Version.ToString();
                // Find the last '.'
                int index = verText.LastIndexOf('.');
                if (index != -1)
                {
                    // Remove everything thereafter
                    verText = verText.Remove(index);
                }
                VersionText.Text = verText;
            }
            catch (Exception)
            {
                // Ignore any version-fetching error, leave the placeholder text
            }
        }

        /// <summary>
        /// If we mouse over the little showing tab, show the whole thing
        /// </summary>
        private void AboutBg_MouseEnter(object sender, MouseEventArgs e)
        {
            // TODO: Animate
            AboutTidalControl.SetValue(Canvas.LeftProperty, SHOWNX);
            AboutTidalControl.SetValue(Canvas.TopProperty, SHOWNY);
        }

        /// <summary>
        /// If the mouse leaves the area, hide ourselves again
        /// </summary>
        private void AboutBg_MouseLeave(object sender, MouseEventArgs e)
        {
            // TODO: Animate
            AboutTidalControl.SetValue(Canvas.LeftProperty, HIDDENX);
            AboutTidalControl.SetValue(Canvas.TopProperty, HIDDENY);
        }

        /// <summary>
        /// Absorb clicks made in the control
        /// </summary>
        private void AboutBg_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        /// <summary>
        /// Absorb clicks made in the control
        /// </summary>
        private void AboutBg_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        /// <summary>
        /// Link clicks take you to the web page
        /// </summary>
        private void LinkNoFailGames_Click(object sender, RoutedEventArgs e)
        {
            HtmlPage.PopupWindow(new Uri("http://www.nofailgames.com/", UriKind.Absolute), "_blank", null);
        }

        /// <summary>
        /// Link clicks take you to the web page
        /// </summary>
        private void LinkKanagawa_Click(object sender, RoutedEventArgs e)
        {
            HtmlPage.PopupWindow(new Uri("http://en.wikipedia.org/wiki/The_Great_Wave_off_Kanagawa", UriKind.Absolute), "_blank", null);
        }

        /// <summary>
        /// Link clicks take you to the web page
        /// </summary>
        private void LinkFuji_Click(object sender, RoutedEventArgs e)
        {
            HtmlPage.PopupWindow(new Uri("http://en.wikipedia.org/wiki/Hokusai", UriKind.Absolute), "_blank", null);
        }

        /// <summary>
        /// Where to put this control when hidden
        /// </summary>
        public const double HIDDENX = 654;
        /// <summary>
        /// Where to put this control when hidden
        /// </summary>
        public const double HIDDENY = 454;

        /// <summary>
        /// Where to put this control when shown
        /// </summary>
        public const double SHOWNX = 400;
        /// <summary>
        /// Where to put this control when shown
        /// </summary>
        public const double SHOWNY = 200;
    }
}
