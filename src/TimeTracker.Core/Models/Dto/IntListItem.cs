namespace TimeTracker.Core.Models.Dto
{
  public class IntListItem
  {
    public int Value { get; set; }
    public string Name { get; set; }

    public IntListItem()
    {
      Value = 0;
      Name = string.Empty;
    }
  }
}
