namespace TimeTracker.Core.Database.Entities
{
  public class KeyValueEntity<TKey, TValue>
  {
    public TKey Key { get; set; }
    public TValue Value { get; set; }

    public KeyValueEntity()
    {
      Key = default;
      Value = default;
    }
  }
}
