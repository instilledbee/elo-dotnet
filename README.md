# EloDotNet
An ELO calculation framework for .NET

# Getting Started
The easiest way to get started is to add the [NuGet package](https://www.nuget.org/packages/EloDotNet) to your project.

Releases are also tagged and downloadable from this repository.

# Building
You may build your own copy of the package from the latest code by pulling from this repository. All of the projects are contained within the solution file (`EloDotNet.sln`).

# Usage
The package provides the following classes:
* `Player` - represents a participant in the Elo ranking system. Typically uniquely identified with a `Guid` property.
* `Match` - represents the participation of two `Player`s in the system. 
   * Describes the match results (who won/lost, or if the match was a draw). 
   * A `Match` can only have one `Result` - whether the first or second player won, or if the match was a draw.
* `RankingSystem` - encapsulates a collection of participating `Player`s, and a list of `Match`es each player has participated in.

Note that for purposes of Elo calculation, all `Player`s must be registered via `RankingSystem.RegisterPlayer()`, and all `Match`es must be recorded via `RankingSystem.RecordMatch()`.

# Extending
All default classes in EloDotNet implement their respective interfaces:
* `IPlayer<TId>` - interface for a player entity with a unique identifier of type `TId`.
   * Note that `TId` must implement `IEquatable<TId>`.
* `IMatch<TPlayer, TIndex>` - interface for a match entity between two `TPlayer` entities, and where each match can be ordered using the `TIndex` property. 
   * Note that `TPlayer` must implement `IPlayer<TId>`.
   * `TIndex` must implement `IComparable<TIndex>`.
* `IRankingSystem<TPlayer, TMatch>` - interface for a ranking system of `TPlayer` entities participating in `TMatch` matches.
   * Note that `TPlayer` must implement `IPlayer<TId>`.
   * Note that `TMatch` must implement `IMatch<TPlayer, TIndex>`.

Thus, it is possible to roll your own player, match and ranking system implementations.

* `RankingSystem` also exposes a generic version with a default implementation, e.g. `RankingSystem<IPlayer<TId>, IMatch<TId, TIndex>>`. 
   * This allows you to replace the player and match types while keeping the default Elo calculation algorithm, for instance.

# Sample Code
Please check the test project `EloDotNet.Tests` to see sample snippets on how to use the default `RankingSystem`, as well as creating your custom `IMatch` and `IPlayer` implementations, and integrating them into a custom `IRankingSystem` implementation, with its own Elo calculation algorithm.

As more projects adapt EloDotNet, they'll be featured here for reference!