namespace TimeTracker.Core.Models.Requests
{
  public class AuthenticationRequest
  {
    public string Username { get; set; }
    public string Password { get; set; }

    public AuthenticationRequest()
    {
      // TODO: [TESTS] (AuthenticationRequest) Add tests
      Username = string.Empty;
      Password = string.Empty;
    }
  }
}
