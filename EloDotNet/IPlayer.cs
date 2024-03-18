using System;

namespace EloDotNet
{
    public interface IPlayer
    {

    }

    public interface IPlayer<TId> : IPlayer
    {
        TId Id { get; set; }
    }

    public class Player
    {
        public Guid Id { get; }

        public Player()
        {
            Id = Guid.NewGuid();
        }
    }
}
