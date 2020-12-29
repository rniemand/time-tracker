using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using Rn.NetCore.Common.Encryption;
using Rn.NetCore.Common.Logging;
using TimeTracker.Core.Database.Entities;
using TimeTracker.Core.Database.Repos;

namespace TimeTracker.Core.Services
{
  public interface IUserService
  {
    Task<string> LoginUser(string username, string password);
  }

  public class UserService : IUserService
  {
    private readonly ILoggerAdapter<UserService> _logger;
    private readonly IEncryptionService _encryptionService;
    private readonly IUserRepo _userRepo;
    // TODO: [CONFIG] (UserService.Secret) Make Configurable
    private readonly string Secret = "2QNMuWSPKeOg4BICql8QooUO6k+e+CS236L6hg1";

    public UserService(
      ILoggerAdapter<UserService> logger,
      IEncryptionService encryptionService,
      IUserRepo userRepo)
    {
      _logger = logger;
      _encryptionService = encryptionService;
      _userRepo = userRepo;
    }

    public async Task<string> LoginUser(string username, string password)
    {
      // TODO: [TESTS] (UserService.LoginUser) Add tests
      var encryptedPass = _encryptionService.Encrypt(password);
      var userEntity = await _userRepo.GetUsingCredentials(username, encryptedPass);

      if (userEntity == null)
      {
        // TODO: [COMPLETE] (UserService.LoginUser) Complete this
        return null;
      }
      
      await _userRepo.UpdateLastLoginDate(userEntity.UserId);
      return GenerateJwtToken(userEntity.UserId);
    }



    private string GenerateJwtToken(int userId)
    {
      // generate token that is valid for 7 days
      var tokenHandler = new JwtSecurityTokenHandler();
      var key = Encoding.ASCII.GetBytes(Secret);

      var tokenDescriptor = new SecurityTokenDescriptor
      {
        Subject = new ClaimsIdentity(new[]
        {
          new Claim("id", userId.ToString())
        }),
        Expires = DateTime.UtcNow.AddDays(7),
        SigningCredentials = new SigningCredentials(
          new SymmetricSecurityKey(key),
          SecurityAlgorithms.HmacSha256Signature
        )
      };

      var token = tokenHandler.CreateToken(tokenDescriptor);
      return tokenHandler.WriteToken(token);
    }
  }
}
