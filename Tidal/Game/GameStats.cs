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
    /// Keeps track of statistics between games
    /// </summary>
    public class GameStats
    {
        /// <summary>
        /// Initialize all stats to 0
        /// </summary>
        public GameStats()
        {
            wins = new Dictionary<GameDifficulty, int>();
            losses = new Dictionary<GameDifficulty, int>();
            streak = new Dictionary<GameDifficulty, int>();

            foreach (GameDifficulty diff in Enum.GetValues(typeof(GameDifficulty)))
            {
                wins.Add(diff, 0);
                losses.Add(diff, 0);
                streak.Add(diff, 0);
            }
        }

        /// <summary>
        /// Game won, update the wins and streak
        /// </summary>
        /// <param name="diff">AI difficulty level</param>
        public void GameWon(GameDifficulty diff)
        {
            wins[diff] = wins[diff] + 1;
            streak[diff] = streak[diff] + 1;
        }

        /// <summary>
        /// Game lost, update the losses and reset the streak
        /// </summary>
        /// <param name="diff">AI difficulty level</param>
        public void GameLost(GameDifficulty diff)
        {
            losses[diff] = losses[diff] + 1;
            streak[diff] = 0;
        }

        /// <summary>
        /// Get the number of games won
        /// </summary>
        /// <param name="diff">AI difficulty level</param>
        public int GetWins(GameDifficulty diff)
        {
            return wins[diff];
        }

        /// <summary>
        /// Get the number of games lost
        /// </summary>
        /// <param name="diff">AI difficulty level</param>
        public int GetLosses(GameDifficulty diff)
        {
            return losses[diff];
        }

        /// <summary>
        /// Get the number of games won in a row
        /// </summary>
        /// <param name="diff">AI difficulty level</param>
        public int GetStreak(GameDifficulty diff)
        {
            return streak[diff];
        }

        private Dictionary<GameDifficulty, int> wins;
        private Dictionary<GameDifficulty, int> losses;
        private Dictionary<GameDifficulty, int> streak;
    }
}
