using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using Rn.NetCore.Common.Encryption;
using Rn.NetCore.Common.Logging;
using TimeTracker.Core.Database.Entities;
using TimeTracker.Core.Database.Repos;
using TimeTracker.Core.Models.Dto;

namespace TimeTracker.Core.Services
{
  public interface IUserService
  {
    Task<string> LoginUser(string username, string password);
    Task<UserDto> GetFromToken(string token);
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

    public async Task<UserDto> GetFromToken(string token)
    {
      // TODO: [TESTS] (UserService.GetFromToken) Add tests
      var userId = ExtractUserId(token);
      if (userId == 0)
      {
        // TODO: [HANDLE] (UserService.GetFromToken) Handle this
        return null;
      }

      var userEntity = await _userRepo.GetUserById(userId);
      if (userEntity == null)
      {
        // TODO: [HANDLE] (UserService.GetFromToken) Handle this
        return null;
      }

      return UserDto.Projection.Compile()(userEntity);
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

    private int ExtractUserId(string token)
    {
      try
      {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(Secret);
        tokenHandler.ValidateToken(token, new TokenValidationParameters
        {
          ValidateIssuerSigningKey = true,
          IssuerSigningKey = new SymmetricSecurityKey(key),
          ValidateIssuer = false,
          ValidateAudience = false,
          // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
          ClockSkew = TimeSpan.Zero
        }, out var validatedToken);

        var jwtToken = (JwtSecurityToken)validatedToken;
        var userId = int.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);
        return userId;
      }
      catch(Exception ex)
      {
        _logger.LogUnexpectedException(ex);
      }

      return 0;
    }
  }
}
