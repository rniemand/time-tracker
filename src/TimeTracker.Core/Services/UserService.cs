using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using Rn.NetCore.Common.Encryption;
using Rn.NetCore.Common.Logging;
using Rn.NetCore.Common.Metrics;
using Rn.NetCore.Common.Metrics.Builders;
using TimeTracker.Core.Database.Entities;
using TimeTracker.Core.Database.Repos;
using TimeTracker.Core.Enums;
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
    private readonly IMetricService _metrics;
    private readonly IEncryptionService _encryptionService;
    private readonly IUserRepo _userRepo;
    private readonly AuthenticationConfig _config;

    public UserService(
      ILoggerAdapter<UserService> logger,
      IMetricService metrics,
      IEncryptionService encryptionService,
      IUserRepo userRepo,
      TimeTrackerConfig config)
    {
      // TODO: [TESTS] (UserService) Add tests
      _logger = logger;
      _encryptionService = encryptionService;
      _userRepo = userRepo;
      _metrics = metrics;
      _config = config.Authentication;

      if (string.IsNullOrWhiteSpace(_config.Secret))
      {
        // TODO: [HANDLE] (UserService.UserService) Handle this
        throw new Exception("Auth Secret is missing!");
      }
    }


    // Interface methods
    public async Task<UserDto> GetFromToken(string token)
    {
      // TODO: [TESTS] (UserService.GetFromToken) Add tests
      var builder = new ServiceMetricBuilder(nameof(UserService), nameof(GetFromToken))
        .WithCategory(MetricCategory.User, MetricSubCategory.Extract);

      try
      {
        using (builder.WithTiming())
        {
          // Try extract the current userId
          int userId;
          using (builder.WithCustomTiming1())
          {
            userId = ExtractUserId(token);
            if (userId == 0)
              return null;
          }

          UserEntity userEntity;
          using (builder.WithCustomTiming2())
          {
            builder.IncrementQueryCount();
            userEntity = await _userRepo.GetUserById(userId);
            builder.CountResult(userEntity);
          }

          return userEntity == null ? null : UserDto.Projection.Compile()(userEntity);
        }
      }
      catch (Exception ex)
      {
        _logger.LogUnexpectedException(ex);
        builder.WithException(ex);
        return null;
      }
      finally
      {
        await _metrics.SubmitPointAsync(builder.Build());
      }
    }

    public async Task<AuthenticationResponse> Authenticate(AuthenticationRequest request)
    {
      // TODO: [TESTS] (UserService.Authenticate) Add tests
      var builder = new ServiceMetricBuilder(nameof(UserService), nameof(Authenticate))
        .WithCategory(MetricCategory.User, MetricSubCategory.GetSingle)
        .WithCustomTag1(request.Username);

      try
      {
        using (builder.WithTiming())
        {
          UserEntity loggedInUser;
          using (builder.WithCustomTiming1())
          {
            builder.IncrementQueryCount();
            loggedInUser = await LoginUser(request.Username, request.Password);
            builder.CountResult(loggedInUser);
          }

          if (loggedInUser == null)
            return null;

          builder.WithCustomInt1(loggedInUser.UserId);

          return new AuthenticationResponse
          {
            FirstName = loggedInUser.FirstName,
            LastName = loggedInUser.LastName,
            UserId = loggedInUser.UserId,
            Username = loggedInUser.Username,
            Token = GenerateJwtToken(loggedInUser.UserId)
          };
        }
      }
      catch (Exception ex)
      {
        _logger.LogUnexpectedException(ex);
        builder.WithException(ex);
        return null;
      }
      finally
      {
        await _metrics.SubmitPointAsync(builder.Build());
      }
    }

    public string ExtendUserSession(int userId)
    {
      // TODO: [TESTS] (UserService.ExtendUserSession) Add tests
      var builder = new ServiceMetricBuilder(nameof(UserService), nameof(ExtendUserSession))
        .WithCategory(MetricCategory.User, MetricSubCategory.Update)
        .WithCustomInt1(userId);

      try
      {
        using (builder.WithTiming())
        {
          return GenerateJwtToken(userId);
        }
      }
      catch (Exception ex)
      {
        _logger.LogUnexpectedException(ex);
        builder.WithException(ex);
        return null;
      }
      finally
      {
        _metrics.SubmitPoint(builder.Build());
      }
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
