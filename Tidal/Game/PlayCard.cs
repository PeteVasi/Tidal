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

namespace Tidal.Game
{
    /// <summary>
    /// Represents a card in play and its state
    /// </summary>
    public class PlayCard : IComparable<PlayCard>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id">The ID of the playing card</param>
        public PlayCard(int id)
        {
            CardID = id;
            Pile = CardPile.DrawPile;
        }

        /// <summary>
        /// Indicates where this card is located
        /// </summary>
        public enum CardPile
        {
            DrawPile,
            DiscardPile,
            OnTable,
            InHand
        }

        /// <summary>
        /// The ID of this card
        /// </summary>
        public int CardID { get; private set; }

        /// <summary>
        /// The location of this card
        /// </summary>
        public CardPile Pile { get; set; }

        #region IComparable<PlayCard> Members

        /// <summary>
        /// Compares this card to another by ID
        /// </summary>
        public int CompareTo(PlayCard other)
        {
            return CardID.CompareTo(other.CardID);
        }

        #endregion
    }
}
