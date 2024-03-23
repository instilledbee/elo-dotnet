using System.Collections.Generic;
using EloDotNet.Enums;

namespace EloDotNet.Interfaces
{
    /// <summary>
    /// Represents a system where players can be ranked based on their calculated Elo rating, based on the historical matches they have participated in that are recorded within the system.
    /// </summary>
    /// <typeparam name="TMatch">The implementation of <see cref="IMatch{TPlayer, TIndex}"/> that will represent matches in the system.</typeparam>
    /// <typeparam name="TPlayer">The implementation of <see cref="IPlayer{TId}"/> that will represent players in the system.</typeparam>
    public interface IRankingSystem<TMatch, TPlayer>
        where TMatch : IMatch
        where TPlayer : IPlayer
    {
        /// <summary>
        /// The starting Elo value for a new player in the system.
        /// </summary>
        double StartingElo { get; }

        /// <summary>
        /// The K-Factor determines how strongly a match affects a player's rating change.
        /// </summary>
        double KFactor { get; }

        /// <summary>
        /// A collection of match records that determine how each player's Elo is calculated.
        /// </summary>
        ICollection<TMatch> Matches { get; }

        /// <summary>
        /// A collection of player entities which are participating in this ranking system.
        /// </summary>
        ICollection<TPlayer> Players { get; }

        /// <summary>
        /// Register a new player in this ranking system.
        /// </summary>
        /// <param name="player">The player entity to add.</param>
        void RegisterPlayer(TPlayer player);

        /// <summary>
        /// Record a new match in this ranking system, specifying two participating players and the match results.
        /// </summary>
        /// <param name="playerA">The first player in the match.</param>
        /// <param name="playerB">The second player in the match.</param>
        /// <param name="winner">The resulting winner in the match, or a draw.</param>
        void RecordMatch(TPlayer playerA, TPlayer playerB, MatchWinner winner);

        /// <summary>
        /// Determine a player's Elo based on the latest matches recorded in the system.
        /// </summary>
        /// <param name="player">Which player to determine the Elo for.</param>
        /// <returns></returns>
        double CalculateElo(TPlayer player);
    }
}
