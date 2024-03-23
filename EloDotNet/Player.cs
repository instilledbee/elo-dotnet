using System;
using EloDotNet.Interfaces;

namespace EloDotNet
{
    /// <summary>
    /// Default <see cref="IPlayer{TId}"/> implementation that uses a <see cref="Guid"/> as a unique identifier
    /// </summary>
    public class Player : IPlayer<Guid>
    {
        public Guid Id { get; }

        public Player()
        {
            Id = Guid.NewGuid();
        }
    }
}
