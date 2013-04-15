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
using System.Windows;
using System.Windows.Controls;
using Tidal.Game;

namespace Tidal.UI
{
    /// <summary>
    /// Represents a single, displayed card
    /// </summary>
    public class DisplayCard : IComparable<DisplayCard>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="image">Image for the card</param>
        /// <param name="card">The playing card</param>
        /// <param name="faceDown">True if this card starts face down</param>
        public DisplayCard(Image image, PlayCard card, bool faceDown)
        {
            Image = image;
            Card = card;
            if (faceDown)
                CardState = DisplayCard.CardStates.FaceDown;
            else
                CardState = DisplayCard.CardStates.FaceUpInPlay;
            }

        /// <summary>
        /// The center of this card
        /// </summary>
        public Point CenterLocation
        {
            get { return new Point(X + Image.ActualWidth / 2, Y + Image.ActualHeight / 2); }
            set { X = value.X - Image.ActualWidth / 2; Y = value.Y - Image.ActualHeight / 2; }
        }

        /// <summary>
        /// The location of this card
        /// </summary>
        public Point Location
        {
            get { return new Point(X, Y); }
            set { X = value.X; Y = value.Y; }
        }

        /// <summary>
        /// The X location of this card
        /// </summary>
        public double X
        {
            get { return (double)Image.GetValue(Canvas.LeftProperty); }
            set { Image.SetValue(Canvas.LeftProperty, value); }
        }

        /// <summary>
        /// The Y location of this card
        /// </summary>
        public double Y
        {
            get { return (double)Image.GetValue(Canvas.TopProperty); }
            set { Image.SetValue(Canvas.TopProperty, value); }
        }

        #region IComparable<DisplayCard> Members

        /// <summary>
        /// Compares the playing cards with each other
        /// </summary>
        public int CompareTo(DisplayCard other)
        {
            if (Card != null && other.Card != null)
                return Card.CompareTo(other.Card);
            else
                return 0;
        }

        #endregion

        /// <summary>
        /// The card's image
        /// </summary>
        public Image Image { get; set; }

        /// <summary>
        /// The playing card this represents
        /// </summary>
        public PlayCard Card { get; set; }

        /// <summary>
        /// The state that the card is in
        /// </summary>
        public CardStates CardState { get; set; }

        /// <summary>
        /// Possible states for a card to be in
        /// </summary>
        public enum CardStates
        {
            FaceDown,
            FaceUpInPlay,
            FaceUpPassToAI,
            FaceUpPassFromAI,
            FaceUpPassedFromAIAndAccepted,
            FaceUpCancellingPass
        }
    }
}
