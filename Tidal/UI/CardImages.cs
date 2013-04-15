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
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Tidal.UI
{
    public class CardImages
    {
        /// <summary>
        /// Private constructor for singleton instance, loads all images
        /// </summary>
        private CardImages()
        {
            this.LoadImages();
        }

        /// <summary>
        /// Access to the CardImages singleton
        /// </summary>
        public static CardImages Instance
        {
            get
            {
                if (singletonInstance == null)
                {
                    singletonInstance = new CardImages();
                }
                return singletonInstance;
            }
        }

        /// <summary>
        /// Load all of the images out of the xap
        /// </summary>
        private void LoadImages()
        {
            images = new Image[55];
            images[0] = new Image();
            images[0].Source = new BitmapImage(new Uri("../images/cards/cardbg3.png", UriKind.Relative));
            images[0].SetValue(Image.NameProperty, "bgcard");
            images[0].Stretch = Stretch.None;
            for (int i = 1; i <= 54; i++)
            {
                images[i] = new Image();
                images[i].Source = new BitmapImage(new Uri(string.Format("../images/cards/{0}.png", i), UriKind.Relative));
                images[i].SetValue(Image.NameProperty, string.Format("card{0}", i));
                images[i].Stretch = Stretch.None;
            }
        }

        /// <summary>
        /// Gets a card image.
        /// </summary>
        public Image GetCardImage(int id)
        {
            images[id].Opacity = 1.0;
            return images[id];
        }

        /// <summary>
        /// Gets a clone of the face down card image.
        /// </summary>
        public Image GetBgCardImage()
        {
            Image img = new Image();
            img.SetValue(Image.SourceProperty, images[0].GetValue(Image.SourceProperty));
            img.SetValue(Image.NameProperty, string.Format("{0}{1}", images[0].GetValue(Image.NameProperty), bgid++));
            img.Stretch = Stretch.None;
            return img;
        }

        public const int CARD_WIDTH = 72;
        public const int CARD_HEIGHT = 96;

        private Image[] images = null;
        private int bgid = 0;
        private static CardImages singletonInstance = null;
    }
}
