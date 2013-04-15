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

namespace Tidal.Game
{
    /// <summary>
    /// Represents a set of 3 cards in the game.
    /// </summary>
    public class CardSet
    {
        /// <summary>
        /// Constructs a card set of three given cards
        /// </summary>
        public CardSet(int card1, int card2, int card3)
        {
            Card1 = card1;
            Card2 = card2;
            Card3 = card3;
        }

        /// <summary>
        /// Checks to see if the 3 cards in this set make up a valid set.
        /// A valid set is either 3 cards that match value, or a run of 3 cards that match suit.
        /// </summary>
        /// <returns>true if valid, false if not</returns>
        public bool IsValidSet()
        {
            return IsValidSet(Card1, Card2, Card3);
        }

        /// <summary>
        /// A 4th card can be used to cancel a set of 3 cards.  Checks to see if a card would
        /// be a valid 4th card for this set.  A valid 4th card either matches the values or
        /// extends the suited run.
        /// </summary>
        /// <returns>true if a valid 4th, false if not</returns>
        public bool IsValid4th(int card4)
        {
            return IsValid4Set(Card1, Card2, Card3, card4);
        }

        #region Static functions...

        /// <summary>
        /// Checks to see if the 3 cards in this set make up a valid set.
        /// A valid set is either 3 cards that match value, or a run of 3 cards that match suit.
        /// </summary>
        /// <returns>true if valid, false if not</returns>
        public static bool IsValidSet(int c1, int c2, int c3)
        {
            bool set = false;
            List<int> div = new List<int>();
            div.Add((c1 - 1) / 4);
            div.Add((c2 - 1) / 4);
            div.Add((c3 - 1) / 4);
            if (div[0] == div[1] && div[0] == div[2])
            {
                // 3 of a kind
                set = true;
            }
            else
            {
                int[] mod = new int[3] { (c1 - 1) % 4, (c2 - 1) % 4, (c3 - 1) % 4 };
                div.Sort();

                if (mod[0] == mod[1] && mod[0] == mod[2] &&
                    div[0] + 1 == div[1] && div[0] + 2 == div[2])
                {
                    // run
                    set = true;
                }
            }
            return set;
        }

        /// <summary>
        /// A 4th card can be used to cancel a set of 3 cards.  Checks to see if a card would
        /// be a valid 4th card for this set.  A valid 4th card either matches the values or
        /// extends the suited run.
        /// </summary>
        /// <returns>true if a valid 4th, false if not</returns>
        public static bool IsValid4Set(int card1, int card2, int card3, int card4)
        {
            bool is4set = false;
            List<int> div = new List<int>();
            div.Add((card1 - 1) / 4);
            div.Add((card2 - 1) / 4);
            div.Add((card3 - 1) / 4);
            div.Add((card4 - 1) / 4);
            if (div[0] == div[1] && div[0] == div[2] && div[0] == div[3])
            {
                // 4 of a kind
                is4set = true;
            }
            else
            {
                // use mod to see if they're the same suit
                int[] mod = new int[4] { (card1 - 1) % 4, (card2 - 1) % 4, (card3 - 1) % 4, (card4 - 1) % 4 };
                div.Sort();

                if (mod[0] == mod[1] && mod[0] == mod[2] && mod[0] == mod[3] &&
                    div[0] + 1 == div[1] && div[0] + 2 == div[2] && div[0] + 3 == div[3])
                {
                    // run
                    is4set = true;
                }
            }
            return is4set;
        }

        /// <summary>
        /// Given a list of cards, find the maximum number of sets that could be
        /// made from those cards.
        /// </summary>
        /// <param name="cards">The list of cards to search</param>
        /// <returns>The maximum sets</returns>
        public static List<CardSet> FormMaxSets(List<PlayCard> cards)
        {
            List<int> cardIDs = new List<int>();
            foreach (PlayCard c in cards)
            {
                cardIDs.Add(c.CardID);
            }
            return FormMaxSets(cardIDs);
        }

        /// <summary>
        /// Given a list of cards, find the maximum number of sets that could be
        /// made from those cards.
        /// </summary>
        /// <param name="cards">The list of cards to search</param>
        /// <returns>The maximum sets</returns>
        public static List<CardSet> FormMaxSets(List<int> cards)
        {
            List<CardSet> allSets = new List<CardSet>();
            List<int> sorted = new List<int>(cards);
            
            // Sort by number, look for sets
            sorted.Sort();
            sorted.Reverse();
            for (int i = 0; i + 2 < sorted.Count; i++)
            {
                if (IsValidSet(sorted[i], sorted[i + 1], sorted[i + 2]))
                {
                    allSets.Add(new CardSet(sorted[i], sorted[i + 1], sorted[i + 2]));
                }
            }

            // Resort by suit and look for sets
            sorted.Sort(CompareBySuit);
            sorted.Reverse(); // reverse because card IDs are increasing while values are decreasing
            for (int i = 0; i + 2 < sorted.Count; i++)
            {
                if (IsValidSet(sorted[i], sorted[i + 1], sorted[i + 2]))
                {
                    allSets.Add(new CardSet(sorted[i], sorted[i + 1], sorted[i + 2]));
                }
            }

            // Remove any conflicts
            List<CardSet> maxSets = RecursiveRemoveConflicts(allSets);
            return maxSets;
        }

        /// <summary>
        /// Go through a list of sets, and remove any conflicting ones (where a card is used in
        /// two different sets).
        /// </summary>
        /// <param name="allSets">The list of sets</param>
        /// <returns>The list of sets without conflicts</returns>
        private static List<CardSet> RecursiveRemoveConflicts(List<CardSet> allSets)
        {
            if (!HasConflicts(allSets))
            {
                return allSets;
            }
            else
            {
                List<CardSet> goodSets = new List<CardSet>();
                List<CardSet> potentialSets = null;
                for (int i = 0; i < allSets.Count; i++)
                {
                    List<CardSet> trySets = new List<CardSet>();
                    for (int j = 0; j < allSets.Count; j++)
                    {
                        if (i != j)
                            trySets.Add(allSets[j]);
                    }
                    potentialSets = RecursiveRemoveConflicts(trySets);
                    if (potentialSets.Count > goodSets.Count)
                        goodSets = potentialSets;
                }
                return goodSets;
            }
        }

        /// <summary>
        /// Look through a list of sets, and see if there are any cards that exist
        /// in multiple sets.
        /// </summary>
        /// <param name="sets">The list of sets</param>
        /// <returns>true if a card exists in multiple sets, false if not</returns>
        private static bool HasConflicts(List<CardSet> sets)
        {
            List<int> cards = new List<int>();
            foreach (CardSet set in sets)
            {
                if (cards.Contains(set.Card1) ||
                    cards.Contains(set.Card2) ||
                    cards.Contains(set.Card3))
                {
                    return true;
                }
                else
                {
                    cards.Add(set.Card1);
                    cards.Add(set.Card2);
                    cards.Add(set.Card3);
                }
            }
            return false;
        }

        /// <summary>
        /// Compares two cards suits against each other
        /// </summary>
        private static int CompareBySuit(int card1, int card2)
        {
            int result = 0;
            int suit1 = (card1 % 4);
            int suit2 = (card2 % 4);
            result = suit1.CompareTo(suit2);
            if (result == 0)
            {
                result = card1.CompareTo(card2);
            }
            return result;
        }

        #endregion

        /// <summary>
        /// First card in the set
        /// </summary>
        public int Card1 { get; protected set; }

        /// <summary>
        /// Second card in the set
        /// </summary>
        public int Card2 { get; protected set; }

        /// <summary>
        /// Third card in the set
        /// </summary>
        public int Card3 { get; protected set; }
    }
}
