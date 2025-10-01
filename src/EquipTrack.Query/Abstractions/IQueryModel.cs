namespace EquipTrack.Query.Abstractions;

public interface IQueryModel;

public interface IQueryModel<TKey> : IQueryModel where TKey : IEquatable<TKey>
{
    TKey? Id { get;  }
}
