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

using System.Collections.Generic;
using Tidal.Game;
using Tidal.UI;

namespace Tidal.Utils
{
    /// <summary>
    /// Static extension methods for working with card lists
    /// </summary>
    public static class ListUtils
    {
        /// <summary>
        /// Inserts a card in a list sorted by "X" value
        /// </summary>
        /// <param name="list">The list to insert into</param>
        /// <param name="card">The card to insert</param>
        public static void InsertInXOrder(this List<DisplayCard> list, DisplayCard card)
        {
            if (list.Count == 0)
            {
                list.Add(card);
            }
            else
            {
                int index = 0;
                bool added = false;
                foreach (DisplayCard d in list)
                {
                    if (card.X < d.X)
                    {
                        list.Insert(index, card);
                        added = true;
                        break;
                    }
                    index++;
                }
                if (!added)
                {
                    list.Add(card);
                }
            }
        }

        /// <summary>
        /// Returns true if the given list contains the card id
        /// </summary>
        /// <param name="list">The list to search</param>
        /// <param name="id">The card ID</param>
        /// <returns>true if found, false if not</returns>
        public static bool ContainsCardID(this List<DisplayCard> list, int id)
        {
            foreach (DisplayCard d in list)
            {
                if (d.Card != null && d.Card.CardID == id)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Returns true if the given list contains the card id
        /// </summary>
        /// <param name="list">The list to search</param>
        /// <param name="id">The card ID</param>
        /// <returns>true if found, false if not</returns>
        public static bool ContainsCardID(this List<CardSet> list, int id)
        {
            foreach (CardSet s in list)
            {
                if (s.Card1 == id || s.Card2 == id || s.Card3 == id)
                    return true;
            }
            return false;
        }
    }
}
