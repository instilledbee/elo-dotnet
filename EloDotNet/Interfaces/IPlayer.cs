using System;

namespace EloDotNet.Interfaces
{
    /// <summary>
    /// Represents a participant in an Elo ranking system.
    /// </summary>
    public interface IPlayer
    {

    }

    /// <summary>
    /// Represents a participant in an Elo ranking system, with a unique identifier property.
    /// </summary>
    /// <typeparam name="TId">The type of the property that uniquely identifies this <see cref="IPlayer{TId}"/> within the ranking system.</typeparam>
    /// <remarks>Note that <see cref="TId"/> needs to implement <see cref="IEquatable{TId}"/>. This allows players to be referenced by their identifier within an <see cref="IRankingSystem{TMatch, TPlayer}"/>.</remarks>
    public interface IPlayer<out TId> : IPlayer
        where TId : IEquatable<TId>
    {
        /// <summary>
        /// The unique identifier for this <see cref="IPlayer{TId}"/> within the ranking system.
        /// </summary>
        TId Id { get; }
    }
}
