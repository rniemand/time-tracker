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
using TimeTracker.Core.Models.Configuration;
using TimeTracker.Core.Models.Dto;
using TimeTracker.Core.Models.Requests;
using TimeTracker.Core.Models.Responses;

namespace TimeTracker.Core.Services
{
  public interface IUserService
  {
    Task<UserDto> GetFromToken(string token);
    Task<AuthenticationResponse> Authenticate(AuthenticationRequest request);
    string ExtendUserSession(int userId);
  }

  public class UserService : IUserService
  {
    private readonly ILoggerAdapter<UserService> _logger;
    private readonly IEncryptionService _encryptionService;
    private readonly IUserRepo _userRepo;
    private readonly AuthenticationConfig _config;

    public UserService(
      ILoggerAdapter<UserService> logger,
      IEncryptionService encryptionService,
      IUserRepo userRepo,
      TimeTrackerConfig config)
    {
      // TODO: [TESTS] (UserService) Add tests
      _logger = logger;
      _encryptionService = encryptionService;
      _userRepo = userRepo;
      _config = config.Authentication;

      if (string.IsNullOrWhiteSpace(_config.Secret))
      {
        // TODO: [HANDLE] (UserService.UserService) Handle this
        throw new Exception("Auth Secret is missing!");
      }
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
      // ReSharper disable once ConvertIfStatementToReturnStatement
      if (userEntity == null)
      {
        // TODO: [HANDLE] (UserService.GetFromToken) Handle this
        return null;
      }

      return UserDto.Projection.Compile()(userEntity);
    }

    public async Task<AuthenticationResponse> Authenticate(AuthenticationRequest request)
    {
      // TODO: [TESTS] (UserService.Authenticate) Add tests

      var loggedInUser = await LoginUser(request.Username, request.Password);
      if (loggedInUser == null)
        return null;

      return new AuthenticationResponse
      {
        FirstName = loggedInUser.FirstName,
        LastName = loggedInUser.LastName,
        UserId = loggedInUser.UserId,
        Username = loggedInUser.Username,
        Token = GenerateJwtToken(loggedInUser.UserId)
      };
    }

    public string ExtendUserSession(int userId)
    {
      // TODO: [TESTS] (UserService.ExtendUserSession) Add tests
      return GenerateJwtToken(userId);
    }


    // Internal methods
    private async Task<UserEntity> LoginUser(string username, string password)
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
      return userEntity;
    }

    private string GenerateJwtToken(int userId)
    {
      var tokenHandler = new JwtSecurityTokenHandler();
      var key = Encoding.ASCII.GetBytes(_config.Secret);

      var tokenDescriptor = new SecurityTokenDescriptor
      {
        Subject = new ClaimsIdentity(new[]
        {
          new Claim("id", userId.ToString())
        }),
        Expires = DateTime.UtcNow.AddMinutes(_config.SessionLengthMin),
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
        var key = Encoding.ASCII.GetBytes(_config.Secret);
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
      catch (Exception ex)
      {
        _logger.LogUnexpectedException(ex);
      }

      return 0;
    }
  }
}
