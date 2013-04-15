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
using System.Collections.Generic;

namespace Tidal.Game
{
    /// <summary>
    /// Represents all of the cards that are in play
    /// </summary>
    public class CardsInPlay
    {
        public CardsInPlay()
        {
        }

        /// <summary>
        /// Initialize and shuffle the deck with 52 cards
        /// </summary>
        public void Initialize52Deck()
        {
            cards.Clear();

            List<PlayCard> unshuffled = new List<PlayCard>();
            for (int i = 1; i <= 52; i++)
            {
                PlayCard card = new PlayCard(i);
                unshuffled.Add(card);
            }

            // shuffle
            Random rnd = new Random((int)DateTime.Now.Ticks);
            while (unshuffled.Count > 0)
            {
                int index = rnd.Next(unshuffled.Count);
                cards.Add(unshuffled[index]);
                unshuffled.RemoveAt(index);
            }
        }

        /// <summary>
        /// Initialize and shuffle the deck with 32 cards (7,8,9,10,J,Q,K,A)
        /// </summary>
        public void Initialize32Deck()
        {
            cards.Clear();

            List<PlayCard> unshuffled = new List<PlayCard>();
            for (int i = 1; i <= 32; i++)
            {
                PlayCard card = new PlayCard(i);
                unshuffled.Add(card);
            }

            // shuffle
            Random rnd = new Random((int)DateTime.Now.Ticks);
            while (unshuffled.Count > 0)
            {
                int index = rnd.Next(unshuffled.Count);
                cards.Add(unshuffled[index]);
                unshuffled.RemoveAt(index);
            }
        }

        /// <summary>
        /// Draw a card from the draw pile and put in on the table
        /// </summary>
        /// <returns>The card drawn</returns>
        public PlayCard DrawOne()
        {
            foreach (PlayCard card in cards)
            {
                if (card.Pile == PlayCard.CardPile.DrawPile)
                {
                    card.Pile = PlayCard.CardPile.OnTable;
                    return card;
                }
            }
            return null;
        }

        /// <summary>
        /// Draw one card, reshuffling the discard pile
        /// into the draw pile if needed
        /// </summary>
        /// <returns></returns>
        public PlayCard DrawOneReshuffleDiscard()
        {
            if (DrawPileCount() == 0)
            {
                // remove discards and shuffle
                List<PlayCard> unshuffled = new List<PlayCard>();
                for (int i = cards.Count - 1; i >= 0; i--)
                {
                    if (cards[i].Pile == PlayCard.CardPile.DiscardPile)
                    {
                        unshuffled.Add(cards[i]);
                        cards.RemoveAt(i);
                    }
                }

                Random rnd = new Random((int)DateTime.Now.Ticks);
                while (unshuffled.Count > 0)
                {
                    int index = rnd.Next(unshuffled.Count);
                    unshuffled[index].Pile = PlayCard.CardPile.DrawPile;
                    cards.Add(unshuffled[index]);
                    unshuffled.RemoveAt(index);
                }
            }
            return DrawOne();
        }

        /// <summary>
        /// Returns a count of the cards remaining in the draw pile
        /// </summary>
        /// <returns>Returns a count of the cards remaining in the draw pile</returns>
        public int DrawPileCount()
        {
            int count = 0;
            foreach (PlayCard card in cards)
            {
                if (card.Pile == PlayCard.CardPile.DrawPile)
                    count++;
            }
            return count;
        }

        /// <summary>
        /// Gets a card by its ID
        /// </summary>
        /// <param name="id">The ID to look for</param>
        /// <returns>The card, or null if not found</returns>
        public PlayCard GetCardByID(int id)
        {
            foreach (PlayCard card in cards)
            {
                if (card.CardID == id)
                    return card;
            }
            return null;
        }

        private List<PlayCard> cards = new List<PlayCard>();
    }
}
