using System.Threading.Tasks;
using TimeTracker.Core.Database.Repos;
using TimeTracker.Core.Models;

namespace TimeTracker.Core.Services
{
  public interface IOptionsService
  {
    Task<RawOptions> GenerateRawOptions(string category, int userId);
  }

  public class OptionsService : IOptionsService
  {
    private readonly IOptionRepo _optionRepo;

    public OptionsService(IOptionRepo optionRepo)
    {
      _optionRepo = optionRepo;
    }

    public async Task<RawOptions> GenerateRawOptions(string category, int userId)
    {
      // TODO: [TESTS] (OptionsService.GetRawOptionsForCategory) Add tests
      var generated = new RawOptions();

      var dbOptions = await _optionRepo.GetRawOptionsForCategory(category, userId);

      foreach (var dbOption in dbOptions)
      {
        generated.AddOption(dbOption);
      }

      return generated;
    }
  }
}
