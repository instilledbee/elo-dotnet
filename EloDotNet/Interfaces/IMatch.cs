using EloDotNet.Enums;
using System;

namespace EloDotNet.Interfaces
{
    /// <summary>
    /// Represents a record of participation between two players.
    /// </summary>
    public interface IMatch
    {

    }

    /// <summary>
    /// Represents a record of participation between two <see cref="TPlayer"/> entities.
    /// </summary>
    /// <remarks>Note that <see cref="TIndex"/> needs to implement <see cref="IComparable{TIndex}"/>. This allows an <see cref="IRankingSystem{TMatch, TPlayer}"/> to iterate through a collection of matches in an ordered fashion.</remarks>
    /// <typeparam name="TPlayer">The type of the player entities that participate in this Match.</typeparam>
    /// <typeparam name="TIndex">The type of the index that historically orders this <see cref="IMatch{TPlayer, TIndex}"/></typeparam>
    public interface IMatch<TPlayer, TIndex> : IMatch
        where TPlayer : IPlayer
        where TIndex : IComparable<TIndex>
    {
        /// <summary>
        /// The first player.
        /// </summary>
        TPlayer PlayerA { get; }

        /// <summary>
        /// The second player.
        /// </summary>
        TPlayer PlayerB { get; }

        /// <summary>
        /// The result of this match
        /// </summary>
        MatchWinner Result { get; }

        /// <summary>
        /// References who won in this match
        /// <para/>
        /// Should return null if the match resulted in a <see cref="MatchWinner.Draw"/>
        /// </summary>
        TPlayer Winner { get; }

        /// <summary>
        /// References who lost in this match
        /// <para/>
        /// Should return null if the match resulted in a <see cref="MatchWinner.Draw"/>
        /// </summary>
        TPlayer Loser { get; }

        /// <summary>
        /// The historical index of this <see cref="IMatch{TPlayer, TIndex}"/>
        /// </summary>
        TIndex RecordIndex { get; }
    }
}
