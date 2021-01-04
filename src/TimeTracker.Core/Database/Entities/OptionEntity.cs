namespace TimeTracker.Core.Database.Entities
{
  public class OptionEntity
  {
    public int OptionId { get; set; }
    public bool Deleted { get; set; }
    public bool IsCollection { get; set; }
    public int UserId { get; set; }
    public string OptionType { get; set; }
    public string OptionCategory { get; set; }
    public string OptionKey { get; set; }
    public string OptionValue { get; set; }

    public OptionEntity()
    {
      // TODO: [TESTS] (OptionEntity) Add tests
      OptionId = 0;
      Deleted = false;
      IsCollection = false;
      UserId = 0;
      OptionType = string.Empty;
      OptionCategory = string.Empty;
      OptionKey = string.Empty;
      OptionValue = string.Empty;
    }
  }
}
