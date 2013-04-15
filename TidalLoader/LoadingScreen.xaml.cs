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
using System.Net;
using System.Reflection;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Controls;
using System.Windows.Resources;

namespace TidalLoader
{
    /// <summary>
    /// Screen to show a progress bar while we load Tidal.xap
    /// </summary>
    public partial class LoadingScreen : UserControl
    {
        /// <summary>
        /// Default constructor for VS designer
        /// </summary>
        public LoadingScreen()
        {
            string fileName = System.IO.Path.GetFileName(App.Current.Host.Source.AbsolutePath);
            if (fileName == string.Empty)
            {
                fileName = HtmlPage.Document.DocumentUri.AbsoluteUri;
                if (fileName.EndsWith("/"))
                {
                    this.baseUri = new Uri(fileName, UriKind.RelativeOrAbsolute);
                }
                else
                {
                    this.baseUri = new Uri(string.Format("{0}/", fileName), UriKind.RelativeOrAbsolute);
                }
            }
            else
            {
                this.baseUri = new Uri(App.Current.Host.Source.AbsoluteUri.Replace(fileName, string.Empty), UriKind.RelativeOrAbsolute);
            }
            InitializeComponent();
        }

        /// <summary>
        /// Normal implementation constructor
        /// </summary>
        /// <param name="root">Reference to the root control</param>
        public LoadingScreen(RootContainer root) :
            this()
        {
            this.rootContainer = root;
        }

        /// <summary>
        /// Start downloading once this screen is loaded
        /// </summary>
        private void LoadingScreen_Loaded(object sender, RoutedEventArgs e)
        {
            Uri address = new Uri("Tidal.xap", UriKind.RelativeOrAbsolute);
            WebClient client = new WebClient();
            client.BaseAddress = this.baseUri.AbsoluteUri;
            client.OpenReadCompleted += new OpenReadCompletedEventHandler(Downloader_OpenReadCompleted);
            client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(Downloader_DownloadProgressChanged);
            client.OpenReadAsync(address);
        }

        /// <summary>
        /// Update the progress bar
        /// </summary>
        void Downloader_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            double pct = ((double)e.BytesReceived) / ((double)e.TotalBytesToReceive);
            ProgressBar.Width = pct * (500.0 - 20.0) + 20.0;
        }

        /// <summary>
        /// Download complete, load the main game
        /// </summary>
        void Downloader_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            StreamResourceInfo xapInfo = new StreamResourceInfo(e.Result, "application/x-silverlight-app");
            StreamResourceInfo resourceStream = Application.GetResourceStream(xapInfo, new Uri("Tidal.dll", UriKind.Relative));
            Assembly assembly = new AssemblyPart().Load(resourceStream.Stream);
            
            if (assembly != null)
            {
                UserControl target = (UserControl)assembly.CreateInstance("Tidal.UI.RootContainer");
                rootContainer.SwitchControl(target);
            }
        }

        private Uri baseUri;
        private RootContainer rootContainer = null;
    }
}
